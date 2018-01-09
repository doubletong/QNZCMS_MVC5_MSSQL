using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml;
using Newtonsoft.Json;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.WeiXin;
using TZGCMS.WeiXin.Model;

namespace TZGCMS.SiteWeb.Controllers
{

    // GET: WeiXin
    public class WeiXinController : Controller
        {
        //string token = "123456";

        //string appID = "wx965f031c1746f165";
        //string appsecret = "daa338083c32106a72c62d439129c478";

        //[HttpGet]
        //[ActionName("Index")]
        //public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        //{
        //    if (CheckSignature.Check(signature, timestamp, nonce, SettingsManager.WeiXin.Token))
        //    {
        //        return Content(echostr);
        //    }
        //    else
        //    {
        //        return Content("err");
        //    }

        //}
        [HttpGet]
            [ActionName("Index")]
            public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
            {
                if (!CheckSignature(signature, timestamp, nonce, SettingsManager.WeiXin.Token)) return null;
                return !string.IsNullOrEmpty(echostr) ? Content(echostr) : null;

            }

            private bool CheckSignature(string signature, string timestamp, string nonce, string token)
            {
                
                string[] arrTmp = { token, timestamp, nonce };
                Array.Sort(arrTmp);     //字典排序
                var tmpStr = string.Join("", arrTmp);
                tmpStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
                if (tmpStr != null)
                {
                    tmpStr = tmpStr.ToLower();
                    return tmpStr == signature;
                }
                else
                {
                    return false;
                }
            }
            //private void Valid()
            //{
            //    string echoStr = Request.QueryString["echoStr"];
            //    if (CheckSignature())
            //    {
            //        if (!string.IsNullOrEmpty(echoStr))
            //        {
            //            Response.Write(echoStr);
            //            Response.End();
            //        }
            //    }
            //}

            [HttpPost]
            [ActionName("Index")]
            public ActionResult Get(string signature, string timestamp, string nonce)
            {
                StreamReader sr = new StreamReader(Request.InputStream, Encoding.UTF8);
                XmlDocument doc = new XmlDocument();
                doc.Load(sr);
                sr.Close();
                sr.Dispose();

                WxMessage wxMessage = new WxMessage();
                wxMessage.ToUserName = doc.SelectSingleNode("xml").SelectSingleNode("ToUserName").InnerText;
                wxMessage.FromUserName = doc.SelectSingleNode("xml").SelectSingleNode("FromUserName").InnerText;
                wxMessage.MsgType = doc.SelectSingleNode("xml").SelectSingleNode("MsgType").InnerText;
                wxMessage.CreateTime = int.Parse(doc.SelectSingleNode("xml").SelectSingleNode("CreateTime").InnerText);

                Log(wxMessage.ToUserName + "," + wxMessage.FromUserName + "," + wxMessage.MsgType);

                if (wxMessage.MsgType == "event")
                {
                    wxMessage.EventName = doc.SelectSingleNode("xml").SelectSingleNode("Event").InnerText;
                    Log(wxMessage.EventName);
                    if (!string.IsNullOrEmpty(wxMessage.EventName) && wxMessage.EventName == "subscribe")
                    {
                        string content = "您好，欢迎访问garfieldzf8测试公众平台\n";
                        content += "<a href='" + Request.Url.Host + Url.Action("OAuthSnsApiBase") + "'>SnsApiBase</a>\n";
                        content += "<a href='" + Request.Url.Host + Url.Action("OAuthSnsApiUserInfo") + "'>SnsApiUserInfo</a>";
                        content = SendTextMessage(wxMessage, content);
                        Log(content);

                        return Content(content);
                    }
                }


                return Content("");
            }


            


            [HttpPost]
            public async Task<JsonResult> JsapiTicket()
            {
             
                if (System.Web.HttpRuntime.Cache["JsapiTicket"] != null)
                {
                    var oldvm = (JsapiTicketVM) System.Web.HttpRuntime.Cache["JsapiTicket"];
                    if (oldvm.ticket != null && oldvm.expires_in == 7200)
                    {
                        return Json((JsapiTicketVM) System.Web.HttpRuntime.Cache["JsapiTicket"]);
                    }
                }
                var vm = await WeChatHepler.GetJsapiTicketAsync(SettingsManager.WeiXin.AppId, SettingsManager.WeiXin.AppSecret);
                if (vm.ticket != null && vm.expires_in == 7200)
                {
                    System.Web.HttpRuntime.Cache.Insert("JsapiTicket", vm, null, DateTime.Now.AddSeconds(7200), Cache.NoSlidingExpiration);
                }
             
                return Json(vm);
            }


        private string SendTextMessage(WxMessage wxmessage, string content)
            {
                string result = string.Format(Message, wxmessage.FromUserName, wxmessage.ToUserName, DateTime.Now.Ticks, content);
                return result;
            }

          
            /**
             * snsapi_base
             * **/
            public async Task<ActionResult> OAuthSnsApiBase()
            {
                string code = Request.QueryString["code"];
                try
                {
                    if (!string.IsNullOrEmpty(code))
                    {

                        OAuthToken oauthToken = await WeChatHepler.GetAsync<OAuthToken>(
                            $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={SettingsManager.WeiXin.AppId}&secret={SettingsManager.WeiXin.AppSecret}&code={code}&grant_type=authorization_code");

                        string accesstoken = string.Empty;
                        AccessTokenVM tokenVm = await WeChatHepler.GetAccessTokenAsync(SettingsManager.WeiXin.AppId, SettingsManager.WeiXin.AppSecret);

                    if (!string.IsNullOrEmpty(tokenVm?.access_token))
                        {
                            accesstoken = tokenVm.access_token;
                        }

                        if (oauthToken != null && !string.IsNullOrEmpty(oauthToken.openid))
                        {

                            OAuthUserInfo userInfo = await WeChatHepler.GetAsync<OAuthUserInfo>(
                                $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={accesstoken}&openid={oauthToken.openid}&lang=zh_CN");
                            if (userInfo != null)
                            {


                                Log("获取到用户信息nickName:" + userInfo.nickname);
                            // return View(userInfo);

                            //BatchSendMessagesVM message = new BatchSendMessagesVM { Touser = new string[] { userInfo.Openid, "oBjaPuA6nUCz6ufJFtse-3sDYB4I" }, Msgtype = "text", Text = new Message { Content = "测试消息！" } };
                            var message = new SingleSendMessagesVM { ToUser =  userInfo.Openid, MsgType = "text", Text = new Message { Content = "单发测试消息！" } };

                            string json = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.Indented,
                                new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() });

                                Log("post:" + json);
                            HttpContent content = new StringContent(json);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                            var oo = await WeChatHepler.SingleSendMessagesAsync(accesstoken, content);


                            Log("群发消息:" + oo.ErrMsg);

                            ViewData["headImage"] = userInfo.headimgurl;
                            ViewData["openid"] = userInfo.Openid;
                            ViewData["nickName"] = userInfo.nickname;
                            if (userInfo.sex == 0)
                            {
                                ViewData["sex"] = "未知";
                            }
                            else if (userInfo.sex == 1)
                            {
                                ViewData["sex"] = "男";
                            }
                            else
                            {
                                ViewData["sex"] = "女";
                            }
                            ViewData["province"] = userInfo.province;
                            ViewData["city"] = userInfo.city;
                        }
                            else
                            {
                                Log("未获取到用户信息");
                            }
                        }
                        else
                        {
                            Log("access_token:" + oauthToken.access_token + ",openid:" + oauthToken.openid);
                        }



                    }
                    else
                    {
                        return Redirect(
                            $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={SettingsManager.WeiXin.AppId}&redirect_uri={"http://" + Request.Url.Host + Url.Action("OAuthSnsApiBase")}&response_type=code&scope=snsapi_base&state=123456#wechat_redirect");
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    ViewData["errmsg"] = ex.Message;
                }

                return View();
            }


            /**
             * snsapi_userinfo
             * **/
            public async Task<ActionResult> OAuthSnsApiUserInfo()
            {
                string code = Request.QueryString["code"];
                try
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        OAuthToken oauthToken = await WeChatHepler.GetAsync<OAuthToken>(
                            $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={SettingsManager.WeiXin.AppId}&secret={SettingsManager.WeiXin.AppSecret}&code={code}&grant_type=authorization_code");


                        if (!string.IsNullOrEmpty(oauthToken?.openid) && !string.IsNullOrEmpty(oauthToken.access_token))
                        {

                            OAuthUserInfo userInfo = await WeChatHepler.GetAsync<OAuthUserInfo>(
                                $"https://api.weixin.qq.com/sns/userinfo?access_token={oauthToken.access_token}&openid={oauthToken.openid}&lang=zh_CN");
                            if (userInfo != null)
                            {

                                Log("获取到用户信息nickName:" + userInfo.nickname);
                                ViewData["headImage"] = userInfo.headimgurl;
                                ViewData["openid"] = userInfo.Openid;
                                ViewData["nickName"] = userInfo.nickname;
                                if (userInfo.sex == 0)
                                {
                                    ViewData["sex"] = "未知";
                                }
                                else if (userInfo.sex == 1)
                                {
                                    ViewData["sex"] = "男";
                                }
                                else
                                {
                                    ViewData["sex"] = "女";
                                }
                                ViewData["province"] = userInfo.province;
                                ViewData["city"] = userInfo.city;
                            }
                            else
                            {
                                Log("未获取到用户信息");
                            }
                        }
                        else
                        {
                            Log("access_token:" + oauthToken.access_token + ",openid:" + oauthToken.openid);
                        }

                    }
                    else
                    {
                        return Redirect(
                            $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={SettingsManager.WeiXin.AppId}&redirect_uri={Server.UrlEncode("http://" + Request.Url.Host + Url.Action("OAuthSnsApiUserInfo"))}&response_type=code&scope=snsapi_userinfo&state=123456#wechat_redirect");
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    ViewData["errmsg"] = ex.Message;
                }

                return View();
            }


            //被动回复用户消息
            public string Message
            {
                get
                {
                    return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[text]]></MsgType>
                            <Content><![CDATA[{3}]]></Content>
                            </xml>";
                }
            }

            private void Log(string text)
            {
                string str = Server.MapPath("~/Log/") + "log.txt";
                FileStream fs = new FileStream(str, FileMode.Append, FileAccess.Write);
                StreamWriter sr = new StreamWriter(fs);
                sr.WriteLine(DateTime.Now + " : " + text);
                sr.Close();
                fs.Close();
            }

        }

       
    }
