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
using System.Web.UI;
using TZGCMS.SiteWeb.Filters;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class AchievementsController : QNZBaseController
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public AchievementsController( IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: Admin/Achievements
   
        public async Task<ActionResult> Index(int? page, int? categoryId, string keyword)
        {
            AchievementListVM vm = await GetElementsAsync(page, categoryId, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            var categoryList = await _db.AchievementCategories.OrderByDescending(c => c.Importance).ToListAsync();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Title");

            return View(vm);

        }

        private async Task<AchievementListVM> GetElementsAsync(int? page, int? categoryId, string keyword)
        {
            var vm = new AchievementListVM()
            {
                CategoryId = categoryId,
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = 10
            };
            var query = _db.Achievements.Include(d=>d.AchievementCategory).AsQueryable();
            if (categoryId > 0)
            {
                query = query.Where(d => d.CategoryId == categoryId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword));
            }

            var pagelist = await query.OrderByDescending(d => d.Id).Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ProjectTo<AchievementVM>().ToListAsync();
            

            vm.TotalCount = await query.CountAsync();
            vm.Achievements = new StaticPagedList<AchievementVM>(pagelist, vm.PageIndex, vm.PageSize, vm.TotalCount); 
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


        // GET: Admin/Achievements/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var vm = new AchievementIM
            {
                Active = true,
                Pubdate = DateTime.Now
            };

            if (id > 0)
            {
                var vAchi = await _db.Achievements.FindAsync(id);
                if (vAchi == null)
                {
                    AR.Setfailure(Messages.HttpNotFound);
                    return Json(AR, JsonRequestBehavior.AllowGet);
                }

                vm = _mapper.Map<Achievement, AchievementIM>(vAchi); 

            }

            var categorys = await _db.AchievementCategories.OrderByDescending(m => m.Importance).ToListAsync();
            var lCategorys = new SelectList(categorys, "Id", "Title");

            ViewBag.Categories = lCategorys;

            return View(vm);
          
          

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(AchievementIM vm)
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
                    var editAchievement = await _db.Achievements.FindAsync(vm.Id);

                    editAchievement = _mapper.Map(vm, editAchievement);
                    editAchievement.UpdatedBy = Site.CurrentUserName;
                    editAchievement.UpdatedDate = DateTime.Now;
                    _db.Entry(editAchievement).State = EntityState.Modified;
                    await _db.SaveChangesAsync();



                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Achievement));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var newAchievement = _mapper.Map<AchievementIM, Achievement>(vm);
                    newAchievement.CreatedBy = Site.CurrentUserName;
                    newAchievement.CreatedDate = DateTime.Now;
                    _db.Achievements.Add(newAchievement);
                    await _db.SaveChangesAsync();


                    AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Achievement));
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
            var page = _db.Achievements.Find(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Achievement));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _db.Entry(page).State = EntityState.Modified;
                _db.SaveChanges();

                AR.Data = RenderPartialViewToString("_AchievementItem", page);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Achievement));
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
            var page = _db.Achievements.Find(id);
            if (page != null)
            {
                _db.Achievements.Remove(page);
                _db.SaveChanges();
                //_pageServices.Delete(page);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Achievement));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Achievement));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }



    }
}