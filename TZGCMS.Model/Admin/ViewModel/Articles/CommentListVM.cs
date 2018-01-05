using PagedList;
using TZGCMS.Data.Entity.Articles;

namespace TZGCMS.Model.Admin.ViewModel.Articles
{
    public class CommentListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Comment> Comments { get; set; }    
        public int? ArticleId { get; set; }
        public string  Keyword { get; set; }

    }
}
