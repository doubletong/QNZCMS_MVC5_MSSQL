using System.Web.Mvc;
using System.Data.Entity;
using TZGCMS.Data.Enums;
using TZGCMS.SiteWeb.Filters;
using System.Linq;
using TZGCMS.Model.Front;
using QNZ.Data;
using TZGCMS.Infrastructure.Cache;
using System.Threading.Tasks;

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
        public async Task<ActionResult> Index()
        {
        
            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d=>d.ModelType ==(short)ModelType.MENU && d.ObjectId == url);

           

            return View();
        }


        public ActionResult CloseSite()
        {           
            return View();
        }
        

        
    }
}