using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;

using System;
using System.Xml.Linq;
using System.Text;
using TZGCMS.Service.Articles;
using TZGCMS.Service.PageMetas;
using TZGCMS.Model.Admin.ViewModel.Articles;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.InputModel.Articles;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.LuceneSearch;
using TZGCMS.Model.Admin.ViewModel.LuceneSearch;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ArticleController : BaseController
    {
        private readonly IArticleCategoryServices _categoryServices;
        private readonly IArticleServices _articleServices;
        private readonly IPageMetaServices _pageMetaServices;       
        private readonly IFilterTemplateServices _templateServices;
        private readonly IMapper _mapper;

        public ArticleController(IArticleCategoryServices categoryServices,
            IArticleServices articleServices,
            IPageMetaServices pageMetaServices,
            IFilterTemplateServices templateServices,
            IMapper mapper)
        {
            _categoryServices = categoryServices;
            _articleServices = articleServices;
            _pageMetaServices = pageMetaServices;
            _templateServices = templateServices;
            _mapper = mapper;


        }
        #region 新闻


        [HttpGet]
        public ActionResult Index(int? page, int? categoryId, string Keyword)
        {

            var articleListVM = new ArticleListVM();

            articleListVM.CategoryId = categoryId ?? 0;
            articleListVM.PageIndex = (page ?? 1);
            articleListVM.PageSize = SettingsManager.Article.PageSize;
            articleListVM.Keyword = Keyword;

            int count;
            var articles = _articleServices.GetPagedElements(articleListVM.PageIndex-1, articleListVM.PageSize,  articleListVM.Keyword, (int)articleListVM.CategoryId, out count);

          

            //   articleListVM.Articles = articleDtos;
            articleListVM.TotalCount = count;

            var categoryList = _categoryServices.GetAll().OrderByDescending(c => c.Importance).ToList();
            var categories = new SelectList(categoryList, "Id", "Title");
            ViewBag.Categories = categories;

           

            articleListVM.Articles = new StaticPagedList<Article>(articles, articleListVM.PageIndex, articleListVM.PageSize, articleListVM.TotalCount);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(articleListVM);

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
        public ActionResult Acquisition()
        {
            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");
            ViewBag.Categories = lCategorys;

            var temps = _templateServices.GetActiveElements().OrderByDescending(m => m.Importance).ToList();
            var templates = new SelectList(temps, "Id", "Name");
            ViewBag.Templates = templates;

            AcquisitionIM im = new AcquisitionIM
            {
                Count = 5
            };
            return View(im);
        }

        public ActionResult Test()
        {
            var template = _templateServices.GetById(7);
            var htmlEncode = string.IsNullOrEmpty(template.Encode) ? Encoding.Default : Encoding.GetEncoding(template.Encode);
            var htmlSource = DataAcquisition.GetRemoteHtmlCode(template.Source, htmlEncode);
            var htmlLinks = DataAcquisition.GetContentByTag(htmlSource, template.LinksContainer);
            var links = DataAcquisition.GetLinkFindBy(htmlLinks, template.Links).Distinct().Take(5);
         //   return Content(string.Join("|", links.ToArray()));
            foreach (var link in links)
            {
               var  onlink = link.StartsWith("//") ? "http:" + link : link;

                Article article = new Article();
                var itemHtml = DataAcquisition.GetRemoteHtmlCode(onlink, htmlEncode);
            //    return Content(itemHtml);
                article.CategoryId = 6;
                article.Title = DataAcquisition.GetContentByTag(itemHtml, template.Title);
               return Content(article.Title);

                article.Body = DataAcquisition.GetContentByTag(itemHtml, template.Body);
                article.Summary = DataAcquisition.GetMetaContent(itemHtml, template.Description);
              //  return Content(article.Body);
                article.Active = true;
                if (!string.IsNullOrWhiteSpace(article.Title.Trim()))
                {
                    _articleServices.Create(article);

                    // DataAcquisition.GetMetaContent(itemHtml, template.Keyword);
                    var pageMeta = new PageMeta()
                    {
                        ObjectId = article.Id.ToString(),
                        Title = article.Title,
                        Keyword = DataAcquisition.GetMetaContent(itemHtml, template.Keyword),
                        Description = article.Summary,
                        ModelType = ModelType.ARTICLE
                    };

                    _pageMetaServices.Create(pageMeta);
                }


            }

            return Content(string.Join("|", links.ToArray()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Acquisition(AcquisitionIM im)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            if(im.Count == 0)
            {
                AR.Setfailure("添加条数必须在于0！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var template = _templateServices.GetById(im.TemplateId);
            if (template == null)
            {
                AR.Setfailure("此采集模板不存在！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                var encode = string.IsNullOrEmpty(template.Encode) ? Encoding.Default : Encoding.GetEncoding(template.Encode);
                var htmlSource = DataAcquisition.GetRemoteHtmlCode(template.Source, encode);
                var htmlLinks = DataAcquisition.GetContentByTag(htmlSource, template.LinksContainer);
                var links = DataAcquisition.GetLinkFindBy(htmlLinks, template.Links).Distinct().Take(im.Count);

                foreach (var link in links)
                {
                    Article article = new Article();
                var onlink = link.StartsWith("//") ? "http:" + link : link;
                var itemHtml = DataAcquisition.GetRemoteHtmlCode(onlink, encode);

                    article.CategoryId = im.CategoryId;
                    article.Title = DataAcquisition.GetContentByTag(itemHtml, template.Title);
                    article.Title = article.Title.Length > 100 ? article.Title.Substring(0, 100): article.Title;

                    var contentHtml = DataAcquisition.GetContentByTag(itemHtml, template.Body);
                    article.Body = ResetContent(contentHtml, template.KeywordSet);
                    article.Summary = DataAcquisition.GetMetaContent(itemHtml, template.Description);

                    article.Active = true;
                    if (!string.IsNullOrWhiteSpace(article.Title))
                    {
                        _articleServices.Create(article);
                
                        var pageMeta = new PageMeta()
                        {
                            ObjectId = article.Id.ToString(),
                            Title = article.Title,
                            Keyword = DataAcquisition.GetMetaContent(itemHtml, template.Keyword),
                            Description = article.Summary,
                            ModelType = ModelType.ARTICLE
                        };

                        _pageMetaServices.Create(pageMeta);
                    }
                }

                
                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Article));
                return Json(AR, JsonRequestBehavior.DenyGet);

        }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
    }

}


        private string ResetContent(string htmlCode,string keywords)
        {

            var plist = DataAcquisition.GetListByTag(htmlCode, "//p");
            if (plist == null)
                return "";

            var listPtag = Site.RandomSortList<string>(plist);

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(keywords))
            {
                var arKeywords = keywords.Split(','); 
                foreach (var item in plist)
                {
                    var itemLength = item.Length;
                   // var itemLength = Encoding.UTF8.GetByteCount(item);

                    int times = (int)Math.Floor((decimal)itemLength / 60);
                    
                    string insertItem = item;
                    Random rnd = new Random();

                    for (int i = times-1; i >= 0; i--)
                    {
                        
                        var keyword = arKeywords[rnd.Next(0, arKeywords.Length)];
                       
                        if (i > 0)
                        {
                            var index = i * 60 - 1;
                            insertItem = insertItem.Substring(0, (index + 1)/2) + keyword + insertItem.Substring(index/2);
                        }
                        else
                        {
                            insertItem =  keyword + insertItem.Substring(0);
                        }

                    }

                    sb.Append("<p>" + insertItem + "</p>");
                                   
                }              
                
            }
            else
            {              
            
                foreach (var item in plist)
                {
                    sb.Append("<p>" + item + "</p>");
                }
               
            }
            return sb.ToString();
        }

       

        public ActionResult Add()
        {
            var article = new ArticleIM {
                Active = true,
                Source = SettingsManager.Site.SiteName,
                Pubdate = DateTime.Now };

            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");

            ViewBag.Categories = lCategorys;
            return View(article);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(ArticleIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                var newArticle = _mapper.Map<ArticleIM, Article>(vm);
      

                var result = _articleServices.Create(newArticle);

                if (result!=null)
                {
                    var pageMeta = new PageMeta()
                    {
                        ObjectId = result.ToString(),
                        Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle,
                        Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ','),
                        Description = vm.SEODescription,
                        ModelType = ModelType.ARTICLE
                    };
                    _pageMetaServices.Create(pageMeta);
                }
                

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Article));
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
            var vArticle = _articleServices.GetById(Id);
            if (vArticle == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }


            var editArticle = _mapper.Map<Article, ArticleIM>(vArticle);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.ARTICLE, editArticle.Id.ToString());
            if (pageMeta != null)
            {
                editArticle.SEOTitle = pageMeta.Title;
                editArticle.Keywords = pageMeta.Keyword;
                editArticle.SEODescription = pageMeta.Description;
            }

            var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
            var lCategorys = new SelectList(categorys, "Id", "Title");

            ViewBag.Categories = lCategorys;

            return View(editArticle);


        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(ArticleIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
               
                var editArticle = _mapper.Map<ArticleIM, Article>(vm);

                _articleServices.Update(editArticle);

                var pageMeta = _pageMetaServices.GetPageMeta(ModelType.ARTICLE, editArticle.Id.ToString());
                pageMeta = pageMeta ?? new PageMeta();

                pageMeta.ObjectId = vm.Id.ToString();
                pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
                pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
                pageMeta.Description = vm.SEODescription;
                pageMeta.ModelType = ModelType.ARTICLE;

                if (pageMeta.Id > 0)
                {
                    _pageMetaServices.Update(pageMeta);
                }
                else
                {
                    _pageMetaServices.Create(pageMeta);
                }
               

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Article));
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

            Article vArticle = _articleServices.GetById(id);

            try
            {
                vArticle.Active = !vArticle.Active;
                _articleServices.Update(vArticle);

                vArticle.ArticleCategory = _categoryServices.GetById(vArticle.CategoryId);
                
                AR.Data = RenderPartialViewToString("_ArticleItem", vArticle);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Article));
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
        public JsonResult Recommend(int id)
        {

            Article vArticle = _articleServices.GetById(id);

            try
            {
                vArticle.Recommend = !vArticle.Recommend;
                _articleServices.Update(vArticle);

                vArticle.ArticleCategory = _categoryServices.GetById(vArticle.CategoryId);

                AR.Data = RenderPartialViewToString("_ArticleItem", vArticle);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Article));
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

            Article vArticle = _articleServices.GetById(id);

            if (vArticle == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _articleServices.Delete(vArticle);

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Article));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }
        /// <summary>
        /// 创建文章索引
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateIndex()
        {
            try
            {
                var list = _articleServices.GetActiveElements().Select(m => new SearchData
                {
                    Id = $"ARTICLE{m.Id}",
                    Name = m.Title,
                    Description = string.IsNullOrEmpty(m.Summary)? StringHelper.StripTagsCharArray(m.Body) : m.Summary,
                    ImageUrl = string.IsNullOrEmpty(m.Thumbnail)?string.Empty: m.Thumbnail,
                    Url = m.CategoryId == 1 ? $"{SettingsManager.Site.SiteDomainName}/news/detail/{m.Id}" : $"{SettingsManager.Site.SiteDomainName}/news/business/{m.Id}"
                }).ToList();
                //var products = _mapper.Map<List<Product>, List<SearchData>>(list);

                GoLucene.AddUpdateLuceneIndex(list);
               
                AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Article));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        #endregion


    }
}
