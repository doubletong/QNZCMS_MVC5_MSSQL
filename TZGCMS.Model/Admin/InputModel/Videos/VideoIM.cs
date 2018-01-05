using System;
using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Videos
{
    public class VideoIM
    {
      
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "Title")]
     
        [StringLength(100, MinimumLength = 3, ErrorMessage = null, ErrorMessageResourceName = "StringLengthWithMiniLength", ErrorMessageResourceType = typeof(Validations))]
        public string Title { get; set; }
      
        [Display(ResourceType = typeof(Labels), Name = "Description")]
        [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Validations))]
        public string Summary { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Category")]
        public int CategoryId { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "VideoUrl")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]      
        public string VideoUrl { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "StartDate")]
        public DateTime StartDate { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "EndDate")]
        public DateTime EndDate { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Thumbnail { get; set; }     

        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string Description { get; set; }

    }
}
