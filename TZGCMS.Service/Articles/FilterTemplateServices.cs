using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Articles;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Articles
{
    public interface IFilterTemplateServices
    {
        IEnumerable<FilterTemplate> GetActiveElements();
        List<FilterTemplate> GetPagedElements(int pageIndex, int pageSize, string keyword,  out int totalCount);
        FilterTemplate GetById(int id);
        bool Update(FilterTemplate filterTemplate);
        FilterTemplate Create(FilterTemplate filterTemplate);
        bool Delete(FilterTemplate filterTemplate);
    }
    public class FilterTemplateServices: IFilterTemplateServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public IEnumerable<FilterTemplate> GetActiveElements()
        {
            return _unitOfWork.FilterTemplateRepository.GetMany(d => d.Active);
        }
        public  List<FilterTemplate> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount)
        {
            var totalIQuery = _unitOfWork.FilterTemplateRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Title.Contains(keyword));
            

            totalCount = totalIQuery.Count();

            //get list
            List<FilterTemplate> templates;
            Expression<Func<FilterTemplate, bool>> filter = g => true;
            Expression<Func<FilterTemplate, bool>> filterByKeyword = g => g.Title.Contains(keyword);
         

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);          

            templates = _unitOfWork.FilterTemplateRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return templates;
        }
        public FilterTemplate GetById(int id)
        {
            return _unitOfWork.FilterTemplateRepository.GetById(id);
        }
        public bool Update(FilterTemplate filterTemplate)
        {
            return _unitOfWork.FilterTemplateRepository.Update(filterTemplate);
        }

        public FilterTemplate Create(FilterTemplate filterTemplate)
        {
            return _unitOfWork.FilterTemplateRepository.Insert(filterTemplate);
        }
        public bool Delete(FilterTemplate filterTemplate)
        {
            return _unitOfWork.FilterTemplateRepository.Delete(filterTemplate);
        }
    }
}
