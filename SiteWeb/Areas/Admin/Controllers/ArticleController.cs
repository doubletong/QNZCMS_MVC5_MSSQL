using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;

using System;
using System.Xml.Linq;
using System.Text;
using TZGCMS.Service.Articles;
using TZGCMS.Service.PageMetas;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.InputModel.Articles;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Resources.Admin;
using TZGCMS.SiteWeb.Filters;
using TZGCMS.Model;
using TZGCMS.Model.Search;
using QNZ.Data;
using System.Data.Entity;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class ArticleController : QNZBaseController
    {
        private IQNZDbContext _db;
        //private readonly IArticleCategoryServices _categoryServices;
        //private readonly IArticleServices _articleServices;
        //private readonly IPageMetaServices _pageMetaServices;       
        //private readonly IFilterTemplateServices _templateServices;
        private readonly IMapper _mapper;

        public ArticleController(         IQNZDbContext db,        IMapper mapper)
        {
            //_categoryServices = categoryServices;
            //_articleServices = articleServices;
            //_pageMetaServices = pageMetaServices;
            //_templateServices = templateServices;
            _mapper = mapper;
            _db = db;

        }
        #region 新闻


        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Index(int? page, int? categoryId, string Keyword)
        {

            var vm = new ArticleListVM
            {
                CategoryId = categoryId ?? 0,
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Article.PageSize,
                Keyword = Keyword
            };

            var query = _db.Articles.AsQueryable();
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = query.Where(d => d.Title.Contains(Keyword) || d.Body.Contains(Keyword));
            }
            if (categoryId > 0)
            {
                query = query.Where(d => d.CategoryId == categoryId);
            }

            var articles = await query.OrderByDescending(d => d.Pubdate)
                .Skip((vm.PageIndex - 1) * vm.PageSize)
                .Take(vm.PageSize).ProjectTo<ArticleVM>().ToListAsync();               
          

            //   articleListVM.Articles = articleDtos;
            vm.TotalCount = await query.CountAsync();            
            vm.Articles = new StaticPagedList<ArticleVM>(articles, vm.PageIndex, vm.PageSize, vm.TotalCount);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            var categoryList = await _db.ArticleCategories.OrderByDescending(c => c.Importance).ToListAsync();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Title");
        

            return View(vm);

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
        //public ActionResult Acquisition()
        //{
        //    var categorys = _categoryServices.GetAll().OrderByDescending(m => m.Importance).ToList();
        //    var lCategorys = new SelectList(categorys, "Id", "Title");
        //    ViewBag.Categories = lCategorys;

        //    var temps = _templateServices.GetActiveElements().OrderByDescending(m => m.Importance).ToList();
        //    var templates = new SelectList(temps, "Id", "Name");
        //    ViewBag.Templates = templates;

        //    AcquisitionIM im = new AcquisitionIM
        //    {
        //        Count = 5
        //    };
        //    return View(im);
        //}

        //public ActionResult Test()
        //{
        //    var template = _templateServices.GetById(7);
        //    var htmlEncode = string.IsNullOrEmpty(template.Encode) ? Encoding.Default : Encoding.GetEncoding(template.Encode);
        //    var htmlSource = DataAcquisition.GetRemoteHtmlCode(template.Source, htmlEncode);
        //    var htmlLinks = DataAcquisition.GetContentByTag(htmlSource, template.LinksContainer);
        //    var links = DataAcquisition.GetLinkFindBy(htmlLinks, template.Links).Distinct().Take(5);
        // //   return Content(string.Join("|", links.ToArray()));
        //    foreach (var link in links)
        //    {
        //       var  onlink = link.StartsWith("//") ? "http:" + link : link;

        //        Article article = new Article();
        //        var itemHtml = DataAcquisition.GetRemoteHtmlCode(onlink, htmlEncode);
        //    //    return Content(itemHtml);
        //        article.CategoryId = 6;
        //        article.Title = DataAcquisition.GetContentByTag(itemHtml, template.Title);
        //       return Content(article.Title);

        //        article.Body = DataAcquisition.GetContentByTag(itemHtml, template.Body);
        //        article.Summary = DataAcquisition.GetMetaContent(itemHtml, template.Description);
        //      //  return Content(article.Body);
        //        article.Active = true;
        //        if (!string.IsNullOrWhiteSpace(article.Title.Trim()))
        //        {
        //            _articleServices.Create(article);

        //            // DataAcquisition.GetMetaContent(itemHtml, template.Keyword);
        //            var pageMeta = new PageMeta()
        //            {
        //                ObjectId = article.Id.ToString(),
        //                Title = article.Title,
        //                Keyword = DataAcquisition.GetMetaContent(itemHtml, template.Keyword),
        //                Description = article.Summary,
        //                ModelType = ModelType.ARTICLE
        //            };

        //            _pageMetaServices.Create(pageMeta);
        //        }


        //    }

        //    return Content(string.Join("|", links.ToArray()));
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult Acquisition(AcquisitionIM im)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }
        //    if (im.Count == 0)
        //    {
        //        AR.Setfailure("添加条数必须在于0！");
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    var template = _templateServices.GetById(im.TemplateId);
        //    if (template == null)
        //    {
        //        AR.Setfailure("此采集模板不存在！");
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    try
        //    {

        //        var encode = string.IsNullOrEmpty(template.Encode) ? Encoding.Default : Encoding.GetEncoding(template.Encode);
        //        var htmlSource = DataAcquisition.GetRemoteHtmlCode(template.Source, encode);
        //        var htmlLinks = DataAcquisition.GetContentByTag(htmlSource, template.LinksContainer);
        //        var links = DataAcquisition.GetLinkFindBy(htmlLinks, template.Links).Distinct().Take(im.Count);

        //        foreach (var link in links)
        //        {
        //            Article article = new Article();
        //            var onlink = link.StartsWith("//") ? "http:" + link : link;
        //            var itemHtml = DataAcquisition.GetRemoteHtmlCode(onlink, encode);

        //            article.CategoryId = im.CategoryId;
        //            article.Title = DataAcquisition.GetContentByTag(itemHtml, template.Title);
        //            article.Title = article.Title.Length > 100 ? article.Title.Substring(0, 100) : article.Title;

        //            var contentHtml = DataAcquisition.GetContentByTag(itemHtml, template.Body);
        //            article.Body = ResetContent(contentHtml, template.KeywordSet);
        //            article.Summary = DataAcquisition.GetMetaContent(itemHtml, template.Description);

        //            article.Active = true;
        //            if (!string.IsNullOrWhiteSpace(article.Title))
        //            {
        //                _articleServices.Create(article);

        //                var pageMeta = new PageMeta()
        //                {
        //                    ObjectId = article.Id.ToString(),
        //                    Title = article.Title,
        //                    Keyword = DataAcquisition.GetMetaContent(itemHtml, template.Keyword),
        //                    Description = article.Summary,
        //                    ModelType = ModelType.ARTICLE
        //                };

        //                _pageMetaServices.Create(pageMeta);
        //            }
        //        }


        //        AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Article));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}


        //private string ResetContent(string htmlCode,string keywords)
        //{

        //    var plist = DataAcquisition.GetListByTag(htmlCode, "//p");
        //    if (plist == null)
        //        return "";

        //    var listPtag = Site.RandomSortList<string>(plist);

        //    StringBuilder sb = new StringBuilder();
        //    if (!string.IsNullOrEmpty(keywords))
        //    {
        //        var arKeywords = keywords.Split(','); 
        //        foreach (var item in plist)
        //        {
        //            var itemLength = item.Length;
        //           // var itemLength = Encoding.UTF8.GetByteCount(item);

        //            int times = (int)Math.Floor((decimal)itemLength / 60);
                    
        //            string insertItem = item;
        //            Random rnd = new Random();

        //            for (int i = times-1; i >= 0; i--)
        //            {
                        
        //                var keyword = arKeywords[rnd.Next(0, arKeywords.Length)];
                       
        //                if (i > 0)
        //                {
        //                    var index = i * 60 - 1;
        //                    insertItem = insertItem.Substring(0, (index + 1)/2) + keyword + insertItem.Substring(index/2);
        //                }
        //                else
        //                {
        //                    insertItem =  keyword + insertItem.Substring(0);
        //                }

        //            }

        //            sb.Append("<p>" + insertItem + "</p>");
                                   
        //        }              
                
        //    }
        //    else
        //    {              
            
        //        foreach (var item in plist)
        //        {
        //            sb.Append("<p>" + item + "</p>");
        //        }
               
        //    }
        //    return sb.ToString();
        //}

       

        //public async System.Threading.Tasks.Task<ActionResult> Add()
        //{
        //    var article = new ArticleIM {
        //        Active = true,
        //        Source = SettingsManager.Site.SiteName,
        //        Pubdate = DateTime.Now };

        //    var categoryList = await _db.ArticleCategories.OrderByDescending(c => c.Importance).ToListAsync();
        //    ViewBag.Categories = new SelectList(categoryList, "Id", "Title");


        //    return View(article);
        //}
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddA(ArticleIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                var newArticle = _mapper.Map<ArticleIM, Article>(vm);
                _db.Articles.Add(newArticle);
                var result = await _db.SaveChangesAsync();

              

                if (result > 0)
                {
                    // _pageMetaServices.SetPageMeta(ModelType.ARTICLE, result.Id.ToString(),result.Title, vm.SEOTitle,vm.Keywords,vm.SEODescription);
                    await SetPageMetaAsync(_db, (short)ModelType.ARTICLE, newArticle.Id.ToString(), newArticle.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);
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
        public async Task<ActionResult> Edit(int? id)
        {
            var categoryList = await _db.ArticleCategories.OrderByDescending(c => c.Importance).ToListAsync();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Title");

            if (id == null)
            {
                var article = new ArticleIM
                {
                    Active = true,
                    Source = SettingsManager.Site.SiteName,
                    Pubdate = DateTime.Now
                };

             

                return View(article);
            }
            else
            {
                var vArticle = await _db.Articles.FindAsync(id);
                if (vArticle == null)
                {
                    AR.Setfailure(Messages.HttpNotFound);
                    return Json(AR, JsonRequestBehavior.AllowGet);
                }

                var editArticle = _mapper.Map<Article, ArticleIM>(vArticle);

                var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.ARTICLE && d.ObjectId == editArticle.Id.ToString());
                if (pageMeta != null)
                {
                    editArticle.SEOTitle = pageMeta.Title;
                    editArticle.Keywords = pageMeta.Keyword;
                    editArticle.SEODescription = pageMeta.Description;
                }
                            

                return View(editArticle);
            }

            


        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(ArticleIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var article = await _db.Articles.FindAsync(vm.Id);

                var editArticle = _mapper.Map(vm, article);

            //    _articleServices.Update(editArticle);
                _db.Entry(editArticle).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                //  _pageMetaServices.SetPageMeta(ModelType.ARTICLE, vm.Id.ToString(), vm.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);
                await SetPageMetaAsync(_db, (short)ModelType.ARTICLE, editArticle.Id.ToString(), editArticle.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);

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
        public async Task<JsonResult> Active(int id)
        {

            Article vArticle = await _db.Articles.Include(d=>d.ArticleCategory).FirstOrDefaultAsync(d=>d.Id == id);

            try
            {
                vArticle.Active = !vArticle.Active;
                // _articleServices.Update(vArticle);
                _db.Entry(vArticle).State = EntityState.Modified;
                await _db.SaveChangesAsync();
             
                
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
        public async Task<JsonResult> Recommend(int id)
        {

            Article vArticle = await _db.Articles.Include(d => d.ArticleCategory).FirstOrDefaultAsync(d => d.Id == id);

            try
            {
                vArticle.Recommend = !vArticle.Recommend;

                _db.Entry(vArticle).State = EntityState.Modified;
                await _db.SaveChangesAsync();

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
        public async Task<JsonResult> Delete(int id)
        {

            Article vArticle = await _db.Articles.FindAsync(id);

            if (vArticle == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _db.Articles.Remove(vArticle);
            await _db.SaveChangesAsync();

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
                var list = _db.Articles.Where(d=>d.Active).Select(m => new SearchData
                {
                    Id = $"ARTICLE{m.Id}",
                    Name = m.Title,
                    Description = string.IsNullOrEmpty(m.Summary)? StringHelper.StripTagsCharArray(m.Body) : m.Summary,
                    ImageUrl = string.IsNullOrEmpty(m.Thumbnail)?string.Empty: m.Thumbnail,
                    Url = m.CategoryId == 1 ? $"{SettingsManager.Site.SiteDomainName}/article/detail/{m.Id}" : $"{SettingsManager.Site.SiteDomainName}/article/detail/{m.Id}",
                    CreatedDate = m.Pubdate
                }).ToList();
                //var products = _mapper.Map<List<Product>, List<SearchData>>(list);

                LuceneHelper.AddUpdateLuceneIndex(list);
               
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
