using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace HotelBookingWeb.Authentication
{
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            if (filterContext.HttpContext.Request.Cookies["JWTToken"] == null)
            {
                filterContext.Result = new RedirectResult("~/Auth/Login");
            }

        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(filterContext);
        }
    }
}
