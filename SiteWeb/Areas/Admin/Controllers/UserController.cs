using AutoMapper;
using Newtonsoft.Json;
using PagedList;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.InputModel.Identity;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Identity;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Identity;


namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class UserController : BaseController
    {
        private new QNZ.Data.IQNZDbContext _db;
        private readonly IUserServices _userServices;
        private readonly IRoleServices _roleServices;
        private readonly IMapper _mapper;

        public UserController(IUserServices userServices, IRoleServices roleServices , IMapper mapper, QNZ.Data.IQNZDbContext db)
        {
            _userServices = userServices;
            _roleServices = roleServices;
            _mapper = mapper;
            _db = db;
        }

        // GET: User
        // [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index(int? page, string keyword, DateTime? startDate, DateTime? endDate, int? roleId)
        {
            var userListVM = new UserListVM
            {
                StartDate = startDate,
                EndDate = endDate,
                Keyword = keyword,
                RoleId = roleId,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.User.PageSize,
                SetPasswordIM = new SetPasswordIM()
            };

            int totalCount;

            
            var users = _userServices.GetPagedElements(userListVM.PageIndex-1, userListVM.PageSize, userListVM.Keyword,
                userListVM.StartDate, userListVM.EndDate, userListVM.RoleId, out totalCount);

            userListVM.TotalCount = totalCount;

            // var userList = _mapper.Map<IList<User>, IList<UserVM>>(users);

            userListVM.Users = new StaticPagedList<User>(users, userListVM.PageIndex, userListVM.PageSize, userListVM.TotalCount);
            // ViewBag.OnePageOfUsers = usersAsIPagedList;
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            var roleList = _roleServices.GetAll().Where(m => m.Id != SettingsManager.Role.Founder).ToList();
            var roles = new SelectList(roleList, "Id", "RoleName");
            ViewBag.Roles = roles;

            return View(userListVM);

        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/UserSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("PageSize").SetValue(pageSize);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        // GET: User/Details/5
        [HttpGet]
        public ActionResult Details(Guid id)
        {

            var user = _userServices.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Detail", user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            RegisterIM vm = new RegisterIM();
            return PartialView("_UserCreate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(RegisterIM model)
        {
            if (!ModelState.IsValid)
            {
                var errorMes = GetModelErrorMessage();
                AR.Setfailure(errorMes);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


            var result = _userServices.Register(model.UserName, model.Email, model.Password, model.DisplayName);
            if (result == 1)
            {
                AR.Setfailure(Messages.CannotRegisterEmail);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            if (result == 2)
            {
                AR.Setfailure(Messages.CannotRegisterUserName);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


            int count;
            int pageSize = SettingsManager.User.PageSize;
            var list = _userServices.GetPagedElements(0, pageSize,  string.Empty, null, null, null, out count);
            //    List<UserVM> userList = _mapper.Map<List<User>, List<UserVM>>(list);
            AR.Data = RenderPartialViewToString("_UserList", list);

            AR.SetSuccess(string.Format(Messages.AlertCreateSuccess, EntityNames.User));
            return Json(AR, JsonRequestBehavior.DenyGet);



        }
        [AllowAnonymous]

        public JsonResult IsUserNameUnique(string UserName)
        {
            var result = _userServices.IsExistUserName(UserName);

            return result
                ? Json(false, JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult IsEmailUnique(string Email)
        {
            var result = _userServices.IsExistEmail(Email);

            return result
                ? Json(false, JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(Guid? id)
        {
            ProfileIM Profiles = new ProfileIM();
            if (id == null)
            {
                return PartialView("_UserEdit", Profiles);
            }
            var user = _userServices.GetById(id.Value);
            if (user == null)
            {
                return HttpNotFound();
            }

            Profiles = _mapper.Map<ProfileIM>(user);
            return PartialView("_UserEdit", Profiles);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "Id,Email,RealName,Birthday,IsActive,Gender,QQ,Mobile")] ProfileIM profile)
        {
            if (!ModelState.IsValid)
            {
                var errorMes = GetModelErrorMessage();
                AR.Setfailure(errorMes);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var user = _userServices.GetById(profile.Id);
            if (user == null)
            {
                AR.Setfailure("不存在此用户！");
                return Json(AR, JsonRequestBehavior.DenyGet);
                // return Json(false, JsonRequestBehavior.DenyGet);
            }
            try
            {
                user.Email = profile.Email;
                user.RealName = profile.RealName;
                user.IsActive = profile.IsActive;
                //user.DepartmentId = profile.DepartmentId;
                //user.PositionId = profile.PositionId;
                user.QQ = profile.QQ;
                user.Mobile = profile.Mobile;
                user.Gender = profile.Gender;
                user.Birthday = profile.Birthday;

                _userServices.Update(user);

                // var userVM = _mapper.Map<UserVM>(user);

                AR.Id = user.Id;
                AR.Data = RenderPartialViewToString("_UserItem", user);


                AR.SetSuccess(string.Format(Messages.AlertUpdateSuccess, EntityNames.User));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }
        [AllowAnonymous]
        public JsonResult IsEmailUniqueAtEdit(string email, Guid id)
        {
            var result = _userServices.IsExistEmail(email, id);

            return result
                ? Json(false, JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public PartialViewResult SetRole(Guid id)
        {
            var user = _userServices.GetById(id);
            var roles = user.Roles;
            SetUserRolesVM vm = new SetUserRolesVM
            {
                UserId = id,
                RoleIds = roles.Select(r => r.Id).ToArray(),
                Roles = _roleServices.GetAll().Where(r => r.Id != SettingsManager.Role.Founder)
            };

            return PartialView("_SetRole", vm);
        }


        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> SetRole(Guid UserId, int[] RoleId)
        {
            try
            {
                // _userServices.SetUserRoles(UserId, RoleId);

                //var user = _userServices.GetById(UserId);
                //var roles = _roleServices.GetAll().Where(r => RoleId.Contains(r.Id)).ToList();

                //user.Roles.Clear();
                //foreach (Role r in roles)
                //{
                //    user.Roles.Add(r);
                //}

                //_userServices.Update(user);
                // var user = _userServices.SetRole(UserId, RoleId);
                var user = await  _db.Users.Include(d => d.Roles).FirstOrDefaultAsync();
                var roles = await _db.Roles.Where(r => RoleId.Contains(r.Id)).ToListAsync();

                var founder = await _db.Roles.FindAsync(SettingsManager.Role.Founder);

                if (user.Roles.Any(d=>d.Id == SettingsManager.Role.Founder))
                {
                    AddRolesToUser(user, roles);
                    user.Roles.Add(founder);
                }
                else
                {
                    AddRolesToUser(user, roles);
                }

                //  var roles = _unitOfWork.RoleRepository.GetAll().Where(r => RoleId.Contains(r.Id)).ToList();

                //user.Roles.Clear();
                //foreach (Role r in roles)
                //{
                //    user.Roles.Add(r);
                //}

                //_unitOfWork.UserRepository.Update(user);

                _db.Entry(user).State = EntityState.Modified;
                await _db.SaveChangesAsync();


                if (User.UserId == UserId)
                {
                    SetUserCookies(true, user);
                }


                //    var userVM = _mapper.Map<UserVM>(user);
                AR.Id = user.Id;
                AR.Data = RenderPartialViewToString("_UserItem", user);

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        private static void AddRolesToUser(QNZ.Data.User user, List<QNZ.Data.Role> roles)
        {
            user.Roles.Clear();
            foreach (QNZ.Data.Role r in roles)
            {
                user.Roles.Add(r);
            }
        }

        public static void SetUserCookies(bool isPersist, QNZ.Data.User user)
        {
            var roles = user.Roles.Select(m => m.RoleName).ToArray();

            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel()
            {
                UserId = user.Id,
                RealName = user.RealName,
                Avatar = string.IsNullOrEmpty(user.PhotoUrl) ? SettingsManager.User.DefaultAvatar : user.PhotoUrl,
                Roles = roles
            };
            //serializeModel.Menus = GetUserMenus(user.);
            TimeSpan timeout = FormsAuthentication.Timeout;
            DateTime expire = DateTime.Now.Add(timeout);


            string userData = JsonConvert.SerializeObject(serializeModel, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });


            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                     1,
                     user.UserName,
                     DateTime.Now,
                     expire,
                     isPersist,
                     userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(faCookie);
        }


        [HttpGet]
        public ActionResult SetPassword(Guid id)
        {
            SetPasswordIM model = new SetPasswordIM
            {
                UserId = id
            };
            return PartialView("_SetPassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetPassword(SetPasswordIM model)
        {
            if (!ModelState.IsValid)
            {
                var errorMes = GetModelErrorMessage();
                AR.Setfailure(errorMes);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


            var result = _userServices.SetPassword(model.UserId, model.NewPassword);
            if (result)
            {

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(Messages.AlertActionFailure);
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        public JsonResult IsActive(Guid id)
        {
            var user = _userServices.GetById(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _userServices.Update(user);

                //    var userVM = _mapper.Map<UserVM>(user);

                AR.Id = user.Id;
                AR.Data = RenderPartialViewToString("_UserItem", user);

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            AR.Setfailure(Messages.AlertActionFailure);
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(Guid id)
        {
            var user = _userServices.GetById(id);

            if (user == null)
            {
                AR.Setfailure("未找到此用户！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            if (user.Id.ToString() == SettingsManager.User.Founder)
            {       
                AR.SetWarning("创始人帐号，不可以删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _userServices.Delete(user);
            return Json(AR, JsonRequestBehavior.DenyGet);
        }


    }
}