namespace MailTrace.Components.Queries.Logs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using MailTrace.Data;

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

    public class ListLogsHandler : IRequestHandler<ListLogs.Query, ListLogs.Result>
    {
        private readonly TraceContext _context;
        private readonly IConfigurationProvider _mapperConfig;

        public ListLogsHandler(TraceContext context, IConfigurationProvider mapperConfig)
        {
            _context = context;
            _mapperConfig = mapperConfig;
        }

        public ListLogs.Result Handle(ListLogs.Query message)
        {
            var result =
                from x in _context.Logs.AsQueryable()
                where x.Timestamp <= message.After && x.Timestamp >= message.Before
                select x;

            var list = result.ProjectTo<ListLogs.Model>(_mapperConfig).ToList();

            return new ListLogs.Result
            {
                Logs = list
            };
        }
    }
}