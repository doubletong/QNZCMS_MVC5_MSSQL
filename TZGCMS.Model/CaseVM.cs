using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TZGCMS.Data.Entity;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class CaseListVM
    {
      
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public StaticPagedList<Case> Cases { get; set; }
            public string Keyword { get; set; }

        
    }
    public class CaseListFrontVM
    {

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<CaseVM> Cases { get; set; }
   
    }

    public class CaseVM
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public DateTime Pubdate { get; set; }
        public string Summary { get; set; }

    }
    public class CaseIM {
     
        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Content")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [AllowHtml]
        public string Body { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Summary")]
        public string Summary { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        //[Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Thumbnail { get; set; }



        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }
 

        [Display(ResourceType = typeof(Labels), Name = "Pubdate")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public DateTime Pubdate { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }
    }
}
