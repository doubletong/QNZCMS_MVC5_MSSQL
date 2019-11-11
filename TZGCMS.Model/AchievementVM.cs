using PagedList;
using QNZ.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class AchievementCategoryListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<AchievementCategoryVM> Categories { get; set; }
    }
    public class AchievementCategoryVM
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Thumbnail { get; set; } // Thumbnail (length: 150)
        public System.DateTime CreatedDate { get; set; } // Pubdate
        public int Importance { get; set; } // ViewCount
        public bool Active { get; set; } // Active
    }
    public class AchievementCategoryIM
    {

        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }


        //[Display(ResourceType = typeof(Labels), Name = "SeoName")]
        //[Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        //[RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormatSEOUrl")]
        //[Remote("IsSeoNameUnique", "AchievementCategories", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        //public string Alias { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        public string Thumbnail { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }
    }

    


    public class AchievementListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<AchievementVM> Achievements { get; set; }
    }

    public class AchievementPagedVM
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Achievement> Achievements { get; set; }
    }
    public class AchievementVM
    {
        public int Id { get; set; }
      
        public string Title { get; set; }
        public string Thumbnail { get; set; } // Thumbnail (length: 150)
        public System.DateTime? Pubdate { get; set; } // Pubdate
        public int ViewCount { get; set; } // ViewCount
        public bool Active { get; set; } // Active
    }
    public class AchievementIM
    {

        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string Title { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        public string Thumbnail { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Content")]
        public string Body { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Pubdate")]
        public DateTime  Pubdate { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Category")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int CategoryId { get; set; }


    }
}
