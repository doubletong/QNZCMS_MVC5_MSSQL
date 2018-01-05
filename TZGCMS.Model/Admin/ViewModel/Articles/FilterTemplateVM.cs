using System;

namespace TZGCMS.Model.Admin.ViewModel.Articles
{
    public class FilterTemplateVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Encode { get; set; }

        public bool Active { get; set; }
        //public string Title { get; set; }
        //public string Description { get; set; }
        //public string Body { get; set; }
        //public string PublishDate { get; set; }
        //public string Author { get; set; }
        //public string Keyword { get; set; }
    }
}
