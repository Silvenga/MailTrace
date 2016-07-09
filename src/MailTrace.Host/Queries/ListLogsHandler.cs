namespace MailTrace.Host.Queries
{
    using System.Linq;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using MailTrace.Host.LogProcessing;

    using MediatR;

    public class ListLogsHandler : IRequestHandler<ListLogs.Query, Result>
    {
        private readonly ILogContext _context;
        private readonly IConfigurationProvider _mapperConfig;

        public ListLogsHandler(ILogContext context, IConfigurationProvider mapperConfig)
        {
            _context = context;
            _mapperConfig = mapperConfig;
        }

        public Result Handle(ListLogs.Query message)
        {
            var result =
                from x in _context.Logs.AsQueryable()
                where x.Timestamp <= message.After && x.Timestamp >= message.Before
                select x;

            var list = result.ProjectTo<Model>(_mapperConfig).ToList();

            return new Result
            {
                Logs = list
            };
        }
    }
}