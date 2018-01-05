using System;
using System.Collections.Generic;
using System.Data;

namespace TZGCMS.Model.Admin.ViewModel.Ads
{
    public class PositionVM
    {
        public int Id { get; set; }
        public string Title { get; set; }   
        public string Code { get; set; }
        public string Sketch { get; set; }
        public int Importance { get; set; }
        public bool Active { get; set; }      
        public DateTime CreatedDate { get; set; }
        public IEnumerable<CarouselVM> Carousels { get; set; }
    }
}
