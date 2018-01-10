using Autofac;
using FluentScheduler;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.SiteWeb.App_Start;

namespace TZGCMS.SiteWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //Remove All View Engine including Webform and Razor
            ViewEngines.Engines.Clear();
            //Register Razor View Engine
            ViewEngines.Engines.Add(new CSharpRazorViewEngine());
            

            //log config
            log4net.Config.XmlConfigurator.Configure();
            log4net.GlobalContext.Properties["user"] = new HttpContextUserNameProvider();

            DependencyRegistrar.Register();
            //定时任务
            JobManager.Initialize(new JobRegistry());


        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            //session 引到webapi
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                CustomPrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket.UserData);
                CustomPrincipal newUser = new CustomPrincipal(authTicket.Name)
                {
                    UserId = serializeModel.UserId,
                    RealName = serializeModel.RealName,
                    Roles = serializeModel.Roles,
                    Avatar = serializeModel.Avatar
                };
                //newUser.Menus = serializeModel.Menus;

                HttpContext.Current.User = newUser;
            }

        }
    }
}
