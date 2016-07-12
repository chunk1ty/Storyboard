using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AnkkBoard.Web.Startup))]
namespace AnkkBoard.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
