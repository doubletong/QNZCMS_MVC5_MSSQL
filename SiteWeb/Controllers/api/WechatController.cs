using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Http.Description;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Front.ViewModel.Articles;
using TZGCMS.WeiXin;
using TZGCMS.WeiXin.Model;

namespace TZGCMS.SiteWeb.Controllers.api
{
    public class WechatController : ApiController
    {
        /// <summary>
        /// get AccessToken
        /// </summary>
        /// <returns></returns>
        
        [ResponseType(typeof(AccessTokenVM))]
        public async Task<IHttpActionResult> GetAccessTokenAsync()
        {
            //if (System.Web.HttpRuntime.Cache["JsapiTicket"] != null)
            //{
            //    var oldvm = (AccessTokenVM)System.Web.HttpRuntime.Cache["JsapiTicket"];
            //    if (oldvm.access_token != null && oldvm.expires_in == 7200)
            //    {
            //        return Json((AccessTokenVM)System.Web.HttpRuntime.Cache["JsapiTicket"]);
            //    }
            //}
            var vm = await WeChatHepler.GetAccessTokenAsync(SettingsManager.WeiXin.AppId, SettingsManager.WeiXin.AppSecret);
            //if (vm.access_token != null && vm.expires_in == 7200)
            //{
            //    System.Web.HttpRuntime.Cache.Insert("JsapiTicket", vm, null, DateTime.Now.AddSeconds(7200), Cache.NoSlidingExpiration);
            //}
            return Ok(vm);
        }

        public IHttpActionResult GetConfig()
        {
            return Ok(new{appId=SettingsManager.WeiXin.AppId ,appSecret = SettingsManager.WeiXin.AppSecret });
        }

    }
}
