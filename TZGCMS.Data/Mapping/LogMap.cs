using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Logs;

namespace TZGCMS.Data.Mapping
{
    public class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Level)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(e => e.Thread)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(e => e.Logger)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Logs");
            //this.Property(t => t.ProductID).HasColumnName("ProductID");
            //this.Property(t => t.ProductName).HasColumnName("ProductName");
        }
    }
}
