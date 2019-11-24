using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QNZ.Data;
using AutoMapper;
using System.Xml.Linq;
using TZGCMS.Infrastructure.Helper;
using PagedList;
using TZGCMS.Model;
using TZGCMS.Infrastructure.Configs;
using AutoMapper.QueryableExtensions;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using log4net;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    public class AlbumsController : QNZBaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AlbumsController));
        private readonly IQNZDbContext _db;
        private readonly IMapper _mapper;
        public AlbumsController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: Admin/Albums
        public async Task<ActionResult> Index(int? page, string keyword)
        {
            var vm = new AlbumListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Album.PageSize
            };
            var query = _db.Albums.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword));
            }

            var list = await query.OrderByDescending(d => d.Importance)
                .Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ProjectTo<AlbumVM>().ToListAsync();
            //_categoryServices.GetPagedElements(vm.PageIndex-1, vm.PageSize, vm.Keyword, out totalCount);

            vm.TotalCount = await query.CountAsync();
            vm.Albums = new StaticPagedList<AlbumVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(vm);

          
        }

        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/AlbumSettings.config");
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

      

        // GET: Admin/Albums/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View(new AlbumIM() { Importance = 0, Active=true});
            }

            Album album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            var im = _mapper.Map<AlbumIM>(album);
            return View(im);
        }

        // POST: Admin/Albums/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Importance,Cover,Banner,Active")] AlbumIM album)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            if (album.Id>0)
            {
                var model = await _db.Albums.FindAsync(album.Id);
                model = _mapper.Map( album, model);
                model.UpdatedBy = Site.CurrentUserName;
                model.UpdatedDate = DateTime.Now;

                _db.Entry(model).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                var message = string.Format(Messages.AlertUpdateSuccess, EntityNames.Album);
                _logger.Info(message);
                AR.SetSuccess(message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var im = _mapper.Map<Album>(album);
                im.CreatedBy = Site.CurrentUserName;
                im.CreatedDate = DateTime.Now;

                _db.Albums.Add(im);
                await _db.SaveChangesAsync();

                var message = string.Format(Messages.AlertCreateSuccess, EntityNames.Album);
                _logger.Info(message);
                AR.SetSuccess(message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> IsActive(int id)
        {

            var album = await _db.Albums.FindAsync(id);
            //_categoryServices.GetById(id);
            if (album == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                album.Active = !album.Active;
                _db.Entry(album).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                var vm = _mapper.Map<AlbumVM>(album);
           
                AR.Data = RenderPartialViewToString("_AlbumItem", vm);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Album));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        // POST: Admin/Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Album album = await _db.Albums.Include(d=>d.Photos).FirstOrDefaultAsync(d=>d.Id == id);
            if (album.Photos.Any())
            {
                AR.Setfailure("此相册下面还有照片存在，不能删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
          

            _db.Albums.Remove(album);
            await _db.SaveChangesAsync();

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Album));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }

      
    }
}
