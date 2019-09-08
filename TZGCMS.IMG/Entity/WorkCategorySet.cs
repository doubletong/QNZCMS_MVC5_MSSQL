namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkCategorySet")]
    public partial class WorkCategorySet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkCategorySet()
        {
            WorkSets = new HashSet<WorkSet>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public int Importance { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public bool? Active { get; set; }

        [StringLength(50)]
        public string SeoName { get; set; }

        [StringLength(150)]
        public string ImageUrl { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkSet> WorkSets { get; set; }
    }
}
