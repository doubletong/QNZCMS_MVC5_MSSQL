
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TZGCMS.Data.Entity.Identity
{
    [Table("Roles")]
    public partial class Role 
    {

        public int Id { get; set; }
        public Role()
        {
         
            this.Users = new HashSet<User>();
            this.Menus = new HashSet<Menu>();
        }

        [Display(Name = "角色名称", Prompt = "必填")]
        [Required(ErrorMessage = "请输入角色名称")]
        //[Remote("IsRoleUnique", "Role", ErrorMessage = "角色名{0}已经被使用")]
        public string RoleName { get; set; }
        [Display(Name = "角色描述", Prompt = "选填")]
        public string Description { get; set; }
        public bool IsSys { get; set; }


        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }


    }
}