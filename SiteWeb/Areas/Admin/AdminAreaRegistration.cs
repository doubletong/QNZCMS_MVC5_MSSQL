using System.Web.Mvc;

namespace TZGCMS.SiteWeb.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {        
            context.MapRoute(
                "Admin_default",
                "bbi-admin/{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               namespaces: new string[] { "TZGCMS.SiteWeb.Areas.Admin.Controllers" }
            );
        }
    }
}