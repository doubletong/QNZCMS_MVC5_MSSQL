using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Chronicles
{
    public class Chronicle : IAuditedEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public short? Day { get; set; }
        public bool Active { get; set; }
        public int ViewCount { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
