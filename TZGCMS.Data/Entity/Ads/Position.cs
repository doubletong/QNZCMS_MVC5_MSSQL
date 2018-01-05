using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Ads
{

    public partial class Position : IAuditedEntity
    {
       
        public  int Id { get; set; }
        public  string Title { get; set; }
        public  string Sketch { get; set; }
        public  string Code { get; set; }
        public  int? Importance { get; set; }
        public  bool Active { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime? UpdatedDate { get; set; }
        public  string UpdatedBy { get; set; }
        public virtual  ICollection<Carousel> Carousels { get; set; }
    }
}
