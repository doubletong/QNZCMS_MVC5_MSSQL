using System.IO;
using System.Linq;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Front.ViewModel;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    [SIGActionFilter]
    public class BaseController : Controller
    {
         public AjaxResultVM AR = new AjaxResultVM();

        //protected new CustomPrincipal User
        //{
        //    get { return HttpContext.User as CustomPrincipal; }
        //}

        protected string GetModelErrorMessage()
        {
            string validationErrors = string.Join("|",
                    ModelState.Values.Where(E => E.Errors.Count > 0)
                    .SelectMany(E => E.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray());
            return validationErrors;
        }


        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }

        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }

        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// 网站主题设置
        /// </summary>
        /// <param name="requestContext"></param>
        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
           // var themeName = SettingsManager.Site.Theme;
            var defaultTheme = "Default";

            if (requestContext.HttpContext.Items["themeName"] == null)
            {
                //first time load
                requestContext.HttpContext.Items["themeName"] = requestContext.HttpContext.Request.Cookies.Get("theme").Value;
            }
            else
            {
                requestContext.HttpContext.Items["themeName"] = defaultTheme;

                var previewTheme = requestContext.RouteData.GetRequiredString("theme");

                if (!string.IsNullOrEmpty(previewTheme))
                {
                    requestContext.HttpContext.Items["themeName"] = previewTheme;
                }
            }

            base.Execute(requestContext);
        }

        public new RedirectToRouteResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }
    }
}