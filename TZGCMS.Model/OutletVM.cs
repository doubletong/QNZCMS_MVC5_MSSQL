using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Model.Validation;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class OutletVM
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public string Address { get; set; } // Address (length: 200)
        public string Coordinate { get; set; } // Coordinate (length: 50)
        public string ContactMan { get; set; } // ContactMan (length: 50)
        public string Phone { get; set; } // Phone (length: 50)
        public string Thumbnail { get; set; } // Thumbnail (length: 150)
        public int Importance { get; set; } // Importance
        public bool Active { get; set; } // Active
        public System.DateTime CreatedDate { get; set; } // CreatedDate
        public string CreatedBy { get; set; } // CreatedBy (length: 50)
     
    }
    public class OutletListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<OutletVM> Outlets { get; set; }
    }

    public class OutletIM
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Labels), Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(200, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Labels), Name = "Address")]
        public string Address { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Labels), Name = "Coordinate")]
        [Coordinate]
        public string Coordinate { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ContactMan")]
        public string ContactMan { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        public string Thumbnail { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Phone")]
        public string Phone { get; set; }
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "ValidNumber")]
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        public bool Active { get; set; }
    }
}
