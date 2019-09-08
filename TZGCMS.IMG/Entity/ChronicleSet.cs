namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChronicleSet")]
    public partial class ChronicleSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public short Year { get; set; }

        public short Month { get; set; }

        public short? Day { get; set; }

        public bool? Active { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public int ViewCount { get; set; }

        public string Body { get; set; }

        [StringLength(150)]
        public string Thumbnail { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }
    }
}
