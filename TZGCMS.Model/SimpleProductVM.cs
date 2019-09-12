using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class SimpleProductListVM
    {

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<SimpleProductVM> Products { get; set; }
        public string Keyword { get; set; }


    }

    public partial class SimpleProductVM
    {
      
        public int Id { get; set; }
        public string ProductName { get; set; }    
        public string ProductNo { get; set; }   
        public int ViewCount { get; set; }
        public int Importance { get; set; }     
        public string Thumbnail { get; set; }     
        public string ImageUrl { get; set; }
        public bool Active { get; set; }
        public bool Recommend { get; set; }
        public string Summary { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class SimpleProductIM
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ProductNo")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string ProductNo { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ProductName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string ProductName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Content")]
        [AllowHtml]
        public string Body { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Parameters")]
        [AllowHtml]
        public string Parameters { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Specific")]
        [AllowHtml]
        public string Specific { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Controls")]
        [AllowHtml]
        public string Controls { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Video")]
        [AllowHtml]
        public string Videos { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Summary")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Summary { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Thumbnail { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "ImageURL")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string ImageUrl { get; set; }

        [Display(Name = "显示")]
        public bool Active { get; set; }
        [Display(Name = "推荐")]
        public bool Recommend { get; set; }

        public int ViewCount { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Title { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        [MaxLength(200, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        [MaxLength(200, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string SEODescription { get; set; }



    }
}
