using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TZGCMS.Infrastructure.Helper
{
    public static class SystemInfo
    {
        private static readonly HttpContext Context = HttpContext.Current;

        /// <summary>
        /// 用户远程IP
        /// </summary>
        /// <returns></returns>
        public static string GetUserIp()
        {
            return Context.Request.ServerVariables["HTTP_VIA"] == null
                ? Context.Request.UserHostAddress
                : Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        }
        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            var ip = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }

        public static string GetRealIP()
        {
            string ip;
            try
            {
                HttpRequest request = HttpContext.Current.Request;

                if (request.ServerVariables["HTTP_VIA"] != null)
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0].Trim();
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ip;
        }
        public static string GetRealIP1()
        {
            string ip;
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                if (request.ServerVariables["HTTP_VIA"] != null)
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ip;
        }
        //获取代理IPview plaincopy to clipboardprint?
        public static string GetViaIP()
        {
            string viaIp = null;

            try
            {
                HttpRequest request = HttpContext.Current.Request;

                if (request.ServerVariables["HTTP_VIA"] != null)
                {
                    viaIp = request.UserHostAddress;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return viaIp;
        }

        /// <summary>
        /// 外网IP
        /// </summary>
        /// <returns></returns>
        public static string WebIp()
        {
            var tempip = "";
            try
            {
                var wr = WebRequest.Create("http://www.ip138.com/ips138.asp");
                var s = wr.GetResponse().GetResponseStream();
                if (s != null)
                {
                    var sr = new StreamReader(s, Encoding.Default);
                    var all = sr.ReadToEnd(); //读取网站的数据

                    var start = all.IndexOf("您的IP地址是：[", StringComparison.Ordinal) + 9;
                    var end = all.IndexOf("]", start, StringComparison.Ordinal);
                    tempip = all.Substring(start, end - start);
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return tempip;
        }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public static string ServerIp
        {
            get { return Context.Request.ServerVariables["LOCAL_ADDR"]; }
        }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public static string MachineName
        {
            get { return Context.Server.MachineName; }
        }
        /// <summary>
        /// NET框架版本
        /// </summary>
        public static string Version
        {
            get { return Environment.Version.ToString(); }
        }
        /// <summary>
        /// 操作系统
        /// </summary>
        public static string OsVersion
        {
            get { return Environment.OSVersion.ToString(); }
        }
        /// <summary>
        /// IIS环境
        /// </summary>
        public static string IisEnvironment
        {
            get { return Context.Request.ServerVariables["SERVER_SOFTWARE"]; }
        }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public static string ServerPort
        {
            get { return Context.Request.ServerVariables["SERVER_PORT"]; }
        }
        /// <summary>
        /// 目录物理路径
        /// </summary>
        public static string DirectoryPhysicalPath
        {
            get { return Context.Request.ServerVariables["APPL_PHYSICAL_PATH"]; }
        }

    }
}
