namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReservationSet")]
    public partial class ReservationSet
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VideoId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string OpenId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? NoticedDate { get; set; }
    }
}
