using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Products;

namespace TZGCMS.Model.Front.ViewModel.Products
{
    public class ProductListFVM
    {
        public string SeoName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public StaticPagedList<Product> Products { get; set; }


      
        public IEnumerable<ProductCategory> Categories {get;set;}
    }
}
