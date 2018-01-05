using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Ads
{
    public interface IPositionServices
    {
        Task<Position> GetByCode(string code);
        IEnumerable<Position> GetAll();
        List<Position> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount);
        Position GetById(int id);
        Position GetByIdWithCarousels(int id);
        bool Update(Position position);
        bool Create(Position position);
        bool Delete(Position position);

        bool IsExistCode(string seoName, int? id);

    }

    public class PositionServices : IPositionServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<Position> GetAll()
        {
            return _unitOfWork.PositionRepository.GetAll();
        }
        public List<Position> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount)
        {
            var totalIQuery = _unitOfWork.PositionRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
         

            totalCount = totalIQuery.Count();

            //get list
            List<Position> categories;
            Expression<Func<Position, bool>> filter = g => true;
            Expression<Func<Position, bool>> filterByKeyword = g => g.Title.Contains(keyword);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);            

            categories = _unitOfWork.PositionRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return categories;
        }
        public async Task<Position> GetByCode(string code)
        {
            return await _unitOfWork.PositionRepository.GetFirstOrDefaultAsync(d => d.Code == code, default(CancellationToken), d=>d.Carousels);
        }
        public Position GetById(int id)
        {
            return _unitOfWork.PositionRepository.GetById(id);
        }
        public Position GetByIdWithCarousels(int id)
        {
            return _unitOfWork.PositionRepository.Table.Include("Carousels").FirstOrDefault(d=>d.Id == id);
        }
        public bool Update(Position position)
        {
            return _unitOfWork.PositionRepository.Update(position);
        }

        public bool Create(Position position)
        {
            return _unitOfWork.PositionRepository.Insert(position);
        }
        public bool Delete(Position position)
        {
            return _unitOfWork.PositionRepository.Delete(position);
        }

        public bool IsExistCode(string code, int? id)
        {
            if (id != null)
            {
                return _unitOfWork.PositionRepository.CountInt(d => d.Code == code && d.Id != id.Value) > 0;
            }

            return _unitOfWork.PositionRepository.CountInt(d => d.Code == code) > 0;
        }
    }
}
