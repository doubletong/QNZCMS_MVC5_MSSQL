using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;

using System;
using System.Xml.Linq;
using System.Text;
using TZGCMS.Service.Cases;
using TZGCMS.Service.PageMetas;
using TZGCMS.Model.Admin.ViewModel.Cases;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Data.Entity.Cases;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.InputModel.Cases;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.LuceneSearch;
using TZGCMS.Model.Admin.ViewModel.LuceneSearch;
using TZGCMS.SiteWeb.Filters;
using TZGCMS.Model;
using System.Data.Entity;
using TZGCMS.Data.Entity;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class CaseController : BaseController
    {

        private readonly IMapper _mapper;

        public CaseController(IMapper mapper)
        {     
            _mapper = mapper;
        }
        #region Cases


        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> IndexAsync(int? page, string Keyword)
        {

            var vm = new CaseListVM
            {
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Case.PageSize,
                Keyword = Keyword
            };

            var query = _db.Cases.AsQueryable();

            if (!string.IsNullOrEmpty(Keyword))
            {
                query = query.Where(d => d.Title.Contains(Keyword) || d.Body.Contains(Keyword));
            }

            vm.TotalCount = await query.CountAsync();


            var cases = await query.Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ToListAsync();
                //_articleServices.GetPagedElements(vm.PageIndex-1, vm.PageSize,  vm.Keyword, (int)vm.CategoryId, out count);

  
            vm.Cases = new StaticPagedList<Case>(cases, vm.PageIndex, vm.PageSize, vm.TotalCount);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(vm);

        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/CaseSettings.config");
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
            var vm = new CaseIM {
                Active = true,          
                Pubdate = DateTime.Now
            };

        
            return View(vm);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(CaseIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                var newCase = _mapper.Map<CaseIM, Case>(vm);
      

                var result = _articleServices.Create(newCase);

                if (result != null)
                {
                    _pageMetaServices.SetPageMeta(ModelType.ARTICLE, result.Id.ToString(),result.Title, vm.SEOTitle,vm.Keywords,vm.SEODescription);

                }

                if (!string.IsNullOrEmpty(vm.Thumbnail))
                {
                    if (ImageHandler.CheckImageSize(Server.MapPath(vm.Thumbnail), SettingsManager.Case.ThumbWidth, SettingsManager.Case.ThumbHeight))
                    {
                        AR.SetWarning(Messages.ThumbnailSizeNotOK);
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }
                    if (ImageHandler.CheckImageType(Server.MapPath(vm.Thumbnail)))
                    {
                        AR.SetWarning(Messages.ThumbnailExtensionNotOK);
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }
                }
                if (!string.IsNullOrEmpty(vm.FullImage))
                {
                    if (ImageHandler.CheckImageSize(Server.MapPath(vm.FullImage), SettingsManager.Case.ImageWidth, SettingsManager.Case.ImageHeight))
                    {
                        AR.SetWarning(Messages.FullImageSizeNotOK);
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }
                    if (ImageHandler.CheckImageType(Server.MapPath(vm.FullImage)))
                    {
                        AR.SetWarning(Messages.FullImageExtensionNotOK);
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }
                }
                   

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Case));
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
            var vCase = _articleServices.GetById(Id);
            if (vCase == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }


            var editCase = _mapper.Map<Case, CaseIM>(vCase);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.ARTICLE, editCase.Id.ToString());
            if (pageMeta != null)
            {
                editCase.SEOTitle = pageMeta.Title;
                editCase.Keywords = pageMeta.Keyword;
                editCase.SEODescription = pageMeta.Description;
            }

            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");

            ViewBag.Categories = lCategorys;

            return View(editCase);


        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(CaseIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
               
                var editCase = _mapper.Map<CaseIM, Case>(vm);

                _articleServices.Update(editCase);
                _pageMetaServices.SetPageMeta(ModelType.ARTICLE, vm.Id.ToString(), vm.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);

                //图片检测
                if (!string.IsNullOrEmpty(vm.Thumbnail))
                {
                    if (!ImageHandler.CheckImageSize(Server.MapPath(vm.Thumbnail), SettingsManager.Case.ThumbWidth, SettingsManager.Case.ThumbHeight))
                    {
                        AR.SetWarning(Messages.ThumbnailSizeNotOK);
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }
                    if (!ImageHandler.CheckImageType(Server.MapPath(vm.Thumbnail)))
                    {
                        AR.SetWarning(Messages.ThumbnailExtensionNotOK);
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }
                }
                if (!string.IsNullOrEmpty(vm.FullImage))
                {
                    if (!ImageHandler.CheckImageSize(Server.MapPath(vm.FullImage), SettingsManager.Case.ImageWidth, SettingsManager.Case.ImageHeight))
                    {
                        AR.SetWarning(Messages.FullImageSizeNotOK);
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }
                    if (!ImageHandler.CheckImageType(Server.MapPath(vm.FullImage)))
                    {
                        AR.SetWarning(Messages.FullImageExtensionNotOK);
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }
                }

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Case));
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

            Case vCase = _articleServices.GetById(id);

            try
            {
                vCase.Active = !vCase.Active;
                _articleServices.Update(vCase);

                vCase.CaseCategory = _categoryServices.GetById(vCase.CategoryId);
                
                AR.Data = RenderPartialViewToString("_CaseItem", vCase);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Case));
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
        public JsonResult Recommend(int id)
        {

            Case vCase = _articleServices.GetById(id);

            try
            {
                vCase.Recommend = !vCase.Recommend;
                _articleServices.Update(vCase);

                vCase.CaseCategory = _categoryServices.GetById(vCase.CategoryId);

                AR.Data = RenderPartialViewToString("_CaseItem", vCase);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Case));
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

            Case vCase = _articleServices.GetById(id);

            if (vCase == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _articleServices.Delete(vCase);

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Case));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }
        /// <summary>
        /// 创建文章索引
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateIndex()
        {
            try
            {
                var list = _articleServices.GetActiveElements().Select(m => new SearchData
                {
                    Id = $"ARTICLE{m.Id}",
                    Name = m.Title,
                    Description = string.IsNullOrEmpty(m.Summary)? StringHelper.StripTagsCharArray(m.Body) : m.Summary,
                    ImageUrl = string.IsNullOrEmpty(m.Thumbnail)?string.Empty: m.Thumbnail,
                    Url = m.CategoryId == 1 ? $"{SettingsManager.Site.SiteDomainName}/news/detail/{m.Id}" : $"{SettingsManager.Site.SiteDomainName}/news/business/{m.Id}"
                }).ToList();
                //var products = _mapper.Map<List<Product>, List<SearchData>>(list);

                GoLucene.AddUpdateLuceneIndex(list);
               
                AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Case));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        #endregion


    }
}
