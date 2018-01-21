using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Service.Chronicles;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Controllers
{

    public class ChronicleController : BaseController
    {
        private readonly IChronicleServices _chronicleService;
        private readonly IPageMetaServices _pageMetaService;
        public ChronicleController(IChronicleServices chronicleService,IPageMetaServices pageMetaService)
        {
            _chronicleService = chronicleService;
                _pageMetaService = pageMetaService;
        }
        // GET: Chronicle
        public ActionResult Index(int? year)
        {
            var url = Request.RawUrl;
            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);
            if (year == null)
            {
                var cyear = DateTime.Now.Year;
                var chs = _chronicleService.GetActiveByYear(cyear);
                ViewData["Year"] = cyear;
                if (!chs.Any())
                {
                    var vm = _chronicleService.GetActiveByYear(cyear - 1);
                    ViewData["Year"] = cyear - 1;
                    return View(vm);
                }
                return View(chs);
            }

            var vm1 = _chronicleService.GetActiveByYear(year.Value);
            ViewData["Year"] = year;
            return View(vm1);

        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            //  ArticleDetailFVM dvm = new ArticleDetailFVM();
            var article = _chronicleService.GetById(id);
            article.ViewCount++;
            _chronicleService.Update(article);

            //var prev = _chronicleService.GetActiveElements().Where(s => s.Active && s.Id < id).OrderByDescending(s => s.Id).FirstOrDefault();
            //if (prev != null)
            //{
            //    ViewBag.Prev = prev.Id;
            //}

            //var next = _chronicleService.GetActiveElements().Where(s => s.Active && s.Id > id).OrderBy(s => s.Id).FirstOrDefault();
            //if (next != null)
            //{
            //    ViewBag.Next = next.Id;
            //}

            //ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.ARTICLE, id.ToString());

            return View(article);
        }
    }
}