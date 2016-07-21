namespace MailTrace.Host.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper.Configuration;

    using MailTrace.Host.Data;

    using MediatR;

    public class ListEmails
    {
        public class Query : IRequest<Result>
        {
            public DateTime? Before { get; set; }

            public DateTime? After { get; set; }
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
            var query = from m in _context.EmailProperties.Where(x => x.Key == "message-id")
                        join attr in _context.EmailProperties on new {m.QueueId, m.Host} equals new {attr.QueueId, attr.Host}
                        where new[] {"to", "dsn", "delay", "status", "size", "from"}.Contains(attr.Key)
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