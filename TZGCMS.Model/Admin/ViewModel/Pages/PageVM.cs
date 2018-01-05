using System;

namespace TZGCMS.Model.Admin.ViewModel.Pages
{
    public class PageVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SeoName { get; set; }
        public string Body { get; set; }
        public bool Active { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
