using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TZGCMS.Data.Entity;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Jobs
{

    public interface IJobServices
    {
        #region Async
        Task<Job> GetBySeoNameAsync(string seoName);
       
        #endregion

        List<Job> GetActiveElements();
        List<Job> GetJobdElements(int pageIndex, int pageSize, string keyword,out int totalCount);
        Job GetById(int id);

        bool Update(Job page);
        Job Create(Job page);
        bool Delete(Job page);

        bool IsExistSeoName(string seoName, int? id);

    }

   public class JobServices: IJobServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<Job> GetBySeoNameAsync(string seoName)
        {
            return await _unitOfWork.JobRepository.GetFirstOrDefaultAsync(d => d.SeoName == seoName);
        }

        #endregion

        public List<Job> GetActiveElements()
        {
            return _unitOfWork.JobRepository.GetMany(d => d.Active).OrderByDescending(d=>d.CreatedDate).ToList();
        }
        public List<Job> GetJobdElements(int pageIndex, int pageSize, string keyword, out int totalCount) {
            //get list count


            var totalIQuery = _unitOfWork.JobRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Post.Contains(keyword));
          

            totalCount = totalIQuery.Count();

            //get list
            List<Job> pages;
            Expression<Func<Job, bool>> filter = g => true;
            Expression<Func<Job, bool>> filterByKeyword = g => g.Post.Contains(keyword);       

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);         


            pages = _unitOfWork.JobRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return pages;
        }

        public Job GetById(int id)
        {
            return _unitOfWork.JobRepository.GetById(id);
        }


        public bool Update(Job page)
        {
            return _unitOfWork.JobRepository.Update(page);
        }

        public Job Create(Job page)
        {
            return _unitOfWork.JobRepository.Insert(page);
        }
        public bool Delete(Job page)
        {
            return _unitOfWork.JobRepository.Delete(page);
        }

        public bool IsExistSeoName(string seoName, int? id)
        {
            if (id != null)
            {
                return _unitOfWork.JobRepository.CountInt(d => d.SeoName == seoName && d.Id != id.Value) > 0;
            }

            return _unitOfWork.JobRepository.CountInt(d => d.SeoName == seoName) > 0;
        }
    }
}
