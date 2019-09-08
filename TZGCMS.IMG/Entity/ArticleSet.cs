namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ArticleSet")]
    public partial class ArticleSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Body { get; set; }

        public string Summary { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int ViewCount { get; set; }

        [StringLength(150)]
        public string Thumbnail { get; set; }

        public bool? Recommend { get; set; }

        public bool? Active { get; set; }

        [StringLength(150)]
        public string FullImage { get; set; }

        [StringLength(100)]
        public string Source { get; set; }

        public DateTime? Pubdate { get; set; }

        [StringLength(150)]
        public string SourceLink { get; set; }

        public bool CanComment { get; set; }

        public virtual ArticleCategorySet ArticleCategorySet { get; set; }
    }
}
