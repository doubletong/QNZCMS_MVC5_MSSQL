using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TZGCMS.Data.Entity;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Teams
{

    public interface ITeamServices
    {


        List<Team> GetActiveElements();
        List<Team> GetTeamdElements(int pageIndex, int pageSize, string keyword,out int totalCount);
        Team GetById(int id);

        bool Update(Team page);
        Team Create(Team page);
        bool Delete(Team page);


    }

   public class TeamServices: ITeamServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

   

        public List<Team> GetActiveElements()
        {
            return _unitOfWork.TeamRepository.GetMany(d => d.Active).OrderByDescending(d=>d.CreatedDate).ToList();
        }
        public List<Team> GetTeamdElements(int pageIndex, int pageSize, string keyword, out int totalCount) {
            //get list count


            var totalIQuery = _unitOfWork.TeamRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Post.Contains(keyword));
          

            totalCount = totalIQuery.Count();

            //get list
            List<Team> pages;
            Expression<Func<Team, bool>> filter = g => true;
            Expression<Func<Team, bool>> filterByKeyword = g => g.Post.Contains(keyword);       

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);         


            pages = _unitOfWork.TeamRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return pages;
        }

        public Team GetById(int id)
        {
            return _unitOfWork.TeamRepository.GetById(id);
        }


        public bool Update(Team page)
        {
            return _unitOfWork.TeamRepository.Update(page);
        }

        public Team Create(Team page)
        {
            return _unitOfWork.TeamRepository.Insert(page);
        }
        public bool Delete(Team page)
        {
            return _unitOfWork.TeamRepository.Delete(page);
        }

      
    }
}
