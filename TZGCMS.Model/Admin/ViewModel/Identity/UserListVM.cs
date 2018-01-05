using PagedList;
using System;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Model.Admin.InputModel.Identity;

namespace TZGCMS.Model.Admin.ViewModel.Identity
{
    public class UserListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public string Keyword { get; set; }

       
        public DateTime? StartDate { get; set; }
       
        public DateTime? EndDate { get; set; }
        public int? RoleId { get; set; }

        public StaticPagedList<User> Users { get; set; }

        public SetPasswordIM SetPasswordIM { get; set; }

    }
}
