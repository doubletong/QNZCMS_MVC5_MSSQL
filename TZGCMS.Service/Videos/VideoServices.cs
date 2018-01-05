using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Infrastructure.Extensions;


namespace TZGCMS.Service.Videos
{
    public interface IVideoServices
    {
        #region async
            Task<IEnumerable<Video>> GetNoticedElementsAsync();
            Task<Video> GetByIdWidthCategoryAsync(int id);
        #endregion

        IEnumerable<Video> GetAll();
        IEnumerable<Video> GetActiveElements();
        
        List<Video> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        List<Video> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        List<Video> GetActiveArchivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount);
        Video GetById(int id);
      
        bool Update(Video article);
        bool Create(Video article);
        bool Delete(Video article);
        void Delete(int id);
        //bool CheckCode(string code);
    }

    public class VideoServices : IVideoServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

      

        public IEnumerable<Video> GetAll()
        {
            return _unitOfWork.VideoRepository.GetAll();
        }
        public IEnumerable<Video> GetActiveElements()
        {
            return _unitOfWork.VideoRepository.GetMany(d => d.Active);
        }
        public async Task<IEnumerable<Video>> GetNoticedElementsAsync()
        {
            return await _unitOfWork.VideoRepository.GetManyAsync(d => d.Active && DbFunctions.AddMinutes(DateTime.Now,60) > d.StartDate && d.StartDate > DateTime.Now, d=>d.Reservations);
        }
        public List<Video> GetPagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.VideoRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            Expression<Func<Video, bool>> filter = g => true;
            Expression<Func<Video, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Video, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            var videos = _unitOfWork.VideoRepository.GetPagedElements(pageIndex, pageSize, (c => c.StartDate), filter, true,(d=>d.VideoCategory)).ToList();

            return videos;
        }

        public List<Video> GetActiveArchivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId,
            out int totalCount)
        {
            var totalIQuery = _unitOfWork.VideoRepository.Table.Where(d => d.Active && d.EndDate < DateTime.Now).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            Expression<Func<Video, bool>> filter = g => g.Active && g.EndDate < DateTime.Now;
            Expression<Func<Video, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Video, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            var videos = _unitOfWork.VideoRepository.GetPagedElements(pageIndex, pageSize, (c => c.StartDate), filter, true, (d => d.VideoCategory)).ToList();

            return videos;
        }


        public List<Video> GetActivePagedElements(int pageIndex, int pageSize, string keyword, int categoryId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.VideoRepository.Table.Where(d => d.Active && d.EndDate >= DateTime.Now).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (categoryId > 0)
                totalIQuery = totalIQuery.Where(g => g.CategoryId == categoryId);

            totalCount = totalIQuery.Count();

            //get list
            Expression<Func<Video, bool>> filter = g => g.Active && g.EndDate >= DateTime.Now;
            Expression<Func<Video, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Video, bool>> filterByCategoryId = g => g.CategoryId == categoryId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (categoryId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            var videos = _unitOfWork.VideoRepository.GetPagedElements(pageIndex, pageSize, (c => c.StartDate), filter, true, (d => d.VideoCategory)).ToList();

            return videos;
        }


        public Video GetById(int id)
        {
            return _unitOfWork.VideoRepository.GetById(id);
        }

        public async Task<Video> GetByIdWidthCategoryAsync(int id)
        {
            return await _unitOfWork.VideoRepository.GetFirstOrDefaultAsync(d=>d.Id == id, default(CancellationToken), d=>d.VideoCategory);
        }


        public bool Update(Video article)
        {
            return _unitOfWork.VideoRepository.Update(article);
        }

        public bool Create(Video article)
        {
            return _unitOfWork.VideoRepository.Insert(article);
        }
        public bool Delete(Video article)
        {
            return _unitOfWork.VideoRepository.Delete(article);
        }

        public void Delete(int id)
        {
            var delobj = _unitOfWork.VideoRepository.Get(p => p.Id == id);
            _unitOfWork.VideoRepository.Delete(delobj);
        }

        //public bool CheckCode(string code)
        //{
        //    var check = _unitOfWork.VideoRepository.Get(p => p.Code.Equals(code));
        //    if (check != null)
        //        return true;
        //    return false;
        //}
    }
}
