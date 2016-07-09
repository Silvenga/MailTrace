using Microsoft.Owin;

[assembly: OwinStartup(typeof(MailTrace.Host.Startup))]

namespace MailTrace.Host
{
    using System.Web.Http;

    using global::Ninject;
    using global::Ninject.Web.Common.OwinHost;
    using global::Ninject.Web.WebApi;
    using global::Ninject.Web.WebApi.OwinHost;

    using MailTrace.Host.Ninject;

    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var kernel = ConfigureNinject(app);
            ConfigureWebApi(app, kernel);
        }

        private IKernel ConfigureNinject(IAppBuilder app)
        {
            var kernel = KernelConfiguration.CreateKernel();
           
#if NCRUNCH
            kernel.Load(typeof(WebApiModule).Assembly);
#endif

            return kernel;
        }

        private static void ConfigureWebApi(IAppBuilder app, IKernel kernel)
        {
            var config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MapHttpAttributeRoutes();
            app.UseNinjectMiddleware(() => kernel).UseNinjectWebApi(config);
        }
    }
}