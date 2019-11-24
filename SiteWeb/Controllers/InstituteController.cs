using AutoMapper;
using QNZ.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using TZGCMS.Data.Enums;
using TZGCMS.Model;

namespace TZGCMS.SiteWeb.Controllers
{
    public class InstituteController : Controller
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public InstituteController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: Platform
        [OutputCache(Duration = 600, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> Index(int? did)
        {
            var query = _db.Institutes.Where(d => d.Active).AsQueryable();
            if (did != null)
            {
                query = query.Where(d => d.DictionaryId == did);
            }
            var vm = await query.OrderByDescending(d => d.Importance).ToListAsync();

            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);

            return View(vm);
        }


        public async Task<ActionResult> Labs(int id)
        {
            var vm = new LaboratoryPagedVM
            {
                Institute = await _db.Institutes.FirstOrDefaultAsync(d => d.Id == id),
                Laboratories = await _db.Laboratories.Where(d => d.Active && d.InstituteId == id).OrderByDescending(d => d.Importance).ToListAsync()
            };

                    
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.INSTITUTE && d.ObjectId == id.ToString());

            return View(vm);
        }

        public async Task<ActionResult> Detail(int id)
        {
            var model = await _db.Institutes.FindAsync(id);
            if (model == null)
                return HttpNotFound();

            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.INSTITUTE && d.ObjectId == id.ToString());

            return View(model);
        }


        public ActionResult InstituteHeader()
        {
            var vm = _db.Dictionaries.Where(d => d.TypeId == 1).OrderByDescending(d=>d.Importance).ToList();           

            return PartialView("_InstituteHeader",vm);
        }
    }
}