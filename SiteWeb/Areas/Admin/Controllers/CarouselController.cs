using System;
using System.Web.Mvc;
using PagedList;
using AutoMapper;
using System.Linq;
using TZGCMS.SiteWeb.Filters;
using TZGCMS.Model.Admin.ViewModel.Ads;
using TZGCMS.Service.Ads;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Model.Admin.InputModel.Ads;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]  
    public class CarouselController : BaseController
    {
     
        private readonly ICarouselServices _carouselService;
        private readonly IPositionServices _positionService;
        private readonly IMapper _mapper;
        public CarouselController(ICarouselServices carouselService, IPositionServices positionService, IMapper mapper)
        {
            _carouselService = carouselService;
            _positionService = positionService;
            _mapper = mapper;
        }

     
        public ActionResult Index(int? page, int? positionId, string keyword)
        {
            CarouselListVM carouselListVM = GetElements(page, positionId, keyword);

            // carouselListVM.Album = _albumService.GetById(positionId);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            var positionList = _positionService.GetAll().OrderByDescending(c => c.Importance).ToList();
            var positions = new SelectList(positionList, "Id", "TitleAndSize");
            ViewBag.Positions = positions;

            return View(carouselListVM);

            
        }

        private CarouselListVM GetElements(int? page, int? positionId, string keyword)
        {
            var carouselListVM = new CarouselListVM();
            carouselListVM.PositionId = positionId??0;
            carouselListVM.Keyword = keyword;
            carouselListVM.PageIndex = (page ?? 1);
            carouselListVM.PageSize = SettingsManager.Ads.PageSize;
            int totalCount;
            var carousellist = _carouselService.GetPagedElements(carouselListVM.PageIndex-1, carouselListVM.PageSize,  carouselListVM.Keyword, (int)carouselListVM.PositionId, out totalCount);
          
            foreach(var item in carousellist)
            {
                item.Position = _positionService.GetById(item.PositionId);
            }
            
            //  var specialistVMList = _mapper.Map<List<Carousel>, List<CarouselVM>>(carousellist);
            carouselListVM.TotalCount = totalCount;
            carouselListVM.Carousels = new StaticPagedList<Carousel>(carousellist, carouselListVM.PageIndex , carouselListVM.PageSize, carouselListVM.TotalCount); ;
            return carouselListVM;
        }

        [HttpGet]
        public PartialViewResult AddCarousel()
        {            
            CarouselIM vCarousel = new CarouselIM {
                Active = true,
                Importance = 0            
            };

            var positionList = _positionService.GetAll().OrderByDescending(c => c.Importance).ToList();
            var positions = new SelectList(positionList, "Id", "TitleAndSize");
            ViewBag.Positions = positions;

            return PartialView("_AddCarousel", vCarousel);
        }


        [HttpPost]    
        public JsonResult AddCarousel(CarouselIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var carousel = _mapper.Map<Carousel>(vm);            
            _carouselService.Create(carousel);

            int count;
            int pageSize = SettingsManager.Ads.PageSize;
            var list = _carouselService.GetPagedElements(0, pageSize, string.Empty, 0,  out count);
          //  List<CarouselVM> carouselList = _mapper.Map<List<Carousel>, List<CarouselVM>>(list);
            AR.Data = RenderPartialViewToString("_CarouselList", list);

            return Json(AR, JsonRequestBehavior.DenyGet);

        }

       [HttpGet]
        public ActionResult Edit(int Id)
        {
            var vCarousel = _carouselService.GetById(Id);
            if (vCarousel != null)
            {

                var vm = _mapper.Map<CarouselIM>(vCarousel);

                var positionList = _positionService.GetAll().OrderByDescending(c => c.Importance).ToList();
                var positions = new SelectList(positionList, "Id", "TitleAndSize");
                ViewBag.Positions = positions;

                return PartialView("_EditCarousel", vm);
            }
            AR.Setfailure("未找到编辑项");
            return Json(AR, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]       
        public JsonResult Edit(CarouselIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
         

            var carousel = _mapper.Map<Carousel>(vm);
            _carouselService.Update(carousel);
            carousel.Position = _positionService.GetById(carousel.PositionId);          

           // var carouselVM = _mapper.Map<CarouselVM>(_carouselService.GetByIdWithPosition(carousel.Id));
            AR.Id = carousel.Id;
            AR.Data = RenderPartialViewToString("_CarouselItem", carousel);

            return Json(AR, JsonRequestBehavior.DenyGet);          

        }

      
        [HttpPost]      
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {

            var vCarousel = _carouselService.GetById(id);

            if (vCarousel != null)
            {
                //string imgurl = Server.MapPath(vCarousel.ImageUrl);
                //if (System.IO.File.Exists(imgurl))
                //{
                //    System.IO.File.Delete(imgurl);    //删除缩略图
                //}

                _carouselService.Delete(vCarousel);
             //   _unit.SaveChanges();
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            AR.Setfailure(Messages.AlertDeleteFailure);
            return Json(AR, JsonRequestBehavior.DenyGet);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {
            var vCarousel = _carouselService.GetById(id);
            if (vCarousel == null)
            {
                AR.Setfailure(Messages.AlertActionFailure);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            try
            {                
                vCarousel.Active = !vCarousel.Active;
                _carouselService.Update(vCarousel);
                vCarousel.Position = _positionService.GetById(vCarousel.PositionId);

                //  var vm = _mapper.Map<CarouselVM>(vCarousel);
                AR.Data = RenderPartialViewToString("_CarouselItem", vCarousel);

                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }           

        }

    }
}
