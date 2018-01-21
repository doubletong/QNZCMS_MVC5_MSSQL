using PagedList;
using TZGCMS.Data.Entity.Links;

namespace TZGCMS.Model.Admin.ViewModel.Links
{
    public class LinkCategoryListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<LinkCategory> Categories { get; set; }
    }
}
