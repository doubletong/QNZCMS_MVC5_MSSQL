using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Model;

namespace TZGCMS.SiteWeb.Controllers
{
    public class AchievementsController : Controller
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public AchievementsController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: Achievements
        public async Task<ActionResult> Index()
        {
            var vm = await _db.AchievementCategories.Where(d => d.Active).OrderByDescending(d => d.Importance).ToListAsync();
            return View(vm);
        }
        public async Task<ActionResult> List(int cid, int? page)
        {
            var category = await _db.AchievementCategories.FirstOrDefaultAsync(d => d.Id == cid);
            var query = _db.Achievements.Where(d => d.Active && d.CategoryId == cid).AsQueryable();

            var vm = new AchievementPagedVM
            {
                CategoryId = cid,               
                CategoryTitle = category?.Title,
                PageIndex = page ?? 1,
                PageSize = 10
            };

            var list = await query.OrderByDescending(d => d.Pubdate)
                .ThenByDescending(d => d.Id).Skip((vm.PageIndex - 1) * vm.PageSize)
                .Take(vm.PageSize).ToListAsync();

            vm.TotalCount = await query.CountAsync();
            vm.Achievements = new StaticPagedList<Achievement>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);


            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetaSets.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);

            return View(vm);
        }
    }
}