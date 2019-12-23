using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcExampleServiceProvider.Startup))]
namespace MvcExampleServiceProvider
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
