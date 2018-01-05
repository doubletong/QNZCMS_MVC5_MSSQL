namespace TZGCMS.Model.Admin.ViewModel.Ads
{ 

    public class PageCarouselListVM
    {

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public CarouselListVM CarouselList { get; set; }
     
    }
}
