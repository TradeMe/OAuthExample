using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TradeMeExampleOAuthApplication.Startup))]
namespace TradeMeExampleOAuthApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
