using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class TestController : ApiController
    {
        // GET
        [ResponseType(te)]
        public ActionResult Index()
        {
            return
            View();
        }
    }
}