using TZGCMS.SiteWeb.Filters;
using System.Web.Mvc;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Roles = string.Join("，", User.Roles);
            return View();
        }
    }
}