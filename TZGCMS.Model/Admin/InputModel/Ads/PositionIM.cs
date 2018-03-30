using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Ads
{
    public class PositionIM
    {
      
        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }
        

        [Display(ResourceType = typeof(Labels), Name = "Code")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^[a-zA-Z|0-9]+$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormatOnlyEnglish")]
        [Remote("IsCodeUnique", "Position", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        public string Code { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Sketch")]
        public string Sketch { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ImageWidth")]
        public int ImageWidth { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ImageHeight")]
        public int ImageHeight { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

          
        }
    }
