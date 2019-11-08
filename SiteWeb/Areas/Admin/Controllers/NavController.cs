using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel.Menus;


namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    public class NavController : BaseController
    {
     
        private readonly ICacheService _cacheService;
        public NavController(ICacheService cacheService)
        {        
            _cacheService = cacheService;
        }

        // GET: Admin/Nav
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 面包屑
        /// </summary>
        [AllowAnonymous]
        public ActionResult Crumbs(string areaName, string controller, string action)
        {
            return PartialView("_Crumbs", CurrenMenuCrumbs(SettingsManager.Menu.BackMenuCId));
        }

        public List<Menu> CurrenMenuCrumbs(int categoryId)
        {
            string area = Site.CurrentArea(), controller = Site.CurrentController(), action = Site.CurrentAction();
            var allmenus = GetAllMenusByCategoryId(categoryId);
            var rource = allmenus.Where(d => d.MenuType == MenuType.NOLINK || d.MenuType == MenuType.PAGE).OrderBy(d => d.Importance).ToList();

                //GetShowMenus(categoryId);
            List<Menu> menus = new List<Menu>();

            Menu vMenu = rource.FirstOrDefault(m => area.Equals(m.Area, StringComparison.OrdinalIgnoreCase)
            && controller.Equals(m.Controller, StringComparison.OrdinalIgnoreCase)
            && action.Equals(m.Action, StringComparison.OrdinalIgnoreCase));

            if (vMenu != null)
                RecursiveLoad(vMenu, menus);

            return menus;
        }

        /// <summary>
        /// 递归获取父项
        /// </summary>
        /// <param name="vMenu"></param>
        /// <param name="Parents"></param>
        private void RecursiveLoad(Menu vMenu, List<Menu> Parents)
        {
          
            Parents.Insert(0, vMenu);
            if (vMenu.ParentId != null)
            {
                var allmenus = GetAllMenusByCategoryId(vMenu.CategoryId);

                var rource = allmenus.Where(d => d.MenuType == MenuType.NOLINK || d.MenuType == MenuType.PAGE).OrderBy(d => d.Importance).ToList();
            
                Menu parentMenu = rource.FirstOrDefault(m => m.Id == vMenu.ParentId);
                if (parentMenu != null)
                    RecursiveLoad(vMenu.ParentMenu, Parents);
            }
        }



        [AllowAnonymous]
        public ActionResult LeftNavs(int categoryId)
        {
            var allmenus = GetAllMenusByCategoryId(categoryId);
            LeftNavVM vm = new LeftNavVM
            {               
                Menus = allmenus.Where(d=>d.MenuType == MenuType.NOLINK || d.MenuType == MenuType.PAGE).OrderBy(d=>d.Importance).ToList(),                
       
                CurrentMenu = GetCurrenMenu()
            };

            return PartialView("_LeftNav", vm);


        }


        /// <summary>
        /// 获取需要高亮的菜单ID
        /// </summary>
        /// <returns></returns>
        public Menu GetCurrenMenu()
        {
            string area = Site.CurrentArea(), controller = Site.CurrentController(), action = Site.CurrentAction();

            var menus = GetAllMenusByCategoryId(SettingsManager.Menu.BackMenuCId);

            if (menus == null)
                return null;

            Menu vMenu = menus.FirstOrDefault(m => area.Equals(m.Area, StringComparison.OrdinalIgnoreCase)
            && controller.Equals(m.Controller, StringComparison.OrdinalIgnoreCase)
            && action.Equals(m.Action, StringComparison.OrdinalIgnoreCase));


            if (vMenu == null)
                return null;

            if (vMenu.Active || vMenu.MenuType == MenuType.PAGE)
                return vMenu;

            return RecursiveLoadMenu(vMenu.ParentId);


        }


        private Menu RecursiveLoadMenu(int? parentId)
        {
            var menus = GetAllMenusByCategoryId(SettingsManager.Menu.BackMenuCId);

            Menu vMenu = menus.Where(m => m.ParentId == parentId && m.MenuType == MenuType.PAGE
           ).FirstOrDefault();

            if (vMenu.ParentMenu != null && (vMenu.ParentMenu.MenuType != MenuType.PAGE || !vMenu.ParentMenu.Active))
            {
                return RecursiveLoadMenu(vMenu.ParentId);
            }
            return vMenu.ParentMenu;
        }


        public List<Menu> GetAllMenusByCategoryId(int categoryId)
        {
            var menus = new List<Menu>();
            string key = "Menus_Category_" + categoryId.ToString();
            if (_cacheService.IsSet(key))
            {
                menus = (List<Menu>)_cacheService.Get(key);
             
            }
            else
            {
                menus = _db.Menu.Where(d => d.CategoryId == categoryId).OrderBy(d=>d.Importance).ToList();
                _cacheService.Set(key, menus, 120);               
            }

            return menus;

        }
    }
}