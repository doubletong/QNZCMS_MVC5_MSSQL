namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CarouselSet")]
    public partial class CarouselSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        [StringLength(150)]
        public string WebLink { get; set; }

        [StringLength(150)]
        public string ImageUrl { get; set; }

        public int Importance { get; set; }

        public bool Active { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(150)]
        public string ImageUrlMobile { get; set; }

        public int PositionId { get; set; }

        public virtual PositionSet PositionSet { get; set; }
    }
}
