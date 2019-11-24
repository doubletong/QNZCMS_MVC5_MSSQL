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
using TZGCMS.Model;

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

        [ResponseType(typeof(ProductCategoryVM))]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> Categories()
        {
            var list = await db.ProductCategories.Where(d => d.Active)
                .OrderByDescending(d => d.Importance).ProjectToListAsync<ProductCategoryVM>();
            return Ok(list);
        }

        [ResponseType(typeof(ProductVM))]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> IndexAsync(int pageIndex,int categoryId)
        {
            var query = db.Products.Where(d => d.Active);
            if (categoryId > 0)
                query = query.Where(d => d.Categories.Any(f => f.Id == categoryId));

            var products = await query
                .OrderByDescending(d => d.CreatedDate)
                .Skip((pageIndex - 1)*10)
                .Take(10)
                .ProjectToListAsync<ProductVM>();

            return Ok(products);
        }
        [ResponseType(typeof(ProductDetailVM))]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> DetailsAsync(int id)
        {
            var product = await db.Products.FindAsync(id);
            if (product == null)
                return NotFound();
           var vm = Mapper.Map<ProductDetailVM>(product);
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
