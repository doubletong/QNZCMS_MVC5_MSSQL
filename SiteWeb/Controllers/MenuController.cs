using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Infrastructure.Configs;
using QNZ.Data;
using System.Data.Entity;

namespace TZGCMS.SiteWeb.Controllers
{

    public class MenuController : Controller
    {
        private readonly IQNZDbContext _db;
        private readonly ICacheService _cacheService;
        public MenuController(ICacheService cacheService, IQNZDbContext db)
        {
            _cacheService = cacheService;
            _db = db;
        }


        public PartialViewResult SiteNav(int cid)
        {
            var vm = new List<Menu>();

            string key = "Menus_Active_Category_" + cid.ToString();
            if (SettingsManager.Site.EnableCaching)
            {
                if (_cacheService.IsSet(key))
                {
                    vm = (List<Menu>)_cacheService.Get(key);

                }
                else
                {
                    vm = _db.Menus.Include(d => d.Menus).AsNoTracking().Where(m => m.Active && m.CategoryId == cid)
                    .OrderBy(d => d.Importance).ToList();

                    _cacheService.Set(key, vm, SettingsManager.Site.CacheDuration);
                }
            }
            else
            {
                vm = _db.Menus.Include(d => d.Menus).AsNoTracking().Where(m => m.Active && m.CategoryId == cid)
                  .OrderBy(d => d.Importance).ToList();
            }

            return PartialView("_SiteNav", vm);
        }
     
        public PartialViewResult SiteFooterNav(int cid)
        {
            var vm = new List<Menu>();
            string key = "Menus_Active_Category_" + cid.ToString();

            if (SettingsManager.Site.EnableCaching)
            {
                if (_cacheService.IsSet(key))
                {
                    vm = (List<Menu>)_cacheService.Get(key);

                }
                else
                {

                    vm = _db.Menus.Include(d => d.Menus).AsNoTracking().Where(m => m.Active && m.CategoryId == cid)
                  .OrderBy(d => d.Importance).ToList();
                    _cacheService.Set(key, vm, SettingsManager.Site.CacheDuration);
                }
            }
            else
            {
                vm = _db.Menus.Include(d => d.Menus).AsNoTracking().Where(m => m.Active && m.CategoryId == cid)
                    .OrderBy(d => d.Importance).ToList();
              
            }            
    
            return PartialView("_SiteFooterNav", vm);
        }
    }
}