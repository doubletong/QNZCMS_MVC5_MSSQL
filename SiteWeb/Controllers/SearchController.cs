using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model;
using TZGCMS.Model.Search;

namespace TZGCMS.SiteWeb.Controllers
{
    public class SearchController : BaseController
    {
        // GET: Search
        public ActionResult Index(string searchTerm, int? page)
        {
            var vm = new SearchListVM()
            {
                SearchTerm = searchTerm,
                //  SearchField = searchField,
                //  SearchDefault = searchDefault ?? false,
                PageIndex = (page ?? 1) - 1,
                PageSize = SettingsManager.Lucene.PageSize
            };
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // create default Lucene search index directory
                if (!Directory.Exists(LuceneHelper._luceneDir)) Directory.CreateDirectory(LuceneHelper._luceneDir);

                int totalCount = 0;
                // perform Lucene search
                List<SearchData> _searchResults;
                _searchResults = LuceneHelper.SearchDefault(vm.PageIndex, vm.PageSize, out totalCount, searchTerm).ToList();

            

                vm.TotalCount = totalCount;
                vm.SearchIndexData = new StaticPagedList<SearchData>(_searchResults, vm.PageIndex + 1, vm.PageSize, vm.TotalCount); ;
            }
            


            return View(vm);

        }
    }
}