using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Products;

namespace TZGCMS.Model.Admin.ViewModel.Products
{
    public class ProductListVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public StaticPagedList<Product> Products { get; set; }
        public int? CategoryId { get; set; }

        public string Keyword { get; set; }
    }
}
