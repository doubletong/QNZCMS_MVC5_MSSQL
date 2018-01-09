using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
    }
}