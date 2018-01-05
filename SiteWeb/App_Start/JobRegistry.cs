using FluentScheduler;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using Newtonsoft.Json;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Service.Systems;
using TZGCMS.Service.Videos;
using TZGCMS.WeiXin;
using TZGCMS.WeiXin.Model;

namespace SiteWeb.App_Start
{
    public class JobRegistry : Registry
    {
        public JobRegistry()
        {
            // 立即执行，每2秒执行一次
            // Schedule<IJob>().ToRunNow().AndEvery(2).Seconds(); 通知任务定时
            // Schedule<JobWeiXinNotice>().ToRunNow().AndEvery(SettingsManager.Video.Timer).Minutes();

            // 延迟一个指定时间间隔执行一次计划任务。（当然，这个间隔依然可以是秒、分、时、天、月、年等。）
            // Schedule<JobWeiXinNotice>().ToRunOnceIn(SettingsManager.Video.Timer).Minutes(); 

            // 在一个指定时间执行计划任务（最常用。这里是在每天的晚上 9:15 分执行）
            // Schedule(() => Console.WriteLine("It's 9:15 PM now.")).ToRunEvery(1).Days().At(21, 15);
            Schedule<BackupDatabase>().ToRunEvery(1).Weeks().On(DayOfWeek.Monday).At(0,0);

            // 立即执行一个在每月的星期一 3:00 的计划任务（可以看出来这个一个比较复杂点的时间，它意思是它也能做到！）
            // Schedule<MyComplexJob>().ToRunNow().AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(3, 0);

            // Schedule a job using a factory method and pass parameters to the constructor.
            // Schedule(() => new MyComplexJob("Foo", DateTime.Now)).ToRunNow().AndEvery(2).Seconds();

            // 在同一个计划中执行两个（多个）任务
            // Schedule<MyJob>().AndThen<MyOtherJob>().ToRunNow().AndEvery(5).Minutes();

        }
    }
    /// <summary>
    /// 微信通知任务
    /// </summary>
    public class JobWeiXinNotice : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;

        public JobWeiXinNotice()
        {
            // Register this job with the hosting environment.
            // Allows for a more graceful stop of the job, in the case of IIS shutting down.
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
           // Trace.WriteLine("现在时间是：" + DateTime.Now);
            lock (_lock)
            {
                if (_shuttingDown)
                    return;

                ILoggingService _logger = new LoggingService();
                try
                {
                    VideoServices videoServices = new VideoServices();
                    ReservationServices reservationServices = new ReservationServices();
                    CacheService cacheServices = new CacheService();
                   
                    var videos = videoServices.GetNoticedElementsAsync().Result;

                    //_logger.Info("消息发送处理视频：" + videos.Count() + "条");
                    foreach (var video in videos)
                    {
                        var res = video.Reservations;
                        //_logger.Info("Reservations.Any：" + video.Reservations.Any());
                        if (!video.Reservations.Any()) continue;

                        string accesstoken;
                        if (cacheServices.IsSet("access_token"))
                        {
                            accesstoken = (string)cacheServices.Get("access_token");
                        }
                        else
                        {
                            var atvm = WeChatHepler.GetAccessTokenAsync(SettingsManager.WeiXin.AppId, SettingsManager.WeiXin.AppSecret).Result;
                            accesstoken = atvm.access_token;
                            cacheServices.Set("access_token", accesstoken, 120);
                       

                        }
                        //_logger.Info("accesstoken：" + accesstoken);

                        var msg = $"您预约的直播视频[{video.Title}]即将在{video.StartDate:F}直播，At@{DateTime.Now}";
                        _logger.Info("消息内容：" + msg);
                        var openIds = res.Select(d => d.OpenId).ToArray();
                    
                        if (openIds.Length == 1)
                        {
                          //  _logger.Info("accesstoken：单发");
                            var message = new SingleSendMessagesVM { ToUser = openIds[0], MsgType = "text", Text = new Message { Content = msg } };

                            var json = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.Indented,
                                new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() });

                            //Log("post:" + json);
                            HttpContent content = new StringContent(json);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var result = WeChatHepler.SingleSendMessagesAsync(accesstoken, content).Result;
                            if (result.ErrCode == 0)
                            {
                                foreach (var r in res)
                                {
                                    r.NoticedDate = DateTime.Now;
                                }
                                videoServices.Update(video);
                            }

                            _logger.Info($"单发结果[errcode:{result.ErrCode},errmsg:{result.ErrMsg}]；消息主体：{json}" );
                        }
                        else
                        {
                           // _logger.Info("accesstoken：群发");
                            var message = new BatchSendMessagesVM { ToUser = openIds, MsgType = "text", Text = new Message { Content = msg } };
                            var json = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.Indented,
                                new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() });

                            //Log("post:" + json);
                            HttpContent content = new StringContent(json);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var result = WeChatHepler.BatchSendMessagesAsync(accesstoken, content).Result;
                            if (result.ErrCode == 0)
                            {
                                foreach (var r in res)
                                {
                                    r.NoticedDate = DateTime.Now;
                                }
                                videoServices.Update(video);
                            }
                            _logger.Info($"群发结果[errcode:{result.ErrCode},errmsg:{result.ErrMsg}]；消息主体：{json}");
                          
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Fatal("发送失败：" + ex.Message);
                }
                
            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }

    /// <summary>
    /// 数据库定时备份
    /// </summary>
    public class BackupDatabase : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;

        public BackupDatabase()
        {
            // Register this job with the hosting environment.
            // Allows for a more graceful stop of the job, in the case of IIS shutting down.
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            // Trace.WriteLine("现在时间是：" + DateTime.Now);
            lock (_lock)
            {
                if (_shuttingDown)
                    return;

                ILoggingService _logger = new LoggingService();
                try
                {
                    IBackupServices _backupServices = new BackupServices();

                    var connectString = ConfigurationManager.ConnectionStrings["TZGEntities"].ToString();
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectString);
                    // Retrieve the DataSource property.    
                    // string IPAddress = builder.DataSource;

                    var localPath = SettingsManager.Site.DatabaseBackupDir;
                    localPath = localPath.Replace("/", "\\");
                    if (localPath.StartsWith("\\"))//确定 String 实例的开头是否与指定的字符串匹配。为下边的合并字符串做准备
                    {
                        localPath = localPath.TrimStart('\\');//从此实例的开始位置移除数组中指定的一组字符的所有匹配项。为下边的合并字符串做准备
                    }
                    //AppDomain表示应用程序域，它是一个应用程序在其中执行的独立环境　　　　　　　
                    //AppDomain.CurrentDomain 获取当前 Thread 的当前应用程序域。
                    //BaseDirectory 获取基目录，它由程序集冲突解决程序用来探测程序集。
                    //AppDomain.CurrentDomain.BaseDirectory综合起来就是返回此代码所在的路径
                    //System.IO.Path.Combine合并两个路径字符串
                    //Path.Combine(@"C:\11","aa.txt") 返回的字符串路径如后： C:\11\aa.txt
                    localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localPath);

                   // string localPath = HttpContext.Current.Server.MapPath(SettingsManager.Site.DatabaseBackupDir);

                    if (!Directory.Exists(localPath))
                        Directory.CreateDirectory(localPath);

                    string _DatabaseName = builder.InitialCatalog;
                    string _BackupName = _DatabaseName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".bak";

                    string strQuery = "BACKUP DATABASE " + _DatabaseName + " TO DISK = '" + Path.Combine(localPath, _BackupName) + "' WITH FORMAT, MEDIANAME = 'Z_SQLServerBackups', NAME = '" + _BackupName + "';";
                    var result = _backupServices.SqlQuery(strQuery);

                    if (result.Any())
                    {
                        string mes = "";
                        result.ForEach(x =>
                        {
                            mes += x.ToString();
                        });
                        _logger.Error("备份失败：" + mes);
                    
                    }

                    _logger.Info("创建数据库备份：" + _BackupName);
                    
                }
                catch (Exception er)
                {
                    _logger.Fatal(er.Message);
                }



            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }
}