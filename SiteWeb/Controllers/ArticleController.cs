using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Front.ViewModel.Articles;
using TZGCMS.Service.Articles;
using TZGCMS.Service.PageMetas;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ArticleController : BaseController
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
        [SIGActionFilter]
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

            vm.Articles = new StaticPagedList<Article>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); 

            //vm.Categories = _categoryService.GetActiveItems().OrderByDescending(c => c.Importance);
            //var categoryList = _categoryService.GetActiveItems().OrderByDescending(c => c.Importance).ToList();
            //var categories = new SelectList(categoryList, "Id", "Title");
            //ViewBag.Categories = categories;

            var url = Request.RawUrl;
            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);

            return View(vm);
        }

        //public ActionResult Search(string keyword, int? page)
        //{

        //    var vm = new ArticleListVM
        //    {
        //        Keyword = keyword,
        //        PageIndex = page ?? 1,
        //        PageSize = SettingsManager.Article.FrontPageSize
        //    };
        //    int totalCount;
        //    var list = _articleServices.GetActivePagedElements(vm.PageIndex - 1, vm.PageSize, vm.Keyword, 0, out totalCount);
        //    //var categoryVMList = _mapper.Map<List<Article>, List<ArticleVM>>(goodslist);
        //    vm.TotalCount = totalCount;

        //    vm.Articles = new StaticPagedList<Article>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);

      

        //    var url = Request.RawUrl;
        //    ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);

        //    return View(vm);
        //}

        //[HttpGet]
        //public PartialViewResult RecentNews(string seoName, int count)
        //{
        //    var articleList = _articleServices.RecentNews(seoName, count);           
        //    return PartialView(articleList);
        //}

        //[HttpGet]
        //public PartialViewResult HotNews(int count)
        //{
        //    var articleList = _articleServices.HotNews(count);
        //    return PartialView(articleList);
        //}

        //[HttpGet]
        //public PartialViewResult HomeNews(string seoName, int count)
        //{
        //    var articleList = _articleServices.RecentNews(seoName, count);
        //    return PartialView(articleList);
        //}
        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {

            var model = await _db.Article.FindAsync(id);
            if (model == null) return HttpNotFound();

            model.ViewCount++;
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.ARTICLE && d.ObjectId == id.ToString());

            var prev = _db.Article.Where(s => s.Active && s.Id < id).OrderByDescending(s => s.Id).FirstOrDefault();
            if (prev != null)
            {
                ViewBag.Prev = prev;
            }

            var next = _db.Article.Where(s => s.Active && s.Id > id).OrderBy(s => s.Id).FirstOrDefault();
            if (next != null)
            {
                ViewBag.Next = next;
            }

            return View(model);

          
        }
    }
}