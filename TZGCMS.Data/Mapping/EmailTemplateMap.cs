using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Emails;

namespace TZGCMS.Data.Mapping
{
    public class EmailTemplateMap : EntityTypeConfiguration<EmailTemplate>
    {
        public EmailTemplateMap()
        {
            this.HasKey(p => p.Id);
            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("EmailTemplateSet");
            this.Property(p => p.Subject).HasMaxLength(100).IsRequired();         
            this.Property(p => p.EmailAccountId).IsRequired();
            this.Property(p => p.Body).IsMaxLength();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);

            this.HasRequired(p => p.EmailAccount)
            .WithMany(h => h.EmailTemplates)
            .HasForeignKey(p => p.EmailAccountId);      
        }
    }
}
