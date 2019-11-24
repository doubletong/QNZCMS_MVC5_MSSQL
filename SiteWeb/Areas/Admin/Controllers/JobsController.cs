using AutoMapper;
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
using PagedList;
using TZGCMS.Model;
using QNZ.Data;
using System.Data.Entity;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class JobsController : QNZBaseController
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public JobsController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }


        // GET: Admin/Jobs
        public async Task<ActionResult> Index(int? page, string keyword)
        {
            JobListVM pageListVM = await GetElementsAsync(page, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(pageListVM);

        }

        private async Task<JobListVM> GetElementsAsync(int? page, string keyword)
        {
            var vm = new JobListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Job.PageSize
            };
            var query = _db.Jobs.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Post.Contains(keyword) || d.Description.Contains(keyword));
            }

            var pagelist = await query.OrderByDescending(d => d.Importance).Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ProjectTo<JobVM>().ToListAsync();
                //_jobServices.GetJobdElements(pageListVM.PageIndex - 1, pageListVM.PageSize, pageListVM.Keyword, out totalCount);

            vm.TotalCount = await query.CountAsync();
            vm.Jobs = new StaticPagedList<JobVM>(pagelist, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
        }
        [HttpPost]
        public JsonResult JobSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/JobSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("JobSize").SetValue(pageSize);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        //// GET: Admin/Jobs/Create
        //public ActionResult Create()
        //{
        //    JobIM job = new JobIM()
        //    {
        //        Active = true
        //    };
        //    return PartialView("_Create", job);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //public async System.Threading.Tasks.Task<JsonResult> Create(JobIM job)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    //try
        //    //{
        //        var newJob = _mapper.Map<JobIM, Job>(job);              

        //        var result = _db.Jobs.Add(newJob);
        //    _db.SaveChanges();

        //        if (result!=null)
        //        {
        //            var pageMeta = new PageMetaSet()
        //            {
        //                ObjectId = result.ToString(),
        //                Title = string.IsNullOrEmpty(job.SEOTitle) ? job.Post : job.SEOTitle,
        //                Keyword = string.IsNullOrEmpty(job.Keywords) ? job.Post : job.Keywords.Replace('，', ','),
        //                Description = job.SEODescription,
        //                ModelType = (short)ModelType.JOB
        //            };
        //          //  _pageMetaServices.Create(pageMeta);
        //            _db.PageMetas.Add(pageMeta);
        //            _db.SaveChanges();
        //        }

        //        int pageSize = SettingsManager.Job.PageSize;
        //        var list = await _db.Jobs.OrderByDescending(d => d.Importance).Take(pageSize).ProjectTo<JobVM>().ToListAsync();
        //    //_jobServices.GetJobdElements(0, pageSize, string.Empty, out count);

        //    AR.Data = RenderPartialViewToString("_JobList", list);

        //        AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Job));
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    //}
        //    //catch (Exception er)
        //    //{
        //    //    AR.Setfailure(er.Message);
        //    //    return Json(AR, JsonRequestBehavior.DenyGet);
        //    //}

        //}

        // GET: Admin/Jobs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

            var vm = new JobIM
            {
                Active = true,
                Importance = 0
            };

            if (id > 0)
            {
                var vAchi = await _db.Jobs.FindAsync(id);
                if (vAchi == null)
                {
                    AR.Setfailure(Messages.HttpNotFound);
                    return Json(AR, JsonRequestBehavior.AllowGet);
                }

                vm = _mapper.Map<Job, JobIM>(vAchi);

                //var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.JOB && d.ObjectId == editJob.Id.ToString());
                //    //_pageMetaServices.GetPageMeta(ModelType.JOB, editJob.Id.ToString());
                //if (pageMeta != null)
                //{
                //    editJob.SEOTitle = pageMeta.Title;
                //    editJob.Keywords = pageMeta.Keyword;
                //    editJob.SEODescription = pageMeta.Description;
                //}
            }

            var dictionaries = await _db.Dictionaries.Where(d=>d.TypeId==2).OrderByDescending(m => m.Importance).ToListAsync();
            ViewBag.Categories = new SelectList(dictionaries, "Title", "Title");

            return View(vm);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(JobIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                //    var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.JOB && d.ObjectId == editJob.Id.ToString());
                //        //_pageMetaServices.GetPageMeta(ModelType.PAGE, editJob.Id.ToString());
                //    pageMeta = pageMeta ?? new PageMetaSet();

                //    pageMeta.ObjectId = page.Id.ToString();
                //    pageMeta.Title = string.IsNullOrEmpty(page.SEOTitle) ? page.Post : page.SEOTitle;
                //    pageMeta.Keyword = string.IsNullOrEmpty(page.Keywords) ? page.Post : page.Keywords.Replace('，', ',');
                //    pageMeta.Description = page.SEODescription;
                //    pageMeta.ModelType = (short)ModelType.JOB;

                if (vm.Id > 0)
                {
                    var job = await _db.Jobs.FindAsync(vm.Id);

                    job = _mapper.Map(vm, job);
                    job.UpdatedBy = Site.CurrentUserName;
                    job.UpdatedDate = DateTime.Now;
                    _db.Entry(job).State = EntityState.Modified;
                    //_db.Entry(pageMeta).State = EntityState.Modified;

                    await _db.SaveChangesAsync();



                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Job));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var newJob = _mapper.Map<JobIM, Job>(vm);
                    newJob.CreatedBy = Site.CurrentUserName;
                    newJob.CreatedDate = DateTime.Now;
                    _db.Jobs.Add(newJob);
                    await _db.SaveChangesAsync();

                    //_db.PageMetas.Add(pageMeta);
                    //await _db.SaveChangesAsync();

                    AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Job));
                    return Json(AR, JsonRequestBehavior.DenyGet);

                }



            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            //if (!ModelState.IsValid)
            //{
            //    AR.Setfailure(GetModelErrorMessage());
            //    return Json(AR, JsonRequestBehavior.DenyGet);
            //}

            //try
            //{
            //    var old = await _db.Jobs.FindAsync(page.Id);
            //    var editJob = _mapper.Map(page, old);

            //    _db.Entry(editJob).State = EntityState.Modified;
            //    await _db.SaveChangesAsync();
            //    //  _jobServices.Update(editJob);

            //    var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.JOB && d.ObjectId == editJob.Id.ToString());
            //        //_pageMetaServices.GetPageMeta(ModelType.PAGE, editJob.Id.ToString());
            //    pageMeta = pageMeta ?? new PageMetaSet();

            //    pageMeta.ObjectId = page.Id.ToString();
            //    pageMeta.Title = string.IsNullOrEmpty(page.SEOTitle) ? page.Post : page.SEOTitle;
            //    pageMeta.Keyword = string.IsNullOrEmpty(page.Keywords) ? page.Post : page.Keywords.Replace('，', ',');
            //    pageMeta.Description = page.SEODescription;
            //    pageMeta.ModelType = (short)ModelType.JOB;

            //    if (pageMeta.Id > 0)
            //    {
            //        _db.Entry(pageMeta).State = EntityState.Modified;
            //        //_pageMetaServices.Update(pageMeta);
            //    }
            //    else
            //    {
            //        _db.PageMetas.Add(pageMeta);
            //       // _pageMetaServices.Create(pageMeta);
            //    }
            //    await _db.SaveChangesAsync();
            //    // var pageVM = _mapper.Map<JobVM>(editJob);

            //    AR.Id = page.Id;
            //    AR.Data = RenderPartialViewToString("_JobItem", editJob);

            //    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Job));
            //    return Json(AR, JsonRequestBehavior.DenyGet);
            //}
            //catch (Exception er)
            //{
            //    AR.Setfailure(er.Message);
            //    return Json(AR, JsonRequestBehavior.DenyGet);
            //}


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> IsActive(int id)
        {
            var page = await _db.Jobs.FindAsync(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Job));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _db.Entry(page).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                // _jobServices.Update(page);

                AR.Data = RenderPartialViewToString("_JobItem", page);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Job));
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
        public async Task<JsonResult> Delete(int id)
        {
            var page = await _db.Jobs.FindAsync(id);
            if (page != null)
            {
              //  _jobServices.Delete(page);
                _db.Jobs.Remove(page);
                await _db.SaveChangesAsync();

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Job));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Job));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }


        //public JsonResult IsSeoNameUnique(string SeoName, int? Id)
        //{
        //    return !IsExist(SeoName, Id)
        //        ? Json(true, JsonRequestBehavior.AllowGet)
        //        : Json(false, JsonRequestBehavior.AllowGet);
        //}
        //public bool IsExist(string SeoName, int? id)
        //{
        //    return _jobServices.IsExistSeoName(SeoName, id);       
        //}


        //[HttpPost]
        //public JsonResult CreateIndex()
        //{

        //    try
        //    {
        //        var list = _jobServices.GetActiveElements().Select(m => new SearchData
        //        {
        //            Id = $"PAGE{m.Id}",
        //            Name = m.Post,
        //            Description = StringHelper.StripTagsCharArray(m.Description),
        //            Url = $"{SettingsManager.Site.SiteDomainName}/jobs/{m.SeoName}"
        //        }).ToList();

        //        LuceneHelper.AddUpdateLuceneIndex(list);

        //        AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Job));
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}
    }
}