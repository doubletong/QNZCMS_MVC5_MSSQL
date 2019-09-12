using TZGCMS.SiteWeb.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class FileController : BaseController
    {
        // GET: Admin/File
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public PartialViewResult PopuFinder()
        {
            return PartialView("_PopuFinder");
        }
        [AllowAnonymous]
        public ActionResult FinderForTinyMCE()
        {
            return View();
        }

        public ActionResult FinderForMultipleImages()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult FinderForCkeditor()
        {
            return View();
        }
        [AllowAnonymous]
        public PartialViewResult SigFinder()
        {
            return PartialView("_SigFinder");
        }

        [AllowAnonymous]
        [HttpPost]  //CKEditor UploadImage
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string url; // url to return
            string message; // message to display (optional)

            string ImageName = Path.GetFileName(upload.FileName);
            var ext = Path.GetExtension(upload.FileName);

            var uploadDir = Server.MapPath(SettingsManager.File.RootDirectory + "/Images");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var filePath = SettingsManager.File.RootDirectory + "/Images/" + ImageName;
            string Physicalpath = Server.MapPath(filePath);
            if (System.IO.File.Exists(Physicalpath))
            {
                //Physicalpath.LastIndexOf('.');
                var newPath = filePath.Replace(ext, DateTime.Now.ToFileTimeUtc().ToString() + ext);
                Physicalpath = Server.MapPath(newPath);
                upload.SaveAs(Physicalpath);
                url = newPath;
            }
            else
            {
                upload.SaveAs(Physicalpath);
                url = filePath;
            }

            message = "文件已上传成功！";

            // since it is an ajax request it requires this string
            string output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\", \"" + message + "\");</script></body></html>";
            return Content(output);
        }

        // webuploader upload file

        [HttpPost]
        public ActionResult UpLoadProcess(string id, string name, string path, string type, string lastModifiedDate, int size, HttpPostedFileBase file)
        {
            string filePathName = string.Empty;

            string dir = string.IsNullOrEmpty(path) ? "~/Uploads" : path;
            string localPath = HostingEnvironment.MapPath(dir);
            // string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Uploads");
            if (Request.Files.Count == 0)
            {
                return Json(new { jsonrpc = 2.0, error = new { code = 102, message = "保存失败" }, id = "id" });
            }

            string ex = Path.GetExtension(file.FileName).ToLower(); ;

            //var allowEx = ".jpg,.gif,.png,.mp4,.webm,.ogv,.jpeg,.flv";
            //if (!allowEx.Contains(ex))
            //{
            //    return Json(new { jsonrpc = 2.0, error = new { code = 102, message = "不允许" + ex + "文件上传！" }, id = "id" });
            //}

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            fileName = FileHelper.GetFileName(fileName, localPath, ex);
            filePathName = fileName + ex;
            file.SaveAs(Path.Combine(localPath, filePathName));


            return Json(new
            {
                jsonrpc = "2.0",
                id = id,
                filePath = dir + "/" + filePathName
            });


        }

       


    }
}