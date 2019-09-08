namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailTemplateSet")]
    public partial class EmailTemplateSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Subject { get; set; }

        public string Body { get; set; }

        [StringLength(50)]
        public string TemplateNo { get; set; }

        public int EmailAccountId { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
