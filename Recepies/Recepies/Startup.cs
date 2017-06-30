using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Recepies.Startup))]
namespace Recepies
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
