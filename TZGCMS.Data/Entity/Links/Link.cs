using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Links
{
    public class Link : IAuditedEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string WebLink { get; set; }
        public string LogoUrl { get; set; }
        public int Importance { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual LinkCategory LinkCategory { get; set; }
        public string CategoryTitle
        {
            get
            {
                if (this.LinkCategory != null)
                {
                    return this.LinkCategory.Title;
                }
                return string.Empty;
            }
        }
    }
}
