namespace MailTrace.Host.Queries
{
    using System.Linq;

    using MailTrace.Host.LogProcessing;

    using MediatR;

    public class ViewLogsHandler : IRequestHandler<ViewLogs.Query, ViewLogs.Result>
    {
        private readonly ILogContext _context;

        public ViewLogsHandler(ILogContext context)
        {
            _context = context;
        }

        public ViewLogs.Result Handle(ViewLogs.Query message)
        {
            var result =
                from x in _context.Logs
                where x.Timestamp <= message.After && x.Timestamp >= message.Before
                select x;

            return new ViewLogs.Result();
        }
    }
}