using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.PageMetas;

namespace TZGCMS.Data.Mapping
{
   public class PageMetaMap : EntityTypeConfiguration<PageMeta>
    {
        public PageMetaMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ModelType)
                .IsRequired();

            this.Property(e => e.ObjectId)
                .IsRequired();

            this.Property(e => e.Keyword).HasMaxLength(250).IsOptional();
            this.Property(e => e.Description).HasMaxLength(500).IsOptional();

            // Table & Column Mappings
            this.ToTable("PageMetas");

       
        }
    }
}
