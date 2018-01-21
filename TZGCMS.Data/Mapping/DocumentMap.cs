using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Doc;

namespace TZGCMS.Data.Mapping
{
    public class DocumentMap : EntityTypeConfiguration<Document>
    {
        public DocumentMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("DocumentSet");

            this.Property(p => p.Title).HasMaxLength(50).IsRequired();
            this.Property(p => p.FileSize).IsOptional();
            this.Property(p => p.Importance).IsRequired();
            this.Property(p => p.Extension).HasMaxLength(50).IsOptional();
            this.Property(p => p.FilePath).HasMaxLength(150).IsOptional();
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.IsVIP).IsRequired();
            this.Property(p => p.IsLink).IsRequired();
            this.Property(p => p.DownloadCount).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);

            this.HasRequired(p => p.Category)
              .WithMany(h => h.Documents)
              .HasForeignKey(p => p.CategoryId)
            .WillCascadeOnDelete(true); 
        }
    }
}
