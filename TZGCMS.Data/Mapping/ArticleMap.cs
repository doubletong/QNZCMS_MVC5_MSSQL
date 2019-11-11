using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Articles;

namespace TZGCMS.Data.Mapping
{
    public class ArticleMap : EntityTypeConfiguration<Article>
    {
        public ArticleMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("Articles");

            this.Property(p => p.Title).HasMaxLength(100).IsRequired();
            this.Property(p => p.CategoryId).IsRequired();
            this.Property(p => p.ViewCount).IsRequired();
            this.Property(p => p.Body).IsMaxLength();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);

            this.HasRequired(p => p.ArticleCategory)
            .WithMany(h => h.Articles)
            .HasForeignKey(p => p.CategoryId)
            .WillCascadeOnDelete(true); 
        }
    }
}
