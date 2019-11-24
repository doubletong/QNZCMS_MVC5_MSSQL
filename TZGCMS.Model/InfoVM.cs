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
    public class ModuleSetIM
    {
        public ArticleSetIM ArticleSet { get; set; }
        public PostSetIM PostSet { get; set; }
        public SiteInfoIM SiteInfo { get; set; }
        public CaseSetIM CaseSet { get; set; }
        public PhotoSetIM PhotoSet { get; set; }
        public ProductSetIM ProductSet { get; set; }
        public WeiXinSetIM WeiXinSet { get; set; }
        public VideoSetIM VideoSet { get; set; }
        public SiteCloseIM SiteCloseSet { get; set; }
    }

    public class VideoSetIM
    {
        [Display(ResourceType = typeof(Labels), Name = "PageSize")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int FrontPageSize { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbWidth { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Timer")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int Timer { get; set; }

    }

    public class WeiXinSetIM
    {

        public string Token { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string EncodingAESKey { get; set; }

    }
    public class ProductSetIM
    {
        [Display(ResourceType = typeof(Labels), Name = "PageSize")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int FrontPageSize { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbWidth { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "ImageHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ImageHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ImageWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ImageWidth { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "CategoryImageHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int CategoryImageHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CategoryImageWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int CategoryImageWidth { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "EnableCaching")]
        public bool EnableCaching { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CacheDuration")]
        public int CacheDuration { get; set; }
    }

    public class PhotoSetIM
    {
        [Display(ResourceType = typeof(Labels), Name = "ThumbHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbWidth { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "EnableCaching")]
        public bool EnableCaching { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CacheDuration")]
        public int CacheDuration { get; set; }

    }

    public class CaseSetIM
    {
        [Display(ResourceType = typeof(Labels), Name = "PageSize")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int FrontPageSize { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbWidth { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "EnableCaching")]
        public bool EnableCaching { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CacheDuration")]
        public int CacheDuration { get; set; }


    }
    public class PostSetIM
    {
        [Display(ResourceType = typeof(Labels), Name = "PageSize")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int FrontPageSize { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbWidth { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "EnableCaching")]
        public bool EnableCaching { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CacheDuration")]
        public int CacheDuration { get; set; }
    }
    public class ArticleSetIM
    {
        [Display(ResourceType = typeof(Labels), Name = "PageSize")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int FrontPageSize { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ThumbWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ThumbWidth { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ImageWidth")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ImageWidth { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "ImageHeight")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ImageHeight { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "EnableCaching")]
        public bool EnableCaching { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CacheDuration")]
        public int CacheDuration { get; set; }
    }
    public class SiteInfoIM
    {
        [Display(ResourceType = typeof(Labels), Name = "SiteName")]
        public string SiteName { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "SiteDomainName")]
        //[Url(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormat")]
        public string SiteDomainName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "WebNumber")]
        public string WebNumber { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "BaiduSiteID")]
        public string BaiduSiteID { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "GoogleAnalyticsID")]
        public string GoogleAnalyticsID { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "EmailAddressInvalidFormat")]
        [StringLength(150, ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "MaxLength")]
        public string MailTo { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "DashboardLogo")]
        public string DashboardLogo { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "LoginLogo")]
        public string LoginLogo { get; set; }


    }
    public class SiteCacheIM
    {

        [Display(ResourceType = typeof(Labels), Name = "EnableCaching")]
        public bool EnableCaching { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CacheDuration")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int CacheDuration { get; set; }
    }

    public class SiteCloseIM
    {

        [Display(ResourceType = typeof(Labels), Name = "IsClose")]
        public bool IsClose { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CloseInfo")]
        public string CloseInfo { get; set; }
    }
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
