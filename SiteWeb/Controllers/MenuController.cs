using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TZGCMS.Service.Identity;
using System.Threading.Tasks;

namespace TZGCMS.SiteWeb.Controllers
{
    public class MenuController : BaseController
    {
    
   
        public PartialViewResult SiteNav(int cid)
        {
            var vm =  _db.Menu.Include(d => d.ChildMenus).Where(m => m.Active && m.CategoryId == cid)
                .OrderBy(d => d.Importance).ToList();
            return PartialView("_SiteNav", vm);
        }
        public PartialViewResult SiteFooterNav(int cid)
        {
            var vm = _db.Menu.Include(d=>d.ChildMenus).Where(m => m.Active && m.CategoryId == cid)
                .OrderBy(d=>d.Importance).ToList();
            return PartialView("_SiteFooterNav", vm);
        }
    }
}