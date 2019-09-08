namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("JobSet")]
    public partial class JobSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Post { get; set; }

        [Required]
        public string Description { get; set; }

        public int Importance { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        public int? Quantity { get; set; }

        [StringLength(150)]
        public string Address { get; set; }

        [StringLength(50)]
        public string SeoName { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }
    }
}
