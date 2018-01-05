using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Emails
{
    public class EmailTemplateIM
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "Subject")]
        public string Subject { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "TemplateNo")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^[a-zA-Z|0-9]+$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormatEnglishAndNumber")]
        [Remote("IsNoUnique", "EmailTemplate", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        public string TemplateNo { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Content")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]  
        [AllowHtml]     
        public string Body { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "EmailAccount")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int EmailAccountId { get; set; }

    }
}
