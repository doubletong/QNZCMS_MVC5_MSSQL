using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using QNZ.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model;

namespace TZGCMS.SiteWeb.Controllers
{
    public class CooperationController : Controller
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public CooperationController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }
        // GET: Cooperation
        public ActionResult Index()
        {
            return View();
        }

        
        public async Task<ActionResult> List(string alias, int? page, int? year)
        {
            var category = await _db.ArticleCategories.FirstOrDefaultAsync(d => d.SeoName == alias);
            var query = _db.Articles.Where(d => d.Active && d.ArticleCategory.SeoName == alias).AsQueryable();

            var vm = new FrontArticleListVM
            {
                Alias = alias,
                Years = await query.Select(d => d.Pubdate.Year).OrderByDescending(d => d).Distinct().ToListAsync(),
                Year = year,
                CategoryTitle = category?.Title,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Article.FrontPageSize
            };

            if (year != null)
            {
                query = query.Where(d => d.Pubdate.Year == year);
            }

            var list = await query.OrderByDescending(d => d.Pubdate)
                .ThenByDescending(d => d.Id).Skip((vm.PageIndex - 1) * vm.PageSize)
                .Take(vm.PageSize).ProjectTo<ArticleVM>(_mapper.ConfigurationProvider).ToListAsync();

            vm.TotalCount = await query.CountAsync();
            vm.Articles = new StaticPagedList<ArticleVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);


            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);

            return View(vm);
        }


        [HttpGet]
        public PartialViewResult RecentNews(string alias, int count)
        {
            var articleList = _db.Articles.Where(d => d.Active && d.ArticleCategory.SeoName == alias)
                .OrderByDescending(d => d.Pubdate).Take(count).ProjectTo<ArticleVM>().ToList();
            return PartialView("_RecentNews", articleList);
        }


        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {

            var model = await _db.Articles.Include(d => d.ArticleCategory).FirstOrDefaultAsync(d => d.Id == id);
            if (model == null) return HttpNotFound();

            model.ViewCount++;
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.ARTICLE && d.ObjectId == id.ToString());

            //var prev = _db.Article.Where(s => s.Active && s.Id < id).OrderByDescending(s => s.Id).FirstOrDefault();
            //if (prev != null)
            //{
            //    ViewBag.Prev = prev;
            //}

            //var next = _db.Article.Where(s => s.Active && s.Id > id).OrderBy(s => s.Id).FirstOrDefault();
            //if (next != null)
            //{
            //    ViewBag.Next = next;
            //}

            return View(model);


        }
    }
}