using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SES_F1.Startup))]
namespace SES_F1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
