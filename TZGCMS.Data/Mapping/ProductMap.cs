using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Products;

namespace SIG.Model.Mapping
{
    public class ProductMap: EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(p => p.ProductNo).IsRequired().HasMaxLength(50);
          
            this.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
            this.Property(p => p.Description).IsOptional();
            this.Property(p => p.Body).IsOptional();
         
            // this.Property(p => p.Parameters).IsOptional();   
            this.Property(p => p.Thumbnail).HasMaxLength(150);
            this.Property(p => p.Cover).HasMaxLength(150);
            //  this.Property(p => p.CategoryIds).HasMaxLength(150);
            this.Property(p => p.CreatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);


            this.HasMany(a => a.Categories).WithMany(b => b.Products).Map(m =>
            {
                m.MapLeftKey("ProductId");
                m.MapRightKey("CategoryId");
                m.ToTable("ProductCategory");
            });



            this.ToTable("ProductSet");
        }
    }
}
