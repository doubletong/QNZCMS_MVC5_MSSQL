using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Products
{
    public class Product : IAuditedEntity
    {
        public Product()
        {
           
            this.Categories = new HashSet<ProductCategory>();
            //this.ProductPhotoes = new HashSet<ProductPhoto>();
        }
        public int Id { get; set; }

        public string ProductNo { get; set; }        
        public string ProductName { get; set; }
        public string Body { get; set; }
        public string Description { get; set; }

        public string Thumbnail { get; set; }
        public string Cover { get; set; }
        public string ImageUrl { get; set; }
        public bool Active { get; set; }       
        public bool Recommend { get; set; }
        public int ViewCount { get; set; }
        public int Importance { get; set; }
        public virtual ICollection<ProductCategory> Categories { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string CategoryTitle
        {
            get
            {
                if (this.Categories != null)
                {
                    string[] q = (from i in this.Categories
                                  select i.Title).ToArray();
                    return string.Join("、", q);
                }

                return string.Empty;
            }

        }
    }
}
