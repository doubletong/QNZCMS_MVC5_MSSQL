namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ClientSet")]
    public partial class ClientSet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ClientName { get; set; }

        [StringLength(150)]
        public string LogoURL { get; set; }

        [StringLength(150)]
        public string Homepage { get; set; }

        public int Importance { get; set; }

        public bool Active { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
