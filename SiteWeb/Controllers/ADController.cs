using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Service.Ads;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ADController :Controller
    {

        private readonly IQNZDbContext _db;
        private readonly ICacheService _cacheService;
        public ADController(ICacheService cacheService, IQNZDbContext db)
        {
            _cacheService = cacheService;
            _db = db;
        }
        // GET: Carousel
        public PartialViewResult Carousels(string code)
        {
            var carousels = _db.CarouselSets.Where(d => d.Active && d.PositionSet.Code == code).OrderByDescending(d => d.ImageUrl).ToList();

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
            var carousel = _db.CarouselSets.OrderByDescending(d => d.ImageUrl).FirstOrDefault(d => d.Active && d.PositionSet.Code == code);

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
            var carousel = _db.CarouselSets.OrderByDescending(d => d.ImageUrl).FirstOrDefault(d => d.Active && d.PositionSet.Code == code);

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