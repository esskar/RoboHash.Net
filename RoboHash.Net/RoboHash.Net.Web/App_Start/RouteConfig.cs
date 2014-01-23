using System.Web.Mvc;
using System.Web.Routing;

namespace RoboHash.Net.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Api",
                url: "{token}/",
                defaults: new { controller = "Api", action = "Render" }
            );
            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Root", action = "Index" }
            );
        }
    }
}