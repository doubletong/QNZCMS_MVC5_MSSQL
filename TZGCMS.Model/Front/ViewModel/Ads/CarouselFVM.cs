using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Infrastructure.Configs;

namespace TZGCMS.Model.Front.ViewModel.Ads
{
    public class CarouselFVM
    {
        public int Id { get; set; }    
        public string Title { get; set; } 
        public string ImageUrl { get; set; }     
        public string WebLink { get; set; }
        public string ImageUrlFull { get {
                return SettingsManager.Site.SiteDomainName + ImageUrl;
            } }
    }
}
