using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using TZGCMS.Data.Entity.Identity;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.WeiXin;
using TZGCMS.WeiXin.Model;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Service.Systems;

namespace SiteWeb.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILoggingService _logger;
        public HomeController(ILoggingService logger)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {
            string userAgent = Request.UserAgent;
            if (!userAgent.ToLower().Contains("micromessenger"))
            {
                Response.Write("请在微信中访问本页。");
            }
            //if (User!=null && User.Identity.IsAuthenticated && User.IsInRole("微信用户"))
            //{
            //    return Redirect($"{SettingsManager.Site.SiteDomainName}/App/#/");
            //}

            //string code = Request.QueryString["code"];
            //try
            //{
            //    if (!string.IsNullOrEmpty(code))
            //    {
            //        //_logger.Info("获取code：" + code);
            //        var oauthToken = await WeChatHepler.GetAsync<OAuthToken>(
            //            $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={SettingsManager.WeiXin.AppId}&secret={SettingsManager.WeiXin.AppSecret}&code={code}&grant_type=authorization_code");



            //        if (!string.IsNullOrEmpty(oauthToken?.openid))
            //        {
            //            SetPrincipal(oauthToken.openid);
            //            _logger.Info("获取openid s：" + oauthToken.openid);

            //           // System.Web.HttpContext.Current.Response.Redirect($"{SettingsManager.Site.SiteDomainName}/App/#/");

            //            return Redirect($"{SettingsManager.Site.SiteDomainName}/App/#/");

            //        }
            //        else
            //        {
            //            _logger.Error("获取openid失败");
            //        }
            //        return Redirect(
            //            $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={SettingsManager.WeiXin.AppId}&redirect_uri={SettingsManager.Site.SiteDomainName}&response_type=code&scope=snsapi_base&state=123456#wechat_redirect");
            //    }
            //    else
            //    {
            //        return Redirect(
            //            $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={SettingsManager.WeiXin.AppId}&redirect_uri={SettingsManager.Site.SiteDomainName}&response_type=code&scope=snsapi_base&state=123456#wechat_redirect");
            //    }


            //}
            //catch (Exception ex)
            //{
            //    _logger.Fatal(ex);
            //    return Redirect($"{SettingsManager.Site.SiteDomainName}/App/#/");;
            //}
            return Redirect($"{SettingsManager.Site.SiteDomainName}/App/#/");

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


        public ActionResult About()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("About.mobile");
            }
            return View();
        }

    
        public ActionResult Celebs()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return View("Celebs.mobile");
            }
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