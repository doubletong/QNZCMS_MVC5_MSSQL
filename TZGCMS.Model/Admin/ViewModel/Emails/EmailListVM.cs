﻿using PagedList;
using TZGCMS.Data.Entity.Emails;

namespace TZGCMS.Model.Admin.ViewModel.Emails
{
    public class EmailListVM
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Email> Emails { get; set; }
    }
}
