using AutoMapper;
using PagedList;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Model;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    public class JoinController : Controller
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public JoinController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }
        [SIGActionFilter]
        // GET: Join
        public async System.Threading.Tasks.Task<ActionResult> Index(string category,int? page)
        {
            var query = _db.Jobs.Where(d => d.Active).AsQueryable();
 
          

            var vm = new JobPagedVM
            {
                Dictionaries = await _db.Dictionaries.Where(d=>d.TypeId == 2).ToListAsync(),
                Category = category,
                PageIndex = page ?? 1,
                PageSize = 10
            };

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(d => d.Category == category);
            }

         

            var list = await query.Select(d=>new FrontJobVM
            {
                Id = d.Id,
                Category = d.Category,
                Address = d.Address,
                Post = d.Post,
                Quantity = d.Quantity,
                Importance = d.Importance,
                Description = d.Description,
                CreatedDate = d.CreatedDate,
                GroupName = d.Category + "招聘·" + d.Address
        }).OrderBy(d=>d.GroupName).ThenByDescending(d => d.Importance)
               .Skip((vm.PageIndex - 1) * vm.PageSize)
                .Take(vm.PageSize).ToListAsync();

            vm.TotalCount = await query.CountAsync();
            vm.Jobs = new StaticPagedList<FrontJobVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);

                       
            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetaSets.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);

            return View(vm);
        }
    }
}