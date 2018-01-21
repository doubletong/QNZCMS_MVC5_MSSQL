using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Doc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Model.Front.ViewModel.Doc;
using TZGCMS.Service.Doc;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Controllers
{
    public class DownloadController : BaseController
    {
        private readonly IDocumentCategoryServices _categoryService;
        private readonly IDocumentServices _docService;
        private readonly ILoggingService _logger;
        private readonly IPageMetaServices _pageMetaService;
        public DownloadController(IDocumentServices docService,IDocumentCategoryServices categoryService,IPageMetaServices pageMetaService, ILoggingService logger)
        {
            _docService = docService;
            _categoryService = categoryService;
            _pageMetaService = pageMetaService;
            _logger = logger;
        }
        // GET: Download
        public ActionResult Index(int? page, int? cateId)
        {
            var vm = new DocumentListFVM
            {
                CateId = cateId??0,
                PageSize = SettingsManager.Doc.PageSize,
                PageIndex = page??1,
                Categories = _categoryService.GetAll().Where(d=>d.Active).OrderByDescending(d=>d.Importance)
            };
            int totalCount;
            var list = _docService.GetActivePagedElements(vm.PageIndex - 1, vm.PageSize, string.Empty, vm.CateId, out totalCount);
            //var categoryVMList = _mapper.Map<List<Article>, List<ArticleVM>>(goodslist);
            vm.TotalCount = totalCount;

            vm.Documents = new StaticPagedList<Document>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);
            var url = Request.RawUrl;
            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);
            return View(vm);
        }

        public ActionResult Detail(int id)
        {           
            var doc = _docService.GetById(id);         

            if (doc == null)
                return HttpNotFound();

            doc.DownloadCount++;
            _docService.Update(doc);
          
            return Redirect(doc.FilePath);
        }


        //public PartialViewResult DocForProduct(string productNo)
        //{
        //    // var docs = _unit.Documents.FindBy(d => d.ProductIds.Split(',').Contains(productNo));
        //    var docs = _unit.Documents.FindBy(d => !string.IsNullOrEmpty(d.ProductIds)).ToList()
        //        .Where(d => d.ProductIds.Split(',').Contains(productNo));

        //    var DocumentList = Mapper.Map<IEnumerable<DocumentDTO>>(docs);
        //    return PartialView("_DocForProduct", DocumentList);
        //}
    }
}