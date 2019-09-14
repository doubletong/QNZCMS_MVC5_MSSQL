using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Model.Admin.InputModel.Doc
{
    public class DocumentIM
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "FileSize")]
       
        public Decimal? FileSize { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Extension")]
        public string Extension { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "FilePath")]
        public string FilePath { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "Importance")]
        public int Importance { get; set; }       

        [Display(ResourceType = typeof(Labels), Name = "Category")]
        public int CategoryId { get; set; }
        [Display(ResourceType = typeof(Labels), Name = "IsVIP")]
        public bool IsVIP { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Product")]
        public string ProductIds { get; set; }

        [Display(ResourceType = typeof(Labels), Name = "Thumbnail")]
        public string Thumbnail { get; set; }     

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
