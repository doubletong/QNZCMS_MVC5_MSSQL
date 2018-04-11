using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TZGCMS.Data.Entity;
using TZGCMS.Model.Front.ViewModel.Outlets;

namespace TZGCMS.SiteWeb.Controllers.api
{
    public class OutletsController : ApiController
    {
        private TZGEntities db = new TZGEntities();

        [AcceptVerbs("GET")]
        [ResponseType(typeof(OutletFVM))]
        public async Task<IHttpActionResult> IndexAsync()
        {
            var vm = await db.Outlets.Where(d => d.Active).ProjectToListAsync<OutletFVM>();
            foreach(var item in vm)
            {
                item.latitude = string.IsNullOrEmpty(item.Coordinate) ? 0 : double.Parse( item.Coordinate.Split(',')[0]);
                item.longitude = string.IsNullOrEmpty(item.Coordinate) ? 0 : double.Parse(item.Coordinate.Split(',')[1]);
                item.width = 30;
                item.height = 30;
                item.iconPath = "/resources/marker.png";
            }
            return Ok(vm);
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
