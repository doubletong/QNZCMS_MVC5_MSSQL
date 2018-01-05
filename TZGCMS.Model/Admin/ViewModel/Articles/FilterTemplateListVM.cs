using PagedList;

namespace TZGCMS.Model.Admin.ViewModel.Articles
{
    public class FilterTemplateListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<FilterTemplateVM> FilterTemplates { get; set; }
    }
}
