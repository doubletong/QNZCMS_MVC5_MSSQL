using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TZGCMS.SiteWeb.Controllers
{
    public class AboutController : BaseController
    {
        // GET: About
        public ActionResult Index()
        {
            return View();
        }
    }
}