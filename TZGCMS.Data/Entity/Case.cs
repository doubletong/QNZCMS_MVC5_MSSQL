namespace TZGCMS.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cases")]
    public partial class Case : IAuditedEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Body { get; set; }

        public string Summary { get; set; }


        public int ViewCount { get; set; }

        [StringLength(150)]
        public string Thumbnail { get; set; }

        public bool? Active { get; set; }

        public DateTime? Pubdate { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
