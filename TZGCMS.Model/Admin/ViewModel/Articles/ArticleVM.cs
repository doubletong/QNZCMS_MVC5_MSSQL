using System;
using System.ComponentModel.DataAnnotations;

namespace TZGCMS.Model.Admin.ViewModel.Articles
{
    public class ArticleVM
    {
        public int Id { get; set; }        
        public string Title { get; set; }
        public string Summary { get; set; }
        public int ViewCount { get; set; }
        public string CategoryTitle { get; set; }
        public string Thumbnail { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }
        public bool Recommend { get; set; }
    }
}
