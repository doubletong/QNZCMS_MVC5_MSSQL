using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.Links;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Links;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Links;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Links;
using TZGCMS.Service.PageMetas;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class LinkController : BaseController
    {
        private readonly ILinkCategoryServices _categoryServices;
        private readonly ILinkServices _linkServices;
        private readonly IPageMetaServices _pageMetaServices;      
        private readonly IMapper _mapper;

        public LinkController(ILinkCategoryServices categoryServices,
            ILinkServices linkServices,
            IPageMetaServices pageMetaServices,
       
            IMapper mapper)
        {
            _categoryServices = categoryServices;
            _linkServices = linkServices;
            _pageMetaServices = pageMetaServices;
       
            _mapper = mapper;


        }
        #region 新闻


        [HttpGet]
        public ActionResult Index(int? page, int? categoryId, string Keyword)
        {

            var linkListVM = new LinkListVM
            {
                CategoryId = categoryId ?? 0,
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Link.PageSize,
                Keyword = Keyword
            };

            int count;
            var links = _linkServices.GetPagedElements(linkListVM.PageIndex - 1, linkListVM.PageSize, linkListVM.Keyword, (int)linkListVM.CategoryId, out count);



            //   linkListVM.Links = linkDtos;
            linkListVM.TotalCount = count;

            var categoryList = _categoryServices.GetAll().OrderByDescending(c => c.Importance).ToList();
            var categories = new SelectList(categoryList, "Id", "Title");
            ViewBag.Categories = categories;



            linkListVM.Links = new StaticPagedList<Link>(links, linkListVM.PageIndex, linkListVM.PageSize, linkListVM.TotalCount);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(linkListVM);

        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/LinkSettings.config");
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
       

        public ActionResult Add()
        {
            var link = new LinkIM
            {
                Active = true            
            };

            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");

            ViewBag.Categories = lCategorys;
            return View(link);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(LinkIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                var newLink = _mapper.Map<LinkIM, Link>(vm);
                var result = _linkServices.Create(newLink);                

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Link));
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            var vLink = _linkServices.GetById(Id);
            if (vLink == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var editLink = _mapper.Map<Link, LinkIM>(vLink);
          

            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");

            ViewBag.Categories = lCategorys;

            return View(editLink);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(LinkIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var old = _linkServices.GetById(vm.Id);
                var editLink = _mapper.Map(vm, old);

                _linkServices.Update(editLink);               

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Link));
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
        public JsonResult Active(int id)
        {

            Link vLink = _linkServices.GetById(id);

            try
            {
                vLink.Active = !vLink.Active;
                _linkServices.Update(vLink);

                vLink.LinkCategory = _categoryServices.GetById(vLink.CategoryId);

                AR.Data = RenderPartialViewToString("_LinkItem", vLink);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Link));
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

            Link vLink = _linkServices.GetById(id);

            if (vLink == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _linkServices.Delete(vLink);

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Link));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }
       
        #endregion


    }
}
