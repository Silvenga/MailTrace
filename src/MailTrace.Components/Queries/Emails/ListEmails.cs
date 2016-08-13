namespace MailTrace.Host.Queries.Emails
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper.Configuration;

    using JetBrains.Annotations;

    using LinqKit;

    using MailTrace.Components.Helpers;
    using MailTrace.Data;
    using MailTrace.Data.Entities;

    using MediatR;

    public class ListEmails
    {
        public class Query : IAsyncRequest<Result>
        {
            public DateTime? Before { get; set; }

            public DateTime? After { get; set; }

            public string From { get; set; }

            public string To { get; set; }

            public int? PageSize { get; set; }

            public int? Page { get; set; }
        }

        public class Result
        {
            public IList<Email> Emails { get; set; }

            public int Page { get; set; }

            public int PageSize { get; set; }

            public int Count { get; set; }
        }

        public class Email
        {
            public string To { get; set; }

            public string From { get; set; }

            public string MessageId { get; set; }

            public string Size { get; set; }

            public string NumberOfRecipients { get; set; }

            public DateTime FirstSeen { get; set; }
        }
    }

    public class ListEmailsHandler : IAsyncRequestHandler<ListEmails.Query, ListEmails.Result>
    {
        private readonly TraceContext _context;

        public ListEmailsHandler(TraceContext context)
        {
            _context = context;
        }

        public async Task<ListEmails.Result> Handle(ListEmails.Query message)
        {
            // TODO
            // [ ] Reduce complexity of this query, R# is sad.

            var page = message.Page ?? 1;
            var takeSize = message.PageSize ?? 50;
            var skipSize = takeSize * (page - 1);

            var baseQuery = BaseQueryAndFilter(message);
            var countTask = await baseQuery.CountAsync();

            var pagedQuery = baseQuery
                .OrderByDescending(x => x.SourceTime)
                .Skip(skipSize)
                .Take(takeSize);

            var filterPropertyQuery = _context
                .EmailProperties
                .AsExpandable()
                .Where(x => new[] {"to", "nrcpt", "size", "from"}.Contains(x.Key));

            var query = await (from m in pagedQuery
                               join attr in
                                   filterPropertyQuery on new {m.QueueId, m.Host}
                                   equals new {attr.QueueId, attr.Host}
                               orderby m.SourceTime descending
                               select new Params
                               {
                                   MessageId = m.Value,
                                   FirstSeen = m.SourceTime,
                                   Key = attr.Key,
                                   Value = attr.Value,
                                   SourceTime = attr.SourceTime
                               })
                .ToListAsync();

            var projection = Projection(query).ToList();

            return new ListEmails.Result
            {
                Emails = projection,
                Page = page,
                PageSize = takeSize,
                Count = countTask
            };
        }

        private IEnumerable<ListEmails.Email> Projection(List<Params> query)
        {
            return query
                .GroupBy(x => new {x.MessageId, x.FirstSeen})
                .Select(
                    g =>
                        new
                        {
                            g.Key.MessageId,
                            g.Key.FirstSeen,
                            Group =
                                g.ToLookup(x => x.Key)
                                 .ToDictionary(x => x.Key, s => s.OrderByDescending(x => x.SourceTime).Select(x => x.Value))
                        })
                .Select(x => new ListEmails.Email
                {
                    MessageId = x.MessageId,
                    FirstSeen = x.FirstSeen.Value,
                    To = ConcatRecipientList(x.Group.GetOrDefault("to")),
                    Size = x.Group.GetOrDefault("size")?.FirstOrDefault(),
                    From = x.Group.GetOrDefault("from")?.FirstOrDefault(),
                    NumberOfRecipients = x.Group.GetOrDefault("nrcpt")?.FirstOrDefault(),
                });
        }

        private string ConcatRecipientList([CanBeNull] IEnumerable<string> enumerable)
        {
            var list = enumerable ?? Enumerable.Empty<string>();
            return string.Join(";", list.Distinct());
        }

        private IQueryable<EmailProperty> BaseQueryAndFilter(ListEmails.Query message)
        {
            var toPredicate = PredicateBuilder.True<EmailProperty>();
            if (message.To != null)
            {
                toPredicate = toPredicate.And(x => x.Key == "to" && x.Value.Contains(message.To));
                toPredicate = toPredicate.Or(x => x.Key == "orig_to" && x.Value.Contains(message.To));
            }
            var fromPredicate = PredicateBuilder.True<EmailProperty>();
            if (message.From != null)
            {
                fromPredicate = fromPredicate.And(x => x.Key == "from" && x.Value.Contains(message.From));
            }
            var sourcePredicate = PredicateBuilder.True<EmailProperty>();
            if (message.Before != null)
            {
                sourcePredicate = sourcePredicate.And(x => x.Key == "message-id" && x.SourceTime <= message.Before);
            }
            if (message.After != null)
            {
                sourcePredicate = sourcePredicate.And(x => x.Key == "message-id" && x.SourceTime >= message.After);
            }

            var filterToQuery = _context
                .EmailProperties
                .AsExpandable()
                .Where(toPredicate)
                .Select(x => new {x.QueueId, x.Host})
                .Distinct();
            var filterFromQuery = _context
                .EmailProperties
                .AsExpandable()
                .Where(fromPredicate)
                .Select(x => new {x.QueueId, x.Host})
                .Distinct();
            var filterSourceTimeQuery = _context
                .EmailProperties
                .AsExpandable()
                .Where(sourcePredicate)
                .Select(x => new {x.QueueId, x.Host})
                .Distinct();

            return from m in _context
                .EmailProperties
                .Where(x => x.Key == "message-id")
                   join filterTo in
                       filterToQuery on new {m.QueueId, m.Host}
                       equals new {filterTo.QueueId, filterTo.Host}
                   join filterFrom in
                       filterFromQuery on new {m.QueueId, m.Host}
                       equals new {filterFrom.QueueId, filterFrom.Host}
                   join filterSourceTime in
                       filterSourceTimeQuery on new {m.QueueId, m.Host}
                       equals new {filterSourceTime.QueueId, filterSourceTime.Host}
                   select m;
        }

        private class Params
        {
            public string MessageId { get; set; }
            public DateTime? FirstSeen { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public DateTime? SourceTime { get; set; }
        }
    }
}