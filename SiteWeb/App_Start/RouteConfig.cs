﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TZGCMS.SiteWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

           // routes.MapRoute(
           //    name: "SinglePage",
           //    url: "{seoName}",
           //    defaults: new { controller = "Pages", action = "Index" },
           //    constraints: new { seoName = @"^(?!article|products|cases|contact|download|about|services|search)([a-z0-9]+)$" },
           //    namespaces: new string[] { "TZGCMS.SiteWeb.Controllers.Pages" }
           //);

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               namespaces: new string[] { "TZGCMS.SiteWeb.Controllers" }
           );
        }
    }
}
