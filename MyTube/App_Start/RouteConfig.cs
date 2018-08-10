using System.Web.Mvc;
using System.Web.Routing;

namespace MyTube
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "UserSort",
                url: "{controller}/{action}/{sortOrder}/{searchString}",
                defaults: new { controller = "Users", action = "SortAndSearchUsers", sortOrder = UrlParameter.Optional, searchString = UrlParameter.Optional }
            );
        }
    }
}
