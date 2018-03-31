using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Service.PageMetas;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILoggingService _logger;
        private readonly IPageMetaServices _pageMetaService;
        public HomeController(IPageMetaServices pageMetaService,ILoggingService logger)
        {
            _pageMetaService = pageMetaService;
            _logger = logger;
        }

        [SIGActionFilter]
        public ActionResult Index()
        {
            var url = Request.RawUrl;
            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);

            return View();
        }



        //private void Log(string text)
        //{
        //    string str = Server.MapPath("~/Log/") + "log.txt";
        //    FileStream fs = new FileStream(str, FileMode.Append, FileAccess.Write);
        //    StreamWriter sr = new StreamWriter(fs);
        //    sr.WriteLine(DateTime.Now + " : " + text);
        //    sr.Close();
        //    fs.Close();
        //}


        public ActionResult CloseSite()
        {           
            return View();
        }
        

        
        public ActionResult testA()
        {
            return Content(System.Web.HttpContext.Current.User.Identity.Name);
        }

        private void SetPrincipal(string openid)
        {
            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel()
            {
                UserId = IdentityGenerator.SequentialGuid(),
                RealName = "微信用户",
                Avatar = SettingsManager.User.DefaultAvatar,
                Roles = new string[] { "微信用户" }
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
                openid,
                DateTime.Now,
                expire,
                true,
                userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(faCookie);
        }
    }
}