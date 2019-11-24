using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers.Pages
{
    public class PagesController : Controller
    {

        private readonly IQNZDbContext _db;
        private readonly ICacheService _cacheService;
        public PagesController(ICacheService cacheService, IQNZDbContext db)
        {
            _cacheService = cacheService;
            _db = db;
        }
        // GET: Pages     
        [SIGActionFilter]
        public ActionResult Index(string seoName)
        {
            var page = _db.PageSets.FirstOrDefault(d => d.Active && d.SeoName == seoName);
            if (page == null)
                return HttpNotFound();

            ViewBag.PageMeta = _db.PageMetas.FirstOrDefault(d=>d.ModelType == (short)ModelType.PAGE && d.ObjectId == page.Id.ToString());
            return View(page);
        }

        public ActionResult Paragraph(string seoName)
        {
            var page = _db.PageSets.FirstOrDefault(d => d.Active && d.SeoName == seoName);
     
            return PartialView("_Paragraph", page);
        }

    }
}