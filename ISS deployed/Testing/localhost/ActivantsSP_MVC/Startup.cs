using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ActivantsSP.Startup))]
namespace ActivantsSP
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
