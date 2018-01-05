using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Emails
{
    public class EmailAccountIM
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]      
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddressInvalidFormat")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "SmtpServer")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]        
        public string SmtpServer { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Port")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int Port { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Port")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string UserName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Password")]        
        public string Password { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "SSL")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public bool EnableSsl { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "IsDefault")]       
        public bool IsDefault { get; set; }

    }
}
