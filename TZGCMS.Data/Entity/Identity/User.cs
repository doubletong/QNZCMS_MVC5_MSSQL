using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Helper;

namespace TZGCMS.Data.Entity.Identity
{
    [Table("Users")]
    public partial class User
    {
        public Guid Id { get; set; }
        public User()
        {
            this.Id = IdentityGenerator.SequentialGuid();          
            this.Roles = new HashSet<Role>();           
        }
        [Required]
       

        public string UserName { get; set; }
        [Required]
     
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
     
        public string RealName { get; set; }

        public string PhotoUrl { get; set; }
     

       
        public bool IsActive { get; set; }
       
        public DateTime CreateDate { get; set; }
       
        public DateTime? LastActivityDate { get; set; }

        //public Nullable<int> DepartmentId { get; set; }
        //public Nullable<int> PositionId { get; set; }

      
        public DateTime? Birthday { get; set; }
      
        public Gender Gender { get; set; }
     
        public string GenderName
        {
            get
            {
                var enumType = typeof(Gender);
                var field = enumType.GetFields()
                           .First(x => x.Name == System.Enum.GetName(enumType, Gender));
                var attribute = field.GetCustomAttribute<DisplayAttribute>();
                return attribute.Name;

            }
        }
       
        public string QQ { get; set; }
      
        public string Mobile { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        //public virtual Department Department { get; set; }
        //public virtual Position Position { get; set; }

        //[NotMapped]
        //public List<UserMenuDTO> Menus { get; set; }
      

        public bool HasMenu(string area, string controller, string action)
        {
            bool bFound = false;
            foreach (Role role in this.Roles)
            {
                bFound = (role.Menus.Where(
                          p => p.Controller == controller && p.Action == action && p.Area == area).ToList().Count > 0);
                if (bFound)
                    break;
            }
            return bFound;
        }

        public bool HasRole(string role)
        {
            return (Roles.Where(p => p.RoleName == role).ToList().Count > 0);
        }

        public bool HasRoles(string roles)
        {
            bool bFound = false;
            string[] _roles = roles.ToLower().Split(',');
            foreach (Role role in this.Roles)
            {
                try
                {
                    bFound = _roles.Contains(role.RoleName.ToLower());
                    if (bFound)
                        return bFound;
                }
                catch (Exception)
                {
                }
            }
            return bFound;
        }
    }


}