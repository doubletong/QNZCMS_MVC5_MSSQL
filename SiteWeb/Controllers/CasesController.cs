using AutoMapper;
using AutoMapper.QueryableExtensions;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Controllers
{
    [SIGActionFilter]
    public class CasesController : BaseController
    {
        private readonly IMapper _mapper;

        public CasesController(IMapper mapper)
        {
           
            _mapper = mapper;


        }
        // GET: Cases
        public async Task<ActionResult> Index(int? page)
        {

            var vm = new CaseListFrontVM
            {
                PageIndex = page ?? 1,
                PageSize = 2
            };

            var query = _db.Cases.Where(d => d.Active).AsQueryable();


            var list = await query.OrderByDescending(d => d.Pubdate).ThenByDescending(d => d.Id)
                .Skip((vm.PageIndex - 1)* vm.PageSize).Take(vm.PageSize)
                .ProjectTo<CaseVM>(_mapper.ConfigurationProvider).ToListAsync();

            vm.TotalCount = await query.CountAsync();
            vm.Cases = new StaticPagedList<CaseVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount);



            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.MENU && d.ObjectId == url);


            return View(vm);

        }

        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {

            var model = await _db.Cases.FindAsync(id);
            if (model == null) return HttpNotFound();

            model.ViewCount++;
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.CASE && d.ObjectId == id.ToString());

            var prev = _db.Cases.Where(s => s.Active && s.Id < id).OrderByDescending(s => s.Id).FirstOrDefault();
            if (prev != null)
            {
                ViewBag.Prev = prev;
            }

            var next = _db.Cases.Where(s => s.Active && s.Id > id).OrderBy(s => s.Id).FirstOrDefault();
            if (next != null)
            {
                ViewBag.Next = next;
            }

            return View(model);


        }
    }
}