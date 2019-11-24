using AutoMapper;
using PagedList;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using System.Data.Entity;
using System.Threading.Tasks;
using TZGCMS.Model;
using QNZ.Data;
using AutoMapper.QueryableExtensions;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class InstitutesController : QNZBaseController
    {

        private IQNZDbContext _db;
        private IMapper _mapper;
        public InstitutesController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: Admin/Institute



        public async Task<ActionResult> Index(int? page, string keyword)
        {
            InstituteListVM categoryListVM = await GetElementsAsync(page, keyword);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(categoryListVM);

        }

        private async System.Threading.Tasks.Task<InstituteListVM> GetElementsAsync(int? page, string keyword)
        {
            var vm = new InstituteListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Article.PageSize
            };
            var query = _db.Institutes.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword));
            }
        
            var list = await query.OrderByDescending(d => d.Importance)
                .Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ProjectTo<InstituteVM>().ToListAsync();
                //_categoryServices.GetPagedElements(vm.PageIndex-1, vm.PageSize, vm.Keyword, out totalCount);

            vm.TotalCount = await query.CountAsync();
            vm.Institutes = new StaticPagedList<InstituteVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
        }

        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/ArticleSettings.config");
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
        public async Task<ActionResult> Edit(int? id)
        {
            var vm = new InstituteIM
            {
                Active = true,
                Importance = 0
            };

            if (id > 0)
            {
                var vCase = await _db.Institutes.FindAsync(id);
                if (vCase == null)
                {
                    AR.Setfailure(Messages.HttpNotFound);
                    return Json(AR, JsonRequestBehavior.AllowGet);
                }

                vm = _mapper.Map<Institute, InstituteIM>(vCase);

                var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.INSTITUTE && d.ObjectId == vm.Id.ToString());
                if (pageMeta != null)
                {
                    vm.SEOTitle = pageMeta.Title;
                    vm.Keywords = pageMeta.Keyword;
                    vm.SEODescription = pageMeta.Description;
                }            
            }

            var dics =  await _db.Dictionaries.Where(d=>d.TypeId == 1).OrderByDescending(m => m.Importance).ToListAsync();
            var lDic = new SelectList(dics, "Id", "Title");

            ViewBag.Dictionaries = lDic;

            return View(vm);
  
         

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<JsonResult> Edit(InstituteIM vm)
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
                    var editCase = await _db.Institutes.FindAsync(vm.Id);

                    editCase = _mapper.Map(vm, editCase);
                    editCase.UpdatedDate = DateTime.Now;
                    editCase.UpdatedBy = Site.CurrentUserName;

                    _db.Entry(editCase).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.INSTITUTE && d.ObjectId == editCase.Id.ToString());

                    pageMeta = pageMeta ?? new PageMeta();
                    pageMeta.ObjectId = vm.Id.ToString();
                    pageMeta.Title = string.IsNullOrEmpty(vm.Title) ? vm.Title : vm.Title;
                    pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
                    pageMeta.Description = vm.SEODescription;
                    pageMeta.ModelType = (short)ModelType.INSTITUTE;

                    if (pageMeta.Id > 0)
                    {
                        _db.Entry(pageMeta).State = EntityState.Modified;
                    }
                    else
                    {
                        _db.PageMetas.Add(pageMeta);
                    }
                    await _db.SaveChangesAsync();



                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Institute));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var newCase = _mapper.Map<InstituteIM, Institute>(vm);
                    newCase.CreatedDate = DateTime.Now;
                    newCase.CreatedBy = Site.CurrentUserName;

                    newCase = _db.Institutes.Add(newCase);
                    var result = await _db.SaveChangesAsync();

                    if (result > 0)
                    {
                        if (!string.IsNullOrEmpty(vm.Keywords) || !string.IsNullOrEmpty(vm.SEODescription))
                        {
                            var pm = new PageMeta
                            {
                                Title = vm.SEOTitle,
                                Description = vm.SEODescription,
                                Keyword = vm.Keywords,
                                ModelType = (short)ModelType.INSTITUTE,
                                ObjectId = newCase.Id.ToString()
                            };
                            _db.PageMetas.Add(pm);
                            await _db.SaveChangesAsync();
                        }
                    }

                    AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Institute));
                    return Json(AR, JsonRequestBehavior.DenyGet);

                }



            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


        }

        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var category = _db.Institutes.Include(d=>d.Laboratories).FirstOrDefault(d=>d.Id == id);
                //_categoryServices.GetByIdWithArticles(id);
         
            if (category.Laboratories.Any())
            {
                AR.Setfailure("此分类下面还有文章存在，不能删除！");
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _db.Institutes.Remove(category);
            _db.SaveChanges();

            //_categoryServices.Delete(category);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Institute));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsActive(int id)
        {

            var vCategory = _db.Institutes.Find(id);
                //_categoryServices.GetById(id);
            if (vCategory == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                vCategory.Active = !vCategory.Active;
                _db.Entry(vCategory).State = EntityState.Modified;
                _db.SaveChanges();
               // _categoryServices.Update(vCategory);

               // var vm = _mapper.Map<InstituteVM>(vCategory);

                AR.Data = RenderPartialViewToString("_CategoryItem", vCategory);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Institute));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        

        //[AllowAnonymous]
        //public async Task<JsonResult> IsSeoNameUnique(string alias, int? id)
        //{
        //    var result = await IsExistSeoName(alias, id);
        //    return !result
        //        ? Json(true, JsonRequestBehavior.AllowGet)
        //        : Json(false, JsonRequestBehavior.AllowGet);
        //}

        //public async Task<bool> IsExistSeoName(string alias, int? id)
        //{
        //    if (id != null)
        //    {
        //        return await _db.Institutes.CountAsync(d => d.Alias == alias && d.Id != id.Value) > 0;
        //    }

        //    return await _db.Institutes.CountAsync(d => d.Alias == alias) > 0;
        //}
     
    }
}