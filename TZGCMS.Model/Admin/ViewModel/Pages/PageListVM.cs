using PagedList;
using TZGCMS.Data.Entity.Pages;

namespace TZGCMS.Model.Admin.ViewModel.Pages
{
    public class PageListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Page> Pages { get; set; }
    }
}
