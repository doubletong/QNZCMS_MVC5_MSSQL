using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Model.Front.ViewModel.Products
{
    public class ProductVM
    {
        public int Id { get; set; }

        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string Thumbnail { get; set; }
    }

    public class ProductDetailVM
    {
        public int Id { get; set; }

        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string Thumbnail { get; set; }
        public string Body { get; set; }
        public string[] Photos { get; set; }
    }
}
