namespace MailTrace.Host.SystemWeb
{
    using System;

    using MailTrace.Data;
    using MailTrace.Data.Postgresql;

    using Owin;

    public class Startup : Host.Startup
    {
        public override void Configuration(IAppBuilder app)
        {
            PostConfigureKernel += kernel => { kernel.Bind<TraceContext>().To<PostgresqlTraceContext>(); };

            Console.WriteLine("Running Migration...");

            var context = new PostgresqlTraceContext();
            context.Migrate();

            base.Configuration(app);
        }
    }
}