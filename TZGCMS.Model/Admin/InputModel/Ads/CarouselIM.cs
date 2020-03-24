using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Ads
{
    public class CarouselIM
    {


        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ImageUrlPC")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string ImageUrl { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ImageUrlMobile")]
        //[Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string ImageUrlMobile { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        [RegularExpression(@"^-?\d*$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormat")]
        public int Importance { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Link")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string WebLink { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string Description { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }
        
        [Display(ResourceType = typeof(Labels), Name = "Position")]
        public int PositionId { get; set; }
    }
}
