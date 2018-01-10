using AutoMapper;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Identity;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Identity;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class RoleController : BaseController
    {
        private readonly IRoleServices _roleServices;
        private readonly IMenuServices _menuServices;

        private readonly IMapper _mapper;
        public RoleController(IRoleServices roleServices, IMenuServices menuServices,IMapper mapper)
        {
            _roleServices = roleServices;
            _menuServices = menuServices;
            _mapper = mapper;
        }

        // GET: Role
        public ActionResult Index()
        {
            var roles = _roleServices.GetAll().Where(r => r.Id != SettingsManager.Role.Founder);
            return View(roles);
        }

        [AllowAnonymous]
        public PartialViewResult List()
        {
            var roles = _roleServices.GetAll().Where(r => r.Id != SettingsManager.Role.Founder);
            return PartialView("_List", roles);
        }



        [HttpGet]
        // GET: Roles/Create
        public ActionResult SetRoleMenus(int id)
        {
            var role = _roleServices.GetById(id);
            var menus = _menuServices.GetMenusByCategoryId(SettingsManager.Menu.BackMenuCId);            
            int[] menuIds = role.Menus.Select(m => m.Id).ToArray();

            SetRoleMenusVM vm = new SetRoleMenusVM
            {
                RoleId = id,
                Menus = menus,
                MenuIds = menuIds
            };

            return PartialView("_SetRoleMenus", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetRoleMenus(int RoleId, int[] menuId)
        {

            if (RoleId > 0)
            {
                _roleServices.SetRoleMenus(RoleId, menuId);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure("编辑角色权限失败");
            return Json(AR, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult EditRole(int? id)
        {
            var role = (id > 0) ? _roleServices.GetById(id.Value) : new Role();
            return PartialView("_EditRole", role);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole([Bind(Include = "Id,RoleName,Description")] Role role)
        {

            if (ModelState.IsValid)
            {
                if (role.Id > 0)
                {
                    Role vRole = _roleServices.GetById(role.Id);
                    if (!vRole.IsSys)
                    {
                        vRole.RoleName = role.RoleName;
                        vRole.Description = role.Description;
                        _roleServices.Update(vRole);
                    }
                    else
                    {
                        //  return new HttpStatusCodeResult(500, "系统角色不可编辑");
                        AR.SetWarning("系统角色不可编辑");
                        return Json(AR, JsonRequestBehavior.DenyGet);
                    }

                }
                else
                {
                    _roleServices.Create(role);
                }

                var roles = _roleServices.GetAll().Where(r => r.Id != SettingsManager.Role.Founder);
                AR.Data = RenderPartialViewToString("_List", roles);
                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Menu));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            else
            {
                //return new HttpStatusCodeResult(500, "编辑角色失败");

                AR.Setfailure("编辑角色失败");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }



        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {

            var role = _roleServices.GetById(id);
            if (role.IsSys)
            {
                AR.SetWarning("系统角色，不可以删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            if (role == null)
            {
                AR.Setfailure("未找到此角色！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            _roleServices.Delete(role);
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

    }
}