namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PostSet")]
    public partial class PostSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string title { get; set; }

        [StringLength(256)]
        public string Summary { get; set; }

        public string Body { get; set; }

        [StringLength(100)]
        public string Keywords { get; set; }

        public int ViewCount { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public bool? Recommend { get; set; }

        public bool? Active { get; set; }

        [StringLength(150)]
        public string Thumbnail { get; set; }
    }
}
