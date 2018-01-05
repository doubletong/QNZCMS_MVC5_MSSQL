using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Ads
{
    public interface ICarouselServices
    {
      
        IEnumerable<Carousel> GetAll();
        IEnumerable<Carousel> GetActiveElements();
        List<Carousel> GetPagedElements(int pageIndex, int pageSize, string keyword, int positionId, out int totalCount);
        Carousel GetById(int id);
      
        bool Update(Carousel carousel);
        bool Create(Carousel carousel);
        bool Delete(Carousel carousel);
        void Delete(int id);
        //bool CheckCode(string code);
    }

    public class CarouselServices : ICarouselServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public IEnumerable<Carousel> GetAll()
        {
            return _unitOfWork.CarouselRepository.GetAll();
        }
        public IEnumerable<Carousel> GetActiveElements()
        {
            return _unitOfWork.CarouselRepository.GetMany(d => d.Active);
        }
        public List<Carousel> GetPagedElements(int pageIndex, int pageSize, string keyword, int positionId, out int totalCount)
        {
            var totalIQuery = _unitOfWork.CarouselRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            if (positionId > 0)
                totalIQuery = totalIQuery.Where(g => g.PositionId == positionId);

            totalCount = totalIQuery.Count();

            //get list
            List<Carousel> carousels;
            Expression<Func<Carousel, bool>> filter = g => true;
            Expression<Func<Carousel, bool>> filterByKeyword = g => g.Title.Contains(keyword);
            Expression<Func<Carousel, bool>> filterByPositionId = g => g.PositionId == positionId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (positionId > 0)
                filter = filter.AndAlso(filterByPositionId);

            carousels = _unitOfWork.CarouselRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false,(d=>d.Position)).ToList();

            return carousels;
        }
        public Carousel GetById(int id)
        {
            return _unitOfWork.CarouselRepository.GetById(id);
        }
        

        public bool Update(Carousel carousel)
        {
            return _unitOfWork.CarouselRepository.Update(carousel);
        }

        public bool Create(Carousel carousel)
        {
            return _unitOfWork.CarouselRepository.Insert(carousel);
        }
        public bool Delete(Carousel carousel)
        {
            return _unitOfWork.CarouselRepository.Delete(carousel);
        }

        public void Delete(int id)
        {
            var delobj = _unitOfWork.CarouselRepository.Get(p => p.Id == id);
            _unitOfWork.CarouselRepository.Delete(delobj);
        }

        //public bool CheckCode(string code)
        //{
        //    var check = _unitOfWork.CarouselRepository.Get(p => p.Code.Equals(code));
        //    if (check != null)
        //        return true;
        //    return false;
        //}
    }
}
