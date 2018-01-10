using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Admin.ViewModel.Menus;
using TZGCMS.Service.Identity;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    public class NavController : BaseController
    {
        private readonly IMenuServices _menuServices;
     
        public NavController(IMenuServices menuServices)
        {
            _menuServices = menuServices;

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
            return PartialView("_Crumbs", _menuServices.CurrenMenuCrumbs(SettingsManager.Menu.BackMenuCId));
        }

        [AllowAnonymous]
        public ActionResult LeftNavs(int categoryId)
        {

            LeftNavVM vm = new LeftNavVM
            {
                Menus = _menuServices.GetShowMenus(categoryId),
                CurrentMenu = _menuServices.GetCurrenMenu()
            };

            return PartialView("_LeftNav", vm);


        }
    }
}