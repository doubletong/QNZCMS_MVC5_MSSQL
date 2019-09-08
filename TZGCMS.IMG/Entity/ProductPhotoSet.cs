namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductPhotoSet")]
    public partial class ProductPhotoSet
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(150)]
        public string ImgURL { get; set; }

        public int Importance { get; set; }

        public int ProductId { get; set; }

        public virtual ProductSet ProductSet { get; set; }
    }
}
