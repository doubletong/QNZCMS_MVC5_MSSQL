using PagedList;
using TZGCMS.Data.Entity.Videos;

namespace TZGCMS.Model.Admin.ViewModel.Videos
{
    public class VideoListVM
    {
        public string Keyword { get; set; }
        public int CategoryId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Video> Videos { get; set; }
    }
}
