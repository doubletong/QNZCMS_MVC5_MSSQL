using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Doc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Model.Front.ViewModel.Doc;
using TZGCMS.Service.Doc;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Controllers
{
    public class DownloadController : BaseController
    {

        // GET: Download
        //public ActionResult Index(int? page, int? cateId)
        //{
        //    var vm = new DocumentListFVM
        //    {
        //        CateId = cateId??0,
        //        PageSize = SettingsManager.Doc.PageSize,
        //        PageIndex = page??1,
        //        Categories = _categoryService.GetAll().Where(d=>d.Active).OrderByDescending(d=>d.Importance)
        //    };
        //    int totalCount;
        //    var list = _docService.GetActivePagedElements(vm.PageIndex - 1, vm.PageSize, string.Empty, vm.CateId, out totalCount);
        //    //var categoryVMList = _mapper.Map<List<Article>, List<ArticleVM>>(goodslist);
        //    vm.TotalCount = totalCount;

        //    vm.Documents = new StaticPagedList<Document>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);
        //    var url = Request.RawUrl;
        //    ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);
        //    return View(vm);
        //}
        public ActionResult NewDocument()
        {
          
            var doc =  _db.Documents.OrderByDescending(d=>d.Importance)
                .ThenByDescending(d=>d.Id).FirstOrDefault(d=>d.Active);

            return PartialView("_NewDocument", doc);

        }
        public ActionResult BotDocument()
        {

            var doc = _db.Documents.OrderByDescending(d => d.Importance)
                .ThenByDescending(d => d.Id).FirstOrDefault(d => d.Active);

            return PartialView("_BotDocument", doc);

        }
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document doc = await _db.Documents.FindAsync(id);
            if (doc == null)
            {
                return HttpNotFound();
            }


            doc.DownloadCount++;
            _db.Entry(doc).State = EntityState.Modified;
            await _db.SaveChangesAsync();


            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(doc.FilePath));
                string fileName = doc.Title + "." + doc.Extension;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception er)
            {
                TempData["Error"] = er.Message.ToString();
                return RedirectToAction("ServerError", "Error");
            }

            
        }


    }
}