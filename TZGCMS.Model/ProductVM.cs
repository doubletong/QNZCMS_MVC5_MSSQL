using PagedList;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class ProductIM
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ProductNo")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string ProductNo { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ProductName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string ProductName { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Description")]
        [AllowHtml]
        public string Body { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Summary")]
        [MaxLength(500, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Summary { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Introduction")]
        [MaxLength(500, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Introduction { get; set; }
        
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Thumbnail { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Background")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Cover { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ProductImage")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string ViewImage { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Icon")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Icon { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ImageURL")]
        public string ImageUrl { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Active")]    
        public bool Active { get; set; }
    
        [Display(ResourceType = typeof(Labels), Name = "Recommend")]
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
    public class ProductListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public StaticPagedList<ProductVM> Products { get; set; }
        public int? CategoryId { get; set; }

        public string Keyword { get; set; }
    }

    public class ProductListFVM
    {
        public string SeoName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<ProductVM> Products { get; set; }
        public ProductCategoryVM Category { get; set; }
       
    }
    public class CategoryListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<ProductCategory> Categories { get; set; }
    }
    public class ProductCategoryVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IconName { get; set; } // IconName (length: 150)
        public string SeoName { get; set; } // SeoName (length: 50)
        public string ImageUrl { get; set; } 
        public string Description { get; set; }
        public int Importance { get; set; } // Importance
        public bool Active { get; set; } // Active
        public DateTime CreatedDate { get; set; }
        public int? ParentId { get; set; } // ParentId
        public IEnumerable<ProductCategoryVM> ProductCategories { get; set; } 
        

    }

    public class ProductCategoryIM
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "SeoName")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormatSEOUrl")]
        [Remote("IsSeoNameUnique", "ProductCategory", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        public string SeoName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string IconName { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ImageURL")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string ImageUrl { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ParentCategory")]
        public int? ParentId { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Description")]
        [MaxLength(500, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string SEOTitle { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string SEODescription { get; set; }
    }
    public class ProductVM
    {
        public int Id { get; set; }

        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string Summary { get; set; }
        public int Importance { get; set; } // Importance
        public string Thumbnail { get; set; }
        public string ViewImage { get; set; }
        public string ImageUrl { get; set; } // ImageUrl
        public int ViewCount { get; set; } // ViewCount
        public bool? Active { get; set; } // Active
        public bool? Recommend { get; set; } // Recommend
        public string CategoryTitle { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public IEnumerable<ProductCategoryVM> ProductCategories { get; set; }
    }

    public class ProductDetailVM
    {
        public int Id { get; set; }

        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string Thumbnail { get; set; }
        public string Body { get; set; }
        public string[] Photos { get; set; }
    }
}
