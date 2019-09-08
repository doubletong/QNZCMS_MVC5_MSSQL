namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserSafetySet")]
    public partial class UserSafetySet
    {
        public int Id { get; set; }

        public Guid Code { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        public short EmailType { get; set; }

        public int Timeout { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
