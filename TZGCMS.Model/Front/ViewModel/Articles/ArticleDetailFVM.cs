using System;
using System.ComponentModel.DataAnnotations;

namespace TZGCMS.Model.Front.ViewModel.Articles
{
    public class ArticleDetailFVM
    {
        public int Id { get; set; }        
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public int ViewCount { get; set; }
        public string CategoryTitle { get; set; }
        public string Thumbnail { get; set; }
        public string Source { get; set; }
        public string SourceLink { get; set; }
        public bool CanComment { get; set; }
        public DateTime Pubdate { get; set; }
        public string PubdateFormat
        {
            get
            {
                return Pubdate.ToShortDateString();
            }
        }
    }
}
