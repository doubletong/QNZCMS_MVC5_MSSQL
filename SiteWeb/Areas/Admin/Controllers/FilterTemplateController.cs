using AutoMapper;
using PagedList;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Articles;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Articles;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Articles;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class FilterTemplateController : BaseController
    {
        private readonly IFilterTemplateServices _filterTemplateService;       
        private IMapper _mapper;
        public FilterTemplateController(IFilterTemplateServices filterTemplateService, IMapper mapper)
        {
            _filterTemplateService = filterTemplateService;          
            _mapper = mapper;

        }

        // GET: Admin/FilterTemplates
        public ActionResult Index(int? filterTemplate, string keyword)
        {
            FilterTemplateListVM filterTemplateListVM = GetElements(filterTemplate, keyword);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(filterTemplateListVM);

        }

        private FilterTemplateListVM GetElements(int? filterTemplate, string keyword)
        {
            var filterTemplateListVM = new FilterTemplateListVM();

            filterTemplateListVM.Keyword = keyword;
            filterTemplateListVM.PageIndex = filterTemplate ?? 1;
            filterTemplateListVM.PageSize = SettingsManager.FilterTemplate.PageSize;
            int totalCount;
            var filterTemplatelist = _filterTemplateService.GetPagedElements(filterTemplateListVM.PageIndex-1, filterTemplateListVM.PageSize, filterTemplateListVM.Keyword, out totalCount);
            var filterTemplateVMList = _mapper.Map<List<FilterTemplate>, List<FilterTemplateVM>>(filterTemplatelist);
            filterTemplateListVM.TotalCount = totalCount;
            filterTemplateListVM.FilterTemplates = new StaticPagedList<FilterTemplateVM>(filterTemplateVMList, filterTemplateListVM.PageIndex, filterTemplateListVM.PageSize, filterTemplateListVM.TotalCount); ;
            return filterTemplateListVM;
        }
        [HttpPost]
        public JsonResult FilterTemplateSizeSet(int filterTemplateSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/FilterTemplateSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("FilterTemplateSize").SetValue(filterTemplateSize);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        // GET: Admin/FilterTemplates/Details/5
        public ActionResult Details(int id)
        {
           

            FilterTemplate filterTemplate = _filterTemplateService.GetById(id);
            if (filterTemplate == null)
            {
                return HttpNotFound();
            }
            return View(filterTemplate);
        }

        // GET: Admin/FilterTemplates/Create
        public ActionResult Add()
        {
            FilterTemplateIM filterTemplate = new FilterTemplateIM()
            {
                Active = true
            };
            return View(filterTemplate);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Add(FilterTemplateIM filterTemplate)
        {
            if (!ModelState.IsValid)
            {
                AR.SetSuccess(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                
                var newFilterTemplate = _mapper.Map<FilterTemplateIM, FilterTemplate>(filterTemplate);
                newFilterTemplate.CreatedBy = Site.CurrentUserName;
                newFilterTemplate.CreatedDate = DateTime.Now;
                //FilterTemplate.
                //newFilterTemplate.CreatedDate.
                _filterTemplateService.Create(newFilterTemplate);

              

                //int count;
                //int filterTemplateSize = SettingsManager.FilterTemplate.PageSize;
                //var list = _filterTemplateService.GetPagedElements(0, filterTemplateSize, string.Empty, out count);
                //List<FilterTemplateVM> filterTemplateList = _mapper.Map<List<FilterTemplate>, List<FilterTemplateVM>>(list);
                //AR.Data = RenderPartialViewToString("_FilterTemplateList", filterTemplateList);

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.FilterTemplate));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        // GET: Admin/FilterTemplates/Edit/5
        public ActionResult Edit(int id)
        {


            FilterTemplate filterTemplate = _filterTemplateService.GetById(id);
            if (filterTemplate == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
                // return HttpNotFound();
            }

            var editFilterTemplate = _mapper.Map<FilterTemplate, FilterTemplateIM>(filterTemplate);


            return View(editFilterTemplate);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(FilterTemplateIM filterTemplate)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            var ft = _filterTemplateService.GetById(filterTemplate.Id);
            try
            {
                var eft = _filterTemplateService.GetById(filterTemplate.Id);
                eft.Title = filterTemplate.Title;
                eft.Links = filterTemplate.Links;
                eft.Importance = filterTemplate.Importance;
                eft.Keyword = filterTemplate.Keyword;
                eft.KeywordSet = filterTemplate.KeywordSet;
                eft.Name = filterTemplate.Name;
                eft.PublishDate = filterTemplate.PublishDate;
                eft.Source = filterTemplate.Source;
                eft.Author = filterTemplate.Author;
                eft.Active = filterTemplate.Active;
                eft.Body = filterTemplate.Body;
                eft.Description = filterTemplate.Description;
                eft.LinksContainer = filterTemplate.LinksContainer;
                eft.Encode = filterTemplate.Encode;

                eft.UpdatedBy = Site.CurrentUserName;
                eft.UpdatedDate = DateTime.Now;
              
                // editFilterTemplate = _mapper.Map<FilterTemplateIM, FilterTemplate>(filterTemplate);
                _filterTemplateService.Update(eft);

                
                //var filterTemplateVM = _mapper.Map<FilterTemplateVM>(editFilterTemplate);

                //AR.Id = filterTemplate.Id;
                //AR.Data = RenderPartialViewToString("_FilterTemplateItem", filterTemplateVM);

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.FilterTemplate));
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
            FilterTemplate filterTemplate = _filterTemplateService.GetById(id);
            if (filterTemplate == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.FilterTemplate));
                return Json(AR, JsonRequestBehavior.DenyGet);


            }


            try
            {

                filterTemplate.Active = !filterTemplate.Active;
                _filterTemplateService.Update(filterTemplate);

                var vm = _mapper.Map<FilterTemplateVM>(filterTemplate);
                AR.Data = RenderPartialViewToString("_FilterTemplateItem", vm);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.FilterTemplate));
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
            FilterTemplate filterTemplate = _filterTemplateService.GetById(id);
            if (filterTemplate != null)
            {
                _filterTemplateService.Delete(filterTemplate);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.FilterTemplate));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.FilterTemplate));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


      

       
    }
}