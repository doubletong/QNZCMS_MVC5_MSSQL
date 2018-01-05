using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Videos;

namespace TZGCMS.Data.Mapping
{
    public class ReservationMap : EntityTypeConfiguration<Reservation>
    {
        public ReservationMap()
        {
            this.HasKey(p =>new{ p.VideoId,p.OpenId});
            //this.Property(p => p.Id)
            //    .HasColumnName("Id")
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("ReservationSet");

            this.Property(p => p.OpenId).HasMaxLength(50).IsRequired();
            this.Property(p => p.VideoId).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired();
           
            this.HasRequired(p => p.Video)
            .WithMany(h => h.Reservations)
            .HasForeignKey(p => p.VideoId)
            .WillCascadeOnDelete(true); 
        }
    }
}
