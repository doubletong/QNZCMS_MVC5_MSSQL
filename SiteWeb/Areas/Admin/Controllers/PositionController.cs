using AutoMapper;
using PagedList;
using SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Ads;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Ads;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Ads;

namespace SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class PositionController : BaseController
    {
        private readonly IPositionServices _positionService;
        private readonly ICarouselServices _carouselService;
        private readonly IMapper _mapper;

        public PositionController(IPositionServices positionService, ICarouselServices carouselService, IMapper mapper)
        {
            _positionService = positionService;
            _carouselService = carouselService;
            _mapper = mapper;

        }
        // GET: Admin/Position

        #region Ad Position

        public ActionResult Index(int? page, string keyword)
        {
            PositionListVM positionListVM = GetElements(page, keyword);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(positionListVM);
            
        }

        private PositionListVM GetElements(int? page, string keyword)
        {
            var positionListVM = new PositionListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Ads.PageSize
            };
            int totalCount;
            var positionlist = _positionService.GetPagedElements(positionListVM.PageIndex-1, positionListVM.PageSize, positionListVM.Keyword, out totalCount);
         //   var positionVMList = _mapper.Map<List<Position>, List<PositionVM>>(positionlist);
            positionListVM.TotalCount = totalCount;
            positionListVM.Positions = new StaticPagedList<Position>(positionlist, positionListVM.PageIndex, positionListVM.PageSize, positionListVM.TotalCount); ;
            return positionListVM;
        }

        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/AdsSettings.config");
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

        [HttpGet]
        public ActionResult Add()
        {
            var vPosition = new PositionIM();
            vPosition.Active = true;
            vPosition.Importance = 0;

            return PartialView("_Add", vPosition);
        }



        [HttpPost]
        public JsonResult Add(PositionIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var newPosition = _mapper.Map<PositionIM, Position>(vm);
            newPosition.CreatedBy = Site.CurrentUserName;
            newPosition.CreatedDate = DateTime.Now;
            _positionService.Create(newPosition);

            int count;
            var pageSize = SettingsManager.Ads.PageSize;
            var list = _positionService.GetPagedElements(0, pageSize, string.Empty, out count);
            //List<PositionVM> categories = _mapper.Map<List<Position>, List<PositionVM>>(list);
            AR.Data = RenderPartialViewToString("_PositionList", list);

            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Position));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpGet]
        public ActionResult Edit(int id)
        {

            Position position = _positionService.GetById(id);
            if (position == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }
            var vm = _mapper.Map<PositionIM>(position);

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Edit(PositionIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            var newPosition = _positionService.GetById(vm.Id);
            newPosition.Title = vm.Title;
            newPosition.Code = vm.Code;
            newPosition.Importance = vm.Importance;
            newPosition.Sketch = vm.Sketch;
            newPosition.Active = vm.Active;
            newPosition.UpdatedBy = Site.CurrentUserName;
            newPosition.UpdatedDate = DateTime.Now;
            //var newPosition = _mapper.Map<PositionIM, Position>(vm);
            _positionService.Update(newPosition);
            

           // var position = _mapper.Map<PositionVM>(newPosition);
            AR.Id = newPosition.Id;
            AR.Data = RenderPartialViewToString("_PositionItem", newPosition);

            AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Position));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {

            Position vPosition = _positionService.GetByIdWithCarousels(id);
           // vPosition.Carousels = _carouselService.GetItems(id);
            if (vPosition.Carousels.Any())
            {
                AR.Setfailure("此分类下面还有广告存在，不能删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _positionService.Delete(vPosition);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Position));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {

            var vPosition = _positionService.GetById(id);
            if (vPosition == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                vPosition.Active = !vPosition.Active;
                _positionService.Update(vPosition);

                //var vm = _mapper.Map<PositionVM>(vPosition);

                AR.Data = RenderPartialViewToString("_PositionItem", vPosition);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Position));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        [AllowAnonymous]
        public JsonResult IsCodeUnique(string code, int? id)
        {
            return !_positionService.IsExistCode(code, id)
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(false, JsonRequestBehavior.AllowGet);
        }
       
        #endregion
    }
}