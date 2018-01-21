using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Chronicles
{
    public class ChronicleIM
    {

        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Year")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public short Year { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Month")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public short Month { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Day")]
        public short? Day { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        [StringLength(150,ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Thumbnail { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Content")]
        [DataType(DataType.Html)]
        public string Body { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Summary")]
        [StringLength(500, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Summary { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }

    }
}
