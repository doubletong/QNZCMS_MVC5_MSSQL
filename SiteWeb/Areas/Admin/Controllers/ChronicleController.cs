using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.Chronicles;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Chronicles;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Chronicles;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Chronicles;
using TZGCMS.Service.PageMetas;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ChronicleController : BaseController
    {
        private readonly IChronicleServices _chronicleService;
        private readonly IPageMetaServices _pageMetaServices;
        private readonly IMapper _mapper;

        public ChronicleController(IChronicleServices chronicleService, IPageMetaServices pageMetaServices, IMapper mapper)
        {
            _chronicleService = chronicleService;
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;

        }
        // GET: Admin/Chronicle

        #region Chronicle

        public ActionResult Index(int? page, string keyword)
        {
            ChronicleListVM chronicleListVM = GetElements(page, keyword);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(chronicleListVM);

        }

        private ChronicleListVM GetElements(int? page, string keyword)
        {
            var chronicleListVM = new ChronicleListVM
            {
                Keyword = keyword,
                PageIndex = (page ?? 1) - 1,
                PageSize = SettingsManager.Chronicle.PageSize
            };
            int totalCount;
            var chroniclelist = _chronicleService.GetPagedElements(chronicleListVM.PageIndex, chronicleListVM.PageSize, chronicleListVM.Keyword, out totalCount);
            var chronicleVMList = _mapper.Map<List<Chronicle>, List<ChronicleVM>>(chroniclelist);
            chronicleListVM.TotalCount = totalCount;
            chronicleListVM.Chronicles = new StaticPagedList<ChronicleVM>(chronicleVMList, chronicleListVM.PageIndex + 1, chronicleListVM.PageSize, chronicleListVM.TotalCount); ;
            return chronicleListVM;
        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/ChronicleSettings.config");
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
            var vCategory = new ChronicleIM
            {
                Active = true
            };

            ViewBag.Years = new SelectList(DateTimeHelper.GetYearList(2010));
            ViewBag.Months = new SelectList(DateTimeHelper.GetMonthList());
            ViewBag.Days = new SelectList(DateTimeHelper.GetDayList());

            return PartialView("_Add", vCategory);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Add(ChronicleIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var chronicle = _mapper.Map<ChronicleIM, Chronicle>(vm);
            var result = _chronicleService.Create(chronicle);
            if (result != null)
            {
                var pageMeta = new PageMeta()
                {
                    ObjectId = result.ToString(),
                    Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
                    Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                    Description = vm.SEODescription,
                    ModelType = ModelType.CHRONICLE
                };
                _pageMetaServices.Create(pageMeta);
            }

            int count;
            int pageSize = SettingsManager.Chronicle.PageSize;
            var list = _chronicleService.GetPagedElements(0, pageSize, string.Empty, out count);
            List<ChronicleVM> chronicleList = _mapper.Map<List<Chronicle>, List<ChronicleVM>>(list);
            AR.Data = RenderPartialViewToString("_ChronicleList", chronicleList);

            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Chronicle));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpGet]
        public ActionResult Edit(int id)
        {

            Chronicle chronicle = _chronicleService.GetById(id);
            if (chronicle == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var vm = _mapper.Map<ChronicleIM>(chronicle);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.CHRONICLE, vm.Id.ToString());
            if (pageMeta != null)
            {
                vm.SEOTitle = pageMeta.Title;
                vm.Keywords = pageMeta.Keyword;
                vm.SEODescription = pageMeta.Description;
            }

            ViewBag.Years = new SelectList(DateTimeHelper.GetYearList(2010));
            ViewBag.Months = new SelectList(DateTimeHelper.GetMonthList());
            ViewBag.Days = new SelectList(DateTimeHelper.GetDayList());

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Edit(ChronicleIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var old = _chronicleService.GetById(vm.Id);
            var chronicle = _mapper.Map(vm, old);

            //chronicle.Active = vm.Active;
            //chronicle.Year = vm.Year;
            //chronicle.Title = vm.Title;
            //chronicle.Month = vm.Month;
            //chronicle.Day = vm.Day;
            //chronicle = _mapper.Map<ChronicleIM, Chronicle>(vm);
            _chronicleService.Update(chronicle);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.CHRONICLE, chronicle.Id.ToString());
            pageMeta = pageMeta ?? new PageMeta();

            pageMeta.ObjectId = vm.Id.ToString();
            pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
            pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
            pageMeta.Description = vm.SEODescription;
            pageMeta.ModelType = ModelType.CHRONICLE;

            if (pageMeta.Id > 0)
            {
                _pageMetaServices.Update(pageMeta);
            }
            else
            {
                _pageMetaServices.Create(pageMeta);
            }

            var chronicleVM = _mapper.Map<ChronicleVM>(chronicle);

            AR.Id = chronicle.Id;
            AR.Data = RenderPartialViewToString("_ChronicleItem", chronicleVM);

            AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Chronicle));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {

            Chronicle vCategory = _chronicleService.GetById(id);


            _chronicleService.Delete(vCategory);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Chronicle));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {

            var vCategory = _chronicleService.GetById(id);
            if (vCategory == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                vCategory.Active = !vCategory.Active;
                _chronicleService.Update(vCategory);
                var vm = _mapper.Map<ChronicleVM>(vCategory);

                AR.Data = RenderPartialViewToString("_ChronicleItem", vm);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Chronicle));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }




        //[AllowAnonymous]
        //public JsonResult IsSeoNameUnique(string seoName, int? Id)
        //{
        //    return !IsExist(seoName, Id)
        //        ? Json(true, JsonRequestBehavior.AllowGet)
        //        : Json(false, JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //public bool IsExist(string seoName, int? id)
        //{
        //    var wType = _chronicleService.GetAll().Where(w => w.SeoName == seoName);
        //    if (id > 0)
        //    {
        //        wType = wType.Where(w => w.Id != id);
        //    }

        //    if (wType.Count() > 0)
        //        return true;

        //    return false;
        //}



        #endregion
    }
}