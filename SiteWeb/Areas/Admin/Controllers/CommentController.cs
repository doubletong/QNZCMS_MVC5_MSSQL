using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;

using System;
using System.Xml.Linq;
using System.Text;
using SiteWeb.Filters;
using TZGCMS.Service.PageMetas;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.LuceneSearch;
using TZGCMS.Model.Admin.ViewModel.LuceneSearch;
using TZGCMS.Service.Articles;
using TZGCMS.Model.Admin.ViewModel.Articles;
using TZGCMS.Data.Entity.Articles;

namespace SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class CommentController : BaseController
    {
        private readonly IArticleServices _articleServices;
        private readonly ICommentServices _commentServices;  
        private readonly IMapper _mapper;

        public CommentController(IArticleServices articleServices,
            ICommentServices commentServices,       
            IMapper mapper)
        {
            _articleServices = articleServices;
            _commentServices = commentServices;
         
            _mapper = mapper;


        }
        #region 新闻


        [HttpGet]
        public ActionResult Index(int? page, int? articleId, string Keyword)
        {

            var commentListVM = new CommentListVM();

            commentListVM.ArticleId = articleId ?? 0;
            commentListVM.PageIndex = (page ?? 1);
            commentListVM.PageSize = SettingsManager.Article.PageSize;
            commentListVM.Keyword = Keyword;

            int count;
            var comments = _commentServices.GetPagedElements(commentListVM.PageIndex-1, commentListVM.PageSize,  commentListVM.Keyword, (int)commentListVM.ArticleId, out count);

          

            //   commentListVM.Comments = commentDtos;
            commentListVM.TotalCount = count;

            //var categoryList = _categoryServices.GetAll().OrderByDescending(c => c.Importance).ToList();
            //var categories = new SelectList(categoryList, "Id", "Title");
            //ViewBag.Categories = categories;

            commentListVM.Comments = new StaticPagedList<Comment>(comments, commentListVM.PageIndex, commentListVM.PageSize, commentListVM.TotalCount);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(commentListVM);

        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/CommentSettings.config");
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




        //public ActionResult Add()
        //{
        //    var comment = new CommentIM {
        //        Active = true,
        //        Source = SettingsManager.Site.SiteName,
        //        Pubdate = DateTime.Now };

        //    var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
        //    var lCategorys = new SelectList(categorys, "Id", "Title");

        //    ViewBag.Categories = lCategorys;
        //    return View(comment);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult Add(CommentIM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    try
        //    {

        //        var newComment = _mapper.Map<CommentIM, Comment>(vm);
        //       // newComment.ViewCount = 0;
        //      //  newComment.CreatedBy = Site.CurrentUserName;
        //      //  newComment.CreatedDate = DateTime.Now;

        //        var result = _commentServices.Create(newComment);

        //        if (result)
        //        {
        //            var pageMeta = new PageMeta()
        //            {
        //                ObjectId = result.ToString(),
        //                Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
        //                Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
        //                Description = vm.SEODescription,
        //                ModelType = ModelType.ARTICLE
        //            };
        //            _pageMetaServices.Create(pageMeta);
        //        }


        //        AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Comment));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }


        //}

        //[HttpGet]
        //public ActionResult Edit(int Id)
        //{
        //    var vComment = _commentServices.GetById(Id);
        //    if (vComment == null)
        //    {
        //        AR.Setfailure(Messages.HttpNotFound);
        //        return Json(AR, JsonRequestBehavior.AllowGet);
        //    }


        //    var editComment = _mapper.Map<Comment, CommentIM>(vComment);

        //    var pageMeta = _pageMetaServices.GetPageMeta(ModelType.ARTICLE, editComment.Id.ToString());
        //    if (pageMeta != null)
        //    {
        //        editComment.SEOTitle = pageMeta.Title;
        //        editComment.Keywords = pageMeta.Keyword;
        //        editComment.SEODescription = pageMeta.Description;
        //    }

        //    var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
        //    var lCategorys = new SelectList(categorys, "Id", "Title");

        //    ViewBag.Categories = lCategorys;

        //    return View(editComment);


        //}

        //[HttpPost]
        //[ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //public JsonResult Edit(CommentIM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    try
        //    {

        //        var editComment = _mapper.Map<CommentIM, Comment>(vm);

        //        _commentServices.Update(editComment);

        //        var pageMeta = _pageMetaServices.GetPageMeta(ModelType.ARTICLE, editComment.Id.ToString());
        //        pageMeta = pageMeta ?? new PageMeta();

        //        pageMeta.ObjectId = vm.Id.ToString();
        //        pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
        //        pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
        //        pageMeta.Description = vm.SEODescription;
        //        pageMeta.ModelType = ModelType.ARTICLE;

        //        if (pageMeta.Id > 0)
        //        {
        //            _pageMetaServices.Update(pageMeta);
        //        }
        //        else
        //        {
        //            _pageMetaServices.Create(pageMeta);
        //        }


        //        AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Comment));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }


        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {

            var comment = _commentServices.GetById(id);
            if (comment == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                comment.Active = !comment.Active;
                _commentServices.Update(comment);

                // var vm = _mapper.Map<ArticleCategoryVM>(comment);

                AR.Data = RenderPartialViewToString("_CommentItem", comment);
                AR.SetSuccess(string.Format(Messages.AlertUpdateSuccess, EntityNames.Comment));
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

            Comment vComment = _commentServices.GetById(id);

            if (vComment == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _commentServices.Delete(vComment);

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Comment));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }
        
        #endregion


    }
}
