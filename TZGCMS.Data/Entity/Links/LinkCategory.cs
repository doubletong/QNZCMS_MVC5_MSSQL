using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Links
{
    public class LinkCategory : IAuditedEntity
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public int Importance { get; set; }
        public bool Active { get; set; }

     
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<Link> Links { get; set; }
    }
}
