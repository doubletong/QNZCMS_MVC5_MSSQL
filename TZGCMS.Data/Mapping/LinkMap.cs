using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Links;

namespace TZGCMS.Data.Mapping
{
    public class LinkMap : EntityTypeConfiguration<Link>
    {
        public LinkMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("LinkSet");
                      
            this.Property(p => p.Title).HasMaxLength(100).IsRequired();
            this.Property(p => p.CategoryId).IsRequired();
            this.Property(p => p.LogoUrl).HasMaxLength(150).IsOptional();
            this.Property(p => p.WebLink).HasMaxLength(150).IsOptional();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);

            this.HasRequired(p => p.LinkCategory)
            .WithMany(h => h.Links)
            .HasForeignKey(p => p.CategoryId)
            .WillCascadeOnDelete(true); 
        }
    }
}
