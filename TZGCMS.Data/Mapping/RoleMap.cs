using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Identity;

namespace TZGCMS.Data.Mapping
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.RoleName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Roles");

            this.HasMany(u => u.Users)
                  .WithMany(r => r.Roles)
                  .Map(m =>
                  {
                      m.ToTable("UserRoles");
                      m.MapLeftKey("RoleId");
                      m.MapRightKey("UserId");
                  });

            this.HasMany(u => u.Menus)
                 .WithMany(r => r.Roles)
                 .Map(m =>
                 {
                     m.ToTable("RoleMenus");
                     m.MapLeftKey("RoleId");
                     m.MapRightKey("MenuId");
                 });
        }
    }
}