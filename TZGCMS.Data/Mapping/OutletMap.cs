using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity;

namespace TZGCMS.Data.Mapping
{
    public class OutletMap : EntityTypeConfiguration<Outlet>
    {
        public OutletMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("OutletSet");

            this.Property(p => p.Address).HasMaxLength(200).IsRequired();
            this.Property(p => p.Coordinate).HasMaxLength(50);
            this.Property(p => p.Name).HasMaxLength(50).IsRequired();
            this.Property(p => p.ContactMan).HasMaxLength(50);
            this.Property(p => p.Phone).HasMaxLength(100);

            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);
    
        }
    }
}
