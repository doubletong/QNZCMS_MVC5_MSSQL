using PagedList;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{

    public class ArticleCategoryListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<ArticleCategory> Categories { get; set; }
    }

    public class ArticleCategoryVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SeoName { get; set; }
        public int Importance { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class ArticleCategoryIM
    {

        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "SeoName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormatSEOUrl")]
        [Remote("IsSeoNameUnique", "ArticleCategory", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        public string SeoName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }


    }
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
        public StaticPagedList<ArticleVM> Articles { get; set; }
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

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<ArticleVM> Articles { get; set; }
        public string Alias { get; set; }
        public IEnumerable<ArticleCategoryFVM> Categories { get; set; }
     

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
