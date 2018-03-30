using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Data.Entity.Ads
{


    public partial class Carousel:IAuditedEntity
    {
        [Key]
        public  int Id { get; set; }
        public  string Title { get; set; }
        public  string Description { get; set; }
        public  string WebLink { get; set; }
        public  string ImageUrl { get; set; }
        public  int Importance { get; set; }
        public  bool Active { get; set; }
        public  string CreatedBy { get; set; }
        public  DateTime CreatedDate { get; set; }
        public  string UpdatedBy { get; set; }
        public  DateTime? UpdatedDate { get; set; }
        public  string ImageUrlMobile { get; set; }
        public  int PositionId { get; set; }
        public virtual Position Position { get; set; }

        [NotMapped]
        public string PositionTitle
        {
            get { return Position != null ? Position.Title : string.Empty; }
        }
        [NotMapped]
        public string PositionTitleAndSize
        {
            get { return Position != null ? Position.TitleAndSize : string.Empty; }
        }
    }
}
