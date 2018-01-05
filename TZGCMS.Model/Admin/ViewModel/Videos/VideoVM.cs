using System;

namespace TZGCMS.Model.Admin.ViewModel.Videos
{
    public class VideoVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string Thumbnail { get; set; }
        public bool Active { get; set; }     
        public DateTime CreatedDate { get; set; }
    }
}
