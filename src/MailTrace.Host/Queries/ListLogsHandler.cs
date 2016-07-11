namespace MailTrace.Host.Queries
{
    using System.Linq;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using MailTrace.Host.Data;

    using MediatR;

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