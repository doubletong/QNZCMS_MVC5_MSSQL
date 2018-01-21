using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;

using System;
using System.Xml.Linq;
using TZGCMS.Service.PageMetas;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using TZGCMS.SiteWeb.Filters;
using TZGCMS.Service.Doc;
using TZGCMS.Model.Admin.ViewModel.Doc;
using TZGCMS.Data.Entity.Doc;
using TZGCMS.Model.Admin.InputModel.Doc;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class DocumentController : BaseController
    {
        private readonly IDocumentCategoryServices _categoryServices;
        private readonly IDocumentServices _documentServices;
        private readonly IPageMetaServices _pageMetaServices;       
       
        private readonly IMapper _mapper;

        public DocumentController(IDocumentCategoryServices categoryServices,
            IDocumentServices DocumentServices,
            IPageMetaServices pageMetaServices,           
            IMapper mapper)
        {
            _categoryServices = categoryServices;
            _documentServices = DocumentServices;
            _pageMetaServices = pageMetaServices;          
            _mapper = mapper;


        }
        #region 新闻


        [HttpGet]
        public ActionResult Index(int? page, int? categoryId, string Keyword)
        {

            var vm = new DocumentListVM
            {
                CategoryId = categoryId ?? 0,
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Doc.PageSize,
                Keyword = Keyword
            };

            int count;
            var Documents = _documentServices.GetPagedElements(vm.PageIndex-1, vm.PageSize, vm.Keyword, (int)vm.CategoryId, out count);
            
            vm.TotalCount = count;

            var categoryList = _categoryServices.GetAll().OrderByDescending(c => c.Importance).ToList();
            var categories = new SelectList(categoryList, "Id", "Title");
            ViewBag.Categories = categories;

            vm.Documents = new StaticPagedList<Document>(Documents, vm.PageIndex, vm.PageSize, vm.TotalCount);         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(vm);

        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/DocumentSettings.config");
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
            var Document = new DocumentIM
            {
                Active = true,
                IsVIP = false
             };

            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");
            ViewBag.Categories = lCategorys;
            var exts = new SelectList(Infrastructure.Helper.File.GetExtensions(), "Value", "Text");         
            ViewBag.Extensions = exts;

            return View(Document);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(DocumentIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                var newDocument = _mapper.Map<DocumentIM, Document>(vm);      

                var result = _documentServices.Create(newDocument);

                //if (result != null)
                //{
                //    var pageMeta = new PageMeta()
                //    {
                //        ObjectId = result.ToString(),
                //        Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
                //        Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                //        Description = vm.SEODescription,
                //        ModelType = ModelType.Document
                //    };
                //    _pageMetaServices.Create(pageMeta);
                //}


                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Document));
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
            var vDocument = _documentServices.GetById(Id);
            if (vDocument == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }


            var editDocument = _mapper.Map<Document, DocumentIM>(vDocument);

            //var pageMeta = _pageMetaServices.GetPageMeta(ModelType.Document, editDocument.Id.ToString());
            //if (pageMeta != null)
            //{
            //    editDocument.SEOTitle = pageMeta.Title;
            //    editDocument.Keywords = pageMeta.Keyword;
            //    editDocument.SEODescription = pageMeta.Description;
            //}

            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");
            ViewBag.Categories = lCategorys;
            var exts = new SelectList(Infrastructure.Helper.File.GetExtensions(), "Value", "Text");
            ViewBag.Extensions = exts;

            return View(editDocument);


        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(DocumentIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var doc = _documentServices.GetById(vm.Id);               
                var editDocument = _mapper.Map(vm, doc);
                _documentServices.Update(editDocument);

                //var pageMeta = _pageMetaServices.GetPageMeta(ModelType.Document, editDocument.Id.ToString());
                //pageMeta = pageMeta ?? new PageMeta();

                //pageMeta.ObjectId = vm.Id.ToString();
                //pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
                //pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
                //pageMeta.Description = vm.SEODescription;
                //pageMeta.ModelType = ModelType.Document;

                //if (pageMeta.Id > 0)
                //{
                //    _pageMetaServices.Update(pageMeta);
                //}
                //else
                //{
                //    _pageMetaServices.Create(pageMeta);
                //}
               

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Document));
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

            Document vDocument = _documentServices.GetById(id);

            try
            {
                vDocument.Active = !vDocument.Active;
                _documentServices.Update(vDocument);

                vDocument.Category = _categoryServices.GetById(vDocument.CategoryId);
                
                AR.Data = RenderPartialViewToString("_DocumentItem", vDocument);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Document));
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult Recommend(int id)
        //{

        //    Document vDocument = _DocumentServices.GetById(id);

        //    try
        //    {
        //        vDocument.Recommend = !vDocument.Recommend;
        //        _DocumentServices.Update(vDocument);

        //        vDocument.DocumentCategory = _categoryServices.GetById(vDocument.CategoryId);

        //        AR.Data = RenderPartialViewToString("_DocumentItem", vDocument);
        //        AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Document));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        AR.Setfailure(ex.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {

            Document vDocument = _documentServices.GetById(id);

            if (vDocument == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _documentServices.Delete(vDocument);

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Document));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }
        ///// <summary>
        ///// 创建文章索引
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult CreateIndex()
        //{
        //    try
        //    {
        //        var list = _DocumentServices.GetActiveElements().Select(m => new SearchData
        //        {
        //            Id = $"Document{m.Id}",
        //            Name = m.Title,
        //            Description = string.IsNullOrEmpty(m.Summary)? StringHelper.StripTagsCharArray(m.Body) : m.Summary,
        //            ImageUrl = string.IsNullOrEmpty(m.Thumbnail)?string.Empty: m.Thumbnail,
        //            Url = m.CategoryId == 1 ? $"{SettingsManager.Site.SiteDomainName}/news/detail/{m.Id}" : $"{SettingsManager.Site.SiteDomainName}/news/business/{m.Id}"
        //        }).ToList();
        //        //var products = _mapper.Map<List<Product>, List<SearchData>>(list);

        //        GoLucene.AddUpdateLuceneIndex(list);
               
        //        AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Document));
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}

        #endregion


    }
}
