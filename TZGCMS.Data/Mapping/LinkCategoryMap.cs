using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Links;

namespace TZGCMS.Data.Mapping
{
    public class LinkCategoryMap: EntityTypeConfiguration<LinkCategory>
    {
        public LinkCategoryMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("LinkCategorySet");
            this.Property(p => p.Title).HasMaxLength(50).IsRequired();     
            this.Property(p => p.Importance).IsRequired();           
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedBy).HasMaxLength(50);
            

        }
    }
}
