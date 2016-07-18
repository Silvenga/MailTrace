namespace MailTrace.Host.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentValidation;

    using MailTrace.Host.Data;
    using MailTrace.Host.LogProcessing;

    using MediatR;

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
            public int LinesProcessed { get; set; }
        }
    }

    public class ImportLogsHandler : IAsyncRequestHandler<ImportLogs.Command, ImportLogs.Result>
    {
        private readonly TraceContext _context;
        private readonly LogParser _parser;

        public ImportLogsHandler(TraceContext context, LogParser parser)
        {
            _context = context;
            _parser = parser;
        }

        public async Task<ImportLogs.Result> Handle(ImportLogs.Command message)
        {
            var lines = _parser.Parse(message.LogLines);

            return new ImportLogs.Result
            {
                LinesProcessed = 0
            };
        }
    }
}