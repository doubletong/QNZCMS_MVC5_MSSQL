using System.Data.Entity.ModelConfiguration;
using TZGCMS.Data.Entity.Identity;

namespace TZGCMS.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Properties
            //this.Property(t => t.Id)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.);

            this.Property(t => t.RealName)
                .IsOptional()
                .HasMaxLength(50);

            this.Property(e => e.UserName).IsRequired()             
                .HasMaxLength(50);

            this.Property(e => e.Email).IsRequired()                
                .HasMaxLength(150);

            this.Property(e => e.QQ)
                .IsOptional()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Users");

            this.HasMany(u => u.Roles)
                 .WithMany(r => r.Users)
                 .Map(m =>
                 {
                     m.ToTable("UserRoles");
                     m.MapLeftKey("UserId");
                     m.MapRightKey("RoleId");
                 }); ;


        }
    }
}