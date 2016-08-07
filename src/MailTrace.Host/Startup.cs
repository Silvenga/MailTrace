namespace MailTrace.Host
{
    using System;
    using System.Web.Http;

    using global::Ninject;
    using global::Ninject.Web.Common.OwinHost;
    using global::Ninject.Web.WebApi.OwinHost;

    using MailTrace.Host.Ninject;

    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;

    using Newtonsoft.Json.Serialization;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    using Owin;

    public class Startup
    {
        [ThreadStatic] public static Action<IKernel> PostConfigureKernel;

        public virtual void Configuration(IAppBuilder app)
        {
            ConfigureLogging();

            ConfigureNinject(app);
            ConfigureWebApi(app);
            ConfigureStaticHosting(app);
        }

        private IKernel ConfigureNinject(IAppBuilder app)
        {
            var kernel = KernelConfiguration.CreateKernel();

            PostConfigureKernel?.Invoke(kernel);

            app.UseNinjectMiddleware(() => kernel);

            return kernel;
        }

        private static void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
            };

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.MapHttpAttributeRoutes();

            app.UseNinjectWebApi(config);
        }

        private static void ConfigureStaticHosting(IAppBuilder app)
        {
            try
            {
                var physicalFileSystem = new PhysicalFileSystem("Content");
                var options = new FileServerOptions
                {
                    EnableDefaultFiles = true,
                    FileSystem = physicalFileSystem,
                    EnableDirectoryBrowsing = true
                };
                options.StaticFileOptions.FileSystem = physicalFileSystem;
                options.StaticFileOptions.ServeUnknownFileTypes = true;
                app.UseFileServer(options);
            }
            catch (Exception)
            {
            }
        }

        private static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/log";
            fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            LogManager.Configuration = config;
        }
    }
}