using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.PageMetas;
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
    public class SimpleProductController : BaseController
    {

     
        private readonly IMapper _mapper;

        public SimpleProductController(IMapper mapper)
        {
         
            _mapper = mapper;

        }


        #region  产品

        [HttpGet]
        public async Task<ActionResult> Index(int? page, string keyword)
        {
            var vm = new SimpleProductListVM
            {
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Product.PageSize,
                Keyword = keyword
            };

            var query = _db.SimpleProducts.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.ProductName.Contains(keyword) || d.ProductNo.Contains(keyword) || d.Body.Contains(keyword));
            }

            vm.TotalCount = await query.CountAsync();


            var cases = await query.OrderByDescending(d => d.Importance).ThenByDescending(d => d.Id)
                .Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize)
                .ProjectTo<SimpleProductVM>(_mapper.ConfigurationProvider).ToListAsync();
            

            vm.Products = new StaticPagedList<SimpleProductVM>(cases, vm.PageIndex, vm.PageSize, vm.TotalCount);

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
        //    var product = new SimpleSimpleProductIM
        //    {
        //        Active = true
        //    };


        //    return View(product);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult AddProduct(SimpleProductIM vm)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }

        //    try
        //    {        

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
            if (id > 0)
            {
                var vProduct = await _db.SimpleProducts.FindAsync(id);
           
                if (vProduct == null)
                {                
                    return HttpNotFound();
                }

                var editProduct = _mapper.Map<SimpleProduct, SimpleProductIM>(vProduct);
                var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.SIMPLEPRODUCT && d.ObjectId == editProduct.Id.ToString());
                if (pageMeta != null)
                {
                    editProduct.Title = pageMeta.Title;
                    editProduct.Keywords = pageMeta.Keyword;
                    editProduct.SEODescription = pageMeta.Description;
                }
                return View(editProduct);
            }
            else
            {
                var product = new SimpleProductIM
                {
                    Active = true,
                    Importance = 0
                    
                };
                return View(product);
            }        


        }



        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(SimpleProductIM vm)
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
                    var editProduct = await _db.SimpleProducts.FindAsync(vm.Id);
                    editProduct = _mapper.Map(vm, editProduct);
                    _db.Entry(editProduct).State = EntityState.Modified;
                    var result = await _db.SaveChangesAsync();

                    if (result>0)
                    {
                        var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.SIMPLEPRODUCT && d.ObjectId == editProduct.Id.ToString());

                        pageMeta = pageMeta ?? new PageMeta();
                        pageMeta.ObjectId = vm.Id.ToString();
                        pageMeta.Title = string.IsNullOrEmpty(vm.Title) ? vm.Title : vm.Title;
                        pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
                        pageMeta.Description = vm.SEODescription;
                        pageMeta.ModelType = ModelType.SIMPLEPRODUCT;

                        if (pageMeta.Id > 0)
                        {
                            _db.Entry(pageMeta).State = EntityState.Modified;                           
                        }
                        else
                        {
                            _db.PageMetas.Add(pageMeta);
                        }
                        await _db.SaveChangesAsync();


                        AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Product));

                    }
                    else
                    {
                        AR.Setfailure(String.Format(Messages.AlertUpdateFailure, EntityNames.Product));
                    }
                }
                else
                {

                    var product = _mapper.Map<SimpleProductIM,SimpleProduct>(vm);
                    _db.SimpleProducts.Add(product);
                    var result = await _db.SaveChangesAsync();
                 
                    if (result > 0)
                    {
                        var pageMeta = new PageMeta()
                        {
                            ObjectId = vm.Id.ToString(),
                            Title = string.IsNullOrEmpty(vm.Title) ? $"{vm.ProductName}_{vm.ProductNo}" : vm.Title,
                            Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                            Description = vm.SEODescription,
                            ModelType = ModelType.SIMPLEPRODUCT
                        };
                        _db.PageMetas.Add(pageMeta);
                        await _db.SaveChangesAsync();
                        AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Product));

                    }
                    else
                    {
                        AR.Setfailure(String.Format(Messages.AlertCreateFailure, EntityNames.Product));
                    }
                                    
                   
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

            var vProduct = await _db.SimpleProducts.FindAsync(id);             

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
        public async Task<JsonResult> Active(int id)
        {

            var vProduct = await _db.SimpleProducts.FindAsync(id);

            try
            {
                vProduct.Active = !vProduct.Active;
                _db.Entry(vProduct).State = EntityState.Modified;
                await _db.SaveChangesAsync();

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
        public async Task<JsonResult> Delete(int id)
        {

            var vProduct = await _db.SimpleProducts.FindAsync(id);

            if (vProduct == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            _db.SimpleProducts.Remove(vProduct);
            await _db.SaveChangesAsync();
           

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

                var list = _db.SimpleProducts.Where(m=>m.Active).Select(m => new SearchData
                {
                    Id = "SP"+ m.Id,
                    Name = m.ProductName,
                    Description = m.Summary,
                    ImageUrl = m.Thumbnail,
                    Url = SettingsManager.Site.SiteDomainName+"/products/detail/"+ m.Id,
                    CreatedDate = m.CreatedDate
                }).ToList();

                LuceneHelper.AddUpdateLuceneIndex(list);

                AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Product));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }



        //[HttpPost]
        //public ActionResult UpLoadImages(string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file)
        //{
        //    string filePathName = string.Empty;

        //    string dir = "/Uploads/Images/Products";
        //    string localPath = HostingEnvironment.MapPath(dir);
        //    // string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Uploads");
        //    if (Request.Files.Count == 0)
        //    {
        //        return Json(new { jsonrpc = 2.0, error = new { code = 102, message = "保存失败" }, id = "id" });
        //    }
        //    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //    string ex = Path.GetExtension(file.FileName);
        //    fileName = FileHelper.GetFileName(fileName, localPath, ex);
        //    filePathName = fileName + ex;
        //    if (!Directory.Exists(localPath))
        //    {
        //        Directory.CreateDirectory(localPath);
        //    }


        //    var orgUrl = Path.Combine(localPath, filePathName);
        //    file.SaveAs(orgUrl);


        //    return Json(new
        //    {
        //        jsonrpc = "2.0",
        //        id = id,
        //        fileName = dir + "/" + filePathName
        //    });

        //}
        //[HttpPost]
        //public ActionResult RemoveImage(string img)
        //{
        //    try
        //    {
        //        string dir = "/Uploads/Images/Porducts";
        //        string localPath = HostingEnvironment.MapPath(dir);
        //        var orgUrl = Path.Combine(localPath, img);


        //        if (System.IO.File.Exists(orgUrl))
        //        {
        //            System.IO.File.Delete(orgUrl);
        //        }

        //        AR.SetSuccess(Messages.AlertActionSuccess);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        AR.Setfailure(ex.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}


        #endregion

        
    }
}