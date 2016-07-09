namespace MailTrace.Host.Queries
{
    using System;
    using System.Collections.Generic;

    using MediatR;

    public class ListLogs
    {
        public class Query : IRequest<Result>
        {
            public DateTime? Before { get; set; }

            public DateTime? After { get; set; }
        }

        public class Result
        {
            public IList<Model> Logs { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }

            public string Message { get; set; }

            public DateTimeOffset Timestamp { get; set; }
        }
    }
}