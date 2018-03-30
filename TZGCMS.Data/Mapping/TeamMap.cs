using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity;

namespace TZGCMS.Data.Mapping
{
    public class TeamMap : EntityTypeConfiguration<Team>
    {
        public TeamMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("TeamSet");

            this.Property(p => p.Post).HasMaxLength(100).IsRequired();
            this.Property(p => p.Description).IsMaxLength();
            this.Property(p => p.Name).HasMaxLength(50);
            this.Property(p => p.PhotoUrl).HasMaxLength(150);
          
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);
    
        }
    }
}
