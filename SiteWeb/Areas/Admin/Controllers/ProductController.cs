using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Products;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.LuceneSearch;
using TZGCMS.Model.Admin.ViewModel.Products;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.LuceneSearch;
using TZGCMS.Service.PageMetas;
using TZGCMS.Service.Products;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ProductController : BaseController
    {

        private readonly IProductCategoryServices _categoryServices;
        private readonly IProductServices _productServices;
        private readonly IPageMetaServices _pageMetaServices;
        private readonly IMapper _mapper;

        public ProductController(IProductCategoryServices categoryServices, IProductServices productServices, IPageMetaServices pageMetaServices, IMapper mapper)
        {
            _categoryServices = categoryServices;
            _productServices = productServices;
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;

        }


        #region  产品

        [HttpGet]
        public ActionResult Index(int? page, int? categoryId, string keyword)
        {
            var vm = new ProductListVM()
            {
                CategoryId = categoryId ?? 0,
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Product.PageSize,
                Keyword = keyword
            };
            int totalCount = 0;
            var list = _productServices.GetPagedElements(vm.PageIndex-1, vm.PageSize, vm.Keyword, (int)vm.CategoryId, out totalCount);

            vm.TotalCount = totalCount;

            var categoryList = _categoryServices.GetAll().OrderByDescending(c => c.Importance).ToList();
            var categories = new SelectList(categoryList, "Id", "Title");
            ViewBag.Categories = categories;

            //foreach (var item in list)
            //{
            //    item.Categories = _categoryServices.GetItemsByProductId(item.Id);
            //}

            vm.Products = new StaticPagedList<Product>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(vm);
        }



        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/ProductSettings.config");
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

        public ActionResult AddProduct()
        {
            ProductIM product = new ProductIM
            {
                Active = true
            };

            var allCategory = _categoryServices.GetActiveItems().OrderByDescending(c => c.Importance);
            //var categoryList = allCategory.Where(d => d.ParentId > 0);
            //foreach (var item in categoryList)
            //{
            //    item.ParentCategory = allCategory.FirstOrDefault(d => d.Id == item.ParentId);
            //}
            //   var catselectList = categoryList.Select(d => new CategorySelectList { Id = d.Id, Title = d.Title, ParentTitle = d.ParentCategory.Title }).ToList();
            // var categories = new SelectList(categoryList, "Id", "Title", "ParentCategory.Title", 0);

            ViewBag.Categories = new SelectList(allCategory, "Id", "Title", 0);
            

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddProduct(ProductIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);

            }

            try
            {
                //var newProduct = _mapper.Map<ProductIM, Product>(vm);
                //newProduct.ViewCount = 0;
                //newProduct.CreatedDate = DateTime.Now;
                //newProduct.CreatedBy = Site.CurrentUserName;

                //var lCategories = (from c in _categoryServices.GetAll()
                //                   where vm.PostCategoryIds.Contains(c.Id.ToString())
                //                   select c).ToList();
                //foreach(var d in lCategories)
                //{
                //    newProduct.Categories.Add(d);
                //}


                var product =  _productServices.Create(vm);

                var pageMeta = new PageMeta()
                {
                    ObjectId = vm.Id.ToString(),
                    Title = string.IsNullOrEmpty(vm.Title) ? $"{vm.ProductName}_{vm.ProductNo}" : vm.Title,
                    Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                    Description = vm.SEODescription,
                    ModelType = ModelType.PRODUCT
                };
                _pageMetaServices.Create(pageMeta);


                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Product));
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }



        }

        [HttpGet]
        public ActionResult EditProduct(int id)
        {

            var vProduct = _productServices.GetById(id);
            //var categories = _categoryServices.GetItemsByProductId(id);

            if (vProduct == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var editProduct = _mapper.Map<Product, ProductIM>(vProduct);

            if (vProduct.Categories.Any())
                editProduct.PostCategoryIds = vProduct.Categories.Select(c => c.Id.ToString()).ToArray();

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.PRODUCT, editProduct.Id.ToString());
            if (pageMeta != null)
            {
                editProduct.Title = pageMeta.Title;
                editProduct.Keywords = pageMeta.Keyword;
                editProduct.SEODescription = pageMeta.Description;
            }

            //var categoryList = _categoryRepository.GetActiveItems().OrderByDescending(c => c.Importance).ToList();
            //var lCategories = new SelectList(categoryList, "Id", "Title");
            //ViewBag.Categories = lCategories;

            var allCategory = _categoryServices.GetActiveItems().OrderByDescending(c => c.Importance);
            var lCategories = allCategory.Where(d => d.ParentId > 0);
            foreach (var item in lCategories)
            {
                item.ParentCategory = allCategory.FirstOrDefault(d => d.Id == item.ParentId);
            }
            //   var catselectList = categoryList.Select(d => new CategorySelectList { Id = d.Id, Title = d.Title, ParentTitle = d.ParentCategory.Title }).ToList();

            // 一级分类
            ViewBag.Categories = new SelectList(allCategory, "Id", "Title", 0);
            // 二级分类
            //ViewBag.Categories = new SelectList(lCategories, "Id", "Title", "ParentCategory.Title", 0);
        
            return View(editProduct);


        }



        [HttpPost]

        [ValidateAntiForgeryToken]
        public JsonResult EditProduct(ProductIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


            try
            {
              

                var result =  _productServices.Update(vm);
                if (result)
                {
                    var pageMeta = _pageMetaServices.GetPageMeta(ModelType.PRODUCT, vm.Id.ToString());
                    pageMeta = pageMeta ?? new PageMeta();


                    pageMeta.ObjectId = vm.Id.ToString();
                    pageMeta.Title = string.IsNullOrEmpty(vm.Title) ? vm.Title : vm.Title;
                    pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
                    pageMeta.Description = vm.SEODescription;
                    pageMeta.ModelType = ModelType.PRODUCT;

                    if (pageMeta.Id > 0)
                    {
                        _pageMetaServices.Update(pageMeta);
                    }
                    else
                    {
                        _pageMetaServices.Create(pageMeta);
                    }
                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Product));

                }
                else
                {
                    AR.Setfailure(String.Format(Messages.AlertUpdateFailure, EntityNames.Product));
                }           
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
              
            }
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public JsonResult Recommend(int id)
        {

            Product vProduct = _productServices.GetById(id);

            if (vProduct == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                vProduct.Recommend = !vProduct.Recommend;
                _productServices.Update(vProduct);

                //   var vm = _mapper.Map<ProductVM>(vProduct);
                AR.Data = RenderPartialViewToString("_ProductItem", vProduct);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Product));
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
        public JsonResult Active(int id)
        {

            Product vProduct = _productServices.GetById(id);

            try
            {
                vProduct.Active = !vProduct.Active;
                _productServices.Update(vProduct);

                //   var vm = _mapper.Map<ProductVM>(vProduct);
                AR.Data = RenderPartialViewToString("_ProductItem", vProduct);

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Product));
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

            // Product vProduct = _productRepository.GetById(id);

            //if (vProduct == null)
            //{
            //    AR.Setfailure(Messages.HttpNotFound);
            //    return Json(AR, JsonRequestBehavior.DenyGet);
            //}

            _productServices.Delete(id);

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Product));
            return Json(AR, JsonRequestBehavior.DenyGet);


        }

        /// <summary>
        /// 创建索引目录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateIndex()
        {
            try
            {
                var list = _productServices.GetActiveElements().Select(m => new SearchData
                {
                    Id = $"PRODUCT{m.Id}",
                    Name = m.ProductName,
                    Description = m.Description,
                    ImageUrl = m.Thumbnail,
                    Url = $"{SettingsManager.Site.SiteDomainName}/product/detail/{m.Id}"
                }).ToList();

                GoLucene.AddUpdateLuceneIndex(list);

                AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Product));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }



        [HttpPost]
        public ActionResult UpLoadImages(string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file)
        {
            string filePathName = string.Empty;

            string dir = "/Uploads/Images/Products";
            string localPath = HostingEnvironment.MapPath(dir);
            // string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Uploads");
            if (Request.Files.Count == 0)
            {
                return Json(new { jsonrpc = 2.0, error = new { code = 102, message = "保存失败" }, id = "id" });
            }
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string ex = Path.GetExtension(file.FileName);
            fileName = FileHelper.GetFileName(fileName, localPath, ex);
            filePathName = fileName + ex;
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }


            var orgUrl = Path.Combine(localPath, filePathName);
            file.SaveAs(orgUrl);


            return Json(new
            {
                jsonrpc = "2.0",
                id = id,
                fileName = dir + "/" + filePathName
            });

        }
        [HttpPost]
        public ActionResult RemoveImage(string img)
        {
            try
            {
                string dir = "/Uploads/Images/Porducts";
                string localPath = HostingEnvironment.MapPath(dir);
                var orgUrl = Path.Combine(localPath, img);


                if (System.IO.File.Exists(orgUrl))
                {
                    System.IO.File.Delete(orgUrl);
                }

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }


        #endregion

        #region 产品图片
        //[HttpGet]
        // public ActionResult UploadPhotos(int productId)
        // {

        //     var photo = new ProductPhoto
        //     {
        //         ProductId = productId
        //     };

        //     return View(photo);
        // }
        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult GetPhotos(int productId)
        //{

        //   var  Photos = _productPhotoRepository.GetPhotos(productId);
        //   return PartialView("_Photos", Photos);
        //}


        //[HttpPost]       
        // public ActionResult AddPhoto(ProductPhoto photo)
        // {
        //    AR.Data = photo.ProductId;
        //    //photo.AddedDate = DateTime.Now;
        //    //photo.AddedBy = Site.CurrentUserName;
        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    _productPhotoRepository.Create(photo);

        //    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.ProductPhoto));
        //    return Json(AR, JsonRequestBehavior.DenyGet);            

        // }

        // // DELETE: /Product/DeletePhoto
        // [HttpPost]      
        // public JsonResult DeletePhoto(int id)
        // {

        //     var photo = _productPhotoRepository.GetById(id);
        //    AR.Data = photo.ProductId;
        //     if (photo == null)
        //    {
        //        AR.Setfailure(Messages.HttpNotFound);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }               

        //    _productPhotoRepository.Delete(photo);
        //    AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.ProductPhoto));
        //    return Json(AR, JsonRequestBehavior.DenyGet);
        //}


        #endregion
    }
}