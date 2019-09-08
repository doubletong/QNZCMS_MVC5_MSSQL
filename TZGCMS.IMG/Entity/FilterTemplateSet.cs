namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FilterTemplateSet")]
    public partial class FilterTemplateSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Body { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Author { get; set; }

        [StringLength(100)]
        public string PublishDate { get; set; }

        [StringLength(100)]
        public string Keyword { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? Active { get; set; }

        [StringLength(150)]
        public string Source { get; set; }

        [StringLength(100)]
        public string LinksContainer { get; set; }

        public int Importance { get; set; }

        [StringLength(100)]
        public string Links { get; set; }

        [StringLength(500)]
        public string KeywordSet { get; set; }

        [StringLength(50)]
        public string Encode { get; set; }
    }
}
