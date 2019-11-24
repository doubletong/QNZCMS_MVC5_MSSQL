using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ProductCategoryController :  QNZBaseController
    {

        private IQNZDbContext _db;
        private IMapper _mapper;
        public ProductCategoryController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

       

        #region 产品分类

        public async Task<ActionResult> Index(int? page, string keyword)
        {
            var vm = new CategoryListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Category.PageSize
            };

            var query = _db.ProductCategories.Include(d=>d.ProductCategories).Where(d=>d.ParentId==null).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword));
            }

            var list = await query.OrderByDescending(d => d.Importance).ThenByDescending(d=>d.CreatedDate)
                .Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ToListAsync();


            vm.TotalCount = await query.CountAsync();
            vm.Categories = new StaticPagedList<ProductCategory>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); ;


          

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(vm);

        
        }

        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/CategorySettings.config");
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
            GetParentCategories(null);
            if (id > 0)
            {
                var vCase = await _db.ProductCategories.FindAsync(id);
                if (vCase == null)
                {
                    AR.Setfailure(Messages.HttpNotFound);
                    return Json(AR, JsonRequestBehavior.AllowGet);
                }

                var editCase = _mapper.Map<ProductCategory, ProductCategoryIM>(vCase);

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
                var vm = new ProductCategoryIM
                {
                    Active = true,
                    Importance = 0
                };
                return View(vm);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<JsonResult> Edit(ProductCategoryIM vm)
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
                    var editCase = await _db.ProductCategories.FindAsync(vm.Id);

                    editCase = _mapper.Map(vm, editCase);
                    editCase.UpdatedDate = DateTime.Now;
                    editCase.UpdatedBy = Site.CurrentUserName;

                    _db.Entry(editCase).State = EntityState.Modified;
                    await _db.SaveChangesAsync();


                    await SetPageMetaAsync(_db, (short)ModelType.CATEGORY, editCase.Id.ToString(), editCase.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);



                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Category));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var newCase = _mapper.Map<ProductCategoryIM, ProductCategory>(vm);
                    newCase.CreatedDate = DateTime.Now;
                    newCase.CreatedBy = Site.CurrentUserName;

                    newCase = _db.ProductCategories.Add(newCase);
                    var result = await _db.SaveChangesAsync();

                    if (result > 0)
                    {
                        await SetPageMetaAsync(_db, (short)ModelType.CATEGORY, newCase.Id.ToString(), newCase.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);
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
         
            var category = _db.ProductCategories.Include(d => d.Products).FirstOrDefault(d => d.Id == id);

            if (category.Products.Any())
            {
                AR.Setfailure("此分类下面还有产品存在，不能删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _db.ProductCategories.Remove(category);

            var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.CATEGORY && d.ObjectId == category.Id.ToString());
            if (pageMeta != null)
            {
                _db.PageMetas.Remove(pageMeta);
            }

            await _db.SaveChangesAsync();

       
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Category));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> IsActive(int id)
        {

            var vCategory = await _db.ProductCategories.FindAsync(id);
            if (vCategory == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                vCategory.Active = !vCategory.Active;
                _db.Entry(vCategory).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                var vm = _mapper.Map<ProductCategoryVM>(vCategory);

                AR.Data = RenderPartialViewToString("_CategoryItem", vCategory);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Category));
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
            var result = await IsExistAsync(seoName, id);
            return !result
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(false, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public async Task<bool> IsExistAsync(string seoName, int? id)
        {
            if (id != null)
            {
                return await _db.ProductCategories.CountAsync(d => d.SeoName == seoName && d.Id != id.Value) > 0;
            }

            return await _db.ProductCategories.CountAsync(d => d.SeoName == seoName) > 0;
        }

        private void GetParentCategories(int? id)
        {
            var parentCategories = _db.ProductCategories.Where(d => d.ParentId == null).OrderByDescending(m => m.Importance).ToList();
            if (id > 0)
            {
                parentCategories = parentCategories.Where(c => c.Id != id).ToList();
            }

            var lCategories = parentCategories.Select(r => new SelectListItem()
            {
                Value = r.Id.ToString(),
                Text = r.Title
            }).ToList();

            var emptyItem = new SelectListItem()
            {
                Value = "",
                Text = "--选择父级分类--"
            };

            lCategories.Insert(0, emptyItem);
            ViewBag.ParentCategories = new SelectList(lCategories, "Value", "Text");

        }

        //// 二级下拉
        //public List<SelectListItem> GetCategoryListWithParent()
        //{
        //    var OptGroups = _categoryServices.GetItems().Select(c => new SelectListGroup { Name = c.Title }).ToList();
        //    var categories = _categoryServices.GetCategoriesWithParent();
        //    //   lCategories.Insert(0, new SelectListItem { Text = "--按分类过滤--", Value = "" });

        //    return (from category in categories
        //            let parId = category.ParentId.ToString()
        //            select new SelectListItem
        //            {
        //                Text = category.Title,
        //                Value = category.Id.ToString(),
        //                Group = OptGroups.FirstOrDefault(o => o.Name == category.ParentCategoryTitle)
        //                // Selected = (blogId == blog.Id)
        //            }).ToList();
        //}

        ////单级下拉
        //public List<SelectListItem> GetCategoryList(int catId)
        //{
        //    var OptGroups = _categoryServices.GetItems();
        //    List<SelectListItem> lCategories = new List<SelectListItem>();
        //    foreach (var category in OptGroups)
        //    {
        //        var sli = new SelectListItem
        //        {
        //            Text = category.Title,
        //            Value = category.Id.ToString(),
        //            Selected = (catId == category.Id)
        //        };

        //        lCategories.Add(sli);

        //    }
        //    lCategories.Insert(0, new SelectListItem { Text = "--按分类过滤--", Value = "" });

        //    return lCategories;
        //}

        //// 带组下拉（二级）
        //public List<SelectListItem> GetCategoryListWidthParent(int catId)
        //{
        //    var OptGroups = _categoryServices.GetItems().Select(c => new SelectListGroup { Name = c.Title }).ToList();
        //    var categories = _categoryServices.GetCategoriesWithParent();
        //    List<SelectListItem> lCategories = new List<SelectListItem>();
        //    foreach (var category in categories)
        //    {
        //        var parId = category.ParentId.ToString();
        //        var sli = new SelectListItem
        //        {
        //            Text = category.Title,
        //            Value = category.Id.ToString(),
        //            Group = OptGroups.FirstOrDefault(o => o.Name == category.ParentCategoryTitle),
        //            Selected = (catId == category.Id)
        //        };

        //        lCategories.Add(sli);

        //    }
        //    lCategories.Insert(0, new SelectListItem { Text = "--按分类过滤--", Value = "" });

        //    return lCategories;
        //}

        #endregion
    }
}