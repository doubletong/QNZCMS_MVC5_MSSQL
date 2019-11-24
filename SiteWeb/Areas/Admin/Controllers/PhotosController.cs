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
using log4net;
using AutoMapper;
using TZGCMS.Infrastructure.Helper;
using PagedList;
using TZGCMS.Model;
using AutoMapper.QueryableExtensions;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using System.Drawing;
using System.IO;
using TZGCMS.SiteWeb.Filters;
using TZGCMS.Infrastructure.Configs;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class PhotosController : QNZBaseController
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AlbumsController));
        private readonly IQNZDbContext _db;
        private readonly IMapper _mapper;
        public PhotosController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }


        public async Task<ActionResult> Index(int? page, int? albumId, string keyword)
        {


            var vm = new PhotoListVM()
            {
                AlbumId = albumId,
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Photo.PageSize
            };
            var query = _db.Photos.Include(d => d.Album).AsQueryable();
            if (albumId > 0)
            {
                query = query.Where(d => d.AlbumId == albumId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword));
            }

            var pagelist = await query.OrderByDescending(d => d.Id).Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ProjectTo<PhotoVM>().ToListAsync();


            vm.TotalCount = await query.CountAsync();
            vm.Photos = new StaticPagedList<PhotoVM>(pagelist, vm.PageIndex, vm.PageSize, vm.TotalCount);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            var categoryList = await _db.Albums.OrderByDescending(c => c.Importance).ToListAsync();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Title");

            return View(vm);

        }

     


        // GET: Admin/Photos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var albums = await _db.Albums.OrderByDescending(d => d.Importance).ThenByDescending(d => d.CreatedDate).ProjectTo<AlbumVM>().ToListAsync();
            ViewBag.AlbumId = new SelectList(albums, "Id", "Title");

            if (id == null)
            {
                return View(new PhotoIM { Importance = 0,Active=true });
            }
            Photo photo = await _db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            var model = _mapper.Map<PhotoIM>(photo);

         

            return View(model);
        }

        // POST: Admin/Photos/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AlbumId,FullImageUrl,Importance,Active,Title")] PhotoIM photo)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            if (photo.Id>0)
            {     

                var model = await _db.Photos.FindAsync(photo.Id);
                model = _mapper.Map(photo, model);
                model.UpdatedBy = Site.CurrentUserName;
                model.UpdatedDate = DateTime.Now;
                model.Thumbnail = CreateThumbnail(model.FullImageUrl);
                _db.Entry(model).State = EntityState.Modified;
             
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Photo));
              
            }
            else
            {

                var vm = _mapper.Map<PhotoIM, Photo>(photo);
                vm.CreatedBy = Site.CurrentUserName;
                vm.CreatedDate = DateTime.Now;
                vm.Thumbnail = CreateThumbnail(vm.FullImageUrl);

                _db.Photos.Add(vm);             
                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Photo));
            }
            
            await _db.SaveChangesAsync();
            return Json(AR, JsonRequestBehavior.DenyGet);


        }

        public string CreateThumbnail(string filePath)
        {

            var fileName = Server.MapPath(filePath);

            Image image = Image.FromFile(fileName);
        

            var dir = Server.MapPath(SettingsManager.Photo.ThumbDir);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }           

            var ori = Path.GetFileName(fileName);
         
            var savePath = Path.Combine(dir , ori);
    
           ImageHandler.MakeThumbnail(fileName, savePath, SettingsManager.Photo.ThumbWidth, SettingsManager.Photo.ThumbHeight, "Cut", Path.GetExtension(fileName).ToLower());

            return SettingsManager.Photo.ThumbDir + "/" + ori;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {
            var page = _db.Photos.Find(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Photo));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _db.Entry(page).State = EntityState.Modified;
                _db.SaveChanges();

                AR.Data = RenderPartialViewToString("_PhotoItem", page);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Photo));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        // POST: Admin/Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            

            var page = _db.Photos.Find(id);
            if (page != null)
            {
                _db.Photos.Remove(page);
                await _db.SaveChangesAsync();
                //_pageServices.Delete(page);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Photo));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Photo));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }

    }
}
