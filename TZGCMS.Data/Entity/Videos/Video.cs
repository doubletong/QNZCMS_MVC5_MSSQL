using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TZGCMS.Data.Entity.Videos
{
    public partial class Video : IAuditedEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string Summary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Thumbnail { get; set; } 
        public bool Active { get; set; }
        public int CategoryId { get; set; }
        public VideoCategory VideoCategory { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }

        [NotMapped]
        public string CategoryTitle => VideoCategory != null ? VideoCategory.Title : string.Empty;
    }
}
