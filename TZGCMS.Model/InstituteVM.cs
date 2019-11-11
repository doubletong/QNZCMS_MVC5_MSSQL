using PagedList;
using QNZ.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class InstituteListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<InstituteVM> Institutes { get; set; }
    }

    public class InstituteVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; } 
        public System.DateTime CreatedDate { get; set; }
        public int Importance { get; set; } 
        public bool Active { get; set; } 
    }

    public class InstituteIM
    {

        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Content")]    
        [AllowHtml]
        public string Introduction { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Category")]
        public int DictionaryId { get; set; }

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

    public class LaboratoryListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<LaboratoryVM> Laboratories { get; set; }
    }

    public class LaboratoryVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Importance { get; set; }
        public bool Active { get; set; }
    }

    public class LaboratoryIM
    {

        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Institute")]
        public int InstituteId { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

    }
    public class LaboratoryPagedVM
    {

        public Institute Institute { get; set; }
        public IEnumerable<Laboratory> Laboratories { get; set; }

    }


}
