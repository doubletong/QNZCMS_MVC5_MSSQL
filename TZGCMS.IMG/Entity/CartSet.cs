namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CartSet")]
    public partial class CartSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CartId { get; set; }

        public int GoodsId { get; set; }

        public int Count { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual GoodsSet GoodsSet { get; set; }
    }
}
