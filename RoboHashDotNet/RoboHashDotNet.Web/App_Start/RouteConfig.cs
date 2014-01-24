using System.Web.Mvc;
using System.Web.Routing;

namespace RoboHashDotNet.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Api",
                url: "{*token}",
                defaults: new { controller = "Main", action = "Default" }
            );            
        }
    }
}