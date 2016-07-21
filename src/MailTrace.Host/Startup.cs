namespace MailTrace.Host
{
    using System;
    using System.Web.Http;

    using global::Ninject;
    using global::Ninject.Web.Common.OwinHost;
    using global::Ninject.Web.WebApi.OwinHost;

    using MailTrace.Host.Ninject;

    using Owin;

    public class Startup
    {
        [ThreadStatic] public static Action<IKernel> PostConfigureKernel;

        public virtual void Configuration(IAppBuilder app)
        {
            ConfigureNinject(app);
            ConfigureWebApi(app);
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
            config.MapHttpAttributeRoutes();

            app.UseNinjectWebApi(config);
        }
    }
}