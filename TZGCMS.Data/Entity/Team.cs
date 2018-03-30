using System;

namespace TZGCMS.Data.Entity
{
    public class Team : IAuditedEntity
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Post { get; set; }
        public string Description { get; set; }
        public int Importance  { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

    }
}
