using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MobileBackEnd.Startup))]
namespace MobileBackEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
