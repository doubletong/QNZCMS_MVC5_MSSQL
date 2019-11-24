using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model;
using TZGCMS.Model.Search;

namespace TZGCMS.SiteWeb.Controllers
{
    public class SearchController : Controller
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public SearchController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        // GET: Lucene 全站搜索 Search
        public async Task<ActionResult> Index(string keyword)
        {
            var vm = new List<ArticleVM>();
          
            var query = _db.Articles.Where(d => d.Active && (d.ArticleCategory.SeoName == "news" || d.ArticleCategory.SeoName == "media" || d.ArticleCategory.SeoName == "notice")).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword) || d.Summary.Contains(keyword));
                vm = await query.AsNoTracking().ProjectTo<ArticleVM>().ToListAsync();
                ViewData["Keyword"] = keyword;
                return View(vm);

            }


            return View();

        }

        //// GET: Lucene 全站搜索 Search
        //public ActionResult Index(string searchTerm, int? page)
        //{
        //    var vm = new SearchListVM()
        //    {
        //        SearchTerm = searchTerm,
        //        //  SearchField = searchField,
        //        //  SearchDefault = searchDefault ?? false,
        //        PageIndex = (page ?? 1) - 1,
        //        PageSize = SettingsManager.Lucene.PageSize
        //    };
        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        // create default Lucene search index directory
        //        if (!Directory.Exists(LuceneHelper._luceneDir)) Directory.CreateDirectory(LuceneHelper._luceneDir);

        //        int totalCount = 0;
        //        // perform Lucene search
        //        List<SearchData> _searchResults;
        //        _searchResults = LuceneHelper.SearchDefault(vm.PageIndex, vm.PageSize, out totalCount, searchTerm).ToList();



        //        vm.TotalCount = totalCount;
        //        vm.SearchIndexData = new StaticPagedList<SearchData>(_searchResults, vm.PageIndex + 1, vm.PageSize, vm.TotalCount); ;
        //    }



        //    return View(vm);

        //}
    }
}