using AutoMapper;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;

using TZGCMS.Model.Admin.ViewModel.Outlets;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.PageMetas;
using TZGCMS.Service.Outlets;
using TZGCMS.Data.Entity;
using PagedList;
using TZGCMS.Model.Admin.InputModel.Outlets;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class OutletController : BaseController
    {
        private readonly IOutletServices _teamServices;
        private readonly IPageMetaServices _pageMetaServices;
        private IMapper _mapper;
        public OutletController(IOutletServices teamServices, IPageMetaServices pageMetaServices, IMapper mapper)
        {
            _teamServices = teamServices;
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;
        }
       
        // GET: Admin/Outlets
        public ActionResult Index(int? page, string keyword)
        {
            OutletListVM pageListVM = GetElements(page, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(pageListVM);

        }

        private OutletListVM GetElements(int? page, string keyword)
        {
            var pageListVM = new OutletListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Outlet.PageSize
            };
            int totalCount;
            var pagelist = _teamServices.GetOutletdElements(pageListVM.PageIndex - 1, pageListVM.PageSize, pageListVM.Keyword, out totalCount);

            pageListVM.TotalCount = totalCount;
            pageListVM.Outlets = new StaticPagedList<Outlet>(pagelist, pageListVM.PageIndex, pageListVM.PageSize, pageListVM.TotalCount); ;
            return pageListVM;
        }
        [HttpPost]
        public JsonResult OutletSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/OutletSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("OutletSize").SetValue(pageSize);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        // GET: Admin/Outlets/Create
        public ActionResult Create()
        {
            OutletIM team = new OutletIM()
            {
                Active = true
            };
            return PartialView("_Create", team);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Create(OutletIM team)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var newOutlet = _mapper.Map<OutletIM, Outlet>(team);              

                var result = _teamServices.Create(newOutlet);
               


                int count;
                int pageSize = SettingsManager.Outlet.PageSize;
                var list = _teamServices.GetOutletdElements(0, pageSize, string.Empty, out count);

                AR.Data = RenderPartialViewToString("_OutletList", list);

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Outlet));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        // GET: Admin/Outlets/Edit/5
        public ActionResult Edit(int id)
        {

            var page = _teamServices.GetById(id);
            if (page == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
                // return HttpNotFound();
            }

            var editOutlet = _mapper.Map<Outlet, OutletIM>(page);           

            return PartialView("_Edit", editOutlet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(OutletIM page)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var old = _teamServices.GetById(page.Id);
                var editOutlet = _mapper.Map(page, old);

                

                _teamServices.Update(editOutlet);

              

                // var pageVM = _mapper.Map<OutletVM>(editOutlet);

                AR.Id = page.Id;
                AR.Data = RenderPartialViewToString("_OutletItem", editOutlet);

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Outlet));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {
            var page = _teamServices.GetById(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Outlet));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _teamServices.Update(page);

                AR.Data = RenderPartialViewToString("_OutletItem", page);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Outlet));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var page = _teamServices.GetById(id);
            if (page != null)
            {
                _teamServices.Delete(page);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Outlet));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Outlet));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }





        //[HttpPost]
        //public JsonResult CreateIndex()
        //{

        //    try
        //    {
        //        var list = _teamServices.GetActiveElements().Select(m => new SearchData
        //        {
        //            Id = $"PAGE{m.Id}",
        //            Name = m.Post,
        //            Description = StringHelper.StripTagsCharArray(m.Description),
        //            Url = $"{SettingsManager.Site.SiteDomainName}/teams/{m.SeoName}"
        //        }).ToList();

        //        GoLucene.AddUpdateLuceneIndex(list);

        //        AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Outlet));
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}
    }
}