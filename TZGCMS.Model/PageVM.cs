using PagedList;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class PageListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<PageVM> Pages { get; set; }
    }

    public class PageVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SeoName { get; set; }
        public bool Active { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class PageIM
    {

        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Content")]
        public string Body { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "SeoName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormatSEOUrl")]
        [Remote("IsSeoNameUnique", "Page", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        public string SeoName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "TemplateName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string TemplateName { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Style")]
        public string HeadCode { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Javascript")]
        public string FooterCode { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }

    }
}
