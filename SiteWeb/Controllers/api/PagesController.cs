using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Pages;
using TZGCMS.Model.Front.ViewModel.Pages;
using TZGCMS.Service.Pages;

namespace SiteWeb.Controllers.api
{
    /// <summary>
    /// 单页管理
    /// </summary>
    public class PagesController : ApiController
    {
        private readonly IPageServices _pageServices;
        private readonly IMapper _mapper;
        public PagesController(IPageServices pageServices, IMapper mapper)
        {
            _pageServices = pageServices;
            _mapper = mapper;
        }


        /// <summary>
        /// 获取单页
        /// </summary>
        /// <param name="seoName">联系页：seoName="contact"</param>
        /// <returns></returns>
        [ResponseType(typeof(PageFVM))]
        public async Task<IHttpActionResult> GetPage(string seoName)
        {
            Page page = await _pageServices.GetBySeoNameAsync(seoName);
            if (page == null)
            {
                return NotFound();
            }
            var vm = _mapper.Map<Page, PageFVM>(page);
            return Ok(vm);
        }

        


    }
}