using PagedList;
using TZGCMS.Data.Entity.Ads;

namespace TZGCMS.Model.Admin.ViewModel.Ads
{
    public class PositionListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Position> Positions { get; set; }
    }
}
