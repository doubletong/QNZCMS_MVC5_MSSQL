using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Service.Identity;

namespace TZGCMS.SiteWeb.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IMenuServices _menuServices;
        public MenuController(IMenuServices menuServices)
        {
            _menuServices = menuServices;            
        }
        // GET: Menu
        public PartialViewResult Mainav()
        {
            var menus = _menuServices.GetMenusByCategoryId(2);

            return PartialView("_SiteNav", menus);
        }
        public PartialViewResult SiteNav()
        {          
            var vm = _menuServices.GetMenusByCategoryId(2);
            return PartialView("_SiteNav", vm);
        }
    }
}