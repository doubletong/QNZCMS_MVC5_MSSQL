using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Products
{
    public class ProductCategory : IAuditedEntity
    {
    
        public int Id { get; set; }
        public string Title { get; set; }
       
        public int Importance { get; set; }
        public string SeoName { get; set; }       
        public bool Active { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public string ImageUrl { get; set; }       
        public int? ParentId { get; set; }
        public virtual ProductCategory ParentCategory { get; set; }
        public virtual ICollection<ProductCategory> ChildCategories { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ParentCategoryTitle
        {
            get
            {
                if (this.ParentCategory != null)
                {
                    return this.ParentCategory.Title;
                }
                return string.Empty;
            }

        }
    }
}
