﻿using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Doc;

namespace TZGCMS.Model.Front.ViewModel.Doc
{
    public class DocumentListFVM
    {
        public int CateId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Document> Documents { get; set; }
        public IEnumerable<DocumentCategory> Categories { get; set; }
    }
}
