using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Model.Admin.ViewModel.Ads
{
    public class CarouselVM
    {
        public int Id { get; set; }    
        public string Title { get; set; } 
        public string ImageUrl { get; set; }     
        public int Importance { get; set; }     
        public string WebLink { get; set; }
        public int PositionId { get; set; }
        public string PositionTitle { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
