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
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ArticleController : Controller
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public ArticleController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }
        [SIGActionFilter]
        public async Task<ActionResult> Index()
        {

            var vm = new FrontArticlePageVM
            {
              
                NewsList = await _db.Articles.Where(d=>d.ArticleCategory.SeoName == "news").Take(5)
                .OrderByDescending(d=>d.Pubdate).Select(d=> new ArticleVM
                {
                    Id = d.Id,
                    Title = d.Title,
                    Thumbnail = d.Thumbnail,
                    Summary = d.Summary,
                    Pubdate = d.Pubdate
                }).ToListAsync(),
                MediaList = await _db.Articles.Where(d => d.ArticleCategory.SeoName == "media").Take(5)
                .OrderByDescending(d => d.Pubdate).Select(d => new ArticleVM
                {
                    Id = d.Id,
                    Title = d.Title,
                    Thumbnail = d.Thumbnail,
                    Summary = d.Summary,
                    Pubdate = d.Pubdate
                }).ToListAsync(),
                NoticeList = await _db.Articles.Where(d => d.ArticleCategory.SeoName == "notice").Take(5)
                .OrderByDescending(d => d.Pubdate).Select(d => new ArticleVM
                {
                    Id = d.Id,
                    Title = d.Title,
                    Thumbnail = d.Thumbnail,
                    Summary = d.Summary,
                    Pubdate = d.Pubdate
                }).ToListAsync()
            };

            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetaSets.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);

            return View(vm);
        }
        public async Task<ActionResult> List(string alias, int? page,int? year)
        {
            var category = await _db.ArticleCategories.FirstOrDefaultAsync(d => d.SeoName == alias);
            var query = _db.Articles.Where(d => d.Active==true && d.ArticleCategory.SeoName == alias).AsQueryable();

            var vm = new FrontArticleListVM
            {
                Alias = alias,
                Years = await query.Select(d => d.Pubdate.Year).OrderByDescending(d=>d).Distinct().ToListAsync(),
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
            ViewBag.PageMeta = await _db.PageMetaSets.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);

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


        [HttpGet]
        public PartialViewResult ArtHeader()
        {
            //var vm = _db.ArticleCategory.Where(d => d.Active)
            //    .OrderByDescending(d => d.Importance)
            //    .Select(d => new ArticleCategoryFVM
            //    {
            //        Id = d.Id,
            //        Title = d.Title,
            //        Alias = d.SeoName
            //    }).ToList();
            //return PartialView("_ArtHeader", vm);
            return PartialView("_ArtHeader");
        }


        [HttpGet]
        public PartialViewResult RecentNews(int count)
        {
            var articleList = _db.Articles.Where(d=>d.Active==true && (d.ArticleCategory.SeoName == "news" || d.ArticleCategory.SeoName == "media" || d.ArticleCategory.SeoName == "notice"))
                .OrderByDescending(d=>d.Pubdate).Take(count).ProjectTo<ArticleVM>().ToList();
            return PartialView("_RecentNews", articleList);
        }

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

            var model = await _db.Articles.Include(d=>d.ArticleCategory).FirstOrDefaultAsync(d=>d.Id == id);
            if (model == null) return HttpNotFound();

            model.ViewCount++;
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            ViewBag.PageMeta = await _db.PageMetaSets.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.ARTICLE && d.ObjectId == id.ToString());

            var prev = _db.Articles.Where(s => s.Active==true && s.Id < id).OrderByDescending(s => s.Id).FirstOrDefault();
            if (prev != null)
            {
                ViewBag.Prev = prev;
            }

            var next = _db.Articles.Where(s => s.Active==true && s.Id > id).OrderBy(s => s.Id).FirstOrDefault();
            if (next != null)
            {
                ViewBag.Next = next;
            }

            return View(model);

          
        }
    }
}