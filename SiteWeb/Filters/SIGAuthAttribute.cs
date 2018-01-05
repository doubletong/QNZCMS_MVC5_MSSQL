using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Service.Identity;

namespace SiteWeb.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SIGAuthAttribute : AuthorizeAttribute
    {
        IUserServices _userService;

        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                                 || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
            {
                return;
            }

            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {

                    var ar = new AjaxResultVM();
                    ar.SetWarning("您还未登录，不能操作此项。");
                    //  filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult
                    {
                        Data = ar,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "AccessDenied" }));
                }
                //filterContext.Result = new HttpUnauthorizedResult();
                return;
            }

            if (CurrentUser.IsInRole("创始人"))
            {
                base.OnAuthorization(filterContext);
                return;
            }

            //string originController = filterContext.RouteData.Values["controller"].ToString();
            //string originAction = filterContext.RouteData.Values["action"].ToString();
            //string originArea = String.Empty;
            //if (filterContext.RouteData.DataTokens.ContainsKey("area"))
            //    originArea = filterContext.RouteData.DataTokens["area"].ToString();

            string areaName = String.Empty;
            if (filterContext.RouteData.DataTokens.ContainsKey("area"))
                areaName = filterContext.RouteData.DataTokens["area"].ToString();

            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;

            _userService = DependencyResolver.Current.GetService<IUserServices>();
            var userMenus = _userService.GetUserMenus(CurrentUser.UserId);
        
          //  var userMenus = _menuRepository.GetMenusByUserId(CurrentUser.UserId);
            //Check if the requesting user has the permission to run the controller's action
            if (!CurrentUser.IsInMenu(areaName, controllerName, actionName, userMenus))
            {

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {

                    var ar = new AjaxResultVM();
                    ar.SetWarning("您的权限不够，不能操作此项");
                    //  filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult
                    {
                        Data = ar,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "AccessDenied" }));
                }


                return;
            }


            //if (!String.IsNullOrEmpty(Roles))
            //{
            //    if (!CurrentUser.IsInRole(Roles))
            //    {
            //        filterContext.Result = new RedirectToRouteResult(new
            //            RouteValueDictionary(new { controller = "Account", action = "AccessDenied" }));
            //        return;
            //        // base.OnAuthorization(filterContext); //returns to login url
            //    }
            //}

            //if (!String.IsNullOrEmpty(Users))
            //{
            //    if (!Users.Contains(CurrentUser.UserId.ToString()))
            //    {
            //        filterContext.Result = new RedirectToRouteResult(new
            //            RouteValueDictionary(new { controller = "Account", action = "AccessDenied" }));
            //        return;
            //        // base.OnAuthorization(filterContext); //returns to login url
            //    }
            //}

            base.OnAuthorization(filterContext);



        }
    }
}