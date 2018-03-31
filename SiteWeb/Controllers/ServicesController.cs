using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ServicesController : BaseController
    {
        // GET: Services
        [SIGActionFilter]
        public ActionResult Index()
        {
            return View();
        }
    }
}