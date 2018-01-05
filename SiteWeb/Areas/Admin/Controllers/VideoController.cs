using AutoMapper;
using PagedList;
using System;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using SiteWeb.Filters;
using TZGCMS.Service.Videos;
using TZGCMS.Service.PageMetas;
using TZGCMS.Model.Admin.ViewModel.Videos;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.InputModel.Videos;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Resources.Admin;

namespace SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class VideoController : BaseController
    {

        private readonly IVideoServices _videoServices;
        private readonly IVideoCategoryServices _categoryServices;
        private readonly IPageMetaServices _pageMetaServices;
        private readonly IMapper _mapper;

        public VideoController(IVideoServices videoServices, IVideoCategoryServices categoryServices, IPageMetaServices pageMetaServices,  IMapper mapper)
        {
            _videoServices = videoServices;
            _categoryServices = categoryServices;
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;

        }
        // GET: Admin/Video

        #region Video

        public ActionResult Index(int? page, int? categoryId, string keyword)
        {
            VideoListVM videoListVM = GetElements(page, categoryId, keyword);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(videoListVM);

        }

        private VideoListVM GetElements(int? page, int? categoryId, string keyword)
        {
            var videoListVM = new VideoListVM()
            {
                Keyword = keyword,
                PageIndex = (page ?? 1),
                CategoryId = categoryId ?? 0,
                PageSize = SettingsManager.Video.PageSize
            };
            int totalCount;

            var categoryList = _categoryServices.GetAll().OrderByDescending(c => c.Importance).ToList();
            var categories = new SelectList(categoryList, "Id", "Title");
            ViewBag.Categories = categories;

            var videolist = _videoServices.GetPagedElements(videoListVM.PageIndex-1, videoListVM.PageSize, videoListVM.Keyword, videoListVM.CategoryId, out totalCount);
           
            videoListVM.TotalCount = totalCount;
            videoListVM.Videos = new StaticPagedList<Video>(videolist, videoListVM.PageIndex, videoListVM.PageSize, videoListVM.TotalCount); ;
            return videoListVM;
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
            var vm = new VideoIM()
            {
                Active = true,
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(3).AddHours(1)

            };
            //vCategory.PageIndex = pageIndex;
            //vCategory.PageSize = pageSize;
            //vCategory.Keyword = keyword;
            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");
            ViewBag.Categories = lCategorys;

            return PartialView("_Add", vm);
        }



        [HttpPost]
        public JsonResult Add(VideoIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var video = _mapper.Map<VideoIM, Video>(vm);
            //video.ViewCount = 0;
            //video.CreatedBy = Site.CurrentUserName;
            //video.CreatedDate = DateTime.Now;

            var result = _videoServices.Create(video);

            if (result)
            {
                var pageMeta = new PageMeta()
                {
                    ObjectId = result.ToString(),
                    Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
                    Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                    Description = vm.Description,
                    ModelType = ModelType.VIDEO
                };
                _pageMetaServices.Create(pageMeta);
            }
          
            

            int count;
            int pageSize = SettingsManager.Video.PageSize;
            var list = _videoServices.GetPagedElements(0, pageSize, string.Empty,0,out count);
       
            AR.Data = RenderPartialViewToString("_VideoList", list);

            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Video));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpGet]
        public ActionResult Edit(int id)
        {

            Video video = _videoServices.GetById(id);
            if (video == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var vm = _mapper.Map<VideoIM>(video);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.VIDEO, vm.Id.ToString());
            if (pageMeta != null)
            {
                vm.SEOTitle = pageMeta.Title;
                vm.Keywords = pageMeta.Keyword;
                vm.Description = pageMeta.Description;
            }

            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");
            ViewBag.Categories = lCategorys;

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<JsonResult> Edit(VideoIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


            var video = _mapper.Map<VideoIM, Video>(vm);
            _videoServices.Update(video);


            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.VIDEO, vm.Id.ToString());
            pageMeta = pageMeta ?? new PageMeta();          

            pageMeta.ObjectId = vm.Id.ToString();
            pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
            pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
            pageMeta.Description = vm.Description;
            pageMeta.ModelType = ModelType.VIDEO;

            if (pageMeta.Id > 0)
            {
                _pageMetaServices.Update(pageMeta);
            }
            else
            {
                _pageMetaServices.Create(pageMeta);
            }
          

          //  var videoVM = _mapper.Map<VideoVM>(video);

            AR.Id = video.Id;
            var data = await _videoServices.GetByIdWidthCategoryAsync(video.Id);
            AR.Data = RenderPartialViewToString("_VideoItem", data);

            AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Video));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
           // Video vCategory = _videoRepository.GetById(id);                    

            _videoServices.Delete(id);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Video));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {

            var vm = _videoServices.GetById(id);
            if (vm == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                vm.Active = !vm.Active;
                _videoServices.Update(vm);
              //  var vm = _mapper.Map<VideoVM>(vCategory);

                AR.Data = RenderPartialViewToString("_VideoItem", vm);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Video));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        


        //[AllowAnonymous]
        //public JsonResult IsSeoNameUnique(string seoName, int? Id)
        //{
        //    return !IsExist(seoName, Id)
        //        ? Json(true, JsonRequestBehavior.AllowGet)
        //        : Json(false, JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //public bool IsExist(string seoName, int? id)
        //{
        //    var wType = _videoService.GetAll().Where(w => w.SeoName == seoName);
        //    if (id > 0)
        //    {
        //        wType = wType.Where(w => w.Id != id);
        //    }

        //    if (wType.Count() > 0)
        //        return true;

        //    return false;
        //}



        #endregion
    }
}