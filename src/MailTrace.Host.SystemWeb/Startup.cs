using MailTrace.Host.SystemWeb;

using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace MailTrace.Host.SystemWeb
{
    using System.Diagnostics;

    using MailTrace.Data;
    using MailTrace.Data.Postgresql;

    using Owin;

    public class Startup : Host.Startup
    {
        public override void Configuration(IAppBuilder app)
        {
            PostConfigureKernel += kernel => { kernel.Bind<TraceContext>().To<PostgresqlTraceContext>(); };

            Trace.WriteLine("Running Migration...");

            var context = new PostgresqlTraceContext();
            context.Migrate();

            Trace.WriteLine("Completed Migration...");

            base.Configuration(app);
        }
    }
}