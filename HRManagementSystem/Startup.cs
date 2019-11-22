using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HRManagementSystem.Startup))]
namespace HRManagementSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
