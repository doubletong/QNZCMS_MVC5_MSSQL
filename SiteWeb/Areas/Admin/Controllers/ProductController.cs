using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using QNZ.Data;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model;
using TZGCMS.Model.Admin.ViewModel;


using TZGCMS.Model.Search;
using TZGCMS.Resources.Admin;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ProductController : QNZBaseController
    {


        private IQNZDbContext _db;
        private IMapper _mapper;
        public ProductController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }


        #region  产品

        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Index(int? page, int? categoryId, string keyword)
        {
            var vm = new ProductListVM()
            {
                CategoryId = categoryId ?? 0,
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Product.PageSize,
                Keyword = keyword
            };

            var query = _db.Products.Include(d=>d.ProductCategories).AsQueryable();
            if (categoryId > 0)
            {
                query = query.Where(d => d.ProductCategories.Any(c => c.Id == categoryId));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.ProductName.Contains(keyword));
            }

            var list = await query.OrderByDescending(d => d.Importance)
                .ThenByDescending(d => d.Id).Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize)
                .ProjectTo<ProductVM>().ToListAsync();
                //_productServices.GetPagedElements(vm.PageIndex-1, vm.PageSize, vm.Keyword, (int)vm.CategoryId, out totalCount);

            vm.TotalCount = await query.CountAsync();

            var categoryList = _db.ProductCategories.OrderByDescending(c => c.Importance).ToList();
            var categories = new SelectList(categoryList, "Id", "Title");
            ViewBag.Categories = categories;


            vm.Products = new StaticPagedList<ProductVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);
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

        //public ActionResult AddProduct()
        //{
        //    ProductIM product = new ProductIM
        //    {
        //        Active = true
        //    };

        //    var allCategory = _categoryServices.GetActiveItems().OrderByDescending(c => c.Importance);
        //    //var categoryList = allCategory.Where(d => d.ParentId > 0);
        //    //foreach (var item in categoryList)
        //    //{
        //    //    item.ParentCategory = allCategory.FirstOrDefault(d => d.Id == item.ParentId);
        //    //}
        //    //   var catselectList = categoryList.Select(d => new CategorySelectList { Id = d.Id, Title = d.Title, ParentTitle = d.ParentCategory.Title }).ToList();
        //    // var categories = new SelectList(categoryList, "Id", "Title", "ParentCategory.Title", 0);

        //    ViewBag.Categories = new SelectList(allCategory, "Id", "Title", 0);


        //    return View(product);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult AddProduct(ProductIM vm)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }

        //    try
        //    {
        //        //var newProduct = _mapper.Map<ProductIM, Product>(vm);
        //        //newProduct.ViewCount = 0;
        //        //newProduct.CreatedDate = DateTime.Now;
        //        //newProduct.CreatedBy = Site.CurrentUserName;

        //        //var lCategories = (from c in _categoryServices.GetAll()
        //        //                   where vm.PostCategoryIds.Contains(c.Id.ToString())
        //        //                   select c).ToList();
        //        //foreach(var d in lCategories)
        //        //{
        //        //    newProduct.Categories.Add(d);
        //        //}


        //        var product =  _productServices.Create(vm);

        //        var pageMeta = new PageMeta()
        //        {
        //            ObjectId = vm.Id.ToString(),
        //            Title = string.IsNullOrEmpty(vm.Title) ? $"{vm.ProductName}_{vm.ProductNo}" : vm.Title,
        //            Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
        //            Description = vm.SEODescription,
        //            ModelType = ModelType.PRODUCT
        //        };
        //        _pageMetaServices.Create(pageMeta);


        //        AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Product));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }



        //}

        [HttpGet]
     
        public async Task<ActionResult> Edit(int? id)
        {
            var im = new ProductIM() { Importance = 0, Active = true };
            if (id != null)            
            {
                var vProduct = await _db.Products.Include(d=>d.ProductCategories).FirstOrDefaultAsync(d=>d.Id == id);           

                if (vProduct == null)
                {                
                    return HttpNotFound();
                }


                im = _mapper.Map<ProductIM>(vProduct);
        

                if (vProduct.ProductCategories.Any())
                    im.PostCategoryIds = vProduct.ProductCategories.Select(c => c.Id.ToString()).ToArray();

                var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d=>d.ModelType == (short)ModelType.PRODUCT && d.ObjectId == im.Id.ToString());
                if (pageMeta != null)
                {
                    im.Title = pageMeta.Title;
                    im.Keywords = pageMeta.Keyword;
                    im.SEODescription = pageMeta.Description;
                }
            }

           

            //var categoryList = _categoryRepository.GetActiveItems().OrderByDescending(c => c.Importance).ToList();
            //var lCategories = new SelectList(categoryList, "Id", "Title");
            //ViewBag.Categories = lCategories;

            var allCategory = _db.ProductCategories.OrderByDescending(c => c.Importance);
            var lCategories = allCategory.Where(d => d.ParentId > 0);
            foreach (var item in lCategories)
            {
                item.Parent = allCategory.FirstOrDefault(d => d.Id == item.ParentId);
            }
            //   var catselectList = categoryList.Select(d => new CategorySelectList { Id = d.Id, Title = d.Title, ParentTitle = d.ParentCategory.Title }).ToList();

            // 一级分类
            ViewBag.Categories = new SelectList(allCategory, "Id", "Title", 0);
            // 二级分类
            //ViewBag.Categories = new SelectList(lCategories, "Id", "Title", "ParentCategory.Title", 0);

            return View(im);


        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(ProductIM vm)
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
                    var product = await _db.Products.Include(d=>d.ProductCategories).FirstOrDefaultAsync(d=>d.Id == vm.Id);
                    product = _mapper.Map(vm, product);

                    product.ProductCategories.Clear();
                    if (vm.PostCategoryIds != null)
                    {
                        var lCategories = (from c in _db.ProductCategories
                                           where vm.PostCategoryIds.Contains(c.Id.ToString())
                                           select c).ToList();

                        product.ProductCategories = lCategories;
                    }

                    _db.Entry(product).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    await SetPageMetaAsync(_db, (short)ModelType.PRODUCT, vm.Id.ToString(), vm.ProductName, vm.Title, vm.Keywords, vm.SEODescription);                   
                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Product));

                }
                else
                {
                   
                    var product = _mapper.Map<Product>(vm);
               
                    if (vm.PostCategoryIds != null)
                    {
                        var lCategories = (from c in _db.ProductCategories
                                           where vm.PostCategoryIds.Contains(c.Id.ToString())
                                           select c).ToList();

                        product.ProductCategories = lCategories;
                    }

                    _db.Products.Add(product);
                    await _db.SaveChangesAsync();

                    await SetPageMetaAsync(_db, (short)ModelType.PRODUCT, product.Id.ToString(), vm.ProductName, vm.Title, vm.Keywords, vm.SEODescription);
                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Product));
                }
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);

            }
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public async Task<JsonResult> Recommend(int id)
        {

            Product vProduct = await _db.Products.FindAsync(id);

            if (vProduct == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                vProduct.Recommend = !vProduct.Recommend;
                _db.Entry(vProduct).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                var vm = _mapper.Map<ProductVM>(vProduct);
                AR.Data = RenderPartialViewToString("_ProductItem", vm);

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
        public async Task<JsonResult> Active(int id)
        {

            Product vProduct = await _db.Products.FindAsync(id);

            try
            {
                vProduct.Active = !vProduct.Active;
                _db.Entry(vProduct).State = EntityState.Modified;
                await _db.SaveChangesAsync();

               var vm = _mapper.Map<ProductVM>(vProduct);

                AR.Data = RenderPartialViewToString("_ProductItem", vm);

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
        public async Task<JsonResult> Delete(int id)
        {

            Product vProduct = await _db.Products.FindAsync(id);

            if (vProduct == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _db.Products.Remove(vProduct);
            await _db.SaveChangesAsync();
          

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Product));
            return Json(AR, JsonRequestBehavior.DenyGet);


        }

        ///// <summary>
        ///// 创建索引目录
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult CreateIndex()
        //{
        //    try
        //    {
        //        var list = _productServices.GetActiveElements().Select(m => new SearchData
        //        {
        //            Id = $"PRODUCT{m.Id}",
        //            Name = m.ProductName,
        //            Description = m.Description,
        //            ImageUrl = m.Thumbnail,
        //            Url = $"{SettingsManager.Site.SiteDomainName}/product/detail/{m.Id}"
        //        }).ToList();

        //        LuceneHelper.AddUpdateLuceneIndex(list);

        //        AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Product));
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}



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