using Microsoft.Owin;

[assembly: OwinStartup(typeof(MailTrace.Host.Startup))]

namespace MailTrace.Host
{
    using System.Web.Http;

    using global::Ninject.Web.Common.OwinHost;
    using global::Ninject.Web.WebApi.OwinHost;

    using MailTrace.Host.Ninject;

    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureNinject(app);
            ConfigureWebApi(app);
        }

        private void ConfigureNinject(IAppBuilder app)
        {
            var kernel = KernelConfiguration.CreateKernel();
            app.UseNinjectMiddleware(() => kernel);
        }

        private static void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseNinjectWebApi(config);
        }
    }
}