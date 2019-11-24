using AutoMapper;
using PagedList;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Entity.Pages;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Pages;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Pages;
using TZGCMS.Resources.Admin;
using TZGCMS.Model;
using TZGCMS.Model.Search;
using System.Data.Entity;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class PageController : BaseController
    {
   
        private IMapper _mapper;
        public PageController( IMapper mapper)
        {         
            _mapper = mapper;
        }
       
        // GET: Admin/Pages
        public async System.Threading.Tasks.Task<ActionResult> Index(int? page, string keyword)
        {
            PageListVM pageListVM = await GetElementsAsync(page, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(pageListVM);

        }

        private async System.Threading.Tasks.Task<PageListVM> GetElementsAsync(int? page, string keyword)
        {
            var vm = new PageListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Page.PageSize
            };
            var query = _db.Pages.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword));
            }

            var pagelist = await query.OrderByDescending(d => d.Id).Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ToListAsync();
            

            vm.TotalCount = await query.CountAsync();
            vm.Pages = new StaticPagedList<Page>(pagelist, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
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
                newPage.SeoName = newPage.SeoName.ToLower();

                var result = _db.Pages.Add(newPage);
                _db.SaveChanges();

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
                    _db.PageMetas.Add(pageMeta);
                    _db.SaveChanges();
                }

              
                int pageSize = SettingsManager.Page.PageSize;
                var list = _db.Pages.OrderByDescending(d => d.Id).Take(pageSize).ToList();                   

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

            var page = _db.Pages.Find(id);
            if (page == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
                // return HttpNotFound();
            }

            var editPage = _mapper.Map<Page, PageIM>(page);

            var pageMeta = _db.PageMetas.FirstOrDefault(d => d.ModelType == ModelType.PAGE && d.ObjectId == editPage.Id.ToString());
         
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
                var editPage = _db.Pages.Find(page.Id);
                editPage = _mapper.Map(page, editPage);
                editPage.SeoName = editPage.SeoName.ToLower();

                _db.Entry(editPage).State = EntityState.Modified;
                _db.SaveChanges();

                var pageMeta = _db.PageMetas.FirstOrDefault(d => d.ModelType == ModelType.PAGE && d.ObjectId == editPage.Id.ToString());
                //_pageMetaServices.GetPageMeta(ModelType.PAGE, editPage.Id.ToString());
                pageMeta = pageMeta ?? new PageMeta();


                pageMeta.ObjectId = page.Id.ToString();
                pageMeta.Title = string.IsNullOrEmpty(page.SEOTitle) ? page.Title : page.SEOTitle;
                pageMeta.Keyword = string.IsNullOrEmpty(page.Keywords) ? page.Title : page.Keywords.Replace('，', ',');
                pageMeta.Description = page.SEODescription;
                pageMeta.ModelType = ModelType.PAGE;

                if (pageMeta.Id > 0)
                {
                    _db.Entry(pageMeta).State = EntityState.Modified;
                   
                }
                else
                {
                    _db.PageMetas.Add(pageMeta);
                }
                _db.SaveChanges();

  

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
            var page = _db.Pages.Find(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Page));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _db.Entry(page).State = EntityState.Modified;
                _db.SaveChanges();

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
            var page = _db.Pages.Find(id);
            if (page != null)
            {
                _db.Pages.Remove(page);
                _db.SaveChanges();
                //_pageServices.Delete(page);

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
            if (id != null)
            {
                return _db.Pages.Count(d => d.SeoName == SeoName && d.Id != id.Value) > 0;
            }

            return _db.Pages.Count(d => d.SeoName == SeoName) > 0;
        }


        [HttpPost]
        public JsonResult CreateIndex()
        {

            try
            {
                var list = _db.Pages.Where(d=>d.Active).Select(m => new SearchData
                {
                    Id = $"PAGE{m.Id}",
                    Name = m.Title,
                    Description = StringHelper.StripTagsCharArray(m.Body),
                    Url = $"{SettingsManager.Site.SiteDomainName}/page-{m.SeoName}"
                }).ToList();

                LuceneHelper.AddUpdateLuceneIndex(list);

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