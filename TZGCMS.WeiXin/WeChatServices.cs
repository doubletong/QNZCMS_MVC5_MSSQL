using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TZGCMS.WeiXin.Model;

namespace TZGCMS.WeiXin
{
    public interface IWeChatServices
    {
        Task<AccessTokenVM> GetAccessToken(string appid, string appsecret);
        Task<string> BatchSendMessagesAsync(string accessToken, HttpContent content);
        Task<string> SingleSendMessagesAsync(string accessToken, HttpContent content);
        Task<T> GetAsync<T>(string url);

    }
    public class WeChatServices: IWeChatServices
    {
        public async Task<AccessTokenVM> GetAccessToken(string appid, string appsecret)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={appsecret}";
            var result = await GetAsync<AccessTokenVM>(url);
            return result;
        }

        /// <summary>
        /// 批量群发消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> BatchSendMessagesAsync(string accessToken, HttpContent content)
        {

            var url = $"https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token={accessToken}";

            var client = new HttpClient();
            var response = await client.PostAsync(url, content);
            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;

        }
        /// <summary>
        /// 单发消息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public  async Task<string> SingleSendMessagesAsync(string accessToken, HttpContent content)
        {

            var url = $"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={accessToken}";

            var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;

        }

        /// <summary>
        /// 获取Json并转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url)
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