namespace TZGCMS.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SimpleProducts")]
    public partial class SimpleProduct : IAuditedEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductNo { get; set; }

        public string Body { get; set; }
        public string Parameters { get; set; }
        public string Specific { get; set; }
        public string Controls { get; set; }
        public string Videos { get; set; }
        public string Summary { get; set; }
        public int ViewCount { get; set; }
        public int Importance { get; set; }
        [StringLength(150)]
        public string Thumbnail { get; set; }
        [StringLength(150)]
        public string ImageUrl { get; set; }
        public bool Active { get; set; }
        public bool Recommend { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
