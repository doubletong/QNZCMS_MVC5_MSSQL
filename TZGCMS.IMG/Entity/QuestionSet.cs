namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuestionSet")]
    public partial class QuestionSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        public string Answer { get; set; }

        public int Importance { get; set; }

        public bool? Active { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public virtual QuestionCategorySet QuestionCategorySet { get; set; }
    }
}
