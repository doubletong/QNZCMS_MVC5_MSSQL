using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Ads;

namespace TZGCMS.Data.Mapping
{
    public class PositionMap: EntityTypeConfiguration<Position>
    {
        public PositionMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("PositionSet");
            this.Property(p => p.Title).HasMaxLength(50).IsRequired();
            this.Property(p => p.Code).HasMaxLength(50).IsRequired();
            this.Property(p => p.Importance).IsRequired();           
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedBy).HasMaxLength(50);
            

        }
    }
}
