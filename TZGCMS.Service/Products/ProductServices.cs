using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Products;
using TZGCMS.Infrastructure.Extensions;
using TZGCMS.Model.Admin.InputModel.Products;

namespace TZGCMS.Service.Products
{
    public interface IProductServices
    {
        #region Async
        Task<Product> GetByIdWithCategoryAsync(int id);
        Task<bool> UpdateAsync(Product Product);
        #endregion
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetActiveElements();
        IEnumerable<Product> GetRecommendElements(int count);
        List<Product> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        List<Product> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        List<Product> GetActivePagedElements(int pageIndex, int pageSize, string keyword, string seoName, out int totalCount);
        Product GetById(int id);

        bool Update(Product Product);
        bool Update(ProductIM Product);
        Product Create(Product Product);
        Product Create(ProductIM Product);
        bool Delete(Product Product);
        //int InsertOrUpdate(ProductIM model);
        void Delete(int id);
        //bool CheckCode(string code);
    }

    public class ProductServices : IProductServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<Product> GetByIdWithCategoryAsync(int id)
        {
            return await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(d => d.Id == id, default(CancellationToken), d => d.Categories);
        }
        public async Task<bool> UpdateAsync(Product Product)
        {
            return await _unitOfWork.ProductRepository.UpdateAsync(Product);
        }
        #endregion

        public IEnumerable<Product> GetAll()
        {
            return _unitOfWork.ProductRepository.GetAll();
        }
        public IEnumerable<Product> GetActiveElements()
        {
            return _unitOfWork.ProductRepository.GetMany(d => d.Active);
        }
        public IEnumerable<Product> GetRecommendElements(int count)
        {
            return _unitOfWork.ProductRepository.GetMany(d => d.Recommend,d=>d.Importance,false).Take(count);
        }
        public List<Product> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.ProductRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.ProductName.Contains(keyword) || g.ProductNo.Contains(keyword) || g.Body.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.Categories.Any(c=>c.Id == categoryId));

            totalCount = totalIQuery.Count();

            //get list
            List<Product> Products;
            Expression<Func<Product, bool>> filter = g => true;
            Expression<Func<Product, bool>> filterByKeyword = g => g.ProductName.Contains(keyword) || g.ProductNo.Contains(keyword) || g.Body.Contains(keyword);
            Expression<Func<Product, bool>> filterByCategoryId = g => g.Categories.Any(c => c.Id == categoryId);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            Products = _unitOfWork.ProductRepository.GetPagedElements(pageIndex, pageSize, (c => c.Id), filter, false, (d => d.Categories)).ToList();

            return Products;
        }

        public List<Product> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.ProductRepository.Table.AsQueryable().Where(d => d.Active == true);
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.ProductName.Contains(keyword) || g.ProductNo.Contains(keyword) || g.Body.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.Categories.Any(c => c.Id == categoryId));

            totalCount = totalIQuery.Count();

            //get list
            List<Product> Products;
            Expression<Func<Product, bool>> filter = g => g.Active == true;
            Expression<Func<Product, bool>> filterByKeyword = g => g.ProductName.Contains(keyword) || g.ProductNo.Contains(keyword) || g.Body.Contains(keyword);
            Expression<Func<Product, bool>> filterByCategoryId = g => g.Categories.Any(c => c.Id == categoryId);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            Products = _unitOfWork.ProductRepository.GetPagedElements(pageIndex, pageSize, (c => c.Id), filter, false, (d => d.Categories)).ToList();

            return Products;
        }

        public List<Product> GetActivePagedElements(int pageIndex, int pageSize, string keyword, string seoName, out int totalCount)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetFirstOrDefault(d => d.SeoName == seoName);
            var totalIQuery = _unitOfWork.ProductRepository.Table.AsQueryable().Where(d => d.Active == true);
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.ProductName.Contains(keyword) || g.ProductNo.Contains(keyword) || g.Body.Contains(keyword));
            if (category!=null)
                totalIQuery = totalIQuery.Where(g => g.Categories.Any(c => c.Id == category.Id));

            totalCount = totalIQuery.Count();

            //get list
            List<Product> Products;
            Expression<Func<Product, bool>> filter = g => g.Active == true;
            Expression<Func<Product, bool>> filterByKeyword = g => g.ProductName.Contains(keyword) || g.ProductNo.Contains(keyword) || g.Body.Contains(keyword);
            Expression<Func<Product, bool>> filterByCategoryId = g => g.Categories.Any(c => c.Id == category.Id);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (category != null)
                filter = filter.AndAlso(filterByCategoryId);

            Products = _unitOfWork.ProductRepository.GetPagedElements(pageIndex, pageSize, (c => c.Id), filter, false, (d => d.Categories)).ToList();

            return Products;
        }

        public Product GetById(int id)
        {
            return _unitOfWork.ProductRepository.GetById(id);
        }

       


        public bool Update(Product Product)
        {
            
            return _unitOfWork.ProductRepository.Update(Product);
        }
        public bool Update(ProductIM im)
        {
            var editProduct = _unitOfWork.ProductRepository.GetById(im.Id);
            editProduct.ProductName = im.ProductName;
            editProduct.ProductNo = im.ProductNo;
            editProduct.Thumbnail = im.Thumbnail;
            editProduct.Cover = im.Cover;
            editProduct.Recommend = im.Recommend;
            editProduct.Active = im.Active;
            editProduct.Body = im.Body;
            editProduct.Description = im.Summary;
            editProduct.ImageUrl = im.ImageUrl;
           
            editProduct.Importance = im.Importance;

            editProduct.Categories.Clear();
            if (im.PostCategoryIds!=null)
            {
                var lCategories = (from c in _unitOfWork.ProductCategoryRepository.GetAll()
                                   where im.PostCategoryIds.Contains(c.Id.ToString())
                                   select c).ToList();

                editProduct.Categories = lCategories;
            }
                

            return _unitOfWork.ProductRepository.Update(editProduct);
        }

        public Product Create(Product Product)
        {
            return _unitOfWork.ProductRepository.Insert(Product);
        }
        public Product Create(ProductIM im)
        {
            var newProduct = new Product
            {
                ProductName = im.ProductName,
                ProductNo = im.ProductNo,
                Thumbnail = im.Thumbnail,
                Cover = im.Cover,
                Recommend = im.Recommend,
                Active = im.Active,
                Body = im.Body,
                Description = im.Summary,
                ImageUrl = im.ImageUrl,
                Importance = im.Importance
            };
            if (im.PostCategoryIds!=null)
            {
                var lCategories = (from c in _unitOfWork.ProductCategoryRepository.GetAll()
                                   where im.PostCategoryIds.Contains(c.Id.ToString())
                                   select c).ToList();

                newProduct.Categories = lCategories;
            }
          

            return _unitOfWork.ProductRepository.Insert(newProduct);
        }
        public bool Delete(Product Product)
        {
            return _unitOfWork.ProductRepository.Delete(Product);
        }

        public void Delete(int id)
        {
            var delobj = _unitOfWork.ProductRepository.Get(p => p.Id == id);
            _unitOfWork.ProductRepository.Delete(delobj);
        }

        //public bool CheckCode(string code)
        //{
        //    var check = _unitOfWork.ProductRepository.Get(p => p.Code.Equals(code));
        //    if (check != null)
        //        return true;
        //    return false;
        //}
    }
}
