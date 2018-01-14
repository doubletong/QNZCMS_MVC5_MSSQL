using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Model.Front.ViewModel.Products;
using TZGCMS.Service.PageMetas;
using TZGCMS.Service.Products;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductCategoryServices _categoryService;
        private readonly IProductServices _productService;
        private readonly ILoggingService _logger;
        private readonly IPageMetaServices _pageMetaService;
        public ProductController(IProductServices productService,IProductCategoryServices categoryService,IPageMetaServices pageMetaService, ILoggingService logger)
        {
            _categoryService = categoryService;
            _productService = productService;
            _pageMetaService = pageMetaService;
            _logger = logger;
        }
        // GET: Product
        public ActionResult Index(int? page, int? cateId)
        {
            var vm = new ProductListFVM
            {
                Categories = _categoryService.GetActiveItems(),
                CategoryId = cateId??0,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Product.PageSize
             };
        
            int totalCount;
            var list = _productService.GetActivePagedElements(vm.PageIndex -1 , vm.PageSize, string.Empty, vm.CategoryId, out totalCount);
         
            vm.TotalCount = totalCount;
            vm.Products = new StaticPagedList<Product>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);
           

            var url = Request.RawUrl;
            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);
            return View(vm);
        }

        public PartialViewResult HomeCategories(int count)
        {
            var cates = _categoryService.GetActiveItems().Take(count);
            return PartialView(cates);
        }

        public PartialViewResult HomeProducts(int count)
        {
            var cates = _productService.GetRecommendElements(count);
            return PartialView(cates);
        }

        public ActionResult Detail(int id)
        {
            var product = _productService.GetById(id);
            product.ViewCount++;
            _productService.Update(product);

            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.PRODUCT, id.ToString());

            return View(product);
        }
    }
}