using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Front.ViewModel.Articles;
using TZGCMS.Service.Articles;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleServices _articleServices;
        private IArticleCategoryServices _categoryService;
        private readonly IPageMetaServices _pageMetaService;
        public ArticleController(IArticleServices articleServices, IArticleCategoryServices categoryService, IPageMetaServices pageMetaService)
        {
            _articleServices = articleServices;
            _categoryService = categoryService;
            _pageMetaService = pageMetaService;
        }
        // GET: Article
        //public ActionResult Index()
        //{
        //    var vm = _articleServices.GetAll();
        //    return View(vm);
        //}
        public ActionResult Index(int? page)
        {

            var vm = new ArticleListVM
            {


                //if (categoryId != null)
                //{
                //    vm.CategoryId = (int)categoryId;
                //    vm.CurrentCategory = _categoryService.GetById(categoryId.Value);
                //}


                //vm.Keyword = keyword;
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Article.FrontPageSize
            };
            int totalCount;
            var list = _articleServices.GetActivePagedElements(vm.PageIndex-1, vm.PageSize,string.Empty, 0, out totalCount);
            //var categoryVMList = _mapper.Map<List<Article>, List<ArticleVM>>(goodslist);
            vm.TotalCount = totalCount;

            vm.Articles = new StaticPagedList<Article>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); ;

            //vm.Categories = _categoryService.GetActiveItems().OrderByDescending(c => c.Importance);
            //var categoryList = _categoryService.GetActiveItems().OrderByDescending(c => c.Importance).ToList();
            //var categories = new SelectList(categoryList, "Id", "Title");
            //ViewBag.Categories = categories;

            var url = Request.RawUrl;
            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);

            return View(vm);
        }
    }
}