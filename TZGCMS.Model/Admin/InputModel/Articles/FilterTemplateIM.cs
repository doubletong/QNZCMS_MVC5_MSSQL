using System.ComponentModel.DataAnnotations;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Articles
{
    public class FilterTemplateIM
    {
        public int Id { get; set; }
      
        [Display(ResourceType = typeof(Labels), Name = "TemplateName")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Source")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]       
        public string Source { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "KeywordSet")]
        public string KeywordSet { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Encode")]
        public string Encode { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "LinksContainer")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string LinksContainer { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "LinksFilter")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Links { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Content")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Body { get; set; }

       
        [Display(ResourceType = typeof(Labels), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Description { get; set; }




        [Display(ResourceType = typeof(Labels), Name = "Pubdate")]     
        public string PublishDate { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Author")]   
        public string Author { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]    
        public string Keyword { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }


    }
}
