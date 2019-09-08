namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PageSet")]
    public partial class PageSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        [StringLength(100)]
        public string SeoName { get; set; }

        public int? ViewCount { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        [StringLength(50)]
        public string TemplateName { get; set; }

        public string HeadCode { get; set; }

        public string FooterCode { get; set; }
    }
}
