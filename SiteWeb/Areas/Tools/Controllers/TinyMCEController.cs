using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Infrastructure.Helper;

namespace TZGCMS.SiteWeb.Areas.Tools.Controllers
{
    public class TinyMCEController : Controller
    {
        /// <summary>
        /// tinymce images_upload_handler 调用
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]

        public ActionResult TinymceUpload(HttpPostedFileBase file)
        {
            string path;
            string saveloc = "/Uploads/Images/";
            string filename = ImageHandler.GetRandomFileName(Path.GetExtension(file.FileName), 10000);
            string relativeloc = saveloc + filename;

            if (file != null && file.ContentLength > 0 && file.ContentType.Contains("image"))
            {
                try
                {
                    path = Path.Combine(HttpContext.Server.MapPath(saveloc), Path.GetFileName(filename));
                    file.SaveAs(path);
                    return Json(new { location = relativeloc });
                }
                catch (Exception e)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.Message);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "错误: 只接受图片格式文件！");
            }


        }
    }
}