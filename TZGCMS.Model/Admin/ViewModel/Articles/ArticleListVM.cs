using PagedList;
using TZGCMS.Data.Entity.Articles;

namespace TZGCMS.Model.Admin.ViewModel.Articles
{
    public class ArticleListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Article> Articles { get; set; }    
        public int? CategoryId { get; set; }
        public string  Keyword { get; set; }

    }
}
