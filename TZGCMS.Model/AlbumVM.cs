using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{
    public class AlbumListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<AlbumVM> Albums { get; set; }
    }
    
    public class AlbumVM
    {
        public int Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 100)
        public string Cover { get; set; }
        public string Banner { get; set; } 
        public int Importance { get; set; } 
        public bool Active { get; set; } // Active
        public DateTime CreatedDate { get; set; } // CreatedDate
        public string CreatedBy { get; set; } // CreatedBy (length: 50)
    }
    public class AlbumIM
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Cover")]
        public string Cover { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Banner")]
        public string Banner { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; } 
    
    }


    public class PhotoListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<PhotoVM> Photos { get; set; }
        public int? AlbumId { get; set; }
        public string Keyword { get; set; }

    }

    public class PhotoVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AlbumId { get; set; }
        public string AlbumName { get; set; }
        public string FullImageUrl { get; set; } 
        public string Thumbnail { get; set; } 
        public int Importance { get; set; }
        public bool Active { get; set; }
        public DateTime  CreatedDate { get; set; }
    }
    public class PhotoIM
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Album")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int AlbumId { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "FullImageUrl")]
        public string FullImageUrl { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        public string Thumbnail { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }
    }
}
