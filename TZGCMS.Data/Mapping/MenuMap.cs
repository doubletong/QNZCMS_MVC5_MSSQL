using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Identity;

namespace TZGCMS.Data.Mapping
{
    public class MenuMap: EntityTypeConfiguration<Menu>
    {
        public MenuMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.ToTable("MenuSet");

            this.Property(p => p.Title).HasMaxLength(50).IsRequired();           
     
            this.Property(p => p.Importance).IsRequired();
            this.Property(p => p.Active).IsRequired();
            this.Property(p => p.IsExpand).IsRequired();
            this.Property(p => p.LayoutLevel).IsOptional();
            this.Property(p => p.CreatedDate).IsRequired().HasColumnType("datetime");
            this.Property(p => p.CreatedBy).HasMaxLength(50);
            this.Property(p => p.UpdatedBy).HasMaxLength(50);

            this.HasOptional(c => c.ParentMenu)
            .WithMany(c => c.ChildMenus)
            .HasForeignKey(c => c.ParentId);

            this.HasRequired(p => p.Category)
              .WithMany(h => h.Menus)
              .HasForeignKey(p => p.CategoryId)
              .WillCascadeOnDelete(true);

            this.HasMany(u => u.Roles)
                 .WithMany(r => r.Menus)
                 .Map(m =>
                 {
                     m.ToTable("RoleMenus");
                     m.MapLeftKey("MenuId");
                     m.MapRightKey("RoleId");
                 });
        }
    }
}
