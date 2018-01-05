using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Front.InputModel.Articles;
using TZGCMS.Model.Front.ViewModel.Articles;
using TZGCMS.Service.Articles;

namespace SiteWeb.Controllers.api
{
   /// <summary>
   /// 文章
   /// </summary>
    public class ArticlesController : ApiController
    {
       
        private readonly IArticleServices _articleServices;
        private readonly IArticleCategoryServices _categoryServices;
        private readonly ICommentServices _commentServices;
        private readonly IMapper _mapper;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="articleServices"></param>
        /// <param name="categoryServices"></param>
        /// <param name="mapper"></param>
        public ArticlesController(IArticleServices articleServices, IArticleCategoryServices categoryServices, ICommentServices commentServices, IMapper mapper)
        {
            _articleServices = articleServices;
            _categoryServices = categoryServices;
            _commentServices = commentServices;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取分页文章
        /// </summary>
        /// <param name="page">页面索引</param>
        /// <param name="categoryId">分类各ID  8：赛事新闻；7：合作商介绍；6：协会介绍；1：队伍介绍</param>
        /// <returns></returns>
        [ResponseType(typeof(ArticleFVM))]
        public IHttpActionResult GetArticles(int categoryId, int page=1)
        {
            int pageIndex = page - 1;
            int count;
            var articles = _articleServices.GetActivePagedElements(pageIndex, SettingsManager.Article.FrontPageSize,string.Empty, categoryId, out count);
                      
            if (articles == null)
            {
                return NotFound();
            }
            var vm = _mapper.Map<List<Article>, List<ArticleFVM>>(articles);
            return Ok(vm);
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ArticleDetailFVM))]
        public async Task<IHttpActionResult> GetArticle(int id)
        {
            Article article = await _articleServices.GetByIdWithCategoryAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            article.ViewCount++;
            await _articleServices.UpdateAsync(article);

            var vm = _mapper.Map<Article, ArticleDetailFVM>(article);
            return Ok(vm);
        }

        /// <summary>
        /// 获取分页评论
        /// </summary>
        /// <param name="page">页面索引</param>
        /// <param name="articleId">文章ID</param>
        /// <returns></returns>
        [ResponseType(typeof(CommentFVM))]
        public IHttpActionResult GetComments(int articleId, int page=1)
        {
            var pageIndex = page-1;
            int count;
            var comments = _commentServices.GetActivePagedElements(pageIndex, SettingsManager.Article.FrontPageSize, string.Empty, articleId, out count);

            if (comments == null)
            {
                return NotFound();
            }
            var vm = _mapper.Map<List<Comment>, List<CommentFVM>>(comments);
            return Ok(vm);
        }

        /// <summary>
        /// 创建评论
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(CommentFIM))]
        public IHttpActionResult PostComment(CommentFIM comment)
        {
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var vm = _mapper.Map<CommentFIM, Comment>(comment);

            vm.Pubdate = DateTime.Now;
            vm.Name = User?.Identity.Name;
            vm.Active = false;

            _commentServices.Create(vm);

            return  CreatedAtRoute("DefaultApiNew", new { id = vm.Id, action= "GetComment" }, vm);
        }

        /// <summary>
        /// 按ID获取单条评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Comment))]
        public IHttpActionResult GetComment(int id)
        {
            Comment comment = _commentServices.GetById(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

    }
}