using ComponentSpace.SAML2.Notifications;
using Microsoft.Owin;
using Owin;
using static ActivantsSP.Controllers.SamlController;

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
