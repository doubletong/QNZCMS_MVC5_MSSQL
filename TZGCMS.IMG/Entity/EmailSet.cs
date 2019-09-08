namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailSet")]
    public partial class EmailSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        public string Body { get; set; }

        [StringLength(250)]
        public string MailTo { get; set; }

        [StringLength(250)]
        public string MailCc { get; set; }

        public bool Readed { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }
    }
}
