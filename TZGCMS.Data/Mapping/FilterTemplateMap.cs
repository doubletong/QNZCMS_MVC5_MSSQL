using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Articles;

namespace TZGCMS.Data.Mapping
{
    public class FilterTemplateMap : EntityTypeConfiguration<FilterTemplate>
    {
        public FilterTemplateMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("FilterTemplateSet");

            this.Property(p => p.Name).HasMaxLength(100).IsRequired();
       
            this.Property(p => p.Source).HasMaxLength(150).IsRequired();
            this.Property(p => p.LinksContainer).HasMaxLength(100).IsRequired();
            this.Property(p => p.Links).HasMaxLength(100).IsRequired();
            this.Property(p => p.Title).HasMaxLength(100).IsRequired();
            this.Property(p => p.Description).HasMaxLength(100).IsRequired();
            this.Property(p => p.Keyword).HasMaxLength(100).IsRequired();
            this.Property(p => p.Author).HasMaxLength(100).IsRequired();
            this.Property(p => p.PublishDate).HasMaxLength(100).IsRequired();
            this.Property(p => p.Body).HasMaxLength(100).IsRequired();
            this.Property(p => p.KeywordSet).HasMaxLength(500).IsOptional();
            this.Property(p => p.Encode).HasMaxLength(50).IsOptional();

            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedDate).IsOptional().HasColumnType("datetime");
            this.Property(p => p.UpdatedBy).IsOptional().HasMaxLength(50);

       
        }
    }
}
