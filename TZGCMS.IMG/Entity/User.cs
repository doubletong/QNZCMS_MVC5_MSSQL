namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Roles = new HashSet<Role>();
        }

        public Guid Id { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        [StringLength(50)]
        public string RealName { get; set; }

        [StringLength(150)]
        public string PhotoUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public DateTime? Birthday { get; set; }

        public short? Gender { get; set; }

        [StringLength(50)]
        public string QQ { get; set; }

        public string Mobile { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Role> Roles { get; set; }
    }
}
