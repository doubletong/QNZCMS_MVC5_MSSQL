using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TZGCMS.Infrastructure.Captcha;
using TZGCMS.Model.Front.InputModel.Identity;
using TZGCMS.Service.Identity;

namespace TZGCMS.SiteWeb.Controllers
{
   
    public class AccountController : BaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AccountController));
        private readonly IUserServices _userServices;

        public AccountController(IUserServices userServices)
        {
            _userServices = userServices;

        }
        public CaptchaImageResult ShowCaptchaImage()
        {
            return new CaptchaImageResult();
        }

        // GET: /Admin/Account/Login      
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                //防止欺诈跳转（回调地址为空也会判定为false）
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {                   
                    //跳转到默认地址
                    return Redirect("/bbi-admin");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginIM model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/bbi-admin");
            }

            if (ModelState.IsValid)
            {
                if (Session["SigCaptcha"] != null && Session["SigCaptcha"].ToString().ToLower() != model.CaptchaText.ToLower())
                {
                    ModelState.AddModelError(string.Empty, "验证码不正确!");
                    return View(model);
                }

                var user = _userServices.SignIn(model.UserName, model.Password);
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "此帐号已经被停用!");
                        return View(model);
                    }
                    var roles = user.Roles.Select(m => m.RoleName).ToArray();

                    user.LastActivityDate = DateTime.Now;
                    _userServices.UpdateLastActivityDate(user);

                    //设置cookies
                    _userServices.SetUserCookies(model.RememberMe, user, roles);

                    GlobalContext.Properties["user"] = user.UserName;
                    _logger.Info("登录");

                    if (roles.Contains("系统管理员") || roles.Contains("创始人"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (roles.Contains("User"))
                    {
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "无效的用户名或密码！");
            }

            return View(model);
        }

       
       [HttpPost]
        public ActionResult LogOff()
        {
            _logger.Info("注销");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}