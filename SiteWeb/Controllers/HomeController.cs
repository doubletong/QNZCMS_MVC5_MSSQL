using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using System.Data.Entity;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Service.PageMetas;
using TZGCMS.SiteWeb.Filters;
using System.Linq;
using TZGCMS.Model.Front;

namespace TZGCMS.SiteWeb.Controllers
{
    public class HomeController : BaseController
    {
       

        [SIGActionFilter]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var carousel = await _db.Carousels.OrderByDescending(d => d.ImageUrl).FirstOrDefaultAsync(d => d.Active && d.Position.Code == "A1001");

            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d=>d.ModelType == ModelType.MENU && d.ObjectId == url);

            var vm = new HomeVM
            {
                Carousel = carousel
            };

            return View(vm);
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
        

        
        //public ActionResult testA()
        //{
        //    return Content(System.Web.HttpContext.Current.User.Identity.Name);
        //}

        //private void SetPrincipal(string openid)
        //{
        //    CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel()
        //    {
        //        UserId = IdentityGenerator.SequentialGuid(),
        //        RealName = "微信用户",
        //        Avatar = SettingsManager.User.DefaultAvatar,
        //        Roles = new string[] { "微信用户" }
        //    };
        //    //serializeModel.Menus = GetUserMenus(user.);
        //    TimeSpan timeout = FormsAuthentication.Timeout;
        //    DateTime expire = DateTime.Now.Add(timeout);


        //    string userData = JsonConvert.SerializeObject(serializeModel, Formatting.Indented,
        //        new JsonSerializerSettings()
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //        });


        //    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
        //        1,
        //        openid,
        //        DateTime.Now,
        //        expire,
        //        true,
        //        userData);

        //    string encTicket = FormsAuthentication.Encrypt(authTicket);
        //    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
        //    System.Web.HttpContext.Current.Response.Cookies.Add(faCookie);
        //}
    }
}