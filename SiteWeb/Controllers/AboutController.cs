using AutoMapper.QueryableExtensions;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Model;

namespace TZGCMS.SiteWeb.Controllers
{
    public class AboutController : BaseController
    {
        private readonly IQNZDbContext _db;
        private readonly ICacheService _cacheService;
        public AboutController(ICacheService cacheService, IQNZDbContext db)
        {
            _cacheService = cacheService;
            _db = db;
        }
        // GET: About
        public async Task<ActionResult> Index()
        {
            var albums = await _db.Albums.AsNoTracking().Where(d => d.Active)
                .OrderByDescending(d => d.Importance)
                .ThenByDescending(d => d.CreatedDate).ProjectTo<AlbumVM>().ToListAsync();


            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);

            return View(albums);
        }

        public async Task<ActionResult> Photos(int albumId)
        {
            var album = await _db.Albums.Include(d=>d.Photos).FirstOrDefaultAsync(d => d.Id == albumId);
            if (album == null) return HttpNotFound();
          //  var photos = await _db.Photos.Where(d => d.AlbumId == albumId).ToListAsync();
            //var url = Request.RawUrl;
            //ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);

            return View(album);
        }
    }
}