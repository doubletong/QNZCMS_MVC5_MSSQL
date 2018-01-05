using System;
using System.ComponentModel.DataAnnotations;
using TZGCMS.Infrastructure.Configs;

namespace TZGCMS.Model.Front.ViewModel.Articles
{
    public class ArticleFVM
    {
        public int Id { get; set; }        
        public string Title { get; set; }
        public string Summary { get; set; }      
        public string Thumbnail { get; set; }      

        public DateTime Pubdate { get; set; } 
        public string PubdateFormat => Pubdate.ToShortDateString();
        public string ThumbnailFull => SettingsManager.Site.SiteDomainName + Thumbnail + "?width=240&height=180&mode=crop";
    }
}
