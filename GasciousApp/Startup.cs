using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GasciousApp.Startup))]
namespace GasciousApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
