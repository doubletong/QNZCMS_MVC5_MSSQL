using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Extensions;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Service.Identity
{
    public interface IMenuServices
    {
        //List<Menu> GetFrontMenus(int categoryId);
        //Menu CreateAndSort(Menu menu);
        //// Menu UpdateAndSort(Menu menu);
        //void ResetSort(int categoryId);
        //Menu GetMenuWithChildMenus(int Id);
        List<Menu> GetShowMenus(int categoryId);

        //Task<IEnumerable<Menu>> GetMenus(int categoryId, CancellationToken cancellationToken = default(CancellationToken));
        //int UpMoveMenu(int id);
        //int DownMoveMenu(int id);
        //List<Menu> GetFaltMenus(int categoryId);
        ////List<MenuVM> GetFaltMenus(int categoryId);

        IEnumerable<Menu> GetMenusByCategoryId(int categoryId);
        IEnumerable<Menu> GetLevelMenusByCategoryId(int categoryId);
        Menu GetByIdWithChilds(int id);
        int UpMoveMenu(int id);
        int DownMoveMenu(int id);

        List<Menu> CurrenMenuCrumbs(int categoryId);

        Menu GetCurrenMenu();
        void ResetSort(int categoryId);

        Menu GetById(int id);
        bool Update(Menu menu);
        Menu Create(Menu menu);
        bool Delete(Menu menu);
    }
    public class MenuServices : IMenuServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public List<Menu> CurrenMenuCrumbs(int categoryId)
        {
            string area = Site.CurrentArea(), controller = Site.CurrentController(), action = Site.CurrentAction();

            var rource = GetShowMenus(categoryId);
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
                var rource = GetShowMenus(vMenu.CategoryId);
                Menu parentMenu = rource.FirstOrDefault(m => m.Id == vMenu.ParentId);
                if (parentMenu != null)
                    RecursiveLoad(vMenu.ParentMenu, Parents);
            }
        }


        public List<Menu> GetShowMenus(int categoryId)
        {
       
            var menus = _unitOfWork.MenuRepository.
                GetMany(m => m.CategoryId == categoryId && (m.MenuType == MenuType.NOLINK || 
                m.MenuType == MenuType.PAGE)).OrderBy(m => m.Importance).ToList();
           
            return menus;

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
           var menus = _unitOfWork.MenuRepository.GetMany(m => m.CategoryId == categoryId).ToList(); 
            return menus;
        }

        /// <summary>
        /// 按分类ID获取菜单
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<Menu> GetMenusByCategoryId(int categoryId)
        {
            return  _unitOfWork.MenuRepository.GetMany(m => m.CategoryId == categoryId).OrderBy(m => m.Importance);
        }

        /// <summary>
        /// 按分类ID获取菜单并缓存
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        //public IEnumerable<Menu> GetMenusByCategoryIdWithCache(int categoryId)
        //{
        //    var key = $"{EntityNames.Menu}S_SetRole_{categoryId}";

        //    IEnumerable<Menu> result;
        //    if (SettingsManager.Menu.EnableCaching)
        //    {
        //        if (_cacheService.IsSet(key))
        //        {
        //            result = (IEnumerable<Menu>)_cacheService.Get(key);
        //        }
        //        else
        //        {
        //            result = GetMenusByCategoryId(categoryId);
        //            _cacheService.Set(key, result, SettingsManager.Menu.CacheDuration);
        //        }
        //    }
        //    else
        //    {
        //        result = GetMenusByCategoryId(categoryId);
        //    }

        //    return result;
        //}

        /// <summary>
        /// 按分类ID获取菜单并层级化（角色菜单权限）
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<Menu> GetLevelMenusByCategoryId(int categoryId)
        {
            return _unitOfWork.MenuRepository.Table.Include("ChildMenus").Where(d => d.CategoryId == categoryId && d.ParentId == null).AsEnumerable();           
        }


        /// <summary>
        /// 按分类重设排序
        /// </summary>
        /// <param name="categoryId"></param>
        public void ResetSort(int categoryId)
        {
            var menuList = GetLevelMenusByCategoryId(categoryId);// _menuRepository.GetFilteredElements(m => m.CategoryId == categoryId && m.ParentId == null, m => m.ChildMenus).OrderBy(m => m.Importance);
            var list = menuList.Where(m => m.ParentId == null).OrderBy(m => m.Importance)
                .SelectDeep<Menu>(m => m.ChildMenus.OrderBy(g => g.Importance));

            int i = 0;
            foreach (var item in list)
            {
                item.Importance = i;
                Update(item);
                i++;
            }
          
            //   _menuRepository.UnitOfWork.Commit();
            // _cacheService.Invalidate(EntityNames.Menu);
            // return menu;
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        /// <param name = "id" ></ param >
        /// < returns ></ returns >
        public int UpMoveMenu(int id)
        {
            Menu vMenu = GetById(id);
            var menuList = GetMenusByCategoryId(vMenu.CategoryId).OrderBy(m => m.Importance);

            Menu prevMenu = menuList.Where(m => m.ParentId == vMenu.ParentId &&
                m.Importance < vMenu.Importance).OrderByDescending(m => m.Importance).FirstOrDefault();

            if (prevMenu == null)
            {
                // 已经在第一位
                return 0;
            }
            var num = prevMenu.Importance - vMenu.Importance;
            prevMenu.Importance = prevMenu.Importance - num;
            vMenu.Importance = vMenu.Importance + num;

            Update(prevMenu);
            Update(vMenu);

            ResetSort(vMenu.CategoryId);
            //SetMenuImportance(vMenu.Id, num);
            //SetMenuImportance(prevMenu.Id, -num);

            // _cacheService.Invalidate(EntityNames.Menu);
            return 1;
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DownMoveMenu(int id)
        {
            Menu vMenu = GetById(id);
            var menuList = GetMenusByCategoryId(vMenu.CategoryId).OrderBy(m => m.Importance); ;

            Menu nextMenu = menuList.Where(m => m.ParentId == vMenu.ParentId &&
                m.Importance > vMenu.Importance).OrderBy(m => m.Importance).FirstOrDefault();


            if (nextMenu == null)
            {
                // 已经在最后一位
                return 0;
            }
                    

            var num = nextMenu.Importance - vMenu.Importance;
            nextMenu.Importance = nextMenu.Importance - num;
            vMenu.Importance = vMenu.Importance + num;
            ResetSort(vMenu.CategoryId);
            return 1;
        }



        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        //        public int SetMenuImportance(int menuId, int num)
        //        {
        //            int result;
        //            using (_connection = Utilities.GetOpenConnection())
        //            {
        //                var query = "with MenuSet_tree as ( " +
        //  "select MenuSet.id as top_MenuSet_id, MenuSet.id as MenuSet_id " +
        //  " from MenuSet union all   select top_MenuSet_id, MenuSet.id   from MenuSet_tree " +
        //  "     join MenuSet on MenuSet.ParentId = MenuSet_tree.MenuSet_id)  " +
        //"update MenuSet Set Importance = Importance + @Num where Id in (select MenuSet_id from MenuSet_tree where top_MenuSet_id = @MenuId)";
        //                result = (int)_connection.Execute(query, new { MenuId = menuId, Num = num });
        //            }
        //            return result;
        //        }


        public Menu GetById(int id)
        {
            return _unitOfWork.MenuRepository.GetById(id);
        }
        public Menu GetByIdWithChilds(int id)
        {
            return _unitOfWork.MenuRepository.GetFirstOrDefault(d=>d.Id == id,d=>d.ChildMenus,d=>d.Roles);
        }


        public bool Update(Menu menu)
        {
            return _unitOfWork.MenuRepository.Update(menu);
        }

        public Menu Create(Menu menu)
        {
            return _unitOfWork.MenuRepository.Insert(menu);
        }
        public bool Delete(Menu menu)
        {
            return _unitOfWork.MenuRepository.Delete(menu);
        }

    }
}
