using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.SiteWeb.Controllers;

namespace TZGCMS.SiteWeb.Filters
{
    public class SIGActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SettingsManager.Site.IsClose)
            {
               // RedirectToRouteResult()
                var controller = (BaseController)filterContext.Controller;
                filterContext.Result = controller.RedirectToAction("CloseSite", "Home");
            }
            //Log("OnActionExecuting", filterContext.RouteData);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
           // Log("OnActionExecuted", filterContext.RouteData);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
          //  Log("OnResultExecuting", filterContext.RouteData);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
           // Log("OnResultExecuted", filterContext.RouteData);
        }

        //public new RedirectToRouteResult RedirectToAction(string action, string controller)
        //{
        //    return base.RedirectToAction(action, controller);
        //}
        //private void Log(string methodName, RouteData routeData)
        //{
        //    var controllerName = routeData.Values["controller"];
        //    var actionName = routeData.Values["action"];
        //    var message = String.Format("{0} controller:{1} action:{2}", methodName, controllerName, actionName);
        //    Debug.WriteLine(message, "Action Filter Log");
        //}
    }
}