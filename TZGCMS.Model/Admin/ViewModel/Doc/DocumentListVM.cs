using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Doc;

namespace TZGCMS.Model.Admin.ViewModel.Doc
{
    public class DocumentListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Document> Documents { get; set; }
        public int? CategoryId { get; set; }
        public string Keyword { get; set; }
    }
}
