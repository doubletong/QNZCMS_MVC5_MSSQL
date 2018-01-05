using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Extensions;
using TZGCMS.Infrastructure.Helper;

namespace TZGCMS.Service.Identity
{

    public interface IUserServices
    {
        bool SetPassword(Guid userId, string password);
        bool IsExistEmail(string email);
        bool IsExistEmail(string email, Guid id);
        bool IsExistUserName(string userName);
        User GetById(Guid id);
        User SignIn(string username, string password);
        void UpdateLastActivityDate(User model);
        List<Menu> GetUserMenus(Guid userId);
        void SetUserCookies(bool isPersist, User user,string[] roles);
        List<User> GetPagedElements(int pageIndex, int pageSize, string keyword, DateTime? startDate, DateTime? endDate, int? roleId, out int totalCount);
        int Register(string userName, string email, string password, string realName);


        bool Update(User user);

        bool Delete(User user);
        User SetRole(Guid UserId, int[] RoleId);
    }

    public class UserServices: IUserServices
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        
        /// <summary>
        /// 重设密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool SetPassword(Guid userId, string password)

        {
            var user = GetById(userId);
            //try
            //{

                var securityStamp = Hash.GenerateSalt();
                var pwdHash = Hash.HashPasswordWithSalt(password, securityStamp);

                user.SecurityStamp = Convert.ToBase64String(securityStamp);
                user.PasswordHash = pwdHash;
                this.Update(user);
                //log 

                //_logger.Info(string.Format(Logs.RestPwdMessage, user.EntityName, user.UserName));
                return true;
            //}
            //catch (Exception er)
            //{
            //    var message = String.Format(Logs.ErrorRestPwdMessage, user.EntityName);
            //    _logger.Error(message, er);
            //    return false;
            //}



        }
        public bool IsExistEmail(string email)
        {
            int result = _unitOfWork.UserRepository.CountInt(d => d.Email == email);
            return result > 0;
        }

        public bool IsExistEmail(string email, Guid id)
        {
            int result = _unitOfWork.UserRepository.CountInt(d => d.Email == email && d.Id != id);
            return result > 0;
        }
        public bool IsExistUserName(string userName)
        {
            int result =_unitOfWork.UserRepository.CountInt(d => d.UserName == userName);           
            return result > 0;
        }
        public User GetById(Guid id)
        {
            return _unitOfWork.UserRepository.GetById(id);
        }
        public bool Create(User user)
        {
            return _unitOfWork.UserRepository.Insert(user);
        }
        public bool Update(User user)
        {
            return _unitOfWork.UserRepository.Update(user);
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="realName"></param>
        /// <returns></returns>
        public int Register(string userName, string email, string password, string realName)
        {
            var orgUsers = _unitOfWork.UserRepository.GetMany(u => u.Email == email);
            if (orgUsers.Count() > 0)
            {
                return 1; //1 邮箱已存在
            }

            orgUsers = _unitOfWork.UserRepository.GetMany(u => u.UserName == userName);
            if (orgUsers.Count() > 0)
            {
                return 2; //1 用户名已存在
            }


            var securityStamp = Hash.GenerateSalt();
            var passwordHash = Hash.HashPasswordWithSalt(password, securityStamp);

            var newUser = new User()
            {
                UserName = userName,
                RealName = realName,
                Email = email,
                SecurityStamp = Convert.ToBase64String(securityStamp),
                PasswordHash = passwordHash,
                CreateDate = DateTime.Now,
                IsActive = true
            };

            //_logger.Info(string.Format(Logs.CreateMessage, newUser.EntityName, userName));
            Create(newUser);

            // SetUserCookies(false, newUser);

            return 0;
        }
        public User SignIn(string username, string password)
        {
            User user;
         
                user = _unitOfWork.UserRepository.Get(m=>m.UserName == username);
                if (user != null)
                {
                    byte[] salt = Convert.FromBase64String(user.SecurityStamp);
                    string pwdHash = Hash.HashPasswordWithSalt(password, salt);
                    if (user.PasswordHash == pwdHash)
                    {
                        //  _logger.Info(string.Format(Logs.UserLoginMessage, user.UserName));
                        return user;
                    }
                    else
                        return null;
                }
                // result = _connection.Get<Album>(id);
            
            return null;
        }


        public void UpdateLastActivityDate(User model)
        {
            model.LastActivityDate = DateTime.Now;
            _unitOfWork.UserRepository.Update(model);  
        }


        public List<Menu> GetUserMenus(Guid userId)
        {
            var roles = _unitOfWork.UserRepository.GetById(userId).Roles; 
                                                              
            List<Menu> menus = new List<Menu>();
            foreach (var item in roles)
            {
                menus.AddRange(item.Menus);
            }
            var result = menus.Distinct().ToList();

            return result;
           
        }

        public User SetRole(Guid UserId, int[] RoleId)
        {
            var user = _unitOfWork.UserRepository.GetById(UserId);
            var roles = _unitOfWork.RoleRepository.GetAll().Where(r => RoleId.Contains(r.Id)).ToList();

            user.Roles.Clear();
            foreach (Role r in roles)
            {
                user.Roles.Add(r);
            }

            _unitOfWork.UserRepository.Update(user);
            return user;
        }

        public void SetUserCookies(bool isPersist, User user, string[] roles)
        {
           // var roles = user.Roles.Select(m => m.RoleName).ToArray();

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
            HttpContext.Current.Response.Cookies.Add(faCookie);
        }


        public List<User> GetPagedElements(int pageIndex, int pageSize, string keyword, DateTime? startDate, DateTime? endDate,
            int? roleId, out int totalCount)
        {

            //get list count
          

            var totalIQuery = _unitOfWork.UserRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                totalIQuery = totalIQuery.Where(g => g.UserName.Contains(keyword));
            if (startDate != null)
                totalIQuery = totalIQuery.Where(m => m.CreateDate >= startDate);
            if (endDate != null)
                totalIQuery = totalIQuery.Where(m => m.CreateDate <= endDate);
            if (roleId > 0)
                totalIQuery = totalIQuery.Where(g => g.Roles.Any(m => m.Id == (int)roleId));

            totalCount = totalIQuery.Count();
         


            //get list


            List<User> users;
            Expression<Func<User, bool>> filter = g => true;
            Expression<Func<User, bool>> filterByKeyword = g => g.UserName.Contains(keyword);
            Expression<Func<User, bool>> filterByStartDate = g => g.CreateDate >= startDate;
            Expression<Func<User, bool>> filterByEndDate = m => m.CreateDate <= endDate;
            Expression<Func<User, bool>> filterByRoleId = m => m.Roles.Any(r => r.Id == roleId);

            if (!string.IsNullOrEmpty(keyword))
                filter = filter.AndAlso(filterByKeyword);
            if (startDate != null)
                filter = filter.AndAlso(filterByStartDate);
            if (endDate != null)
                filter = filter.AndAlso(filterByEndDate);
            if (roleId > 0)
                filter = filter.AndAlso(filterByRoleId);


            users = _unitOfWork.UserRepository.GetPagedElements(pageIndex, pageSize, (c => c.CreateDate), filter, false, g => g.Roles).ToList();          

            return users;

        }

        public bool Delete(User user)
        {
            return _unitOfWork.UserRepository.Delete(user);
        }
    }

}
