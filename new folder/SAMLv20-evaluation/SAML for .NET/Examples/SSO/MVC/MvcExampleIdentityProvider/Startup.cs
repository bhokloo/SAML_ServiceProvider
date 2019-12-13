using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcExampleIdentityProvider.Startup))]
namespace MvcExampleIdentityProvider
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
