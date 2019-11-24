using AutoMapper;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using TZGCMS.Model;
using System.Data.Entity;
using QNZ.Data;
using PagedList;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    //[SIGAuth]
    public class LaboratoriesController : QNZBaseController
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public LaboratoriesController( IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }
       
        // GET: Admin/Laboratories
        public async Task<ActionResult> Index(int? page, int? instituteId, string keyword)
        {
            LaboratoryListVM vm = await GetElementsAsync(page, instituteId, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            var categoryList = await _db.Institutes.OrderByDescending(c => c.Importance).ToListAsync();
            ViewBag.Institutes = new SelectList(categoryList, "Id", "Title");

            return View(vm);

        }

        private async Task<LaboratoryListVM> GetElementsAsync(int? page, int? instituteId, string keyword)
        {
            var vm = new LaboratoryListVM()
            {
                InstituteId = instituteId,
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = 10
            };
            var query = _db.Laboratories.Include(d=>d.Institute).AsQueryable();
            if (instituteId > 0)
            {
                query = query.Where(d => d.InstituteId == instituteId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword));
            }

            var pagelist = await query.OrderByDescending(d => d.Id).Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ProjectTo<LaboratoryVM>().ToListAsync();
            

            vm.TotalCount = await query.CountAsync();
            vm.Laboratories = new StaticPagedList<LaboratoryVM>(pagelist, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
        }
        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/PageSettings.config");
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


        // GET: Admin/Laboratories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var vm = new LaboratoryIM
            {
                Active = true,
                Importance = 0
            };

            if (id > 0)
            {
                var vAchi = await _db.Laboratories.FindAsync(id);
                if (vAchi == null)
                {
                    AR.Setfailure(Messages.HttpNotFound);
                    return Json(AR, JsonRequestBehavior.AllowGet);
                }

                vm = _mapper.Map<Laboratory, LaboratoryIM>(vAchi); 

            }

            var institutes = await _db.Institutes.OrderByDescending(m => m.Importance).ToListAsync();
            ViewBag.Institutes = new SelectList(institutes, "Id", "Title");

            return View(vm);
          
          

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(LaboratoryIM vm)
        {
          

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                if (vm.Id > 0)
                {
                    var editLaboratory = await _db.Laboratories.FindAsync(vm.Id);

                    editLaboratory = _mapper.Map(vm, editLaboratory);
                    editLaboratory.UpdatedBy = Site.CurrentUserName;
                    editLaboratory.UpdatedDate = DateTime.Now;
                    _db.Entry(editLaboratory).State = EntityState.Modified;
                    await _db.SaveChangesAsync();



                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Laboratory));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var newLaboratory = _mapper.Map<LaboratoryIM, Laboratory>(vm);
                    newLaboratory.CreatedBy = Site.CurrentUserName;
                    newLaboratory.CreatedDate = DateTime.Now;
                    _db.Laboratories.Add(newLaboratory);
                    await _db.SaveChangesAsync();


                    AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Laboratory));
                    return Json(AR, JsonRequestBehavior.DenyGet);

                }



            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }



        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {
            var page = _db.Laboratories.Find(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Laboratory));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _db.Entry(page).State = EntityState.Modified;
                _db.SaveChanges();

                AR.Data = RenderPartialViewToString("_LaboratoryItem", page);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Laboratory));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var page = _db.Laboratories.Find(id);
            if (page != null)
            {
                _db.Laboratories.Remove(page);
                _db.SaveChanges();
                //_pageServices.Delete(page);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Laboratory));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Laboratory));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }



    }
}