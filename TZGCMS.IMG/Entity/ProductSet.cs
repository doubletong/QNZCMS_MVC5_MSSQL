namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductSet")]
    public partial class ProductSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductSet()
        {
            ProductPhotoSets = new HashSet<ProductPhotoSet>();
            TechnologySets = new HashSet<TechnologySet>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductNo { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public string Parameters { get; set; }

        [StringLength(150)]
        public string Thumbnail { get; set; }

        public string ImageUrl { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int ViewCount { get; set; }

        [StringLength(50)]
        public string Keywords { get; set; }

        [StringLength(150)]
        public string ViewImage { get; set; }

        public bool? Active { get; set; }

        public bool? Recommend { get; set; }

        public int Importance { get; set; }

        [StringLength(150)]
        public string Cover { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductPhotoSet> ProductPhotoSets { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TechnologySet> TechnologySets { get; set; }
    }
}
