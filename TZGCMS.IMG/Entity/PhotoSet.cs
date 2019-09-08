namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhotoSet")]
    public partial class PhotoSet
    {
        public int Id { get; set; }

        public int AlbumId { get; set; }

        [StringLength(150)]
        public string FullImageURL { get; set; }

        [StringLength(150)]
        public string Thumbnail { get; set; }

        public int Importance { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public virtual AlbumSet AlbumSet { get; set; }
    }
}
