using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Emails;

namespace TZGCMS.Data.Mapping
{
    public class EmailMap : EntityTypeConfiguration<Email>
    {
        public EmailMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("EmailSet");
            this.Property(p => p.Subject).HasMaxLength(150).IsRequired();
            this.Property(p => p.Body).IsMaxLength().IsOptional();
            this.Property(p => p.MailTo).HasMaxLength(250).IsOptional();
            this.Property(p => p.MailCc).HasMaxLength(250).IsOptional();
            this.Property(p => p.Readed).IsRequired();
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);
        }

    }
}
