using PagedList;
using TZGCMS.Data.Entity.Emails;

namespace TZGCMS.Model.Admin.ViewModel.Emails
{
    public class EmailAccountListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<EmailAccount> EmailAccounts { get; set; }
    }
}
