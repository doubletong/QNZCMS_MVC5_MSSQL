using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Videos
{
    public interface IVideoCategoryServices
    {
        #region Async
        Task<IEnumerable<VideoCategory>> GetActiveElementsAsync();
        #endregion
        IEnumerable<VideoCategory> GetAll();
   
        List<VideoCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount);
        VideoCategory GetById(int id);
        VideoCategory GetByIdWithVideos(int id);
        bool Update(VideoCategory category);
        bool Create(VideoCategory category);
        bool Delete(VideoCategory category);

        bool IsExistSeoName(string seoName, int? id);

    }

    public class VideoCategoryServices : IVideoCategoryServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<IEnumerable<VideoCategory>> GetActiveElementsAsync() {
            return await _unitOfWork.VideoCategoryRepository.GetManyAsync(d => d.Active,d=>d.Importance,false);
        }
        #endregion
        public IEnumerable<VideoCategory> GetAll()
        {
            return _unitOfWork.VideoCategoryRepository.GetAll();
        }
        public List<VideoCategory> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount)
        {
            var totalIQuery = _unitOfWork.VideoCategoryRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
         

            totalCount = totalIQuery.Count();

            //get list
            List<VideoCategory> categories;
            Expression<Func<VideoCategory, bool>> filter = g => true;
            Expression<Func<VideoCategory, bool>> filterByKeyword = g => g.Title.Contains(keyword);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);            

            categories = _unitOfWork.VideoCategoryRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return categories;
        }

        public VideoCategory GetById(int id)
        {
            return _unitOfWork.VideoCategoryRepository.GetById(id);
        }
        public VideoCategory GetByIdWithVideos(int id)
        {
            return _unitOfWork.VideoCategoryRepository.Table.Include("Videos").FirstOrDefault(d=>d.Id == id);
        }
        public bool Update(VideoCategory category)
        {
            return _unitOfWork.VideoCategoryRepository.Update(category);
        }

        public bool Create(VideoCategory category)
        {
            return _unitOfWork.VideoCategoryRepository.Insert(category);
        }
        public bool Delete(VideoCategory category)
        {
            return _unitOfWork.VideoCategoryRepository.Delete(category);
        }

        public bool IsExistSeoName(string seoName, int? id)
        {
            if (id != null)
            {
                return _unitOfWork.VideoCategoryRepository.CountInt(d => d.SeoName == seoName && d.Id != id.Value) > 0;
            }

            return _unitOfWork.VideoCategoryRepository.CountInt(d => d.SeoName == seoName) > 0;
        }
    }
}
