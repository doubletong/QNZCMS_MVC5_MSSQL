using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Emails;

namespace TZGCMS.Data.Mapping
{
    public class EmailAccountMap : EntityTypeConfiguration<EmailAccount>
    {
        public EmailAccountMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("EmailAccountSet");
            this.Property(p => p.Smtpserver).HasMaxLength(150).IsRequired();
            this.Property(p => p.Email).HasMaxLength(150).IsRequired();
            this.Property(p => p.UserName).HasMaxLength(150).IsRequired();
            this.Property(p => p.Password).HasMaxLength(150).IsOptional();
            this.Property(p => p.Port).IsRequired();
            this.Property(p => p.IsDefault).IsRequired();
            this.Property(p => p.EnableSsl).IsRequired();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);
          }
       
    }
}
