using System;

namespace TZGCMS.Data.Entity
{

    public partial class Job : IAuditedEntity
    {
       
        public int Id { get; set; }       
        public string Post { get; set; }
        public string SeoName { get; set; }
        public string Description { get; set; }
        public int Importance { get; set; }
        public bool Active { get; set; }
        public string Category { get; set; }       
        public string Address { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

    }
}
