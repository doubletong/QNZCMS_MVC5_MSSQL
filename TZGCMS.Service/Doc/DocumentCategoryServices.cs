using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TZGCMS.Data.Entity.Doc;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Doc
{
    public interface IDocumentCategoryServices
    {
        IEnumerable<DocumentCategory> GetAll();
        List<DocumentCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount);
        DocumentCategory GetById(int id);
        DocumentCategory GetByIdWithDocuments(int id);
        bool Update(DocumentCategory category);
        DocumentCategory Create(DocumentCategory category);
        bool Delete(DocumentCategory category);

        bool IsExistSeoName(string seoName, int? id);

    }

    public class DocumentCategoryServices : IDocumentCategoryServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<DocumentCategory> GetAll()
        {
            return _unitOfWork.DocumentCategoryRepository.GetAll();
        }
        public List<DocumentCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount)
        {
            var totalIQuery = _unitOfWork.DocumentCategoryRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
         

            totalCount = totalIQuery.Count();

            //get list
            List<DocumentCategory> categories;
            Expression<Func<DocumentCategory, bool>> filter = g => true;
            Expression<Func<DocumentCategory, bool>> filterByKeyword = g => g.Title.Contains(keyword);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);            

            categories = _unitOfWork.DocumentCategoryRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return categories;
        }

        public DocumentCategory GetById(int id)
        {
            return _unitOfWork.DocumentCategoryRepository.GetById(id);
        }
        public DocumentCategory GetByIdWithDocuments(int id)
        {
            return _unitOfWork.DocumentCategoryRepository.Table.Include("Documents").FirstOrDefault(d=>d.Id == id);
        }
        public bool Update(DocumentCategory category)
        {
            return _unitOfWork.DocumentCategoryRepository.Update(category);
        }

        public DocumentCategory Create(DocumentCategory category)
        {
            return _unitOfWork.DocumentCategoryRepository.Insert(category);
        }
        public bool Delete(DocumentCategory category)
        {
            return _unitOfWork.DocumentCategoryRepository.Delete(category);
        }

        public bool IsExistSeoName(string seoName, int? id)
        {
            if (id != null)
            {
                return _unitOfWork.DocumentCategoryRepository.CountInt(d => d.SeoName == seoName && d.Id != id.Value) > 0;
            }

            return _unitOfWork.DocumentCategoryRepository.CountInt(d => d.SeoName == seoName) > 0;
        }
    }
}
