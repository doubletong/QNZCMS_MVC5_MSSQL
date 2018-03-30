using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TZGCMS.Data.Entity;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Outlets
{

    public interface IOutletServices
    {


        List<Outlet> GetActiveElements();
        List<Outlet> GetOutletdElements(int pageIndex, int pageSize, string keyword,out int totalCount);
        Outlet GetById(int id);

        bool Update(Outlet page);
        Outlet Create(Outlet page);
        bool Delete(Outlet page);


    }

   public class OutletServices: IOutletServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

   

        public List<Outlet> GetActiveElements()
        {
            return _unitOfWork.OutletRepository.GetMany(d => d.Active).OrderByDescending(d=>d.CreatedDate).ToList();
        }
        public List<Outlet> GetOutletdElements(int pageIndex, int pageSize, string keyword, out int totalCount) {
            //get list count


            var totalIQuery = _unitOfWork.OutletRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Name.Contains(keyword));
          

            totalCount = totalIQuery.Count();

            //get list
            List<Outlet> pages;
            Expression<Func<Outlet, bool>> filter = g => true;
            Expression<Func<Outlet, bool>> filterByKeyword = g => g.Name.Contains(keyword);       

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);         


            pages = _unitOfWork.OutletRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return pages;
        }

        public Outlet GetById(int id)
        {
            return _unitOfWork.OutletRepository.GetById(id);
        }


        public bool Update(Outlet page)
        {
            return _unitOfWork.OutletRepository.Update(page);
        }

        public Outlet Create(Outlet page)
        {
            return _unitOfWork.OutletRepository.Insert(page);
        }
        public bool Delete(Outlet page)
        {
            return _unitOfWork.OutletRepository.Delete(page);
        }

      
    }
}
