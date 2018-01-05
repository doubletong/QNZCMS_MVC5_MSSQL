using PagedList;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Entity.Videos;

namespace TZGCMS.Model.Admin.ViewModel.Videos
{
    public class ReservationListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Reservation> Reservations { get; set; }    
        public int? VideoId { get; set; }
     

    }
}
