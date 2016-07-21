namespace MailTrace.Host.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

            public DateTime LastUpdate { get; set; }
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
            var a = from m in _context.EmailProperties.Where(x => x.Key == "message-id")
                    let lasts = _context.EmailProperties.Where(x => x.QueueId == m.QueueId).OrderByDescending(x => x.SourceTime)
                    select new ListEmails.Email
                    {
                        MessageId = m.Value,
                        To = lasts.FirstOrDefault(x => x.Key == "to").Value,
                        DsnCode = lasts.FirstOrDefault(x => x.Key == "dsn").Value,
                        Delay = lasts.FirstOrDefault(x => x.Key == "delay").Value,
                        Status = lasts.FirstOrDefault(x => x.Key == "status").Value,
                        Size = lasts.FirstOrDefault(x => x.Key == "size").Value,
                        From = lasts.FirstOrDefault(x => x.Key == "from").Value,
                    };

            var b = a.ToList();

            return new ListEmails.Result
            {
                Logs = b
            };
        }
    }
}