using Microsoft.Owin;

[assembly: OwinStartup(typeof(MailTrace.Host.Startup))]

namespace MailTrace.Host
{
    using System.Web.Http;

    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureWebApi(app);
        }

        private static void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }
    }
}