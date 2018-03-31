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
        private TZGEntities db = new TZGEntities();
        // GET: Pages     
        [SIGActionFilter]
        public ActionResult Index(string seoName)
        {
            var page = db.Pages.FirstOrDefault(d => d.Active && d.SeoName == seoName);
            if (page == null)
                return HttpNotFound();

            ViewBag.PageMeta = db.PageMetas.FirstOrDefault(d=>d.ModelType == ModelType.PAGE && d.ObjectId == page.Id.ToString());
            return View(page);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}