namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PageMetaSet")]
    public partial class PageMetaSet
    {
        public int Id { get; set; }

        public short ModelType { get; set; }

        [Required]
        [StringLength(50)]
        public string ObjectId { get; set; }

        [StringLength(250)]
        public string Keyword { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(150)]
        public string Title { get; set; }
    }
}
