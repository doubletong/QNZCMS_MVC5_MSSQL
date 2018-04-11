using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;

using System;
using System.Xml.Linq;
using TZGCMS.SiteWeb.Filters;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Articles;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Model.Admin.ViewModel.Videos;
using TZGCMS.Service.Videos;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ReservationController : BaseController
    {

        private readonly IReservationServices _reservationServices;  
        private readonly IMapper _mapper;

        public ReservationController(IArticleServices articleServices,
            IReservationServices reservationServices,       
            IMapper mapper)
        {

            _reservationServices = reservationServices;
         
            _mapper = mapper;


        }
        #region 新闻


        [HttpGet]
        public ActionResult Index(int? page, int? videoId)
        {

            var reservationListVM = new ReservationListVM();

            reservationListVM.VideoId = videoId ?? 0;
            reservationListVM.PageIndex = (page ?? 1);
            reservationListVM.PageSize = SettingsManager.Article.PageSize;
        

            int count;
            var reservations = _reservationServices.GetPagedElements(reservationListVM.PageIndex-1, reservationListVM.PageSize,  (int)reservationListVM.VideoId, out count);
            
            //   reservationListVM.Reservations = reservationDtos;
            reservationListVM.TotalCount = count;

            //var categoryList = _categoryServices.GetAll().OrderByDescending(c => c.Importance).ToList();
            //var categories = new SelectList(categoryList, "Id", "Title");
            //ViewBag.Categories = categories;

            reservationListVM.Reservations = new StaticPagedList<Reservation>(reservations, reservationListVM.PageIndex, reservationListVM.PageSize, reservationListVM.TotalCount);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(reservationListVM);

        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/ReservationSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("PageSize").SetValue(pageSize);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }
      
     
       

        //public ActionResult Add()
        //{
        //    var reservation = new ReservationIM {
        //        Active = true,
        //        Source = SettingsManager.Site.SiteName,
        //        Pubdate = DateTime.Now };

        //    var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
        //    var lCategorys = new SelectList(categorys, "Id", "Title");

        //    ViewBag.Categories = lCategorys;
        //    return View(reservation);
        //}
        

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult Add(ReservationIM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    try
        //    {

        //        var newReservation = _mapper.Map<ReservationIM, Reservation>(vm);
        //       // newReservation.ViewCount = 0;
        //      //  newReservation.CreatedBy = Site.CurrentUserName;
        //      //  newReservation.CreatedDate = DateTime.Now;

        //        var result = _reservationServices.Create(newReservation);

        //        if (result)
        //        {
        //            var pageMeta = new PageMeta()
        //            {
        //                ObjectId = result.ToString(),
        //                Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
        //                Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
        //                Description = vm.SEODescription,
        //                ModelType = ModelType.ARTICLE
        //            };
        //            _pageMetaServices.Create(pageMeta);
        //        }
                

        //        AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Reservation));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }


        //}

        //[HttpGet]
        //public ActionResult Edit(int Id)
        //{
        //    var vReservation = _reservationServices.GetById(Id);
        //    if (vReservation == null)
        //    {
        //        AR.Setfailure(Messages.HttpNotFound);
        //        return Json(AR, JsonRequestBehavior.AllowGet);
        //    }


        //    var editReservation = _mapper.Map<Reservation, ReservationIM>(vReservation);

        //    var pageMeta = _pageMetaServices.GetPageMeta(ModelType.ARTICLE, editReservation.Id.ToString());
        //    if (pageMeta != null)
        //    {
        //        editReservation.SEOTitle = pageMeta.Title;
        //        editReservation.Keywords = pageMeta.Keyword;
        //        editReservation.SEODescription = pageMeta.Description;
        //    }

        //    var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
        //    var lCategorys = new SelectList(categorys, "Id", "Title");

        //    ViewBag.Categories = lCategorys;

        //    return View(editReservation);


        //}

        //[HttpPost]
        //[ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //public JsonResult Edit(ReservationIM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    try
        //    {
               
        //        var editReservation = _mapper.Map<ReservationIM, Reservation>(vm);

        //        _reservationServices.Update(editReservation);

        //        var pageMeta = _pageMetaServices.GetPageMeta(ModelType.ARTICLE, editReservation.Id.ToString());
        //        pageMeta = pageMeta ?? new PageMeta();

        //        pageMeta.ObjectId = vm.Id.ToString();
        //        pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
        //        pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
        //        pageMeta.Description = vm.SEODescription;
        //        pageMeta.ModelType = ModelType.ARTICLE;

        //        if (pageMeta.Id > 0)
        //        {
        //            _pageMetaServices.Update(pageMeta);
        //        }
        //        else
        //        {
        //            _pageMetaServices.Create(pageMeta);
        //        }
               

        //        AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Reservation));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }


        //}
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int videoId,string openId)
        {

            Reservation vReservation = _reservationServices.GetById(videoId,openId);

            if (vReservation == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _reservationServices.Delete(vReservation);

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Reservation));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }
        
        #endregion


    }
}
