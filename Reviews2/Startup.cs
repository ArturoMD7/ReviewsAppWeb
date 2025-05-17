using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Reviews2.Startup))]
namespace Reviews2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
