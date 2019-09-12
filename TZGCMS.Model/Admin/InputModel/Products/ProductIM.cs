using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Products
{
    public class ProductIM
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ProductNo")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
      
        public string ProductNo { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ProductName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string ProductName { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Description")]
        [AllowHtml]
        public string Body { get; set; }
     
        [Display(ResourceType = typeof(Labels), Name = "Summary")]
        public string Summary { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        public string Thumbnail { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Package")]
        public string Cover { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ImageURL")]
        public string ImageUrl { get; set; }

        [Display(Name = "显示")]
        public bool Active { get; set; }
        [Display(Name = "推荐")]
        public bool Recommend { get; set; }

        public int ViewCount { get; set; }

        //public string CategoryIds { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Category")]
        public string[] PostCategoryIds { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string Title { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }



    }
}
