using PagedList;
using TZGCMS.Data.Entity.Ads;

namespace TZGCMS.Model.Admin.ViewModel.Ads
{
    public class CarouselListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }  
        public int? PositionId { get; set; }
        public string Keyword { get; set; }
        public StaticPagedList<Carousel> Carousels { get; set; }

        //public int PageIndex { get; set; }
        //public IEnumerable<CarouselVM> Carousels { get; set; }
    }
}
