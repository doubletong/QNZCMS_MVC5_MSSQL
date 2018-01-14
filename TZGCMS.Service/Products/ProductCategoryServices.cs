using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Products
{
    public interface IProductCategoryServices
    {
        IEnumerable<ProductCategory> GetAll();
        IEnumerable<ProductCategory> GetActiveItems();
        List<ProductCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount);
        ProductCategory GetById(int id);
        ProductCategory GetByIdWithArticles(int id);
        bool Update(ProductCategory category);
        ProductCategory Create(ProductCategory category);
        bool Delete(ProductCategory category);
        bool Delete(int id);
        bool IsExistSeoName(string seoName, int? id);

    }

    public class ProductCategoryServices : IProductCategoryServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<ProductCategory> GetAll()
        {
            return _unitOfWork.ProductCategoryRepository.GetAll();
        }
        public IEnumerable<ProductCategory> GetActiveItems()
        {
            return _unitOfWork.ProductCategoryRepository.GetMany(d => d.Active,d=>d.Importance,false);
        }
        public List<ProductCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount)
        {
            var totalIQuery = _unitOfWork.ProductCategoryRepository.Table.Where(d=>d.ParentId==null).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
         

            totalCount = totalIQuery.Count();

            //get list
            List<ProductCategory> categories;
            Expression<Func<ProductCategory, bool>> filter = g => g.ParentId == null;
            Expression<Func<ProductCategory, bool>> filterByKeyword = g => g.Title.Contains(keyword);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);            

            categories = _unitOfWork.ProductCategoryRepository.GetPagedElements(pageIndex, pageSize, (c => c.Importance), filter, false).ToList();

            return categories;
        }

        public ProductCategory GetById(int id)
        {
            return _unitOfWork.ProductCategoryRepository.GetById(id);
        }
        public ProductCategory GetByIdWithArticles(int id)
        {
            return _unitOfWork.ProductCategoryRepository.Table.Include("Articles").FirstOrDefault(d=>d.Id == id);
        }
        public bool Update(ProductCategory category)
        {
            return _unitOfWork.ProductCategoryRepository.Update(category);
        }

        public ProductCategory Create(ProductCategory category)
        {
            return _unitOfWork.ProductCategoryRepository.Insert(category);
        }
        public bool Delete(ProductCategory category)
        {
            return _unitOfWork.ProductCategoryRepository.Delete(category);
        }
        public bool Delete(int id)
        {
            var category =  _unitOfWork.ProductCategoryRepository.GetById(id);           
            return _unitOfWork.ProductCategoryRepository.Delete(category);
        }

        public bool IsExistSeoName(string seoName, int? id)
        {
            if (id != null)
            {
                return _unitOfWork.ProductCategoryRepository.CountInt(d => d.SeoName == seoName && d.Id != id.Value) > 0;
            }

            return _unitOfWork.ProductCategoryRepository.CountInt(d => d.SeoName == seoName) > 0;
        }
    }
}
