using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Front;

namespace TZGCMS.Model.Front.InputModel.Emails
{
    public class EmailIM
    {
        [Display(ResourceType = typeof(Labels), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Phone")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Phone { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessage = null, ErrorMessageResourceName = "EmailAddressInvalidFormat")]
        public string Email { get; set; }
        public string Subject { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Message")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Body { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CaptchaText")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string CaptchaText { get; set; }
    }

    public class ContactIM
    {
        [Display(ResourceType = typeof(Labels), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Country")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Country { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Company")]
        public string Company { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Phone")]      
        public string Phone { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddressInvalidFormat")]
        public string Email { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Message")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Message { get; set; }
    }
}
