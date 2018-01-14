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
    public interface IEmailAccountServices
    {
        List<EmailAccount> GetPagedElements(int pageIndex, int pageSize, string keyword,  out int totalCount);
        IEnumerable<EmailAccount> GetElements();
        EmailAccount GetById(int id);
        bool Update(EmailAccount emailAccount);
        EmailAccount Create(EmailAccount emailAccount);
        bool Delete(EmailAccount emailAccount);
        EmailAccount SetDefault(int id);
    }
    public class EmailAccountServices: IEmailAccountServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public List<EmailAccount> GetPagedElements(int pageIndex, int pageSize, string keyword, out int totalCount)
        {

            var totalIQuery = _unitOfWork.EmailAccountRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Email.Contains(keyword));
           

            totalCount = totalIQuery.Count();

            //get list
            List<EmailAccount> account;
            Expression<Func<EmailAccount, bool>> filter = g => true;
            Expression<Func<EmailAccount, bool>> filterByKeyword = g => g.Email.Contains(keyword);
        

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            account = _unitOfWork.EmailAccountRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return account;
        }
        public IEnumerable<EmailAccount> GetElements()
        {            
            return _unitOfWork.EmailAccountRepository.GetAll();
        }

        public EmailAccount GetById(int id)
        {
            return _unitOfWork.EmailAccountRepository.GetById(id);
        }
        public bool Update(EmailAccount emailAccount)
        {
            return _unitOfWork.EmailAccountRepository.Update(emailAccount);
        }

        public EmailAccount Create(EmailAccount emailAccount)
        {
            return _unitOfWork.EmailAccountRepository.Insert(emailAccount);
        }
        public bool Delete(EmailAccount emailAccount)
        {
            return _unitOfWork.EmailAccountRepository.Delete(emailAccount);
        }

        public EmailAccount SetDefault(int id)
        {
            EmailAccount result = _unitOfWork.EmailAccountRepository.GetById(id);
            if (result == null)            
                return null;            

            if (!result.IsDefault)
            {
                var accounts = _unitOfWork.EmailAccountRepository.GetMany(d => d.IsDefault && d.Id != id); //_connection.GetList<EmailAccount>("where IsDefault=1 and ID<>@Id", new { Id = id });
                foreach (var item in accounts)
                {
                    item.IsDefault = false;                   
                }
                _unitOfWork.EmailAccountRepository.Update(accounts);
                result.IsDefault = true;
            }
            else
            {
                result.IsDefault = false;
            }
            _unitOfWork.EmailAccountRepository.Update(result);
            
            return result;

        }
    }
}
