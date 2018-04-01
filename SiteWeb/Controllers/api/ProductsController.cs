using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using TZGCMS.Model.Front.ViewModel.Products;

namespace TZGCMS.SiteWeb.Controllers.api
{
   
    public class ProductsController : ApiController
    {
        private TZGEntities db = new TZGEntities();

        /// <summary>
        /// 获取推荐产品
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [ResponseType(typeof(ProductVM))]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> RecommendAsync(int count)
        {
            var products = await db.Products.Where(d=>d.Active && d.Recommend)
                .OrderByDescending(d=>d.CreatedDate).Take(count).ProjectToListAsync<ProductVM>();
            return Ok(products);
        }

        [ResponseType(typeof(ProductVM))]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> IndexAsync(int pageSize)
        {
            var products = await db.Products.Where(d => d.Active)
                .OrderByDescending(d => d.CreatedDate).Skip((pageSize-1)*10).Take(10).ProjectToListAsync<ProductVM>();
            return Ok(products);
        }
        [ResponseType(typeof(ProductVM))]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> DetailsAsync(int id)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
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
