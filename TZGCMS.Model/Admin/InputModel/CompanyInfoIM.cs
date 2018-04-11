using System.ComponentModel.DataAnnotations;
using TZGCMS.Model.Validation;

namespace TZGCMS.Model.Admin.InputModel
{
    public class CompanyInfoIM
    {
        public ContactIM Contact { get; set; }
    }
    public class ContactIM
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }
        [Display(Name = "公司简称")]
        [StringLength(50)]
        public string CompanyShortName { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "公司地址")]       
        [StringLength(250)]
        public string Address { get; set; }

        [Display(Name = "地理坐标", Prompt = "地理坐标")]
        [StringLength(50)]
        [Coordinate]
         public string Coordinate { get; set; }

        [Required]
        [Display(Name = "联系人")]
        [StringLength(50)]
        public string ContactMan { get; set; }

        [Required]
        [Display(Name = "手机")]
        [ChinaMobile]       
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "电话")]
        [StringLength(50)]
        public string Phone { get; set; }
        [Display(Name = "传真")]
        [StringLength(50)]
        public string Fax { get; set; }
      
        [Display(Name = "邮政编码")]
        [StringLength(50)]
        public string ZipCode { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]     
        [Display(Name = "邮箱")]
        [StringLength(150)]
        public string MailTo { get; set; }

     
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]    
        [Display(Name = "备用邮箱")]
        [StringLength(150)]
        public string MailCC { get; set; }
       
    }

   
}
