using AutoMapper;
using PagedList;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;

using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;

using System.Data.Entity;
using System.Threading.Tasks;
using QNZ.Data;
using TZGCMS.Model;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ArticleCategoryController : QNZBaseController
    {

        private IQNZDbContext _db;
        private IMapper _mapper;
        public ArticleCategoryController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: Admin/ArticleCategory

        #region 新闻分类

        public async Task<ActionResult> Index(int? page, string keyword)
        {
            ArticleCategoryListVM categoryListVM = await GetElementsAsync(page, keyword);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(categoryListVM);

        }

        private async Task<ArticleCategoryListVM> GetElementsAsync(int? page, string keyword)
        {
            var vm = new ArticleCategoryListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Article.PageSize
            };
            var query = _db.ArticleCategories.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword));
            }
        
            var list = await query.OrderByDescending(d => d.Importance)
                .Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ToListAsync();
                //_categoryServices.GetPagedElements(vm.PageIndex-1, vm.PageSize, vm.Keyword, out totalCount);

            vm.TotalCount = await query.CountAsync();
            vm.Categories = new StaticPagedList<ArticleCategory>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
        }

        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/ArticleSettings.config");
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
        public async Task<ActionResult> Edit(int? id)
        {

            if (id > 0)
            {
                var vCase = await _db.ArticleCategories.FindAsync(id);
                if (vCase == null)
                {
                    AR.Setfailure(Messages.HttpNotFound);
                    return Json(AR, JsonRequestBehavior.AllowGet);
                }

                var editCase = _mapper.Map<ArticleCategory, ArticleCategoryIM>(vCase);

                var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.ARTICLECATEGORY && d.ObjectId == editCase.Id.ToString());
                if (pageMeta != null)
                {
                    editCase.SEOTitle = pageMeta.Title;
                    editCase.Keywords = pageMeta.Keyword;
                    editCase.SEODescription = pageMeta.Description;
                }

                return View(editCase);

            }
            else
            {
                var vm = new ArticleCategoryIM
                {
                    Active = true,
                    Importance = 0
                };
                return View(vm);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<JsonResult> Edit(ArticleCategoryIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                if (vm.Id > 0)
                {
                    var editCase = await _db.ArticleCategories.FindAsync(vm.Id);

                    editCase = _mapper.Map(vm, editCase);
                    editCase.UpdatedDate = DateTime.Now;
                    editCase.UpdatedBy = Site.CurrentUserName;

                    _db.Entry(editCase).State = EntityState.Modified;
                    await _db.SaveChangesAsync();


                    await SetPageMetaAsync(_db, (short)ModelType.ARTICLECATEGORY, editCase.Id.ToString(), editCase.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);
                 


                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Category));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var newCase = _mapper.Map<ArticleCategoryIM, ArticleCategory>(vm);
                    newCase.CreatedDate = DateTime.Now;
                    newCase.CreatedBy = Site.CurrentUserName;

                    newCase = _db.ArticleCategories.Add(newCase);
                    var result = await _db.SaveChangesAsync();

                    if (result > 0)
                    {
                        await SetPageMetaAsync(_db, (short)ModelType.ARTICLECATEGORY, newCase.Id.ToString(), newCase.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);                     
                    }

                    AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Category));
                    return Json(AR, JsonRequestBehavior.DenyGet);

                }

            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }





        }

        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var category = _db.ArticleCategories.Include(d=>d.Articles).FirstOrDefault(d=>d.Id == id);
        
            if (category.Articles.Any())
            {
                AR.Setfailure("此分类下面还有文章存在，不能删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _db.ArticleCategories.Remove(category);

            var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.ARTICLECATEGORY && d.ObjectId == category.Id.ToString());
            if (pageMeta != null)
            {
                _db.PageMetas.Remove(pageMeta);
            }          

            await  _db.SaveChangesAsync();

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.ArticleCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {

            var vCategory = _db.ArticleCategories.Find(id);
                //_categoryServices.GetById(id);
            if (vCategory == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                vCategory.Active = !vCategory.Active;
                _db.Entry(vCategory).State = EntityState.Modified;
                _db.SaveChanges();
               // _categoryServices.Update(vCategory);

               // var vm = _mapper.Map<ArticleCategoryVM>(vCategory);

                AR.Data = RenderPartialViewToString("_CategoryItem", vCategory);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.ArticleCategory));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        

        [AllowAnonymous]
        public async Task<JsonResult> IsSeoNameUnique(string seoName, int? id)
        {
            var result = await IsExistSeoName(seoName, id);
            return !result
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(false, JsonRequestBehavior.AllowGet);
        }

        public async Task<bool> IsExistSeoName(string seoName, int? id)
        {
            if (id != null)
            {
                return await _db.ArticleCategories.CountAsync(d => d.SeoName == seoName && d.Id != id.Value) > 0;
            }

            return await _db.ArticleCategories.CountAsync(d => d.SeoName == seoName) > 0;
        }
        #endregion
    }
}