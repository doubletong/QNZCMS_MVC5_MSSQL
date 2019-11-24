using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Products;

namespace SIG.Model.Mapping
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("ProductCategories");
            this.Property(p => p.Title).HasMaxLength(50).IsRequired();     
            this.Property(p => p.Importance).IsRequired();
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedBy).HasMaxLength(50);

            this.HasMany(a => a.Products).WithMany(b => b.Categories).Map(m =>
            {
                m.MapLeftKey("CategoryId");
                m.MapRightKey("ProductId");
                m.ToTable("ProductWithCategory");
            });

            this.HasOptional(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentId)
            .WillCascadeOnDelete(true);
        }
    }
}
