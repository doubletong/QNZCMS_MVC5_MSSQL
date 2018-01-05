using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Admin.ViewModel.FileManager;

namespace SiteWeb.Areas.Admin.api
{

    //[Authorize]
    public class FileManagerController : ApiController
    {
      
        // GET: api/FileManager/RootDirectories
        [HttpGet]       
        public IEnumerable<DirectoryVM> RootDirectories()
        {

            var rootPath = HostingEnvironment.MapPath(SettingsManager.File.RootDirectory);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            IEnumerable<DirectoryVM> vm = FileHelper.GetDirectories(rootPath, SettingsManager.File.RootDirectory);
            return  vm;
        }

        // GET: api/FileManager/RootDirFiles
        [HttpGet]
        public IEnumerable<FileVM> RootDirFiles()
        {

            var rootPath = HostingEnvironment.MapPath(SettingsManager.File.RootDirectory);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            IEnumerable<FileVM> vm = FileHelper.GetFileList(rootPath, SettingsManager.File.RootDirectory);

            return vm;
        }

        /// <summary>
        /// ajax 获取子目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DirectoryVM> GetSubDirectories(string dir)
        {
            var subPath = HostingEnvironment.MapPath(dir);
            IEnumerable<DirectoryVM> vm = FileHelper.GetDirectories(subPath, dir);
            return vm;
        }

        /// <summary>
        /// ajax 获取子目录文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<FileVM> GetSubFiles(string dir)
        {
            var subPath = HostingEnvironment.MapPath(dir);
            IEnumerable<FileVM> vm = FileHelper.GetFileList(subPath, dir);
            return vm;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Download(string filePath)
        {
            var path = HostingEnvironment.MapPath(filePath); ;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(path);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = stream.Length;
            return result;


        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteFile(string filePath)
        {
            try
            {
                var downPath = HostingEnvironment.MapPath(filePath);
                File.Delete(downPath);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "OK");
                return response;
            }
            catch (Exception er)
            {
               // LoggingFactory.Error("文件删除失败！", er);
                HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, er.Message);
                return response;
            }


         
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage RenameFile(string filePath, string newFilePath)
        {
          
            try
            {
                newFilePath = newFilePath.Replace(" ", "_");

                filePath = HostingEnvironment.MapPath(filePath);
                newFilePath = HostingEnvironment.MapPath(newFilePath);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "OK");

                if (File.Exists(newFilePath))
                {
                    // File.Delete(newFilePath);
                    response = Request.CreateResponse(HttpStatusCode.OK, "NO");
                    return response;
                }
                File.Move(filePath, newFilePath);
                              
                return response;
            }
            catch (Exception er)
            {
               // LoggingFactory.Error("文件重命名失败！", er);
                HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, er.Message);
                return response;
            }          
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CreateDir(string filePath, string dir)
        {
            try
            {
                dir = dir.Replace(" ", "_");

                var newDir = Path.Combine(filePath, dir);
                newDir = HostingEnvironment.MapPath(newDir);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "OK");

                if (Directory.Exists(newDir))
                {
                    // File.Delete(newFilePath);
                    response = Request.CreateResponse(HttpStatusCode.OK, "NO");
                    return response;
                }

                Directory.CreateDirectory(newDir);
                return response;
            }
            catch (Exception er)
            {
              //  LoggingFactory.Error("文件重命名失败！", er);
                HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, er.Message);
                return response;
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteDir(string filePath)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "OK");
                filePath = HostingEnvironment.MapPath(filePath); 
                if (Directory.Exists(filePath))
                {
                   
                    bool isEmpty = !Directory.EnumerateFiles(filePath).Any() && !Directory.EnumerateDirectories(filePath).Any();
                    if (!isEmpty)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, "NO");
                        return response;
                    }

                    Directory.Delete(filePath);
                }
                return response;
            }
            catch (Exception er)
            {
              //  LoggingFactory.Error("目录删除失败！", er);
                HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, er.Message);
                return response;
            }



        }


        /// <summary>
        /// 重命名目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage RenameDir(string filePath, string newFilePath)
        {
            try
            {
                newFilePath = newFilePath.Replace(" ", "_");

                filePath = HostingEnvironment.MapPath(filePath);
                newFilePath = HostingEnvironment.MapPath(newFilePath);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "OK");

                if (Directory.Exists(newFilePath))
                {
                    // File.Delete(newFilePath);
                    response = Request.CreateResponse(HttpStatusCode.OK, "NO");
                    return response;
                }
                Directory.Move(filePath, newFilePath);

                return response;
            }
            catch (Exception er)
            {
               // LoggingFactory.Error("目录重命名失败！", er);
                HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, er.Message);
                return response;
            }
        }

    }
}
