using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TZGCMS.WeiXin.Model;

namespace TZGCMS.WeiXin
{
    
    public class WeChatHepler
    {
        public static async Task<AccessTokenVM> GetAccessTokenAsync(string appid,string appsecret)
        {
          
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={appsecret}";
            var result = await GetAsync<AccessTokenVM>(url);
            return result;
        }

        public static async Task<JsapiTicketVM> GetJsapiTicketAsync(string appid, string appsecret)
        {
            var tk = await GetAccessTokenAsync(appid, appsecret);
            string url = $"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={tk.access_token}&type=jsapi";
            var result = await GetAsync<JsapiTicketVM>(url);

            return result;
        }


        /// <summary>
        /// 批量群发消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<MassSendErrorMsg> BatchSendMessagesAsync(string accessToken, HttpContent content)
        {
          
            var url = $"https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token={accessToken}";

            var client = new HttpClient();
            var response = await client.PostAsync(url, content);
            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MassSendErrorMsg>(responseBody);
            return result;

        }
        /// <summary>
        /// 单发消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<ErrorMsg> SingleSendMessagesAsync(string accessToken, HttpContent content)
        {

            var url = $"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={accessToken}";

            var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ErrorMsg>(responseBody);
            return result;

        }

        /// <summary>
        /// 获取Json并转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string url)
        {

            var client = new HttpClient();
            var response = await client.GetAsync(url);

            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseBody);

        }
    }
}
