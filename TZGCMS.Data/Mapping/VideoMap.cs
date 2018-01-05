
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Videos;

namespace TZGCMS.Data.Mapping
{
    public class VideoMap : EntityTypeConfiguration<Video>
    {
        public VideoMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("VideoSet");
            this.Property(p => p.Title).HasMaxLength(100).IsRequired();
            this.Property(p => p.Thumbnail).HasMaxLength(150).IsOptional();           
            this.Property(p => p.VideoUrl).HasMaxLength(150).IsOptional();
            this.Property(p => p.StartDate).IsRequired();
            this.Property(p => p.EndDate).IsRequired();
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);

            this.HasRequired(p => p.VideoCategory)
              .WithMany(h => h.Videos)
              .HasForeignKey(p => p.CategoryId)
              .WillCascadeOnDelete(true);
        }
       
    }
}
