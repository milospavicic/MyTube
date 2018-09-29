using MyTube.Helpers;
using System.Web.Mvc;

namespace MyTube.App_Start
{
    public class SessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            UsersHelper.RefreshLoggedInUserSession(context.HttpContext.Session);
        }
    }
}