using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TZGCMS.SiteWeb.Filters;
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
using QNZ.Data;
using TZGCMS.Model;
using Menu = TZGCMS.Data.Entity.Identity.Menu;
using System.Data.Entity;
using TZGCMS.Infrastructure.Extensions;
using System.Threading.Tasks;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class FrontMenuController : QNZBaseController
    {
        // GET: Admin/FrontMenu
        private readonly IMenuCategoryServices _menuCategoryService;
        private readonly IMenuServices _menuService;
        private readonly IPageMetaServices _pageMetaService;
        private readonly IMapper _mapper;
        private IQNZDbContext _db;

        public FrontMenuController(IMenuCategoryServices menuCategoryService, IMenuServices menuService, IPageMetaServices pageMetaService, IMapper mapper, IQNZDbContext db)
        {
            _menuService = menuService;
            _menuCategoryService = menuCategoryService;
            _pageMetaService = pageMetaService;
            _mapper = mapper;
            _db = db;
        }
        //
        // GET: /Admin/Menu/ 
        public ActionResult Index()
        {
            IEnumerable<QNZ.Data.MenuCategory> menuCategories = _db.MenuCategories.Where(m=>m.Id!= SettingsManager.Menu.BackMenuCId);
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

            var newCategory = _mapper.Map<MenuCategoryIM, Data.Entity.Identity.MenuCategory>(vm);

            _menuCategoryService.Create(newCategory);



            IEnumerable<Data.Entity.Identity.MenuCategory> menuCategories = _menuCategoryService.GetAll().Where(m => m.Id != SettingsManager.Menu.BackMenuCId);
        
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

            var newCategory = _mapper.Map<MenuCategoryIM, Data.Entity.Identity.MenuCategory>(vm);

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
            var menus =  _db.Menus.Where(d => d.CategoryId == categoryId).OrderBy(d => d.Importance).ToList();
            return PartialView("_MenuList", menus);
        }





        //[HttpGet]
        //public ActionResult CreateMenu(int categoryId, int? parentId)
        //{
        //    var vMenu = new Menu();
        //    FrontMenuIM newDto = _mapper.Map<FrontMenuIM>(vMenu);

        //    newDto.CategoryId = (int)categoryId;
        //    newDto.ParentId = parentId??null;
        //    return PartialView("_MenuCreate", newDto);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<JsonResult> CreateMenu(FrontMenuIM vm)
        {

            if (ModelState.IsValid)
            {
                vm.Url = vm.Url.ToLower();
                var vMenu = _mapper.Map<Data.Entity.Identity.Menu>(vm);
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

                await SetPageMetaAsync(_db, (short)ModelType.ARTICLECATEGORY, vm.Url, vm.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);
               

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
        public ActionResult EditMenu(int categoryId, int? parentId, int? id)
        {

            if (id > 0)
            {
                var vMenu = _db.Menus.Find(id);
                FrontMenuIM dto = _mapper.Map<FrontMenuIM>(vMenu);

                if (!string.IsNullOrEmpty(dto.Url))
                {
                    var pageMeta = _db.PageMetas.FirstOrDefault(d=>d.ModelType == (short)ModelType.MENU && d.ObjectId == dto.Url);
                    if (pageMeta != null)
                    {
                        dto.SEOTitle = pageMeta.Title;
                        dto.Keywords = pageMeta.Keyword;
                        dto.SEODescription = pageMeta.Description;
                    }
                }
                return PartialView("_MenuEdit", dto);
            }
            else
            {
               
                FrontMenuIM newDto = new FrontMenuIM {
                    CategoryId = categoryId,
                    ParentId = parentId
                };

                return PartialView("_MenuEdit", newDto);
            }

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<JsonResult> EditMenu(FrontMenuIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            vm.Url = vm.Url.ToLower();
            if (vm.Id > 0)
            {
                var orgMenu = await _db.Menus.FindAsync(vm.Id);
                var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == orgMenu.Url);

                orgMenu = _mapper.Map(vm, orgMenu);
                orgMenu.UpdatedDate = DateTime.Now;
                orgMenu.UpdatedBy = Site.CurrentUserName;

                _db.Entry(orgMenu).State = EntityState.Modified;



                //orgMenu.Title = vMenu.Title;
                //orgMenu.MenuType = vMenu.MenuType;
                //orgMenu.Active = vMenu.Active;
                //orgMenu.Action = vMenu.Action;
                //orgMenu.Area = vMenu.Area;
                //orgMenu.CategoryId = vMenu.CategoryId;
                //orgMenu.Controller = vMenu.Controller;
                //orgMenu.Iconfont = vMenu.Iconfont;
                //orgMenu.ParentId = vMenu.ParentId;
                //orgMenu.Url = string.IsNullOrEmpty(vMenu.Url) ? string.Empty : vMenu.Url.ToLower();




                if (string.IsNullOrEmpty(vm.Url))
                {
                    if (pageMeta != null)
                    {
                        _db.PageMetas.Remove(pageMeta);
                    }

                }
                else
                {
                    await SetPageMetaAsync(_db, (short)ModelType.ARTICLECATEGORY, vm.Url, vm.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);

                }

                await _db.SaveChangesAsync();

                // _menuService.ResetSort(orgMenu.CategoryId);

                var menus = await _db.Menus.Where(d => d.CategoryId == orgMenu.CategoryId).OrderBy(d => d.Importance).ToListAsync(); 
                AR.Id = orgMenu.CategoryId;
                AR.Data = RenderPartialViewToString("_MenuList", menus);

                AR.SetSuccess("已成功保存菜单");
                return Json(AR, JsonRequestBehavior.DenyGet);


            }
            else
            {


                var vMenu = _mapper.Map<QNZ.Data.Menu>(vm);
                if (vm.ParentId != null)
                {
                    var parentMenu = _menuService.GetById(vMenu.ParentId.Value);
                    vMenu.LayoutLevel = parentMenu.LayoutLevel + 1;
                }
                else
                {
                    vMenu.LayoutLevel = 0;
                }
                vMenu.CreatedDate = DateTime.Now;
                vMenu.CreatedBy = Site.CurrentUserName;

                _db.Menus.Add(vMenu);
                await _db.SaveChangesAsync();

                //  _menuService.ResetSort((vMenu.CategoryId));
                List<QNZ.Data.Menu> menus = await RestSortMenus(vMenu.CategoryId);


                await SetPageMetaAsync(_db, (short)ModelType.ARTICLECATEGORY, vm.Url, vm.Title, vm.SEOTitle, vm.Keywords, vm.SEODescription);
                //var menus = await _db.Menus.Where(d => d.CategoryId == vMenu.CategoryId).OrderBy(d => d.Importance).ToListAsync();                    
                //    _menuService.GetMenusByCategoryId(vMenu.CategoryId);
                AR.Id = vm.CategoryId;
                AR.Data = RenderPartialViewToString("_MenuList", menus.OrderBy(d => d.Importance).ToList());

                AR.SetSuccess("已成功新增菜单");
                return Json(AR, JsonRequestBehavior.DenyGet);

            }

           



        }



        // POST: Admin/User/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Delete(int id)
        {

            var vMenu = await _db.Menus.Include(d=>d.Menus).FirstOrDefaultAsync(d=>d.Id == id);

            if (vMenu != null)
            {
               
                if (vMenu.Menus.Any())
                {
                    AR.Setfailure(string.Format(Messages.AlertDeleteFailureHasChild, EntityNames.Menu));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }


                if (!string.IsNullOrEmpty(vMenu.Url))
                {
                    var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == vMenu.Url); ;
                    if (pageMeta != null)
                    {
                        _db.PageMetas.Remove(pageMeta);                     
                    }
                }

              
                _db.Menus.Remove(vMenu);
                await _db.SaveChangesAsync();
              

                AR.SetSuccess(string.Format(Messages.AlertDeleteSuccess, EntityNames.Menu));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(string.Format(Messages.AlertDeleteFailure, EntityNames.Menu));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpDownMove(int id, bool isUp, int categoryId)
        {

            if (isUp)
            {
                var result = await UpMoveMenuAsync(id);

                if (result == 0)
                {
                    AR.SetInfo("已经在第一位！");
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
            }
            else
            {
                var result = await DownMoveMenuAsync(id);

                if (result == 0)
                {
                    AR.SetInfo("已经在末位！");
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }

            }

            var menus = await _db.Menus.Where(d => d.CategoryId == categoryId).OrderBy(d => d.Importance).ToListAsync(); 
            AR.Id = categoryId;
            AR.Data = RenderPartialViewToString("_MenuList", menus);

            AR.SetSuccess("菜单排位成功！");
            return Json(AR, JsonRequestBehavior.DenyGet);

            //AR.Setfailure("菜单排位失败！");
            //return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        public async Task<JsonResult> IsExpand(int id)
        {
            var menu = await _db.Menus.FindAsync(id);
            if (menu != null)
            {
                menu.IsExpand = !menu.IsExpand;

                _db.Entry(menu).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            AR.Setfailure(Messages.AlertActionFailure);
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public async Task<JsonResult> IsActive(int id)
        {
            var menu = await _db.Menus.FindAsync(id);
            if (menu != null)
            {
                menu.Active = !menu.Active;

                _db.Entry(menu).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            AR.Setfailure(Messages.AlertActionFailure);
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public async Task<JsonResult> ResetSort(int id)
        {
            try
            {
                // _menuService.ResetSort(id);
                List<QNZ.Data.Menu> menus = await RestSortMenus(id);

                menus = menus.OrderBy(d => d.Importance).ToList();
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

        private async Task<List<QNZ.Data.Menu>> RestSortMenus(int id)
        {
            var menus = await _db.Menus.Where(d => d.CategoryId == id).OrderBy(d => d.Importance).ToListAsync();


            var list = menus.Where(m => m.ParentId == null).OrderBy(m => m.Importance)
                .SelectDeep<QNZ.Data.Menu>(m => m.Menus.OrderBy(g => g.Importance));

            int i = 0;
            foreach (var item in list)
            {
                item.Importance = i;
                _db.Entry(item).State = EntityState.Modified;

                i++;
            }
            await _db.SaveChangesAsync();
            return menus;
        }

        [HttpGet]
        // GET: Roles/Create
        public ActionResult MoveMenu(int id)
        {
            var menu = _db.Menus.Find(id);
            var menus =   _db.Menus.Where(d => d.CategoryId == menu.CategoryId).OrderBy(d => d.Importance).ToList(); ;
            MoveFrontMenuVM vm = new MoveFrontMenuVM
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
        public async System.Threading.Tasks.Task<ActionResult> MoveMenu(int Id, int menuId)
        {

            if (Id > 0 && menuId > 0)
            {
                var parentMenu = await _db.Menus.FindAsync(menuId); 
                var menu = await _db.Menus.FindAsync(Id); 
                menu.ParentId = menuId;
                menu.LayoutLevel = parentMenu.LayoutLevel + 1;

                _db.Entry(menu).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                List<QNZ.Data.Menu> menus = await RestSortMenus(menu.CategoryId);
             //   _menuService.ResetSort(menu.CategoryId);

             //   var menus = _menuService.GetMenusByCategoryId(menu.CategoryId);
                AR.Id = menu.CategoryId;
                AR.Data = RenderPartialViewToString("_MenuList", menus.OrderBy(d=>d.Importance).ToList());

                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            AR.Setfailure("移动菜单失败");
            return Json(AR, JsonRequestBehavior.DenyGet);
        }


        /// <summary>
        /// 向上移动
        /// </summary>
        /// <param name = "id" ></ param >
        /// < returns ></ returns >
        private async Task<int> UpMoveMenuAsync(int id)
        {
            var vMenu = await _db.Menus.FindAsync(id);
            var menuList = await _db.Menus.Where(d => d.CategoryId == vMenu.CategoryId).OrderBy(d => d.Importance).ToListAsync();

            var prevMenu = menuList.Where(m => m.ParentId == vMenu.ParentId &&
                m.Importance < vMenu.Importance).OrderByDescending(m => m.Importance).FirstOrDefault();

            if (prevMenu == null)
            {
                // 已经在第一位
                return 0;
            }
            var num = prevMenu.Importance - vMenu.Importance;
            prevMenu.Importance = prevMenu.Importance - num;
            vMenu.Importance = vMenu.Importance + num;

      
            _db.Entry(prevMenu).State = EntityState.Modified;
            _db.Entry(vMenu).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            
            return 1;
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<int> DownMoveMenuAsync(int id)
        {
            var vMenu = await _db.Menus.FindAsync(id);
            var menuList = await _db.Menus.Where(d => d.CategoryId == vMenu.CategoryId).OrderBy(d => d.Importance).ToListAsync();

            var nextMenu = menuList.Where(m => m.ParentId == vMenu.ParentId &&
                m.Importance > vMenu.Importance).OrderBy(m => m.Importance).FirstOrDefault();


            if (nextMenu == null)
            {
                // 已经在最后一位
                return 0;
            }


            var num = nextMenu.Importance - vMenu.Importance;
            nextMenu.Importance = nextMenu.Importance - num;
            vMenu.Importance = vMenu.Importance + num;

            _db.Entry(nextMenu).State = EntityState.Modified;
            _db.Entry(vMenu).State = EntityState.Modified;

            await _db.SaveChangesAsync();
            // ResetSort(vMenu.CategoryId);
            return 1;
        }

    }
}