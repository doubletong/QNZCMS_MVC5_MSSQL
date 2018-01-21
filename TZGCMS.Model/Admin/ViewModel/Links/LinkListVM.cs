using PagedList;
using TZGCMS.Data.Entity.Links;

namespace TZGCMS.Model.Admin.ViewModel.Links
{
    public class LinkListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Link> Links { get; set; }    
        public int? CategoryId { get; set; }
        public string  Keyword { get; set; }

    }
}
