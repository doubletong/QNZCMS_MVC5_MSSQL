using AutoMapper;
using PagedList;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Entity.Pages;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Pages;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.LuceneSearch;
using TZGCMS.Model.Admin.ViewModel.Pages;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.LuceneSearch;
using TZGCMS.Service.PageMetas;
using TZGCMS.Service.Pages;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class PageController : BaseController
    {
        private readonly IPageServices _pageServices;
        private readonly IPageMetaServices _pageMetaServices;
        private IMapper _mapper;
        public PageController(IPageServices pageServices, IPageMetaServices pageMetaServices, IMapper mapper)
        {
            _pageServices = pageServices;
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;
        }
       
        // GET: Admin/Pages
        public ActionResult Index(int? page, string keyword)
        {
            PageListVM pageListVM = GetElements(page, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(pageListVM);

        }

        private PageListVM GetElements(int? page, string keyword)
        {
            var pageListVM = new PageListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Page.PageSize
            };
            int totalCount;
            var pagelist = _pageServices.GetPagedElements(pageListVM.PageIndex-1, pageListVM.PageSize, pageListVM.Keyword, out totalCount);

            pageListVM.TotalCount = totalCount;
            pageListVM.Pages = new StaticPagedList<Page>(pagelist, pageListVM.PageIndex, pageListVM.PageSize, pageListVM.TotalCount); ;
            return pageListVM;
        }
        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/PageSettings.config");
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


        // GET: Admin/Pages/Create
        public ActionResult Create()
        {
            PageIM page = new PageIM()
            {
                Active = true
            };
            return PartialView("_Create", page);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Create(PageIM page)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var newPage = _mapper.Map<PageIM, Page>(page);
                newPage.ViewCount = 0;

                var result = _pageServices.Create(newPage);
                if (result!=null)
                {
                    var pageMeta = new PageMeta()
                    {
                        ObjectId = result.ToString(),
                        Title = string.IsNullOrEmpty(page.SEOTitle) ? page.Title : page.SEOTitle,
                        Keyword = string.IsNullOrEmpty(page.Keywords) ? page.Title : page.Keywords.Replace('，', ','),
                        Description = page.SEODescription,
                        ModelType = ModelType.PAGE
                    };
                    _pageMetaServices.Create(pageMeta);
                }


                int count;
                int pageSize = SettingsManager.Page.PageSize;
                var list = _pageServices.GetPagedElements(0, pageSize, string.Empty, out count);

                AR.Data = RenderPartialViewToString("_PageList", list);

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Page));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        // GET: Admin/Pages/Edit/5
        public ActionResult Edit(int id)
        {

            var page = _pageServices.GetById(id);
            if (page == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
                // return HttpNotFound();
            }

            var editPage = _mapper.Map<Page, PageIM>(page);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.PAGE, editPage.Id.ToString());
            if (pageMeta != null)
            {
                editPage.SEOTitle = pageMeta.Title;
                editPage.Keywords = pageMeta.Keyword;
                editPage.SEODescription = pageMeta.Description;
            }

            return PartialView("_Edit", editPage);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(PageIM page)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var old = _pageServices.GetById(page.Id);
                var editPage = _mapper.Map(page, old);


                //editPage.Title = page.Title;
                //editPage.Body = page.Body;
                //editPage.Active = page.Active;
                //editPage.SeoName = page.SeoName;
                //editPage.TemplateName = page.TemplateName;

                _pageServices.Update(editPage);

                var pageMeta = _pageMetaServices.GetPageMeta(ModelType.PAGE, editPage.Id.ToString());
                pageMeta = pageMeta ?? new PageMeta();


                pageMeta.ObjectId = page.Id.ToString();
                pageMeta.Title = string.IsNullOrEmpty(page.SEOTitle) ? page.Title : page.SEOTitle;
                pageMeta.Keyword = string.IsNullOrEmpty(page.Keywords) ? page.Title : page.Keywords.Replace('，', ',');
                pageMeta.Description = page.SEODescription;
                pageMeta.ModelType = ModelType.PAGE;

                if (pageMeta.Id > 0)
                {
                    _pageMetaServices.Update(pageMeta);
                }
                else
                {
                    _pageMetaServices.Create(pageMeta);
                }


                // var pageVM = _mapper.Map<PageVM>(editPage);

                AR.Id = page.Id;
                AR.Data = RenderPartialViewToString("_PageItem", editPage);

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Page));
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
            var page = _pageServices.GetById(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Page));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _pageServices.Update(page);

                AR.Data = RenderPartialViewToString("_PageItem", page);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Page));
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
            var page = _pageServices.GetById(id);
            if (page != null)
            {
                _pageServices.Delete(page);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Page));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Page));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }


        public JsonResult IsSeoNameUnique(string SeoName, int? Id)
        {
            return !IsExist(SeoName, Id)
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(false, JsonRequestBehavior.AllowGet);
        }
        public bool IsExist(string SeoName, int? id)
        {
            return _pageServices.IsExistSeoName(SeoName, id);       
        }


        [HttpPost]
        public JsonResult CreateIndex()
        {

            try
            {
                var list = _pageServices.GetActiveElements().Select(m => new SearchData
                {
                    Id = $"PAGE{m.Id}",
                    Name = m.Title,
                    Description = StringHelper.StripTagsCharArray(m.Body),
                    Url = $"{SettingsManager.Site.SiteDomainName}/page-{m.SeoName}"
                }).ToList();

                GoLucene.AddUpdateLuceneIndex(list);

                AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Page));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }
    }
}