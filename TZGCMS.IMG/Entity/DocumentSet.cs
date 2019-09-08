namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DocumentSet")]
    public partial class DocumentSet
    {
        [Key]
        [Column(Order = 0)]
        public int id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Title { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }

        public decimal? FileSize { get; set; }

        [StringLength(50)]
        public string Extension { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Importance { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool Active { get; set; }

        [Key]
        [Column(Order = 5)]
        public bool IsVIP { get; set; }

        [Key]
        [Column(Order = 6)]
        public bool IsLink { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DownloadCount { get; set; }

        [StringLength(50)]
        public string ProductIds { get; set; }

        [Key]
        [Column(Order = 8)]
        public DateTime CreatedDate { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        [StringLength(150)]
        public string FilePath { get; set; }

        [StringLength(50)]
        public string Password { get; set; }
    }
}
