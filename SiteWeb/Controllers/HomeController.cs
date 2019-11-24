using System.Web.Mvc;
using System.Data.Entity;
using TZGCMS.Data.Enums;
using TZGCMS.SiteWeb.Filters;
using System.Linq;
using TZGCMS.Model.Front;
using QNZ.Data;
using TZGCMS.Infrastructure.Cache;

namespace TZGCMS.SiteWeb.Controllers
{
    public class HomeController : BaseController
    {

        private readonly IQNZDbContext _db;
        private readonly ICacheService _cacheService;
        public HomeController(ICacheService cacheService, IQNZDbContext db)
        {
            _cacheService = cacheService;
            _db = db;
        }
        [SIGActionFilter] 
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var carousels = await _db.CarouselSets.Include(d=>d.PositionSet).Where(d => d.Active && (d.PositionSet.Code == "A1001" || d.PositionSet.Code == "A1002")).ToListAsync();

            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d=>d.ModelType ==(short)ModelType.MENU && d.ObjectId == url);

            var vm = new HomeVM
            {
                Carousel = carousels.FirstOrDefault(d => d.PositionSet.Code == "A1001"),
                Carousel2 = carousels.FirstOrDefault(d => d.PositionSet.Code == "A1002")
            };

            return View(vm);
        }


        public ActionResult CloseSite()
        {           
            return View();
        }
        

        
    }
}