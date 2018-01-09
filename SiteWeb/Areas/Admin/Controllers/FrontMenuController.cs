using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SiteWeb.Filters;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Menus;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Menus;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Identity;
using TZGCMS.Service.PageMetas;

namespace SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class FrontMenuController : BaseController
    {
        // GET: Admin/FrontMenu
        private readonly IMenuCategoryServices _menuCategoryService;
        private readonly IMenuServices _menuService;
        private readonly IPageMetaServices _pageMetaService;
        private readonly IMapper _mapper;

       
        public FrontMenuController(IMenuCategoryServices menuCategoryService, IMenuServices menuService, IPageMetaServices pageMetaService, IMapper mapper)
        {
            _menuService = menuService;
            _menuCategoryService = menuCategoryService;
            _pageMetaService = pageMetaService;
            _mapper = mapper;

        }
        //
        // GET: /Admin/Menu/ 
        public ActionResult Index()
        {
            IEnumerable<MenuCategory> menuCategories = _menuCategoryService.GetAll().Where(m=>m.Id!= SettingsManager.Menu.BackMenuCId);
            return View(menuCategories);
        }



        [HttpGet]
        public ActionResult AddCategory()
        {
            var vCategory = new MenuCategoryIM
            {
                Active = true,
                Importance = 0
            };

            return PartialView("_AddCategory", vCategory);
        }

        [HttpPost]
        public JsonResult AddCategory(MenuCategoryIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var newCategory = _mapper.Map<MenuCategoryIM, MenuCategory>(vm);

            _menuCategoryService.Create(newCategory);



            IEnumerable<MenuCategory> menuCategories = _menuCategoryService.GetAll().Where(m => m.Id != SettingsManager.Menu.BackMenuCId);
        
            AR.Data = RenderPartialViewToString("_CategoryList", menuCategories);

            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.MenuCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }




        [HttpGet]
        public ActionResult EditCategory(int id)
        {

            var category = _menuCategoryService.GetById(id);
            if (category == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var vm = _mapper.Map<MenuCategoryIM>(category);

            return PartialView("_EditCategory", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult EditCategory(MenuCategoryIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var newCategory = _mapper.Map<MenuCategoryIM, MenuCategory>(vm);

            _menuCategoryService.Update(newCategory);


            var menuCategories = _menuCategoryService.GetAll().Where(m => m.Id != SettingsManager.Menu.BackMenuCId);

            AR.Data = RenderPartialViewToString("_CategoryList", menuCategories);

            AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.MenuCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteCategory(int id)
        {

            var vCategory = _menuCategoryService.GetById(id);
            if (vCategory.IsSys)
            {
                AR.Setfailure("系统内置栏目，不能删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            //if (vCategory.Menus.Any())
            //{
            //    AR.Setfailure("此分类下面还有文章存在，不能删除！");
            //    return Json(AR, JsonRequestBehavior.DenyGet);
            //}

            var menuCategories = _menuCategoryService.GetAll().Where(m => m.Id != SettingsManager.Menu.BackMenuCId);
            AR.Data = RenderPartialViewToString("_CategoryList", menuCategories);

            _menuCategoryService.Delete(vCategory);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.MenuCategory));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }




        /// <summary>
        /// 获取单组菜单
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetMenus(int categoryId)
        {
            // var menus = _menuService.GetLevelMenusByCategoryId(categoryId);
            var menus = _menuService.GetMenusByCategoryId(categoryId);
            return PartialView("_MenuList", menus);
        }





        [HttpGet]
        public ActionResult CreateMenu(int categoryId, int? parentId)
        {
            var vMenu = new Menu();
            FrontMenuIM newDto = _mapper.Map<FrontMenuIM>(vMenu);

            newDto.CategoryId = (int)categoryId;
            newDto.ParentId = parentId??null;
            return PartialView("_MenuCreate", newDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateMenu(FrontMenuIM vm)
        {

            if (ModelState.IsValid)
            {
                vm.Url = vm.Url.ToLower();
                var vMenu = _mapper.Map<Menu>(vm);
                if (vm.ParentId != null)
                {
                    var parentMenu = _menuService.GetById(vMenu.ParentId.Value);
                    vMenu.LayoutLevel = parentMenu.LayoutLevel + 1;
                }
                else
                {
                    vMenu.LayoutLevel = 0;
                }
                //vMenu.CreatedDate = DateTime.Now;
                //vMenu.CreatedBy = Site.CurrentUserName;

                _menuService.Create(vMenu);
                _menuService.ResetSort((vMenu.CategoryId));


                if (!string.IsNullOrEmpty(vm.Url))
                {
                    var pageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, vm.Url);
                    pageMeta = pageMeta ?? new PageMeta();
                    

                    pageMeta.ObjectId = vm.Url;
                    pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
                    pageMeta.Keyword = vm.Keywords;
                    pageMeta.Description = vm.SEODescription;
                    pageMeta.ModelType = ModelType.MENU;

                    if (pageMeta.Id > 0)
                    {
                        _pageMetaService.Update(pageMeta);
                    }
                    else
                    {
                        _pageMetaService.Create(pageMeta);
                    }
                  
                }

                var menus = _menuService.GetMenusByCategoryId(vMenu.CategoryId);
                AR.Id = vm.CategoryId;
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
            var vMenu = _menuService.GetById(id);
            FrontMenuIM dto = _mapper.Map<FrontMenuIM>(vMenu);

            if (!string.IsNullOrEmpty(dto.Url))
            {
                var pageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, dto.Url);
                if (pageMeta != null)
                {
                    dto.SEOTitle = pageMeta.Title;
                    dto.Keywords = pageMeta.Keyword;
                    dto.SEODescription = pageMeta.Description;
                }
            }           

            return PartialView("_MenuEdit", dto);

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditMenu(FrontMenuIM vm)
        {

            if (ModelState.IsValid)
            {
                var vMenu = _mapper.Map<Menu>(vm);

                var orgMenu = _menuService.GetById(vMenu.Id);

                var pageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, orgMenu.Url);

                orgMenu.Title = vMenu.Title;
                orgMenu.MenuType = vMenu.MenuType;
                orgMenu.Active = vMenu.Active;
                orgMenu.Action = vMenu.Action;
                orgMenu.Area = vMenu.Area;
                orgMenu.CategoryId = vMenu.CategoryId;
                orgMenu.Controller = vMenu.Controller;
                orgMenu.Iconfont = vMenu.Iconfont;
                orgMenu.ParentId = vMenu.ParentId;
                orgMenu.Url = string.IsNullOrEmpty(vMenu.Url)?string.Empty:vMenu.Url.ToLower();
                orgMenu.UpdatedDate = DateTime.Now;
                orgMenu.UpdatedBy = Site.CurrentUserName;

                _menuService.Update(orgMenu);
                
                if (string.IsNullOrEmpty(vm.Url))
                {
                    if (pageMeta != null)
                    {
                        _pageMetaService.Delete(pageMeta);
                    }

                }else
                {
                    pageMeta = pageMeta ?? new PageMeta();

                   

                    pageMeta.ObjectId = vm.Url;
                    pageMeta.Title = string.IsNullOrEmpty(vm.SEOTitle) ? vm.Title : vm.SEOTitle;
                    pageMeta.Keyword = vm.Keywords;
                    pageMeta.Description = vm.SEODescription;
                    pageMeta.ModelType = ModelType.MENU;

                    if (pageMeta.Id > 0)
                    {
                        _pageMetaService.Update(pageMeta);
                    }
                    else
                    {
                        _pageMetaService.Create(pageMeta);
                    }
                   
                }
                

                // _menuService.ResetSort(orgMenu.CategoryId);

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

            var vMenu = _menuService.GetById(id);

            if (vMenu != null)
            {
               
                if (vMenu.ChildMenus.Any())
                {
                    AR.Setfailure(string.Format(Messages.AlertDeleteFailureHasChild, EntityNames.Menu));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }


                if (!string.IsNullOrEmpty(vMenu.Url))
                {
                    var pageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, vMenu.Url.Trim());
                    if (pageMeta != null)
                    {
                        _pageMetaService.Delete(pageMeta);
                    }
                }

               // vMenu.Roles.Clear();
                _menuService.Delete(vMenu);
                //_menuService.ResetSort(vMenu.CategoryId);

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

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            AR.Setfailure(Messages.AlertActionFailure);
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public  JsonResult ResetSort(int id)
        {
            try
            {
                _menuService.ResetSort(id);

                var menus = _menuService.GetMenusByCategoryId(id);
                AR.Id = id;
                AR.Data = RenderPartialViewToString("_MenuList", menus);

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
        // GET: Roles/Create
        public ActionResult MoveMenu(int id)
        {
            var menu = _menuService.GetById(id);
            var menus = _menuService.GetMenusByCategoryId(menu.CategoryId);
            MoveMenuVM vm = new MoveMenuVM
            {
                Id = id,
                Menus = menus,//_mapper.Map<List<Menu>, List<MenuVM>>(menus),
                CurrentParentId = menu.ParentId,
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