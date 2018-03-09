using AutoMapper;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.LuceneSearch;
using TZGCMS.Model.Admin.ViewModel.Jobs;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.LuceneSearch;
using TZGCMS.Service.PageMetas;
using TZGCMS.Service.Jobs;
using TZGCMS.Data.Entity;
using PagedList;
using TZGCMS.Model.Admin.InputModel.Jobs;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class JobController : BaseController
    {
        private readonly IJobServices _jobServices;
        private readonly IPageMetaServices _pageMetaServices;
        private IMapper _mapper;
        public JobController(IJobServices jobServices, IPageMetaServices pageMetaServices, IMapper mapper)
        {
            _jobServices = jobServices;
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;
        }
       
        // GET: Admin/Jobs
        public ActionResult Index(int? page, string keyword)
        {
            JobListVM pageListVM = GetElements(page, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(pageListVM);

        }

        private JobListVM GetElements(int? page, string keyword)
        {
            var pageListVM = new JobListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Job.PageSize
            };
            int totalCount;
            var pagelist = _jobServices.GetJobdElements(pageListVM.PageIndex - 1, pageListVM.PageSize, pageListVM.Keyword, out totalCount);

            pageListVM.TotalCount = totalCount;
            pageListVM.Jobs = new StaticPagedList<Job>(pagelist, pageListVM.PageIndex, pageListVM.PageSize, pageListVM.TotalCount); ;
            return pageListVM;
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


        // GET: Admin/Jobs/Create
        public ActionResult Create()
        {
            JobIM job = new JobIM()
            {
                Active = true
            };
            return PartialView("_Create", job);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Create(JobIM job)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            //try
            //{
                var newJob = _mapper.Map<JobIM, Job>(job);              

                var result = _jobServices.Create(newJob);
                if (result!=null)
                {
                    var pageMeta = new PageMeta()
                    {
                        ObjectId = result.ToString(),
                        Title = string.IsNullOrEmpty(job.SEOTitle) ? job.Post : job.SEOTitle,
                        Keyword = string.IsNullOrEmpty(job.Keywords) ? job.Post : job.Keywords.Replace('，', ','),
                        Description = job.SEODescription,
                        ModelType = ModelType.JOB
                    };
                    _pageMetaServices.Create(pageMeta);
                }


                int count;
                int pageSize = SettingsManager.Job.PageSize;
                var list = _jobServices.GetJobdElements(0, pageSize, string.Empty, out count);

                AR.Data = RenderPartialViewToString("_JobList", list);

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Job));
                return Json(AR, JsonRequestBehavior.DenyGet);
            //}
            //catch (Exception er)
            //{
            //    AR.Setfailure(er.Message);
            //    return Json(AR, JsonRequestBehavior.DenyGet);
            //}

        }

        // GET: Admin/Jobs/Edit/5
        public ActionResult Edit(int id)
        {

            var page = _jobServices.GetById(id);
            if (page == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
                // return HttpNotFound();
            }

            var editJob = _mapper.Map<Job, JobIM>(page);

            var pageMeta = _pageMetaServices.GetPageMeta(ModelType.JOB, editJob.Id.ToString());
            if (pageMeta != null)
            {
                editJob.SEOTitle = pageMeta.Title;
                editJob.Keywords = pageMeta.Keyword;
                editJob.SEODescription = pageMeta.Description;
            }

            return PartialView("_Edit", editJob);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(JobIM page)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var old = _jobServices.GetById(page.Id);
                var editJob = _mapper.Map(page, old);

                

                _jobServices.Update(editJob);

                var pageMeta = _pageMetaServices.GetPageMeta(ModelType.PAGE, editJob.Id.ToString());
                pageMeta = pageMeta ?? new PageMeta();

                pageMeta.ObjectId = page.Id.ToString();
                pageMeta.Title = string.IsNullOrEmpty(page.SEOTitle) ? page.Post : page.SEOTitle;
                pageMeta.Keyword = string.IsNullOrEmpty(page.Keywords) ? page.Post : page.Keywords.Replace('，', ',');
                pageMeta.Description = page.SEODescription;
                pageMeta.ModelType = ModelType.PAGE;

                if (pageMeta.Id > 0)
                {
                    _pageMetaServices.Update(pageMeta);
                }
                else
                {
                    _pageMetaServices.Create(pageMeta);
                }

                // var pageVM = _mapper.Map<JobVM>(editJob);

                AR.Id = page.Id;
                AR.Data = RenderPartialViewToString("_JobItem", editJob);

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Job));
                return Json(AR, JsonRequestBehavior.DenyGet);
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
            var page = _jobServices.GetById(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Job));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _jobServices.Update(page);

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
        public JsonResult Delete(int id)
        {
            var page = _jobServices.GetById(id);
            if (page != null)
            {
                _jobServices.Delete(page);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Job));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Job));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }


        public JsonResult IsSeoNameUnique(string SeoName, int? Id)
        {
            return !IsExist(SeoName, Id)
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(false, JsonRequestBehavior.AllowGet);
        }
        public bool IsExist(string SeoName, int? id)
        {
            return _jobServices.IsExistSeoName(SeoName, id);       
        }


        [HttpPost]
        public JsonResult CreateIndex()
        {

            try
            {
                var list = _jobServices.GetActiveElements().Select(m => new SearchData
                {
                    Id = $"PAGE{m.Id}",
                    Name = m.Post,
                    Description = StringHelper.StripTagsCharArray(m.Description),
                    Url = $"{SettingsManager.Site.SiteDomainName}/jobs/{m.SeoName}"
                }).ToList();

                GoLucene.AddUpdateLuceneIndex(list);

                AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Job));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }
    }
}