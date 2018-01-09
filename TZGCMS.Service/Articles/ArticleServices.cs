﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Infrastructure.Extensions;
using TZGCMS.Model.Inputs.Articles;
using TZGCMS.Model.Views.Articles;

namespace TZGCMS.Service.Articles
{
    public interface IArticleServices
    {
        #region Async
        Task<Article> GetByIdWithCategoryAsync(int id);
        Task<bool> UpdateAsync(Article article);
        #endregion
        IEnumerable<Article> GetAll();
        IEnumerable<Article> GetActiveElements();
        List<Article> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        List<Article> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        Article GetById(int id);
      
        bool Update(Article article);
        bool Create(Article article);
        bool Delete(Article article);
        int InsertOrUpdate(ArticleIM model);
        void Delete(int id);
        //bool CheckCode(string code);
    }

    public class ArticleServices : IArticleServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<Article> GetByIdWithCategoryAsync(int id)
        {
            return await _unitOfWork.ArticleRepository.GetFirstOrDefaultAsync(d => d.Id == id, default(CancellationToken), d => d.ArticleCategory);
        }
        public async Task<bool> UpdateAsync(Article article)
        {
            return await _unitOfWork.ArticleRepository.UpdateAsync(article);
        }
        #endregion

        public IEnumerable<Article> GetAll()
        {
            return _unitOfWork.ArticleRepository.GetAll();
        }
        public IEnumerable<Article> GetActiveElements()
        {
            return _unitOfWork.ArticleRepository.GetMany(d => d.Active);
        }
        public List<Article> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.ArticleRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            List<Article> articles;
            Expression<Func<Article, bool>> filter = g => true;
            Expression<Func<Article, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Article, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            articles = _unitOfWork.ArticleRepository.GetPagedElements(pageIndex, pageSize, (c => c.Pubdate), filter, false,(d=>d.ArticleCategory)).ToList();

            return articles;
        }

        public List<Article> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.ArticleRepository.Table.AsQueryable().Where(d=>d.Active==true);
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            List<Article> articles;
            Expression<Func<Article, bool>> filter = g => g.Active==true;
            Expression<Func<Article, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Article, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            articles = _unitOfWork.ArticleRepository.GetPagedElements(pageIndex, pageSize, (c => c.Pubdate), filter, false, (d => d.ArticleCategory)).ToList();

            return articles;
        }
        public Article GetById(int id)
        {
            return _unitOfWork.ArticleRepository.GetById(id);
        }

        public int InsertOrUpdate(ArticleIM model)
        {
            if (model.Id == 0)// add new
            {
                var add = new Article
                {
                    Title = model.Title,
                    Body = model.Body,
                    CategoryId = model.CategoryId,
                    Thumbnail = model.Thumbnail,
                    Active = model.Active,
                    Pubdate = model.Pubdate,
                    CreatedBy = "Admin",
                    CreatedDate = DateTime.Now,
                    Recommend = model.Recommend,
                    Source = model.Source,
                    Summary = model.Summary,
                    FullImage = model.FullImage                    
                    
                };
                _unitOfWork.ArticleRepository.Insert(add);
                return 1;
            }
            else //edit
            {
                var edit = _unitOfWork.ArticleRepository.Get(p => p.Id == model.Id);
                    edit.Title = model.Title;
                    edit.Body = model.Body;
                    edit.CategoryId = model.CategoryId;
                    edit.Thumbnail = model.Thumbnail;
                    edit.Active = model.Active;
                    edit.Pubdate = model.Pubdate;
                    edit.UpdatedBy = "Admin";
                    edit.UpdatedDate = DateTime.Now;
                    edit.Recommend = model.Recommend;
                    edit.Source = model.Source;
                    edit.Summary = model.Summary;
                    edit.FullImage = model.FullImage;
                _unitOfWork.ArticleRepository.Update(edit);
                return 2;
            }
        }


        public bool Update(Article article)
        {
            return _unitOfWork.ArticleRepository.Update(article);
        }

        public bool Create(Article article)
        {
            return _unitOfWork.ArticleRepository.Insert(article);
        }
        public bool Delete(Article article)
        {
            return _unitOfWork.ArticleRepository.Delete(article);
        }

        public void Delete(int id)
        {
            var delobj = _unitOfWork.ArticleRepository.Get(p => p.Id == id);
            _unitOfWork.ArticleRepository.Delete(delobj);
        }

        //public bool CheckCode(string code)
        //{
        //    var check = _unitOfWork.ArticleRepository.Get(p => p.Code.Equals(code));
        //    if (check != null)
        //        return true;
        //    return false;
        //}
    }
}