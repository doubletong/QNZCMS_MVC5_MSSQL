using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Controllers
{
    public class CustomizeController : BaseController
    {
        private readonly ILoggingService _logger;
        private readonly IPageMetaServices _pageMetaService;
        public CustomizeController(IPageMetaServices pageMetaService, ILoggingService logger)
        {
            _pageMetaService = pageMetaService;
            _logger = logger;
        }
        // GET: About
        public ActionResult Index()
        {
            var url = Request.RawUrl;
            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);

            return View();
        }
    }
}