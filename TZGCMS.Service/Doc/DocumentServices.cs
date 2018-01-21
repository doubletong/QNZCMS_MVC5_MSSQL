using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Doc;

using TZGCMS.Infrastructure.Extensions;


namespace TZGCMS.Service.Doc
{
    public interface IDocumentServices
    {
        #region Async
        Task<Document> GetByIdWithCategoryAsync(int id);
        Task<bool> UpdateAsync(Document Document);
        #endregion
        IEnumerable<Document> GetAll();
        IEnumerable<Document> GetActiveElements();
        List<Document> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        List<Document> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        Document GetById(int id);
      
        bool Update(Document Document);
        Document Create(Document Document);
        bool Delete(Document Document);

        void Delete(int id);
        //bool CheckCode(string code);
    }

    public class DocumentServices : IDocumentServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<Document> GetByIdWithCategoryAsync(int id)
        {
            return await _unitOfWork.DocumentRepository.GetFirstOrDefaultAsync(d => d.Id == id, default(CancellationToken), d => d.Category);
        }
        public async Task<bool> UpdateAsync(Document Document)
        {
            return await _unitOfWork.DocumentRepository.UpdateAsync(Document);
        }
        #endregion

        public IEnumerable<Document> GetAll()
        {
            return _unitOfWork.DocumentRepository.GetAll();
        }
        public IEnumerable<Document> GetActiveElements()
        {
            return _unitOfWork.DocumentRepository.GetMany(d => d.Active);
        }
        public List<Document> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.DocumentRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            List<Document> Documents;
            Expression<Func<Document, bool>> filter = g => true;
            Expression<Func<Document, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Document, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            Documents = _unitOfWork.DocumentRepository.GetPagedElements(pageIndex, pageSize, (c => c.Id), filter, false,(d=>d.Category)).ToList();

            return Documents;
        }

        public List<Document> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.DocumentRepository.Table.AsQueryable().Where(d=>d.Active==true);
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            List<Document> Documents;
            Expression<Func<Document, bool>> filter = g => g.Active==true;
            Expression<Func<Document, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Document, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            Documents = _unitOfWork.DocumentRepository.GetPagedElements(pageIndex, pageSize, (c => c.Id), filter, false, (d => d.Category)).ToList();

            return Documents;
        }
        public Document GetById(int id)
        {
            return _unitOfWork.DocumentRepository.GetById(id);
        }

        


        public bool Update(Document Document)
        {
            return _unitOfWork.DocumentRepository.Update(Document);
        }

        public Document Create(Document Document)
        {
            return _unitOfWork.DocumentRepository.Insert(Document);
        }
        public bool Delete(Document Document)
        {
            return _unitOfWork.DocumentRepository.Delete(Document);
        }

        public void Delete(int id)
        {
            var delobj = _unitOfWork.DocumentRepository.Get(p => p.Id == id);
            _unitOfWork.DocumentRepository.Delete(delobj);
        }

        //public bool CheckCode(string code)
        //{
        //    var check = _unitOfWork.DocumentRepository.Get(p => p.Code.Equals(code));
        //    if (check != null)
        //        return true;
        //    return false;
        //}
    }
}
