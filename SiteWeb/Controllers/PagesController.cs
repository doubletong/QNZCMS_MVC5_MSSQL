using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Enums;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers.Pages
{
    public class PagesController : BaseController
    {
       
        // GET: Pages     
        [SIGActionFilter]
        public ActionResult Index(string seoName)
        {
            var page = _db.Pages.FirstOrDefault(d => d.Active && d.SeoName == seoName);
            if (page == null)
                return HttpNotFound();

            ViewBag.PageMeta = _db.PageMetas.FirstOrDefault(d=>d.ModelType == ModelType.PAGE && d.ObjectId == page.Id.ToString());
            return View(page);
        }

        public ActionResult Paragraph(string seoName)
        {
            var page = _db.Pages.FirstOrDefault(d => d.Active && d.SeoName == seoName);
     
            return PartialView("_Paragraph", page);
        }

    }
}