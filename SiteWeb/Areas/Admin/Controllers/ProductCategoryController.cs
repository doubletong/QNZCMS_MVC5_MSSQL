using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Products;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Products;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.PageMetas;
using TZGCMS.Service.Products;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ProductCategoryController :  BaseController
    {
        private readonly IProductCategoryServices _categoryServices;
        private readonly IPageMetaServices _pageMetaServices;
        private readonly IMapper _mapper;

        public ProductCategoryController(IProductCategoryServices categoryServices, IPageMetaServices pageMetaServices,IMapper mapper)
        {
            _categoryServices = categoryServices;         
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;

        }
        // GET: Admin/Category

        #region 产品分类

        public ActionResult Index(int? page, string keyword)
        {
            CategoryListVM categoryListVM = GetElements(page, keyword);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(categoryListVM);

            //List<CategoryVM> vm = GetCategoryList();
            //return View(vm);
        }

        private CategoryListVM GetElements(int? page, string keyword)
        {
            var vm = new CategoryListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Category.PageSize
            };
            int totalCount;
            var list = _categoryServices.GetPagedElements(vm.PageIndex-1, vm.PageSize, vm.Keyword, out totalCount);

            //var allCategory = _categoryServices.GetAll();
            //foreach (var c in list)
            //{
            //    c.ChildCategories = allCategory.Where(d => d.ParentId == c.Id);
            //}

            vm.TotalCount = totalCount;
            vm.Categories = new StaticPagedList<ProductCategory>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
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
        public ActionResult Add()
        {

            ProductCategoryIM vCategory = new ProductCategoryIM()
            {
                Active = true,
                Importance = 0
            };
            GetParentCategories(null);
            return PartialView("_Add", vCategory);
        }



        [HttpPost]
        public JsonResult Add(ProductCategoryIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var newCategory = _mapper.Map<ProductCategoryIM, ProductCategory>(vm);
            newCategory.CreatedBy = Site.CurrentUserName;
            newCategory.CreatedDate = DateTime.Now;

            var result = _categoryServices.Create(newCategory);

            if (result!=null)
            {
                var pageMeta = new PageMeta()
                {
                    ObjectId = result.ToString(),
                    Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
                    Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                    Description = vm.SEODescription,
                    ModelType = ModelType.CATEGORY
                };
                _pageMetaServices.Create(pageMeta);
            }


            int count;
            var pageSize = SettingsManager.Category.PageSize;
            var list = _categoryServices.GetPagedElements(1, pageSize, string.Empty, out count);
            AR.Data = RenderPartialViewToString("_CategoryList", list);
            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Category));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpGet]
        public ActionResult Edit(int id)
        {

            ProductCategory category = _categoryServices.GetById(id);
            if (category == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var vm = _mapper.Map<ProductCategoryIM>(category);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.CATEGORY, category.Id.ToString());
            if (pageMeta != null)
            {
                vm.SEOTitle = pageMeta.Title;
                vm.Keywords = pageMeta.Keyword;
                vm.SEODescription = pageMeta.Description;
            }

            GetParentCategories(id);

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Edit(ProductCategoryIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            var newCategory = _categoryServices.GetById(vm.Id);
            newCategory.Title = vm.Title;
            newCategory.SeoName = vm.SeoName;
            newCategory.Importance = vm.Importance;
            newCategory.Active = vm.Active;
            newCategory.ImageUrl = vm.ImageUrl;
            if (vm.Id != vm.ParentId)
            {
                newCategory.ParentId = vm.ParentId;
            }
            newCategory.UpdatedBy = Site.CurrentUserName;
            newCategory.UpdatedDate = DateTime.Now;
            //var newCategory = _mapper.Map<ProductCategoryIM, Category>(vm);

            _categoryServices.Update(newCategory);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.CATEGORY, vm.Id.ToString());
            pageMeta = pageMeta ?? new PageMeta();

            pageMeta.ObjectId = vm.Id.ToString();
            pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
            pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
            pageMeta.Description = vm.SEODescription;
            pageMeta.ModelType = ModelType.CATEGORY;

            if (pageMeta.Id > 0)
            {
                _pageMetaServices.Update(pageMeta);
            }
            else
            {
                _pageMetaServices.Create(pageMeta);
            }


            //   var category = _mapper.Map<CategoryVM>(newCategory);
            AR.Id = newCategory.Id;
            AR.Data = RenderPartialViewToString("_CategoryItem", newCategory);

            AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Category));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }







        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            //var articleCount = _articleRepository.GetCountByParentId(id);

            //if (articleCount>0)
            //{
            //    AR.Setfailure("此分类下面还有文章存在，不能删除！");
            //    return Json(AR, JsonRequestBehavior.DenyGet);
            //}

            _categoryServices.Delete(id);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Category));
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

                // var vm = _mapper.Map<CategoryVM>(vCategory);

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
        public JsonResult IsSeoNameUnique(string seoName, int? Id)
        {
            return !IsExist(seoName, Id)
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(false, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public bool IsExist(string seoName, int? id)
        {
            return _categoryServices.IsExistSeoName(seoName, id);          
        }

        private void GetParentCategories(int? id)
        {
            var parentCategories = _categoryServices.GetAll().Where(d => d.ParentId == null).OrderByDescending(m => m.Importance).ToList();
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