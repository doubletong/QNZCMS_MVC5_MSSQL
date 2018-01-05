using System.Collections.Generic;
using TZGCMS.Data.Entity.Identity;

namespace TZGCMS.Model.Admin.ViewModel.Menus
{
    public class LeftNavVM
    {     
        public IEnumerable<Menu> Menus { get; set; }
        public Menu CurrentMenu { get; set; }
    }
}
