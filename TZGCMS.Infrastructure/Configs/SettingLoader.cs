
using TZGCMS.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace TZGCMS.Infrastructure.Configs
{
    internal sealed class SettingLoader
    {
        /// <summary>
        /// Private constructor
        /// </summary>
        private SettingLoader() { }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadConfig<T>() where T : class
        {
            return LoadConfig<T>(null);
        }

        /// <summary>
        /// Return Settings object from cache or from the xml file  
        /// </summary>
        /// <typeparam name="T">The type we will passing</typeparam>
        /// <param name="fileName"> </param>
        /// <returns></returns>
        ///
        public static T LoadConfig<T>(string fileName) where T : class
        {
            T configObj = null;
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    if (HttpContext.Current != null)
                    {
                        fileName = HttpContext.Current.Server.MapPath(string.Concat("~/Config/", typeof(T).Name, ".config"));
                    }
                    else
                    {
                        //多线程执行这里
                        fileName = string.Concat("/Config/", typeof(T).Name, ".config").Replace("/", "\\");
                        if (fileName.StartsWith("\\"))//确定 String 实例的开头是否与指定的字符串匹配。为下边的合并字符串做准备
                        {
                            fileName = fileName.TrimStart('\\');//从此实例的开始位置移除数组中指定的一组字符的所有匹配项。为下边的合并字符串做准备
                        }
                        //AppDomain表示应用程序域，它是一个应用程序在其中执行的独立环境　　　　　　　
                        //AppDomain.CurrentDomain 获取当前 Thread 的当前应用程序域。
                        //BaseDirectory 获取基目录，它由程序集冲突解决程序用来探测程序集。
                        //AppDomain.CurrentDomain.BaseDirectory综合起来就是返回此代码所在的路径
                        //System.IO.Path.Combine合并两个路径字符串
                        //Path.Combine(@"C:\11","aa.txt") 返回的字符串路径如后： C:\11\aa.txt
                        fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                    }


                  
                }
                string cacheKey = fileName;
                // Load the setting object from HttpRuntime Cache if the Settings.xml file is not change recently after last Chache.
                configObj = HttpRuntime.Cache[cacheKey] as T;
                if (configObj == null)// Try populate the config from cache
                {
                    configObj = LoadFromXml<T>(fileName);
                    // insert the config instance into cache use CacheDependency
                    HttpRuntime.Cache.Insert(cacheKey, configObj, new System.Web.Caching.CacheDependency(fileName));
                }
            }
            catch (Exception ex)
            {
                //write error log
                // ILoggingService _logger = new LoggingService();
                // _logger.Error(LogUtility.BuildExceptionMessage(ex));
                //LoggingFactory.InitializeLogFactory(_logger);               
                LoggingFactory.GetLogger().Fatal(LogUtility.BuildExceptionMessage(ex));
             
                return null;
            }
            return configObj;
        }
        /// <summary>
        /// Load the settings xml file and retun the Settings Type with Deserialize with xml content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">File Name of the custom xml Settings file</param>
        /// <returns>The T type which we have have paased with LoadFromXml<T> </returns>
        private static T LoadFromXml<T>(string fileName) where T : class
        {
            FileStream fs = null;
            try
            {
                //Serialize of the Type
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return (T)serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                //write error log
                //ILoggingService _logger = new LoggingService();
                //_logger.Error(LogUtility.BuildExceptionMessage(ex));
                LoggingFactory.GetLogger().Fatal(LogUtility.BuildExceptionMessage(ex));
             
                return null;
            }
            finally
            {
                fs?.Close();
            }
        }
    }
}
