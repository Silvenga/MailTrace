namespace MailTrace.Host.Selfhost
{
    using System;
    using System.Linq;

    using MailTrace.Data.Postgresql;
    using MailTrace.Host.Data;

    using Microsoft.Owin.Hosting;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var baseAddress = args.FirstOrDefault() ?? "http://localhost:9900";

            Startup.PostConfigureKernel += kernel => { kernel.Bind<TraceContext>().To<PostgresqlTraceContext>(); };

            Console.WriteLine("Running Migration...");

            var context = new PostgresqlTraceContext();
            context.Migrate();

            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.WriteLine("Ready.");
                Console.ReadLine();
            }
        }
    }
}