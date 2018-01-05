
using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Articles
{
    public class AcquisitionIM
    {
        [Display(ResourceType = typeof(Labels), Name = "Category")]
        public int CategoryId { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "TemplateName")]
        public int TemplateId { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Quantity")]
        public int Count { get; set; }
    }
}
