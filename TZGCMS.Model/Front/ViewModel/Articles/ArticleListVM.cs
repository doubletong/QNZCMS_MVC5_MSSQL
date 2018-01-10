using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Articles;

namespace TZGCMS.Model.Front.ViewModel.Articles
{
    public class ArticleListVM
    {
        //public int CategoryId { get; set; }
        //public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Article> Articles { get; set; }
        //public PageMeta PageMeta { get; set; }
        //public IEnumerable<ArticleCategory> Categories { get; set; }
        //public ArticleCategory CurrentCategory { get; set; }
    }
}
