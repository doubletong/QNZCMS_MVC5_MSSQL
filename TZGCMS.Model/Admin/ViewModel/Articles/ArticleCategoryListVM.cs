using PagedList;
using QNZ.Data;

namespace TZGCMS.Model.Admin.ViewModel.Articles
{
    public class ArticleCategoryListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<ArticleCategory> Categories { get; set; }
    }
}
