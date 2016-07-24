namespace MailTrace.Host.Queries.Emails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper.Configuration;

    using LinqKit;

    using MailTrace.Data;
    using MailTrace.Data.Entities;

    using MediatR;

    public class ListEmails
    {
        public class Query : IRequest<Result>
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

            public int CurrentPage { get; set; }

            public int CurrentPageSize { get; set; }
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

    public class ListEmailsHandler : IRequestHandler<ListEmails.Query, ListEmails.Result>
    {
        private readonly TraceContext _context;

        public ListEmailsHandler(TraceContext context)
        {
            _context = context;
        }

        public ListEmails.Result Handle(ListEmails.Query message)
        {
            // TODO
            // [ ] Reduce complexity of this query, R# is sad.

            var page = message.Page ?? 1;
            var takeSize = message.PageSize ?? 50;
            var skipSize = takeSize * (page - 1);

            var toPredicate = PredicateBuilder.True<EmailProperty>();
            if (message.To != null)
            {
                toPredicate = toPredicate.And(x => x.Key == "to" && x.Value.Contains(message.To));
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
            
            var baseQuery = (from m in _context
                .EmailProperties
                .AsExpandable().Where(x => x.Key == "message-id")
                             join filterTo in
                                 filterToQuery on new {m.QueueId, m.Host}
                                 equals new {filterTo.QueueId, filterTo.Host}
                             join filterFrom in
                                 filterFromQuery on new {m.QueueId, m.Host}
                                 equals new {filterFrom.QueueId, filterFrom.Host}
                             join filterSourceTime in
                                 filterSourceTimeQuery on new {m.QueueId, m.Host}
                                 equals new {filterSourceTime.QueueId, filterSourceTime.Host}
                             select m)
                .OrderByDescending(x => x.SourceTime)
                .Skip(skipSize)
                .Take(takeSize);

            var filterPropertyQuery = _context
                .EmailProperties
                .AsExpandable()
                .Where(x => new[] {"to", "nrcpt", "size", "from"}.Contains(x.Key));

            var query = (from m in baseQuery
                         join attr in
                             filterPropertyQuery on new {m.QueueId, m.Host}
                             equals new {attr.QueueId, attr.Host}
                         orderby m.SourceTime descending
                         select new
                         {
                             MessageId = m.Value,
                             FirstSeen = m.SourceTime,
                             attr.Key,
                             attr.Value,
                             attr.SourceTime
                         })
                .ToList();

            var projection = query.AsEnumerable()
                                  .GroupBy(x => new {x.MessageId, x.FirstSeen})
                                  .Select(
                                      g =>
                                          new
                                          {
                                              g.Key.MessageId,
                                              g.Key.FirstSeen,
                                              Group =
                                                  g.ToLookup(x => x.Key).
                                                    ToDictionary(x => x.Key, s => s.OrderByDescending(x => x.SourceTime).Select(x => x.Value))
                                          })
                                  .Select(x => new ListEmails.Email
                                  {
                                      MessageId = x.MessageId,
                                      FirstSeen = x.FirstSeen.Value,
                                      To = string.Join(";", (x.Group.GetOrDefault("to") ?? Enumerable.Empty<string>()).Distinct()),
                                      Size = x.Group.GetOrDefault("size")?.FirstOrDefault(),
                                      From = x.Group.GetOrDefault("from")?.FirstOrDefault(),
                                      NumberOfRecipients = x.Group.GetOrDefault("nrcpt")?.FirstOrDefault(),
                                  });

            return new ListEmails.Result
            {
                Emails = projection.ToList(),
                CurrentPage = page,
                CurrentPageSize = takeSize
            };
        }
    }
}