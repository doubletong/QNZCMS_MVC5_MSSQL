using ElFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace TZGCMS.SiteWeb.Areas.Tools.Controllers
{
    public class ElFinderController : ElFinderControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ElFinderController()
        {
            ViewBag.RootUrl = "/";
            FileSystemDriver driver = new FileSystemDriver();
            var app_data = HostingEnvironment.MapPath("~/Uploads");
            string scriptsPath = app_data;  //@"C:\Users\Administrator";
            string thumbsPath = HostingEnvironment.MapPath("~/App_Data");
            DirectoryInfo thumbsStorage = new DirectoryInfo(thumbsPath);
            driver.AddRoot(new Root(new DirectoryInfo(scriptsPath), "/Documents/")
            {
                IsLocked = false,
                IsReadOnly = false,
                IsShowOnly = false,
                ThumbnailsStorage = thumbsStorage,
                ThumbnailsUrl = "/Tools/ElFinder/Thumbs?tmb=",
                ThumbnailsSize=100
            });
            InitDriver(driver);
        }

        // GET: ElFinder
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ElFinder()
        {
            return View();
        }
        public ActionResult Default()
        {
            return View();
        }
        public ActionResult ForTinyMCE4()
        {
            return View();
        }
    }
}