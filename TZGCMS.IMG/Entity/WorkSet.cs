namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkSet")]
    public partial class WorkSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkSet()
        {
            WorkCategorySets = new HashSet<WorkCategorySet>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public string Body { get; set; }

        [StringLength(300)]
        public string Abstract { get; set; }

        public int? FinishYear { get; set; }

        [StringLength(150)]
        public string Thumbnail { get; set; }

        public string Imageurl { get; set; }

        [StringLength(150)]
        public string Demourl { get; set; }

        public int Viewcount { get; set; }

        [StringLength(100)]
        public string Keywords { get; set; }

        public bool? Recommend { get; set; }

        public bool Active { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        [StringLength(50)]
        public string CategoryIds { get; set; }

        public int? SolutionId { get; set; }

        public int? ClientId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkCategorySet> WorkCategorySets { get; set; }
    }
}
