using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Outlets
{
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
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Labels), Name = "Coordinate")]
        public string Coordinate { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ContactMan")]
        public string ContactMan { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Phone")]
        public string Phone { get; set; }
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "ValidNumber")]
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        public bool Active { get; set; }
    }
}
