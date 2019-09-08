namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommentSet")]
    public partial class CommentSet
    {
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime Pubdate { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int ArticleId { get; set; }

        public bool? Active { get; set; }
    }
}
