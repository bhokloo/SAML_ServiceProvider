using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClientApplication.Startup))]
namespace ClientApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
