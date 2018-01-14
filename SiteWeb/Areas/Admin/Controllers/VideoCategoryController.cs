using AutoMapper;
using PagedList;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Videos;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Videos;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Videos;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class VideoCategoryController : BaseController
    {
        private readonly IVideoCategoryServices _categoryServices;
        private readonly IPageMetaServices _pageMetaServices;
        private readonly IMapper _mapper;

        public VideoCategoryController(
            IVideoCategoryServices categoryServices,          
            IPageMetaServices pageMetaServices,
            IMapper mapper)
        {
            _categoryServices = categoryServices;         
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;

        }
        // GET: Admin/VideoCategory

        #region Video Category

        public ActionResult Index(int? page, string keyword)
        {
            VideoCategoryListVM categoryListVM = GetElements(page, keyword);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(categoryListVM);

        }

        private VideoCategoryListVM GetElements(int? page, string keyword)
        {
            var vm = new VideoCategoryListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Video.PageSize
            };
            int totalCount;
            var list = _categoryServices.GetPagedElements(vm.PageIndex-1, vm.PageSize, vm.Keyword, out totalCount);
           
            vm.TotalCount = totalCount;
            vm.Categories = new StaticPagedList<VideoCategory>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
        }

        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/VideoSettings.config");
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
            var vCategory = new VideoCategoryIM()
            {
                Active = true,
                Importance = 0
            };
            return PartialView("_Add", vCategory);
        }



        [HttpPost]
        public JsonResult Add(VideoCategoryIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var newCategory = _mapper.Map<VideoCategoryIM, VideoCategory>(vm);
            //newCategory.CreatedBy = Site.CurrentUserName;
            //newCategory.CreatedDate = DateTime.Now;

            var result =  _categoryServices.Create(newCategory);

            if (result!=null)
            {
                var pageMeta = new PageMeta()
                {
                    ObjectId = result.ToString(),
                    Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
                    Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                    Description = vm.SEODescription,
                    ModelType = ModelType.VIDEOCATEGORY
                };
                _pageMetaServices.Create(pageMeta);
            }
               

            int count;
            var pageSize = SettingsManager.Video.PageSize;
            var list = _categoryServices.GetPagedElements(0, pageSize, string.Empty, out count);            
            AR.Data = RenderPartialViewToString("_CategoryList", list);
            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.VideoCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpGet]
        public ActionResult Edit(int id)
        {

            VideoCategory category = _categoryServices.GetById(id);
            if (category == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var vm = _mapper.Map<VideoCategoryIM>(category);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.VIDEOCATEGORY, category.Id.ToString());
            if (pageMeta != null)
            {
                vm.SEOTitle = pageMeta.Title;
                vm.Keywords = pageMeta.Keyword;
                vm.SEODescription = pageMeta.Description;
            }

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Edit(VideoCategoryIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            //var newCategory = _categoryServices.GetById(vm.Id);
            //newCategory.Title = vm.Title;
            //newCategory.SeoName = vm.SeoName;
            //newCategory.Importance = vm.Importance;
            //newCategory.Active = vm.Active;
            //newCategory.UpdatedBy = Site.CurrentUserName;
            //newCategory.UpdatedDate = DateTime.Now;
            var newCategory = _mapper.Map<VideoCategoryIM, VideoCategory>(vm);

            _categoryServices.Update(newCategory);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.VIDEOCATEGORY, vm.Id.ToString());
            pageMeta = pageMeta ?? new PageMeta();
          
            pageMeta.ObjectId = vm.Id.ToString();
            pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
            pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
            pageMeta.Description = vm.SEODescription;
            pageMeta.ModelType = ModelType.VIDEOCATEGORY;

            if (pageMeta.Id > 0)
            {
                _pageMetaServices.Update(pageMeta);
            }
            else
            {
                _pageMetaServices.Create(pageMeta);
            }
         

         //   var category = _mapper.Map<VideoCategoryVM>(newCategory);
            AR.Id = newCategory.Id;
            AR.Data = RenderPartialViewToString("_CategoryItem", newCategory);

            AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.VideoCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var category = _categoryServices.GetByIdWithVideos(id);
         
            if (category.Videos.Any())
            {
                AR.Setfailure("此分类下面还有视频存在，不能删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _categoryServices.Delete(category);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.VideoCategory));
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

               // var vm = _mapper.Map<VideoCategoryVM>(vCategory);

                AR.Data = RenderPartialViewToString("_CategoryItem", vCategory);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.VideoCategory));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        [AllowAnonymous]
        public JsonResult IsSeoNameUnique(string seoName, int? id)
        {
            return !_categoryServices.IsExistSeoName(seoName, id)
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(false, JsonRequestBehavior.AllowGet);
        }
      

        #endregion
    }
}