using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Model.Front.ViewModel.Ads;
using TZGCMS.Service.Ads;

namespace TZGCMS.SiteWeb.Controllers.api
{
    /// <summary>
    /// 轮播图，广告图
    /// </summary>
    public class CarouselsController : ApiController
    {

        private TZGEntities db = new TZGEntities();

        /// <summary>
        /// 获取广告位图片
        /// </summary>
        /// <param name="code">广告位代码</param>
        /// <returns></returns>
        // GET: api/Carousels/5
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetCarousels(string code)
        {
            var position = db.Positions.Include("Carousels").FirstOrDefault(d => d.Code == code);
            if (position == null || !position.Carousels.Any())
            {
                return NotFound();
            }
            var list = position.Carousels.Select(d => d.ImageUrl).ToList();         
            return Ok(list);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}