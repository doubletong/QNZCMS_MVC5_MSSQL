using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Model.Validation;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model
{

    public class CompanyIM
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

        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddressInvalidFormat")]   
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Email { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddressInvalidFormat")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Email2 { get; set; }
                    
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Facebook { get; set; }
      
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string LinkedIn { get; set; }
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Youtube { get; set; }

        [Display(Name = "腾讯QQ一")]
        [QQ]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string Oicq { get; set; }

        [Display(Name = "腾讯QQ二")]
        [QQ]
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string OicqTwo { get; set; }


        [Display(Name = "新浪微博")]
        [Url(ErrorMessage = "网址格式不正确")]     
        [MaxLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string SinaWeibo { get; set; }

        [Display(Name = "微信帐号")]
        public string WeiXing { get; set; }

        [Display(Name = "微信二维码")]
        [DataType(DataType.ImageUrl)]
        public string WeiXingCode { get; set; }

        [Display(Name = "微信帐号2")]
        public string WeiXing2 { get; set; }

        [Display(Name = "微信二维码2")]
        [DataType(DataType.ImageUrl)]
        public string WeiXingCode2 { get; set; }

    }


}
