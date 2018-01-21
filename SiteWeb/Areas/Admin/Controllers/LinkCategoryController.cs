using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.Links;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
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
    public class LinkCategoryController : BaseController
    {
        private readonly ILinkCategoryServices _categoryServices;
        private readonly IPageMetaServices _pageMetaServices;
        private readonly IMapper _mapper;

        public LinkCategoryController(
            ILinkCategoryServices categoryServices,
            IPageMetaServices pageMetaServices,
            IMapper mapper)
        {
            _categoryServices = categoryServices;
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;

        }
        // GET: Admin/LinkCategory

        #region 新闻分类

        public ActionResult Index(int? page, string keyword)
        {
            LinkCategoryListVM categoryListVM = GetElements(page, keyword);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(categoryListVM);

        }

        private LinkCategoryListVM GetElements(int? page, string keyword)
        {
            var vm = new LinkCategoryListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Link.PageSize
            };
            int totalCount;
            var list = _categoryServices.GetPagedElements(vm.PageIndex - 1, vm.PageSize, vm.Keyword, out totalCount);

            vm.TotalCount = totalCount;
            vm.Categories = new StaticPagedList<LinkCategory>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
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

        [HttpGet]
        public ActionResult Add()
        {
            var vCategory = new LinkCategoryIM()
            {
                Active = true,
                Importance = 0
            };
            return PartialView("_Add", vCategory);
        }



        [HttpPost]
        public JsonResult Add(LinkCategoryIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var newCategory = _mapper.Map<LinkCategoryIM, LinkCategory>(vm);

            var result = _categoryServices.Create(newCategory);

            if (result != null)
            {
                var pageMeta = new PageMeta()
                {
                    ObjectId = result.ToString(),
                    Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
                    Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                    Description = vm.SEODescription,
                    ModelType = ModelType.LINKCATEGORY
                };
                _pageMetaServices.Create(pageMeta);
            }


            int count;
            var pageSize = SettingsManager.Link.PageSize;
            var list = _categoryServices.GetPagedElements(0, pageSize, string.Empty, out count);
            AR.Data = RenderPartialViewToString("_CategoryList", list);
            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.LinkCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpGet]
        public ActionResult Edit(int id)
        {

            LinkCategory category = _categoryServices.GetById(id);
            if (category == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var vm = _mapper.Map<LinkCategoryIM>(category);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.LINKCATEGORY, category.Id.ToString());
            if (pageMeta != null)
            {
                vm.SEOTitle = pageMeta.Title;
                vm.Keywords = pageMeta.Keyword;
                vm.SEODescription = pageMeta.Description;
            }

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Edit(LinkCategoryIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            var old = _categoryServices.GetById(vm.Id);
            
            var newCategory = _mapper.Map(vm, old);
            _categoryServices.Update(newCategory);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.ARTICLECATEGORY, vm.Id.ToString());
            pageMeta = pageMeta ?? new PageMeta();

            pageMeta.ObjectId = vm.Id.ToString();
            pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
            pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
            pageMeta.Description = vm.SEODescription;
            pageMeta.ModelType = ModelType.ARTICLECATEGORY;

            if (pageMeta.Id > 0)
            {
                _pageMetaServices.Update(pageMeta);
            }
            else
            {
                _pageMetaServices.Create(pageMeta);
            }


            //   var category = _mapper.Map<LinkCategoryVM>(newCategory);
            AR.Id = newCategory.Id;
            AR.Data = RenderPartialViewToString("_CategoryItem", newCategory);

            AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.LinkCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var category = _categoryServices.GetByIdWithLinks(id);

            if (category.Links.Any())
            {
                AR.Setfailure($"此分类下面还有{EntityNames.Link}存在，不能删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _categoryServices.Delete(category);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.LinkCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {

            var vCategory = _categoryServices.GetById(id);
            if (vCategory == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                vCategory.Active = !vCategory.Active;
                _categoryServices.Update(vCategory);

                // var vm = _mapper.Map<LinkCategoryVM>(vCategory);

                AR.Data = RenderPartialViewToString("_CategoryItem", vCategory);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.LinkCategory));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


      

        #endregion
    }
}