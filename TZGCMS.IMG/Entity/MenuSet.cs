namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MenuSet")]
    public partial class MenuSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MenuSet()
        {
            MenuSet1 = new HashSet<MenuSet>();
            Roles = new HashSet<Role>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(150)]
        public string Url { get; set; }

        public int Importance { get; set; }

        public bool? Hidden { get; set; }

        public bool Active { get; set; }

        [StringLength(50)]
        public string Iconfont { get; set; }

        public int? ParentId { get; set; }

        public short? MenuType { get; set; }

        [StringLength(50)]
        public string Area { get; set; }

        [StringLength(50)]
        public string Controller { get; set; }

        [StringLength(50)]
        public string Action { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public bool IsExpand { get; set; }

        public int? LayoutLevel { get; set; }

        public virtual MenuCategorySet MenuCategorySet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MenuSet> MenuSet1 { get; set; }

        public virtual MenuSet MenuSet2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Role> Roles { get; set; }
    }
}
