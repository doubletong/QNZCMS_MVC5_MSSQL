namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GoodsPhotoSet")]
    public partial class GoodsPhotoSet
    {
        public int Id { get; set; }

        public int GoodsId { get; set; }

        [Required]
        [StringLength(150)]
        public string ImageURL { get; set; }

        [Required]
        [StringLength(150)]
        public string Thumbnail { get; set; }

        public virtual GoodsSet GoodsSet { get; set; }
    }
}
