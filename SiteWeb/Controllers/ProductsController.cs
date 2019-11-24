using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Model;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    //[RoutePrefix("products")]
    public class ProductsController : BaseController
    {
        private readonly IQNZDbContext _db;
        private readonly ICacheService _cacheService; 
        private readonly IMapper _mapper;

        public ProductsController(IMapper mapper, ICacheService cacheService, IQNZDbContext db)
        {
            _cacheService = cacheService;
            _db = db;
            _mapper = mapper;
        }
   
        //private readonly IProductCategoryServices _categoryService;
        //private readonly IProductServices _productService;
        //private readonly ILoggingService _logger;
        //private readonly IPageMetaServices _pageMetaService;
        //public ProductsController(IProductServices productService,IProductCategoryServices categoryService,IPageMetaServices pageMetaService, ILoggingService logger)
        //{
        //    _categoryService = categoryService;
        //    _productService = productService;
        //    _pageMetaService = pageMetaService;
        //    _logger = logger;
        //}
        // GET: Product
        [SIGActionFilter]
        [Route("products")]
        [Route("products/page-{page}", Name = "pageProducts")]
        [Route("products/category-{seoName}", Name = "caegoryProducts")]
        [Route("products/category-{seoName}/page-{page}", Name = "caegoryPageProducts")]
       
        public async Task<ActionResult> Index(int? page, string seoName= "pu-erh")
        {
            var category = await _db.ProductCategories.OrderByDescending(d => d.Importance).ThenByDescending(d => d.CreatedDate).FirstOrDefaultAsync(d => d.Active && d.SeoName == seoName);
            if (category == null)
            {
                return HttpNotFound();
            }
            var vm = new ProductListFVM
            {
                Category = _mapper.Map<ProductCategoryVM>(category),
                SeoName = seoName,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Product.PageSize
             };

            var query = _db.Products.AsNoTracking().Where(d=>d.Active==true).AsQueryable();
            if (!string.IsNullOrEmpty(seoName))
            {
                query = query.Where(d => d.ProductCategories.Any(c=>c.SeoName == seoName));
            }
        
           
            var list = await query.OrderByDescending(d=>d.Importance).ThenByDescending(d=>d.CreatedDate).Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ProjectTo<ProductVM>().ToListAsync();
         
            vm.TotalCount = await query.CountAsync();
            vm.Products = new StaticPagedList<ProductVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);
           

          // var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d=>d.ModelType == (short)ModelType.CATEGORY && d.ObjectId == category.Id.ToString());
            return View(vm);
        }

        //public PartialViewResult HomeCategories(int count)
        //{
        //    var cates = _categoryService.GetActiveItems().Take(count);
        //    return PartialView(cates);
        //}

        //public PartialViewResult HomeProducts(int count)
        //{
        //    var cates = _productService.GetRecommendElements(count);
        //    return PartialView(cates);
        //}

        public async Task<ActionResult> Detail(int id)
        {
            var product = await _db.Products.FindAsync(id);
            product.ViewCount++;

            _db.Entry(product).State = EntityState.Modified;
            await _db.SaveChangesAsync();
       

            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d=>d.ModelType == (short)ModelType.PRODUCT && d.ObjectId == id.ToString());

            return View(product);
        }
    }
}