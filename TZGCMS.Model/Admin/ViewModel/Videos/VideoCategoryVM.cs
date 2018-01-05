using System;

namespace TZGCMS.Model.Admin.ViewModel.Videos
{
    public class VideoCategoryVM
    {
        public int Id { get; set; }
        public string Title { get; set; }   
        public string SeoName { get; set; }
        public int Importance { get; set; }
        public bool Active { get; set; }      
        public DateTime CreatedDate { get; set; }
    }
}
