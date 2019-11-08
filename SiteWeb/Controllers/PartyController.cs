using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model;

namespace TZGCMS.SiteWeb.Controllers
{
    public class PartyController : BaseController
    {
        private readonly IMapper _mapper;

        public PartyController(IMapper mapper)
        {
            _mapper = mapper;
        }
        // GET: Party
        public async Task<ActionResult> Index()
        {
            var vm = new FrontArticlePageVM
            {

                NewsList = await _db.Article.Where(d => d.ArticleCategory.SeoName == "zznews" && d.Active)
                 .OrderByDescending(d => d.Pubdate).Select(d => new ArticleVM
                 {
                     Id = d.Id,
                     Title = d.Title,
                     Thumbnail = d.Thumbnail,
                     Summary = d.Summary,
                     Pubdate = d.Pubdate
                 }).Take(5).ToListAsync(),
                MediaList = await _db.Article.Where(d => d.ArticleCategory.SeoName == "central_spirit" && d.Active)
                 .OrderByDescending(d => d.Pubdate).Select(d => new ArticleVM
                 {
                     Id = d.Id,
                     Title = d.Title,
                     Thumbnail = d.Thumbnail,
                     Summary = d.Summary,
                     Pubdate = d.Pubdate
                 }).Take(5).ToListAsync(),
                NoticeList = await _db.Article.Where(d => d.ArticleCategory.SeoName == "theoretical" && d.Active)
                 .OrderByDescending(d => d.Pubdate).Select(d => new ArticleVM
                 {
                     Id = d.Id,
                     Title = d.Title,
                     Thumbnail = d.Thumbnail,
                     Summary = d.Summary,
                     Pubdate = d.Pubdate
                 }).Take(5).ToListAsync()
            };

            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.MENU && d.ObjectId == url);

            return View(vm);
        }
        [Route("List/{alias}")]
        public async Task<ActionResult> List(string alias, int? page, int? year)
        {
            var category = await _db.ArticleCategory.FirstOrDefaultAsync(d => d.SeoName == alias);
            var query = _db.Article.Where(d => d.Active && d.ArticleCategory.SeoName == alias).AsQueryable();

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
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.MENU && d.ObjectId == url);

            return View(vm);
        }

        public PartialViewResult RecommendArticle(int count)
        {
            var NewsList = _db.Article.Where(d => d.ArticleCategory.SeoName == "zznews" && d.Active && d.Recommend)
                .OrderByDescending(d => d.Pubdate).Select(d => new ArticleVM
                {
                    Id = d.Id,
                    Title = d.Title,
                    Thumbnail = d.Thumbnail,
                    Summary = d.Summary,
                    Pubdate = d.Pubdate
                }).Take(count).ToList();
            return PartialView("_RecommendArticle", NewsList);
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {

            var model = await _db.Article.Include(d => d.ArticleCategory).FirstOrDefaultAsync(d => d.Id == id);
            if (model == null) return HttpNotFound();

            model.ViewCount++;
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.ARTICLE && d.ObjectId == id.ToString());

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