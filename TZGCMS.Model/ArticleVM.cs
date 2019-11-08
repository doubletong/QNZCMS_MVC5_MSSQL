using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Articles;

namespace TZGCMS.Model
{
    public class ArticleVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int ViewCount { get; set; }
        public string CategoryTitle { get; set; }
        public string Thumbnail { get; set; }
        public bool Active { get; set; }
        public DateTime Pubdate { get; set; }
        public string CreatedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }
        public bool Recommend { get; set; }
    }




    public class ArticleListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Article> Articles { get; set; }
        public int? CategoryId { get; set; }
        public string Keyword { get; set; }

    }

    public class ArticleCategoryFVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
    public class FrontArticlePageVM
    {
     
        public IEnumerable<ArticleVM> NewsList { get; set; }
        public IEnumerable<ArticleVM> MediaList { get; set; }
        public IEnumerable<ArticleVM> NoticeList { get; set; }
        //public string Keyword { get; set; }
        //public int PageIndex { get; set; }
        //public int PageSize { get; set; }
        //public int TotalCount { get; set; }
        //public StaticPagedList<ArticleVM> Articles { get; set; }

    }
    public class FrontArticleListVM
    {
        public IEnumerable<int> Years { get; set; }
        public string CategoryTitle { get; set; }
        public string Alias { get; set; }
        public int? Year { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<ArticleVM> Articles { get; set; }

    }
}
