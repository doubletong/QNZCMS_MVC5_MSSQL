using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ServicesController : BaseController
    {

        private readonly IQNZDbContext _db;
        private readonly ICacheService _cacheService;
        public ServicesController(ICacheService cacheService, IQNZDbContext db)
        {
            _cacheService = cacheService;
            _db = db;
        }

        // GET: Services
        [SIGActionFilter]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);


            return View();
        }
    }
}