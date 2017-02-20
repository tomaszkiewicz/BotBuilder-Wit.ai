using System.Web;
using System.Web.Http;

namespace Wit.Bot.Framework.Builder.Sample
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}