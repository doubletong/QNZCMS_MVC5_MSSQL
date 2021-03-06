﻿using System.Web.Mvc;
using AutoMapper;
using System;
using TZGCMS.SiteWeb.Filters;
using TZGCMS.Service.Identity;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Model.Admin.InputModel.Menus;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Data.Enums;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using TZGCMS.Model.Admin.ViewModel.Menus;
using TZGCMS.Infrastructure.Cache;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    //  [Authorize(Roles = "创始人")]
    [SIGAuth]
    public class MenuController : BaseController
    {
        private readonly ICacheService _cacheService;
        private readonly IMenuCategoryServices _menuCategoryService;
        private readonly IMenuServices _menuService;
        private readonly IMapper _mapper;

        public MenuController(ICacheService cacheService,IMenuServices menuService, IMenuCategoryServices menuCategoryService, IMapper mapper)
        {
            _cacheService = cacheService;
            _menuService = menuService;
            _menuCategoryService = menuCategoryService;
            _mapper = mapper;
        }

        //
        // GET: /Admin/Menu/ 
        public ActionResult Index()
        {
            var menuCategory = _menuCategoryService.GetById(SettingsManager.Menu.BackMenuCId);
            //  var vm = _mapper.Map<MenuCategoryVM>(menuCategory);      
            return View(menuCategory);
        }

        /// <summary>
        /// 获取单组菜单
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetMenus(int categoryId)
        {
            var menus = _menuService.GetMenusByCategoryId(categoryId);
            return PartialView("_MenuList", menus);
        }





        [HttpGet]
        public ActionResult CreateMenu(int categoryId, int parentId)
        {
            var vMenu = new Menu();
            MenuIM newDto = _mapper.Map<MenuIM>(vMenu);

            newDto.CategoryId = (int)categoryId;
            newDto.ParentId = parentId;
            return PartialView("_MenuCreate", newDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateMenu(MenuIM menu)
        {

            if (ModelState.IsValid)
            {
                var vMenu = _mapper.Map<Menu>(menu);

                var parentMenu = _menuService.GetById(vMenu.ParentId.Value);
                vMenu.LayoutLevel = parentMenu.LayoutLevel + 1;
                vMenu.CreatedBy = Site.CurrentUserName;
                vMenu.CreatedDate = DateTime.Now;



                //自动添加通用操作
                if (vMenu.CategoryId == SettingsManager.Menu.BackMenuCId && vMenu.MenuType == MenuType.PAGE)
                {
                   


                    vMenu.ChildMenus.Add(new Menu
                    {
                        Title = "编辑",
                        Controller = vMenu.Controller,
                        Action = "Edit",
                        Area = vMenu.Area,
                        MenuType = MenuType.PAGE,
                        CategoryId = vMenu.CategoryId,
                        LayoutLevel = vMenu.LayoutLevel + 1,

                        CreatedBy = Site.CurrentUserName,
                        CreatedDate = DateTime.Now,
                    });

                    vMenu.ChildMenus.Add(new Menu
                    {
                        Title = "显示/隐藏",
                        Controller = vMenu.Controller,
                        Action = "IsActive",
                        Area = vMenu.Area,
                        MenuType = MenuType.ACTION,
                        CategoryId = vMenu.CategoryId,
                        LayoutLevel = vMenu.LayoutLevel + 1,

                        CreatedBy = Site.CurrentUserName,
                        CreatedDate = DateTime.Now,
                    });

                    vMenu.ChildMenus.Add(new Menu
                    {
                        Title = "删除",
                        Controller = vMenu.Controller,
                        Action = "Delete",
                        Area = vMenu.Area,
                        MenuType = MenuType.ACTION,
                        CategoryId = vMenu.CategoryId,
                        LayoutLevel = vMenu.LayoutLevel + 1,

                        CreatedBy = Site.CurrentUserName,
                        CreatedDate = DateTime.Now,
                    });

                    vMenu.ChildMenus.Add(new Menu
                    {
                        Title = "分页设置",
                        Controller = vMenu.Controller,
                        Action = "PageSizeSet",
                        Area = vMenu.Area,
                        MenuType = MenuType.ACTION,
                        CategoryId = vMenu.CategoryId,
                        LayoutLevel = vMenu.LayoutLevel + 1,
                        // ParentId = result,
                        CreatedBy = Site.CurrentUserName,
                        CreatedDate = DateTime.Now,
                    });

                }

                var result = _menuService.Create(vMenu);
                //_menuService.CreateAndSort(vMenu);           
                _menuService.ResetSort(menu.CategoryId);

                _cacheService.Invalidate("Menu");

                var menus = _menuService.GetMenusByCategoryId(vMenu.CategoryId);
                AR.Id = menu.CategoryId;
                AR.Data = RenderPartialViewToString("_MenuList", menus);

                AR.SetSuccess("已成功新增菜单");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            AR.Setfailure("编辑菜单失败");
            return Json(AR, JsonRequestBehavior.DenyGet);
            //   return RedirectToAction("Index");

        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult EditMenu(int id)
        {
            Menu vMenu = _menuService.GetById(id);
            MenuIM dto = _mapper.Map<MenuIM>(vMenu);
            return PartialView("_MenuEdit", dto);

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditMenu(MenuIM menu)
        {

            if (ModelState.IsValid)
            {
                Menu vMenu = _mapper.Map<Menu>(menu);

                Menu orgMenu = _menuService.GetById(vMenu.Id);
                orgMenu.Title = vMenu.Title;
                orgMenu.MenuType = vMenu.MenuType;
                orgMenu.Active = vMenu.Active;
                orgMenu.Action = vMenu.Action;
                orgMenu.Area = vMenu.Area;
                orgMenu.CategoryId = vMenu.CategoryId;
                orgMenu.Controller = vMenu.Controller;
                orgMenu.Iconfont = vMenu.Iconfont;
                orgMenu.ParentId = vMenu.ParentId;
                orgMenu.Url = vMenu.Url;
                orgMenu.UpdatedBy = Site.CurrentUserName;
                orgMenu.UpdatedDate = DateTime.Now;

                _menuService.Update(orgMenu);

                // _menuService.ResetSort(orgMenu.CategoryId);
                _cacheService.Invalidate("Menu");

                var menus = _menuService.GetMenusByCategoryId(vMenu.CategoryId);
                AR.Id = vMenu.CategoryId;
                AR.Data = RenderPartialViewToString("_MenuList", menus);

                AR.SetSuccess("已成功保存菜单");
                return Json(AR, JsonRequestBehavior.DenyGet);

            }

            AR.Setfailure("编辑菜单失败");
            return Json(AR, JsonRequestBehavior.DenyGet);



        }



        // POST: Admin/User/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {

            Menu vMenu = _menuService.GetByIdWithChilds(id);

            if (vMenu != null)
            {

                int cid = vMenu.CategoryId;
                if (SettingsManager.Menu.BackMenuCId == cid)
                {
                    if (User.UserId.ToString() != SettingsManager.User.Founder)
                    {
                        AR.SetWarning(string.Format(Messages.NotFounderCanNotDelete, EntityNames.Menu));
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }

                }

                // var childMenuCount = _menuService.GetMenuCount(id);
                if (vMenu.ChildMenus.Count > 1)
                {
                    AR.Setfailure(string.Format(Messages.AlertDeleteFailureHasChild, EntityNames.Menu));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }

                //  vMenu.Roles.Clear();
                _menuService.Delete(vMenu);
                //  _menuService.DeleteMenuWithRoles(id);
                // _menuService.ResetSort(vMenu.CategoryId);

                _cacheService.Invalidate("Menu");

                //var menus = await _menuService.GetMenus(cid);
                //return PartialView("_MenuList", menus);
                AR.SetSuccess(string.Format(Messages.AlertDeleteSuccess, EntityNames.Menu));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(string.Format(Messages.AlertDeleteFailure, EntityNames.Menu));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpDownMove(int id, bool isUp, int categoryId)
        {

            if (isUp)
            {
                var result = _menuService.UpMoveMenu(id);

                if (result == 0)
                {
                    AR.SetInfo("已经在第一位！");
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
            }
            else
            {
                var result = _menuService.DownMoveMenu(id);

                if (result == 0)
                {
                    AR.SetInfo("已经在末位！");
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }

            }

            var menus = _menuService.GetMenusByCategoryId(categoryId);
            _cacheService.Invalidate("Menu");

            AR.Id = categoryId;
            AR.Data = RenderPartialViewToString("_MenuList", menus);

            AR.SetSuccess("菜单排位成功！");
            return Json(AR, JsonRequestBehavior.DenyGet);

            //AR.Setfailure("菜单排位失败！");
            //return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        public JsonResult IsExpand(int id)
        {
            var menu = _menuService.GetById(id);
            if (menu != null)
            {
                menu.IsExpand = !menu.IsExpand;
                _menuService.Update(menu);

                _cacheService.Invalidate("Menu");

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            AR.Setfailure(Messages.AlertActionFailure);
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public JsonResult IsActive(int id)
        {
            var menu = _menuService.GetById(id);
            if (menu != null)
            {
                menu.Active = !menu.Active;
                _menuService.Update(menu);

                _cacheService.Invalidate("Menu");

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            AR.Setfailure(Messages.AlertActionFailure);
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public JsonResult ResetSort(int id)
        {
            try
            {
                _menuService.ResetSort(id);

                var menus = _menuService.GetMenusByCategoryId(id);
                AR.Id = id;
                AR.Data = RenderPartialViewToString("_MenuList", menus);

                _cacheService.Invalidate("Menu");

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        [HttpGet]
        public ActionResult MoveMenu(int id)
        {
            var menu = _menuService.GetById(id);
            var menus = _menuService.GetMenusByCategoryId(menu.CategoryId);
            MoveMenuVM vm = new MoveMenuVM
            {
                Id = id,
                Menus = menus, //_mapper.Map<List<Menu>, List<MenuVM>>(menus),
                CurrentParentId = (int)menu.ParentId,
                CategoryId = menu.CategoryId
            };

            return PartialView("_MoveMenu", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoveMenu(int Id, int menuId)
        {

            if (Id > 0 && menuId > 0)
            {
                var parentMenu = _menuService.GetById(menuId);
                var menu = _menuService.GetById(Id);
                menu.ParentId = menuId;
                menu.LayoutLevel = parentMenu.LayoutLevel + 1;
                _menuService.Update(menu);

                _menuService.ResetSort(menu.CategoryId);

                _cacheService.Invalidate("Menu");

                var menus = _menuService.GetMenusByCategoryId(menu.CategoryId);
                AR.Id = menu.CategoryId;
                AR.Data = RenderPartialViewToString("_MenuList", menus);

                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            AR.Setfailure("移动菜单失败");
            return Json(AR, JsonRequestBehavior.DenyGet);
        }


    }
}
