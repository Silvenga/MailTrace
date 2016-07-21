namespace MailTrace.Host.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentValidation;

    using MailTrace.Data;
    using MailTrace.Data.Entities;
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
            public int ChangesProcessed { get; set; }
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
                        SourceTime = line.SourceTime
                    });

                _context.EmailProperties.AddRange(lines);

                var changed = await _context.SaveChangesAsync();

                Console.WriteLine($"Changed: {changed}");

                return new ImportLogs.Result
                {
                    ChangesProcessed = changed
                };
            }
        }
    }
}