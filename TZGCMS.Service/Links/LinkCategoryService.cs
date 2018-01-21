using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TZGCMS.Data.Entity.Links;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Links
{
    public interface ILinkCategoryServices
    {
        IEnumerable<LinkCategory> GetAll();
        List<LinkCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount);
        LinkCategory GetById(int id);
        LinkCategory GetByIdWithLinks(int id);
        bool Update(LinkCategory category);
        LinkCategory Create(LinkCategory category);
        bool Delete(LinkCategory category);

 
    }

    public class LinkCategoryServices : ILinkCategoryServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<LinkCategory> GetAll()
        {
            return _unitOfWork.LinkCategoryRepository.GetAll();
        }
        public List<LinkCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount)
        {
            var totalIQuery = _unitOfWork.LinkCategoryRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
         

            totalCount = totalIQuery.Count();

            //get list
            List<LinkCategory> categories;
            Expression<Func<LinkCategory, bool>> filter = g => true;
            Expression<Func<LinkCategory, bool>> filterByKeyword = g => g.Title.Contains(keyword);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);            

            categories = _unitOfWork.LinkCategoryRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return categories;
        }

        public LinkCategory GetById(int id)
        {
            return _unitOfWork.LinkCategoryRepository.GetById(id);
        }
        public LinkCategory GetByIdWithLinks(int id)
        {
            return _unitOfWork.LinkCategoryRepository.Table.Include("Links").FirstOrDefault(d=>d.Id == id);
        }
        public bool Update(LinkCategory category)
        {
            return _unitOfWork.LinkCategoryRepository.Update(category);
        }

        public LinkCategory Create(LinkCategory category)
        {
            return _unitOfWork.LinkCategoryRepository.Insert(category);
        }
        public bool Delete(LinkCategory category)
        {
            return _unitOfWork.LinkCategoryRepository.Delete(category);
        }

    
    }
}
