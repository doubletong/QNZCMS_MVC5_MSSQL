using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Chronicles;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Chronicles
{

    public interface IChronicleServices
    {
        #region Async
        //Task<Chronicle> GetBySeoNameAsync(string seoName);
       
        #endregion

        List<Chronicle> GetActiveElements();
        List<Chronicle> GetChronicledElements(int pageIndex, int pageSize, string keyword,out int totalCount);
        Chronicle GetById(int id);

        bool Update(Chronicle page);
        Chronicle Create(Chronicle page);
        bool Delete(Chronicle page);

        //bool IsExistSeoName(string seoName, int? id);

    }

   public class ChronicleServices: IChronicleServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        //public async Task<Chronicle> GetBySeoNameAsync(string seoName)
        //{
        //    return await _unitOfWork.ChronicleRepository.GetFirstOrDefaultAsync(d => d.SeoName == seoName);
        //}

        #endregion

        public List<Chronicle> GetActiveElements()
        {
            return _unitOfWork.ChronicleRepository.GetMany(d => d.Active).OrderByDescending(d=>d.CreatedDate).ToList();
        }
        public List<Chronicle> GetChronicledElements(int pageIndex, int pageSize, string keyword, out int totalCount) {
            //get list count


            var totalIQuery = _unitOfWork.ChronicleRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
          

            totalCount = totalIQuery.Count();

            //get list
            List<Chronicle> pages;
            Expression<Func<Chronicle, bool>> filter = g => true;
            Expression<Func<Chronicle, bool>> filterByKeyword = g => g.Title.Contains(keyword);       

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);         


            pages = _unitOfWork.ChronicleRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return pages;
        }

        public Chronicle GetById(int id)
        {
            return _unitOfWork.ChronicleRepository.GetById(id);
        }


        public bool Update(Chronicle page)
        {
            return _unitOfWork.ChronicleRepository.Update(page);
        }

        public Chronicle Create(Chronicle page)
        {
            return _unitOfWork.ChronicleRepository.Insert(page);
        }
        public bool Delete(Chronicle page)
        {
            return _unitOfWork.ChronicleRepository.Delete(page);
        }

        //public bool IsExistSeoName(string seoName, int? id)
        //{
        //    if (id != null)
        //    {
        //        return _unitOfWork.ChronicleRepository.CountInt(d => d.SeoName == seoName && d.Id != id.Value) > 0;
        //    }

        //    return _unitOfWork.ChronicleRepository.CountInt(d => d.SeoName == seoName) > 0;
        //}
    }
}
