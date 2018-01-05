using System;
using System.Linq;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Captcha;
using TZGCMS.Service.Identity;

namespace SiteWeb.Areas.Admin.Controllers
{

    public class AccountController : BaseController
    {
        // GET: Admin/Account
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}