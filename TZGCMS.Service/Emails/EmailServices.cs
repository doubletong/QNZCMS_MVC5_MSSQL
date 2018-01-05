using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Emails
{
    public interface IEmailServices
    {
        List<Email> GetPagedElements(int pageIndex, int pageSize, string keyword,bool? active, out int totalCount);
        Email GetById(int id);
        bool Update(Email email);
        bool Create(Email email);
        bool Delete(Email email);
        bool DeleteEmails(int[] ids);
        bool IsTrashEmails(int[] ids, bool active);
    }
    public class EmailServices: IEmailServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public List<Email> GetPagedElements(int pageIndex, int pageSize, string keyword, bool? active, out int totalCount)
        {

            var totalIQuery = _unitOfWork.EmailRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Subject.Contains(keyword));
            if(active!=null)
                totalIQuery = totalIQuery.Where(g => g.Active == active);

            totalCount = totalIQuery.Count();

            //get list
            List<Email> emails;
            Expression<Func<Email, bool>> filter = g => true;
            Expression<Func<Email, bool>> filterByKeyword = g => g.Subject.Contains(keyword);
            Expression<Func<Email, bool>> filterByActive = g => g.Active == active;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (active != null)
                filter = filter.AndAlso(filterByActive);

            emails = _unitOfWork.EmailRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return emails;
        }

        public Email GetById(int id)
        {
            return _unitOfWork.EmailRepository.GetById(id);
        }
        public bool Update(Email email)
        {
            return _unitOfWork.EmailRepository.Update(email);
        }

        public bool Create(Email email)
        {
            return _unitOfWork.EmailRepository.Insert(email);
        }
        public bool Delete(Email email)
        {
            return _unitOfWork.EmailRepository.Delete(email);
        }
        public bool DeleteEmails(int[] ids)
        {
            var emails = _unitOfWork.EmailRepository.GetMany(d => ids.Contains(d.Id));
            return _unitOfWork.EmailRepository.Delete(emails);
        }
        
        public bool IsTrashEmails(int[] ids, bool active)
        {

            var emails = _unitOfWork.EmailRepository.GetMany(d => ids.Contains(d.Id));
            foreach (var item in emails)
            {
                item.Active = active;
            }

            return _unitOfWork.EmailRepository.Update(emails);

        }
 
    }
}
