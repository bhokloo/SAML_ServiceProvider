using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClientApplication.Startup))]
namespace ClientApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureAuth(app);
        }
    }
}
