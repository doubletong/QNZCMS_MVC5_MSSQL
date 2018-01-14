using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Doc
{
    public class DocumentCategoryIM
    {

        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "SeoName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "InvalidFormatSEOUrl")]
        [Remote("IsSeoNameUnique", "ArticleCategory", AdditionalFields = "Id", ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "IsExist")]
        public string SeoName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }

    }
}
