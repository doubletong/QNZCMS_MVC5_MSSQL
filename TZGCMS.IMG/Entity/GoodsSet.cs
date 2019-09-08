namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GoodsSet")]
    public partial class GoodsSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GoodsSet()
        {
            CartSets = new HashSet<CartSet>();
            GoodsPhotoSets = new HashSet<GoodsPhotoSet>();
        }

        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string FullDescription { get; set; }

        [Required]
        [StringLength(150)]
        public string Thumbnail { get; set; }

        public int ViewCount { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool Active { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CartSet> CartSets { get; set; }

        public virtual GoodsCategorySet GoodsCategorySet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GoodsPhotoSet> GoodsPhotoSets { get; set; }
    }
}
