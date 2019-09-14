using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Search;
using TZGCMS.Resources.Admin;
namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    public class LuceneController : BaseController
    {
        private readonly IMapper _mapper;

        public LuceneController(IMapper mapper)
        {
            _mapper = mapper;
        }
        public ActionResult Index(string searchTerm, int? page)
        {
            var vm = new LuceneListVM()
            {
                SearchTerm = searchTerm,            
                PageIndex = (page ?? 1) - 1,
                PageSize = SettingsManager.Lucene.PageSize
            };

            // create default Lucene search index directory
            if (!Directory.Exists(LuceneHelper._luceneDir))
                Directory.CreateDirectory(LuceneHelper._luceneDir);

            int totalCount = 0;
            // perform Lucene search
            List<SearchData> _searchResults = LuceneHelper.SearchDefault(vm.PageIndex, vm.PageSize, out totalCount, searchTerm).ToList();
                                

            // setup and return view model
            var search_field_list = new
                List<SelectedList> {
                                         new SelectedList {Text = "--所有字段--", Value = ""},
                                         new SelectedList {Text = "标题", Value = "Name"},
                                         new SelectedList {Text = "网址", Value = "Name"},
                                         new SelectedList {Text = "内容", Value = "Description"}

                                     };


            vm.TotalCount = totalCount;
            vm.SearchIndexData = new StaticPagedList<SearchData>(_searchResults, vm.PageIndex + 1, vm.PageSize, vm.TotalCount); ;
            vm.SearchFieldList = search_field_list;

            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(vm);

        }

        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/LuceneSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("PageSize").SetValue(pageSize);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

    


        [HttpGet]
        public PartialViewResult AddToIndex()
        {

            return PartialView("_AddToIndex");
        }

        [HttpPost]
        public ActionResult AddToIndex(SearchDataIM sampleData)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var im = _mapper.Map<SearchData>(sampleData);
            LuceneHelper.AddUpdateLuceneIndex(im);

            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, "索引目录"));
            return Json(AR, JsonRequestBehavior.DenyGet);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ClearIndex()
        {
            if (LuceneHelper.ClearLuceneIndex())
            {
                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, "索引"));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            else
            {
                AR.Setfailure("索引被锁，不能被清除，迟点再试！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ClearIndexRecord(string id)
        {
            try
            {
                LuceneHelper.ClearLuceneIndexRecord(id);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, "索引"));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

      
    }
}