using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Ads;

namespace TZGCMS.Data.Mapping
{
    public class CarouselMap: EntityTypeConfiguration<Carousel>
    {
        public CarouselMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("CarouselSet");

            this.Property(p => p.Title).HasMaxLength(50).IsRequired();
            this.Property(p => p.WebLink).HasMaxLength(150).IsOptional();
            this.Property(p => p.Importance).IsRequired();
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);

            this.HasRequired(p => p.Position)
           .WithMany(h => h.Carousels)
           .HasForeignKey(p => p.PositionId)
           .WillCascadeOnDelete(true);
        }


       
    }
}
