using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Pages;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Pages
{

    public interface IPageServices
    {
        #region Async
        Task<Page> GetBySeoNameAsync(string seoName);
       
        #endregion

        List<Page> GetActiveElements();
        List<Page> GetPagedElements(int pageIndex, int pageSize, string keyword,out int totalCount);
        Page GetById(int id);

        bool Update(Page page);
        Page Create(Page page);
        bool Delete(Page page);

        bool IsExistSeoName(string seoName, int? id);

    }

   public class PageServices: IPageServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<Page> GetBySeoNameAsync(string seoName)
        {
            return await _unitOfWork.PageRepository.GetFirstOrDefaultAsync(d => d.SeoName == seoName);
        }

        #endregion

        public List<Page> GetActiveElements()
        {
            return _unitOfWork.PageRepository.GetMany(d => d.Active).OrderByDescending(d=>d.CreatedDate).ToList();
        }
        public List<Page> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount) {
            //get list count


            var totalIQuery = _unitOfWork.PageRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
          

            totalCount = totalIQuery.Count();

            //get list
            List<Page> pages;
            Expression<Func<Page, bool>> filter = g => true;
            Expression<Func<Page, bool>> filterByKeyword = g => g.Title.Contains(keyword);       

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);         


            pages = _unitOfWork.PageRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return pages;
        }

        public Page GetById(int id)
        {
            return _unitOfWork.PageRepository.GetById(id);
        }


        public bool Update(Page page)
        {
            return _unitOfWork.PageRepository.Update(page);
        }

        public Page Create(Page page)
        {
            return _unitOfWork.PageRepository.Insert(page);
        }
        public bool Delete(Page page)
        {
            return _unitOfWork.PageRepository.Delete(page);
        }

        public bool IsExistSeoName(string seoName, int? id)
        {
            if (id != null)
            {
                return _unitOfWork.PageRepository.CountInt(d => d.SeoName == seoName && d.Id != id.Value) > 0;
            }

            return _unitOfWork.PageRepository.CountInt(d => d.SeoName == seoName) > 0;
        }
    }
}
