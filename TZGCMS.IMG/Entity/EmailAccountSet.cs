namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailAccountSet")]
    public partial class EmailAccountSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(150)]
        public string Smtpserver { get; set; }

        [Required]
        [StringLength(150)]
        public string UserName { get; set; }

        [StringLength(150)]
        public string Password { get; set; }

        public int? Port { get; set; }

        public bool EnableSsl { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }
    }
}
