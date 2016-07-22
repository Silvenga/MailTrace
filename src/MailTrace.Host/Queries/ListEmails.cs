namespace MailTrace.Host.Queries
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
            public IList<Email> Logs { get; set; }
        }

        public class Email
        {
            public string To { get; set; }

            public string From { get; set; }

            public string MessageId { get; set; }

            public string DsnCode { get; set; }

            public string Status { get; set; }

            public string Delay { get; set; }

            public string Size { get; set; }

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

            var baseQuery = _context
                .EmailProperties
                .AsExpandable()
                .Where(x => x.Key == "message-id")
                .OrderByDescending(x => x.SourceTime)
                .Skip(skipSize)
                .Take(takeSize);
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
            var filterPropertyQuery = _context
                .EmailProperties
                .AsExpandable()
                .Where(x => new[] {"to", "dsn", "delay", "status", "size", "from"}.Contains(x.Key));

            var query = from m in baseQuery
                        join attr in
                            filterPropertyQuery on new {m.QueueId, m.Host}
                            equals new {attr.QueueId, attr.Host}
                        join filterTo in
                            filterToQuery on new {m.QueueId, m.Host}
                            equals new {filterTo.QueueId, filterTo.Host}
                        join filterFrom in
                            filterFromQuery on new {m.QueueId, m.Host}
                            equals new {filterFrom.QueueId, filterFrom.Host}
                        join filterSourceTime in
                            filterSourceTimeQuery on new {m.QueueId, m.Host}
                            equals new {filterSourceTime.QueueId, filterSourceTime.Host}
                        select new
                        {
                            MessageId = m.Value,
                            FirstSeen = m.SourceTime,
                            attr.Key,
                            attr.Value,
                            attr.SourceTime
                        };

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
                                                    ToDictionary(x => x.Key, s => s.OrderByDescending(x => x.SourceTime).Select(x => x.Value).FirstOrDefault())
                                          })
                                  .Select(x => new ListEmails.Email
                                  {
                                      MessageId = x.MessageId,
                                      FirstSeen = x.FirstSeen.Value,
                                      To = x.Group.GetOrDefault("to"),
                                      DsnCode = x.Group.GetOrDefault("dsn"),
                                      Delay = x.Group.GetOrDefault("delay"),
                                      Status = x.Group.GetOrDefault("status"),
                                      Size = x.Group.GetOrDefault("size"),
                                      From = x.Group.GetOrDefault("from"),
                                  });

            return new ListEmails.Result
            {
                Logs = projection.ToList()
            };
        }
    }
}