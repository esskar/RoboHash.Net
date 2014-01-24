using System.Web.Mvc;
using System.Web.Routing;
using RoboHashDotNet.Internals;

namespace RoboHashDotNet.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RoboHash.ImageFileProvider = new DefaultImageFileProvider(Server.MapPath("bin"));

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}