using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Model;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IMapper _mapper;
        public ProductsController(IMapper mapper)
        {

            _mapper = mapper;
        }

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
                    

            var list = await _db.SimpleProducts.Where(d => d.Active)
                .OrderByDescending(d => d.Importance).ThenByDescending(d => d.Id)            
                .ProjectTo<SimpleProductVM>(_mapper.ConfigurationProvider).ToListAsync();

       

            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.MENU && d.ObjectId == url);

            return View(list);


        }
        // GET: Products
        //public async System.Threading.Tasks.Task<ActionResult> Index(int? page)
        //{

        //    var vm = new SimpleProductListVM
        //    {
        //        PageIndex = page ?? 1,
        //        PageSize = 6
        //    };

        //    var query = _db.SimpleProducts.Where(d => d.Active).AsQueryable();


        //    var list = await query.OrderByDescending(d => d.Importance).ThenByDescending(d => d.Id)
        //        .Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize)
        //        .ProjectTo<SimpleProductVM>(_mapper.ConfigurationProvider).ToListAsync();

        //    vm.TotalCount = await query.CountAsync();
        //    vm.Products = new StaticPagedList<SimpleProductVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);


        //    var url = Request.RawUrl;
        //    ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.MENU && d.ObjectId == url);

        //    return View(vm);


        //}

        public async Task<ActionResult> Detail(int id)
        {
            var product = await _db.SimpleProducts.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            product.ViewCount++;
            _db.Entry(product).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.SIMPLEPRODUCT && d.ObjectId == id.ToString());       

            return View(product);
        }

        [Route("products/train/{seoName}")]
        public async Task<ActionResult> Train(string seoName)
        {

            var page = await _db.Pages.FirstOrDefaultAsync(d => d.Active && d.SeoName == seoName);
            if (page == null)
                return HttpNotFound();

            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.PAGE && d.ObjectId == page.Id.ToString());

            return View(page);
            
        }
    }
}