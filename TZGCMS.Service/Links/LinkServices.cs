using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Links;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Links
{
    public interface ILinkServices
    {
        #region Async
        Task<Link> GetByIdWithCategoryAsync(int id);
        Task<bool> UpdateAsync(Link article);
        #endregion
        IEnumerable<Link> GetAll();
        IEnumerable<Link> GetActiveElements();
        List<Link> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        List<Link> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);

        Link GetById(int id);
      
        bool Update(Link article);
        Link Create(Link article);
        bool Delete(Link article);
       
        void Delete(int id);
        //bool CheckCode(string code);
    }

    public class LinkServices : ILinkServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<Link> GetByIdWithCategoryAsync(int id)
        {
            return await _unitOfWork.LinkRepository.GetFirstOrDefaultAsync(d => d.Id == id, default(CancellationToken), d => d.LinkCategory);
        }
        public async Task<bool> UpdateAsync(Link article)
        {
            return await _unitOfWork.LinkRepository.UpdateAsync(article);
        }

        #endregion

        public IEnumerable<Link> GetAll()
        {
            return _unitOfWork.LinkRepository.GetAll();
        }
        public IEnumerable<Link> GetActiveElements()
        {
            return _unitOfWork.LinkRepository.GetMany(d => d.Active);
        }
        public List<Link> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.LinkRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            List<Link> articles;
            Expression<Func<Link, bool>> filter = g => true;
            Expression<Func<Link, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Link, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            articles = _unitOfWork.LinkRepository.GetPagedElements(pageIndex, pageSize, (c => c.Importance), filter, false,(d=>d.LinkCategory)).ToList();

            return articles;
        }

        public List<Link> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.LinkRepository.Table.AsQueryable().Where(d=>d.Active==true);
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            List<Link> articles;
            Expression<Func<Link, bool>> filter = g => g.Active==true;
            Expression<Func<Link, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Link, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            articles = _unitOfWork.LinkRepository.GetPagedElements(pageIndex, pageSize, (c => c.Importance), filter, false, (d => d.LinkCategory)).ToList();

            return articles;
        }
        public Link GetById(int id)
        {
            return _unitOfWork.LinkRepository.GetById(id);
        }

     


        public bool Update(Link article)
        {
            return _unitOfWork.LinkRepository.Update(article);
        }

        public Link Create(Link article)
        {
            return _unitOfWork.LinkRepository.Insert(article);
        }
        public bool Delete(Link article)
        {
            return _unitOfWork.LinkRepository.Delete(article);
        }

        public void Delete(int id)
        {
            var delobj = _unitOfWork.LinkRepository.Get(p => p.Id == id);
            _unitOfWork.LinkRepository.Delete(delobj);
        }

        //public bool CheckCode(string code)
        //{
        //    var check = _unitOfWork.LinkRepository.Get(p => p.Code.Equals(code));
        //    if (check != null)
        //        return true;
        //    return false;
        //}
    }
}
