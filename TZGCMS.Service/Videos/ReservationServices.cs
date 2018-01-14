using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Infrastructure.Extensions;


namespace TZGCMS.Service.Videos
{
    public interface IReservationServices
    {
        #region Async
        Task<Reservation> GetByIdWithCategoryAsync(int videoId, string openId);
        Task<bool> UpdateAsync(Reservation article);
        Task<IEnumerable<Reservation>> GetElementsAsync(Expression<Func<Reservation, bool>> expression);
        #endregion
        IEnumerable<Reservation> GetAll();
        IEnumerable<Reservation> GetElementsByOpenId(string openId);
       

        List<Reservation> GetPagedElements(int pageIndex, int pageSize, int articleId, out int totalCount);
        Reservation GetById(int videoId, string openId);

        bool Update(Reservation article);
        Reservation Create(Reservation article);
        bool Delete(Reservation article);

        bool Delete(int videoId, string openId);
        //bool CheckCode(string code);
    }

    public class ReservationServices : IReservationServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Async
        public async Task<Reservation> GetByIdWithCategoryAsync(int videoId, string openId)
        {
            return await _unitOfWork.ReservationRepository.GetFirstOrDefaultAsync(d => d.VideoId == videoId && d.OpenId == openId, default(CancellationToken), d => d.Video);
        }
        public async Task<bool> UpdateAsync(Reservation article)
        {
            return await _unitOfWork.ReservationRepository.UpdateAsync(article);
        }

        public async Task<IEnumerable<Reservation>> GetElementsAsync(Expression<Func<Reservation, bool>> expression)
        {
            return await _unitOfWork.ReservationRepository.GetManyAsync(expression);
        }
        #endregion

        public IEnumerable<Reservation> GetAll()
        {
            return _unitOfWork.ReservationRepository.GetAll();
        }

        public IEnumerable<Reservation> GetElementsByOpenId(string openId)
        {
           return _unitOfWork.ReservationRepository.GetMany(d => d.OpenId == openId);
        }


        public List<Reservation> GetPagedElements(int pageIndex, int pageSize,  int articleId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.ReservationRepository.Table.AsQueryable();
            if (articleId > 0)
                totalIQuery = totalIQuery.Where(g => g.VideoId == articleId);

            totalCount = totalIQuery.Count();

            //get list
            Expression<Func<Reservation, bool>> filter = g => true;
          
            Expression<Func<Reservation, bool>> filterByCategoryId = g => g.VideoId == articleId;

            if (articleId > 0)
                filter = filter.AndAlso(filterByCategoryId);

            var reservations = _unitOfWork.ReservationRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false, (d=>d.Video)).ToList();

            return reservations;
        }

       
        public Reservation GetById(int videoId, string openId)
        {
            return _unitOfWork.ReservationRepository.GetFirstOrDefault(p => p.VideoId == videoId && p.OpenId == openId);
        }
        

        public bool Update(Reservation article)
        {
            return _unitOfWork.ReservationRepository.Update(article);
        }

        public Reservation Create(Reservation article)
        {
            return _unitOfWork.ReservationRepository.Insert(article);
        }
        public bool Delete(Reservation article)
        {
            return _unitOfWork.ReservationRepository.Delete(article);
        }

        public bool Delete(int videoId,string openId)
        {
            var delobj = _unitOfWork.ReservationRepository.GetFirstOrDefault(p => p.VideoId == videoId && p.OpenId == openId);
            return Delete(delobj);
        }

    }
}
