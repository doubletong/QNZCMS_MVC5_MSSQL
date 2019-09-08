namespace TZGCMS.IMG.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RecipientInfoSet")]
    public partial class RecipientInfoSet
    {
        public int Id { get; set; }

        public int ProvinceId { get; set; }

        public int? CityId { get; set; }

        public int? DistrictId { get; set; }

        [Required]
        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Postcode { get; set; }

        [Required]
        [StringLength(50)]
        public string Recipient { get; set; }

        [Required]
        [StringLength(50)]
        public string Mobile { get; set; }

        public bool? IsDefault { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        public virtual T_City T_City { get; set; }

        public virtual T_District T_District { get; set; }

        public virtual T_Province T_Province { get; set; }
    }
}
