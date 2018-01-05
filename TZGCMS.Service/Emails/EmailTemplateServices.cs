using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Extensions;

namespace TZGCMS.Service.Emails
{
    public interface IEmailTemplateServices
    {
        List<EmailTemplate> GetPagedElements(int pageIndex, int pageSize, string keyword, int accountId, out int totalCount);
        EmailTemplate GetById(int id);
        bool Update(EmailTemplate template);
        bool Create(EmailTemplate template);
        bool Delete(EmailTemplate template);

        bool IsExistTemplate(string templateNo, int? id);
        EmailTemplate GetEmailTemplateByTemplateNo(string templateNo);
        string ReplaceTemplate(string body);
    }
    public class EmailTemplateServices : IEmailTemplateServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public List<EmailTemplate> GetPagedElements(int pageIndex, int pageSize, string keyword, int accountId, out int totalCount)
        {

            var totalIQuery = _unitOfWork.EmailTemplateRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.Subject.Contains(keyword));
            if (accountId > 0)
                totalIQuery = totalIQuery.Where(g => g.EmailAccountId == accountId);

            totalCount = totalIQuery.Count();

            //get list
            List<EmailTemplate> emails;
            Expression<Func<EmailTemplate, bool>> filter = g => true;
            Expression<Func<EmailTemplate, bool>> filterByKeyword = g => g.Subject.Contains(keyword);
            Expression<Func<EmailTemplate, bool>> filterByAccount = g => g.EmailAccountId == accountId;

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);

            if (accountId > 0)
                filter = filter.AndAlso(filterByAccount);

            emails = _unitOfWork.EmailTemplateRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreatedDate), filter, false).ToList();

            return emails;
        }

        public EmailTemplate GetById(int id)
        {
            return _unitOfWork.EmailTemplateRepository.GetById(id);
        }
        public bool Update(EmailTemplate template)
        {
            return _unitOfWork.EmailTemplateRepository.Update(template);
        }

        public bool Create(EmailTemplate template)
        {
            return _unitOfWork.EmailTemplateRepository.Insert(template);
        }
        public bool Delete(EmailTemplate template)
        {
            return _unitOfWork.EmailTemplateRepository.Delete(template);
        }

        public bool IsExistTemplate(string templateNo, int? id)
        {
            if (id != null)
            {
                return _unitOfWork.EmailTemplateRepository.CountInt(d => d.TemplateNo == templateNo && d.Id != id.Value) > 0;
            }

            return _unitOfWork.EmailTemplateRepository.CountInt(d => d.TemplateNo == templateNo) > 0;
        }

        public EmailTemplate GetEmailTemplateByTemplateNo(string templateNo)
        {
            return _unitOfWork.EmailTemplateRepository.GetMany(d => d.TemplateNo == templateNo).FirstOrDefault();
        }

        public string ReplaceTemplate(string body)
        {
            body = body.Replace("{SiteName}", SettingsManager.Site.SiteName);
            body = body.Replace("{SiteURL}", SettingsManager.Site.SiteDomainName);

            return body;
        }
    }
}
