using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Pages;

namespace TZGCMS.Data.Mapping
{
    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("PageSet");

            this.Property(p => p.Title).HasMaxLength(100).IsRequired();
            this.Property(p => p.SeoName).HasMaxLength(100).IsRequired();
            this.Property(p => p.Body).IsMaxLength();
            this.Property(p => p.HeadCode).IsMaxLength();
            this.Property(p => p.FooterCode).IsMaxLength();
            this.Property(p => p.ViewCount).IsOptional();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);
    
        }
    }
}
