using ElFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TZGCMS.SiteWeb.Areas.Tools.Controllers
{
    public class ElFinderControllerBase : Controller
    {
        private FileSystemDriver _driver;
        protected void InitDriver(FileSystemDriver driver)
        {
            _driver = driver;
        }
        private Connector _connector;
        /// <summary>
        /// 
        /// </summary>
        public Connector Connector
        {
            get
            {
                if (_connector == null)
                {
                    _connector = new Connector(_driver);
                }
                return _connector;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return Connector.Process(this.HttpContext.Request);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ActionResult SelectFile(string target)
        {
            return Json(Connector.GetFileByHash(target).FullName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmb"></param>
        /// <returns></returns>
        [Route("ElFinder/Thumbnails/{tmb}")]
        public ActionResult Thumbs(string tmb)
        {
            return Connector.GetThumbnail(Request, Response, tmb);
        }
    }
}