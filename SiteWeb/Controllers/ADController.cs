using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Service.Ads;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ADController : BaseController
    {
        private readonly ICarouselServices _carouselService;
        private readonly IPositionServices _postionService;
      //  private readonly IMapper _mapper;
        public ADController(ICarouselServices carouselService, IPositionServices postionService /*IMapper mapper*/)
        {
            _carouselService = carouselService;
            _postionService = postionService;
          
            //_mapper = mapper;
        }


        // GET: Carousel
        public PartialViewResult Carousels(string code)
        {
            var carousels = _carouselService.GetActiveElements(code);

            if (carousels == null)
            {
                TempData["Error"] = $"{code}广告位不存在！";
                return PartialView(null);
            }
            else
            {
                //var carousels = _carouselService.f(position.Id).OrderByDescending(m => m.Importance);
                ////var list = _mapper.Map<IEnumerable<Carousel>, IEnumerable<CarouselVM>>(position.Carousels.OrderByDescending(m => m.Importance));
                return PartialView(carousels);
            }
        }

        public PartialViewResult SingleAd(string code)
        {
            var carousels = _carouselService.GetActiveElements(code);

            if (carousels == null)
            {
                TempData["Error"] = $"{code}广告位不存在！";
                return PartialView(null);
            }
            else
            {
                //var carousels = _carouselService.f(position.Id).OrderByDescending(m => m.Importance);
                ////var list = _mapper.Map<IEnumerable<Carousel>, IEnumerable<CarouselVM>>(position.Carousels.OrderByDescending(m => m.Importance));
                return PartialView(carousels.FirstOrDefault());
            }
        }
    }
}