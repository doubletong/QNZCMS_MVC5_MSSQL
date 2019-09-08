namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TechnologySet")]
    public partial class TechnologySet
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(100)]
        public string EnglishTitle { get; set; }

        public string ImagesURL { get; set; }

        public int Importance { get; set; }

        public int ProductId { get; set; }

        public string Body { get; set; }

        public bool? Active { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public virtual ProductSet ProductSet { get; set; }
    }
}
