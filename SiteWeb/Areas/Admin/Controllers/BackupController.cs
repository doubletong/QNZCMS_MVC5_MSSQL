using TZGCMS.SiteWeb.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.System;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.Systems;
using log4net;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class BackupController : BaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(BackupController));
        IBackupServices _backupServices;
     
        public BackupController(IBackupServices backupServices,ILoggingService logger)
        {
            _backupServices = backupServices;
           
        }
        // GET: Admin/Backup
        public ActionResult Index()
        {
            var rootPath = Server.MapPath(SettingsManager.Site.DatabaseBackupDir);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            IEnumerable<BakFileVM> vm = GetFileList(rootPath);

            return View(vm);
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="webPath"></param>
        /// <returns></returns>
        private static IEnumerable<BakFileVM> GetFileList(string rootPath)
        {


            return new DirectoryInfo(rootPath).GetFiles()
                .Where(dir => !dir.Name.StartsWith("_")).Select(f => new BakFileVM
                {
                    Name = f.Name,
                    CreatedDate = f.CreationTime,
                    FilePath = Path.Combine(SettingsManager.Site.DatabaseBackupDir, f.Name),
                    FileSize = f.Length / 1024
                }).OrderByDescending(m => m.CreatedDate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Backup()
        {
            try
            {

                string connectString = ConfigurationManager.ConnectionStrings["QNZDbContext"].ToString();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectString);
                // Retrieve the DataSource property.    
                // string IPAddress = builder.DataSource;


                string localPath = Server.MapPath(SettingsManager.Site.DatabaseBackupDir);

                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);

                string _DatabaseName = builder.InitialCatalog;
                string _BackupName = _DatabaseName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".bak";

                string strQuery = "BACKUP DATABASE " + _DatabaseName + " TO DISK = '" + Path.Combine(localPath, _BackupName) + "' WITH FORMAT, MEDIANAME = 'Z_SQLServerBackups', NAME = '" + _BackupName + "';";
                var result = _backupServices.SqlQuery(strQuery);

                if (result.Count() > 0)
                {
                    string mes = "";
                    result.ForEach(x =>
                    {
                        mes += x.ToString();
                    });
                    AR.Setfailure(mes);
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }

                _logger.Info("创建数据库备份：" + _BackupName);

                var rootPath = Server.MapPath(SettingsManager.Site.DatabaseBackupDir);
                IEnumerable<BakFileVM> vm = GetFileList(rootPath);
                AR.Data = RenderPartialViewToString("_FileList", vm);

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);

               
            }
            catch (Exception er)
            {
                _logger.Error("备份数据库", er);

                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

            

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Restore(string filePath)
        {
            try
            {
                string connectString = ConfigurationManager.ConnectionStrings["TZGEntities"].ToString();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectString);
                string _BackupName = Server.MapPath(filePath);

                string _DatabaseName = builder.InitialCatalog;
                //    string _BackupName = _DatabaseName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".bak";

                string strQuery = "USE MASTER; RESTORE DATABASE " + _DatabaseName + " FROM DISK = '" + _BackupName + "'";
                var result = _backupServices.SqlQuery(strQuery);

                if (result.Count() > 0)
                {
                    string mes = "";
                    result.ForEach(x =>
                    {
                        mes += x.ToString();
                    });
                    AR.Setfailure(mes);
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }

                _logger.Info("还原数据库：" + _BackupName);

                AR.SetSuccess(Messages.AlertActionSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            catch (Exception er)
            {
                _logger.Error($"还原数据库文件：{filePath}", er);

                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public FileResult Download(string filePath, string fileName)
        {
            if (filePath.StartsWith(SettingsManager.Site.DatabaseBackupDir))
            {
                var downPath = Server.MapPath(filePath);
                if (System.IO.File.Exists(downPath))
                {
                    return File(downPath, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                
            }
           
            return null;
        }

        [HttpPost]
        public JsonResult Delete(string filePath)
        {
            try
            {
                var downPath = Server.MapPath(filePath);
                System.IO.File.Delete(downPath);
            }
            catch (Exception er)
            {
                _logger.Error($"删除备份文件：{filePath}", er);
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            return Json(AR, JsonRequestBehavior.AllowGet);
        }

    }
}