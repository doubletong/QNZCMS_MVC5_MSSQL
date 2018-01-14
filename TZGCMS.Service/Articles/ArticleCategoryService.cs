using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Articles
{
    public interface IArticleCategoryServices
    {
        IEnumerable<ArticleCategory> GetAll();
        List<ArticleCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount);
        ArticleCategory GetById(int id);
        ArticleCategory GetByIdWithArticles(int id);
        bool Update(ArticleCategory category);
        ArticleCategory Create(ArticleCategory category);
        bool Delete(ArticleCategory category);

        bool IsExistSeoName(string seoName, int? id);

    }

    public class ArticleCategoryServices : IArticleCategoryServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<ArticleCategory> GetAll()
        {
            return _unitOfWork.ArticleCategoryRepository.GetAll();
        }
        public List<ArticleCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount)
        {
            var totalIQuery = _unitOfWork.ArticleCategoryRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
         

            totalCount = totalIQuery.Count();

            //get list
            List<ArticleCategory> categories;
            Expression<Func<ArticleCategory, bool>> filter = g => true;
            Expression<Func<ArticleCategory, bool>> filterByKeyword = g => g.Title.Contains(keyword);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);            

            categories = _unitOfWork.ArticleCategoryRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return categories;
        }

        public ArticleCategory GetById(int id)
        {
            return _unitOfWork.ArticleCategoryRepository.GetById(id);
        }
        public ArticleCategory GetByIdWithArticles(int id)
        {
            return _unitOfWork.ArticleCategoryRepository.Table.Include("Articles").FirstOrDefault(d=>d.Id == id);
        }
        public bool Update(ArticleCategory category)
        {
            return _unitOfWork.ArticleCategoryRepository.Update(category);
        }

        public ArticleCategory Create(ArticleCategory category)
        {
            return _unitOfWork.ArticleCategoryRepository.Insert(category);
        }
        public bool Delete(ArticleCategory category)
        {
            return _unitOfWork.ArticleCategoryRepository.Delete(category);
        }

        public bool IsExistSeoName(string seoName, int? id)
        {
            if (id != null)
            {
                return _unitOfWork.ArticleCategoryRepository.CountInt(d => d.SeoName == seoName && d.Id != id.Value) > 0;
            }

            return _unitOfWork.ArticleCategoryRepository.CountInt(d => d.SeoName == seoName) > 0;
        }
    }
}
