using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Articles
{
    public class ArticleIM
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Content")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [AllowHtml]
        public string Body { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Summary")]
        public string Summary { get; set; }


        [Display(ResourceType = typeof(Labels), Name = "Category")]
        public int CategoryId { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        //[Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Thumbnail { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "FullImage")]
        //[Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string FullImage { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Source")]
        public string Source { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "SourceLink")]
        public string SourceLink { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Active")]
        public bool Active { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "CanComment")]
        public bool CanComment { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Recommend")]
        public bool Recommend { get; set; }

        public virtual ArticleCategoryIM Category { get; set; }

        public string CategoryTitle
        {
            get
            {
                if (this.Category != null)
                {
                    return this.Category.Title;
                }
                return string.Empty;
            }
        }
        [Display(ResourceType = typeof(Labels), Name = "Pubdate")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public DateTime Pubdate { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Title")]
        public string SEOTitle { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "KeyWords")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Description")]
        public string SEODescription { get; set; }

    }
}
