namespace MailTrace.Components.Queries.Logs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentValidation;

    using MailTrace.Components.LogProcessing;
    using MailTrace.Data;
    using MailTrace.Data.Entities;

    using MediatR;

    using NLog;

    public class ImportLogs
    {
        public class Command : IAsyncRequest<Result>
        {
            public IList<string> LogLines { get; set; }
        }

        public class QueryValidator : AbstractValidator<Command>
        {
            public QueryValidator()
            {
                RuleFor(m => m).NotNull();
                RuleFor(m => m.LogLines).NotNull().NotEmpty().WithMessage("Logs should have value");
            }
        }

        public class Result
        {
            public int ChangesProcessed { get; set; }
        }
    }

    public class ImportLogsHandler : IAsyncRequestHandler<ImportLogs.Command, ImportLogs.Result>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TraceContext _context;
        private readonly LogParser _parser;

        public ImportLogsHandler(TraceContext context, LogParser parser)
        {
            _context = context;
            _parser = parser;
        }

        public async Task<ImportLogs.Result> Handle(ImportLogs.Command message)
        {
            using (_context)
            {
                var lines = _parser
                    .Parse(message.LogLines)
                    .SelectMany(x => x.Attributes, (line, attribute) => new EmailProperty
                    {
                        QueueId = line.QueueId,
                        Host = line.Host,
                        Key = attribute.Name,
                        Value = attribute.Value,
                        SourceTime = line.SourceTime,
                        LogId = line.LogId
                    })
                    .ToList();

                Logger.Info("-------------------------");
                foreach (var emailProperty in lines)
                {
                    Logger.Info(emailProperty.ToString());
                }
                Logger.Info("-------------------------");

                _context.EmailProperties.AddRange(lines);

                var changed = await _context.SaveChangesAsync();

                Logger.Info($"Changed: {changed}");

                return new ImportLogs.Result
                {
                    ChangesProcessed = changed
                };
            }
        }
    }
}