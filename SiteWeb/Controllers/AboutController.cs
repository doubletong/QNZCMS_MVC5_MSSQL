using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Controllers
{
    public class AboutController : BaseController
    {
      
        // GET: About
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.MENU && d.ObjectId == url);

            return View();
        }
    }
}