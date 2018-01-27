using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TZGCMS.SiteWeb.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        //[Route("{seoName}")]
        public ActionResult Index(string seoName)
        {
            return View();
        }
    }
}