using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Service.Links;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Controllers
{
    public class LinkController : BaseController
    {
        private readonly ILinkServices _linkServices;
        private ILinkCategoryServices _categoryService;
        private readonly IPageMetaServices _pageMetaService;
        public LinkController(ILinkServices linkServices, ILinkCategoryServices categoryService, IPageMetaServices pageMetaService)
        {
            _linkServices = linkServices;
            _categoryService = categoryService;
            _pageMetaService = pageMetaService;
        }
        // GET: Link
        [HttpGet]
        public PartialViewResult GetLinks(int categoryId, int count)
        {
            //var category = _categoryService.GetById(categoryId);
            int totalCount;
            var linkList = _linkServices.GetActivePagedElements(0, count, string.Empty, categoryId,out totalCount);
            return PartialView(linkList);
        }
    }
}