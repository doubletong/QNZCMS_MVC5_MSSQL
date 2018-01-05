using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Articles;

namespace TZGCMS.Data.Mapping
{
    public class CommentMap : EntityTypeConfiguration<Comment>
    {
        public CommentMap()
        {
            this.HasKey(p => p.Id);

            this.Property(p => p.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.ToTable("CommentSet");
            
            this.Property(p => p.ArticleId).IsRequired();
            this.Property(p => p.Pubdate).IsRequired();
            this.Property(p => p.Body).IsMaxLength();
            this.Property(p => p.Name).IsOptional();


            this.HasRequired(p => p.Article)
            .WithMany(h => h.Comments)
            .HasForeignKey(p => p.ArticleId)
            .WillCascadeOnDelete(true); 
        }
    }
}
