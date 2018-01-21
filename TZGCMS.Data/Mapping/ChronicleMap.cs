using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Chronicles;

namespace TZGCMS.Data.Mapping
{
    public class ChronicleMap : EntityTypeConfiguration<Chronicle>
    {
        public ChronicleMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("ChronicleSet");

            this.Property(p => p.Title).HasMaxLength(100).IsRequired();
            this.Property(p => p.Year).IsRequired();
            this.Property(p => p.Month).IsRequired();
            this.Property(p => p.Day).IsOptional();
            this.Property(p => p.Body).IsMaxLength();
            this.Property(p => p.ViewCount).IsOptional();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);
    
        }
    }
}
