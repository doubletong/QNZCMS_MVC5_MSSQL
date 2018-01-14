using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Doc;

namespace TZGCMS.Data.Mapping
{
    public class DocumentCategoryMap: EntityTypeConfiguration<DocumentCategory>
    {
        public DocumentCategoryMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("DocumentCategorySet");
            this.Property(p => p.Title).HasMaxLength(50).IsRequired();     
            this.Property(p => p.Importance).IsRequired();           
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);


        }
    }
}
