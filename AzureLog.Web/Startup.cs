using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureLog.Web.Startup))]
namespace AzureLog.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
