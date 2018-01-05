using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Front;

namespace TZGCMS.Model.Front.InputModel.Articles
{
    public class CommentFIM
    {
        public int Id { get; set; }
        

        [Display(ResourceType = typeof(Labels), Name = "Comment")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [AllowHtml]
        public string Body { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Article")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public int ArticleId { get; set; }


        //[Display(ResourceType = typeof(Labels), Name = "Pubdate")]
        //[Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        //public DateTime Pubdate { get; set; }

        //[Display(ResourceType = typeof(Labels), Name = "Name")]
        //public string Name { get; set; }
    }
}
