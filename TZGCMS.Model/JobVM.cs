using PagedList;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class JobListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<JobVM> Jobs { get; set; }
    }

    public class JobVM
    {
        public int Id { get; set; } // Id (Primary key)
        public string Post { get; set; } // Post (length: 100)   
        public int Importance { get; set; } // Importance
        public string Category { get; set; } // Category (length: 50)
        public int? Quantity { get; set; } // Quantity
        public string Address { get; set; } // Address (length: 150)   
        public bool Active { get; set; } // Active
        public System.DateTime CreatedDate { get; set; } // CreatedDate
        public string CreatedBy { get; set; } // CreatedBy (length: 50)

    }

    public class FrontJobVM
    {
        public int Id { get; set; } // Id (Primary key)
        public string Post { get; set; } // Post (length: 100)   
        public int Importance { get; set; } // Importance
        public string Category { get; set; }
        public string Description { get; set; } 
        public int? Quantity { get; set; } // Quantity
        public string Address { get; set; } // Address (length: 150)   
        public bool Active { get; set; } // Active
        public System.DateTime CreatedDate { get; set; } // CreatedDate
        public string CreatedBy { get; set; } // CreatedBy (length: 50)

        public string GroupName { get; set; } 

    }

    public class JobIM
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "Post")]
        public string Post { get; set; }
        //[Display(ResourceType = typeof(Labels), Name = "SeoName")]
        //[Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        //[RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormatSEOUrl")]
        //[Remote("IsSeoNameUnique", "Job", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        //public string SeoName { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string Description { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "RangeInt")]
        public int Importance { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "WorkType")]
        public string Category { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Institute")]
        public string Address { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }
    }

    public class JobPagedVM
    {
        public IEnumerable<Dictionary> Dictionaries { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<FrontJobVM> Jobs { get; set; }     
        public string Category { get; set; }
    }
}
