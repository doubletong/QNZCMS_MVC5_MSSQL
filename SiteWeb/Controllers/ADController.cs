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
       

        // GET: Carousel
        public PartialViewResult Carousels(string code)
        {
            var carousels = _db.Carousels.Where(d => d.Active && d.Position.Code == code).OrderByDescending(d => d.ImageUrl).ToList();

            if (!carousels.Any())
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
            var carousel = _db.Carousels.OrderByDescending(d => d.ImageUrl).FirstOrDefault(d => d.Active && d.Position.Code == code);

            if (carousel == null)
            {
                TempData["Error"] = $"{code}广告位不存在！";
                return PartialView(null);
            }
            else
            {              
                return PartialView(carousel);
            }
        }
        public PartialViewResult SingleAdForHome(string code)
        {
            var carousel = _db.Carousels.OrderByDescending(d => d.ImageUrl).FirstOrDefault(d => d.Active && d.Position.Code == code);

            if (carousel == null)
            {
                TempData["Error"] = $"{code}广告位不存在！";
                return PartialView(null);
            }
            else
            {
                return PartialView(carousel);
            }
        }
    }
}