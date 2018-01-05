using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Identity;

namespace TZGCMS.Model.Admin.ViewModel.Identity
{
    public class SetRoleMenusVM
    {
        public int[] MenuIds { get; set; }
        public IEnumerable<Menu> Menus { get; set; }
        public int RoleId { get; set; }
    }
}
