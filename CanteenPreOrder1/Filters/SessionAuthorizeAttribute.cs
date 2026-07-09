using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CanteenPreOrder1.Filters
{
    
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public SessionAuthorizeAttribute(string role = null)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            var userId  = session["UserID"];

            if (userId == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Account" }, { "action", "Login" }
                });
                return;
            }

            if (!string.IsNullOrEmpty(_role))
            {
                var userRole = session["UserRole"] != null ? session["UserRole"].ToString() : null;
                if (!string.Equals(userRole, _role, StringComparison.OrdinalIgnoreCase))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Home" }, { "action", "AccessDenied" }
                    });
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
