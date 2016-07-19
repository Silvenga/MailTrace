namespace MailTrace.Host.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Ninject;

    using MailTrace.Host.Controllers;
    using MailTrace.Host.Data;
    using MailTrace.Host.Ninject;
    using MailTrace.Host.Queries;
    using MailTrace.Host.Tests.Fixtures;

    using Xunit;

    public class LogsControllerFacts : IDisposable
    {
        private readonly IKernel _kernel;

        public LogsControllerFacts()
        {
            _kernel = KernelConfiguration.CreateKernel();

            var connection = Effort.DbConnectionFactory.CreateTransient();

            _kernel.Rebind<TraceContext>().ToMethod(x => new TestTraceContext(connection));
        }

        [Fact]
        public async Task Can_import_logs()
        {
            var controller = _kernel.Get<LogsController>();
            var command = new ImportLogs.Command
            {
                LogLines = new List<string>
                {
                    "Jul 10 20:31:32 d0 postfix/cleanup[16286]: EF59F17F531: message-id=<sig.200092563b.SN1PR10MB06400081E699D2524DDED785D53F0@SN1PR10MB0640.namprd10.prod.outlook.com>"
                }
            };

            var context = _kernel.Get<TraceContext>();

            // Act
            var result = await controller.ImportAsync(command);

            // Assert

            var results = context.EmailProperties.ToList();
        }

        public void Dispose()
        {
            _kernel.Dispose();
        }
    }
}