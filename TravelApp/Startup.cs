using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TravelApp.Startup))]
namespace TravelApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

        }
    }
}
