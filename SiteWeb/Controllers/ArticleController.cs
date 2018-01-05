using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Service.Articles;

namespace SiteWeb.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleServices _articleServices;
        private IArticleCategoryServices _categoryService;

        public ArticleController(IArticleServices articleServices, IArticleCategoryServices categoryService)
        {
            _articleServices = articleServices;
            _categoryService = categoryService;
        }
        // GET: Article
        public ActionResult Index()
        {
            var vm = _articleServices.GetAll();
            return View(vm);
        }
    }
}