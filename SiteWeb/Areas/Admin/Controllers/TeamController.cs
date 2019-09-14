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

using TZGCMS.Model.Admin.ViewModel.Teams;
using TZGCMS.Resources.Admin;
using TZGCMS.Service.PageMetas;
using TZGCMS.Service.Teams;
using TZGCMS.Data.Entity;
using PagedList;
using TZGCMS.Model.Admin.InputModel.Teams;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class TeamController : BaseController
    {
        private readonly ITeamServices _teamServices;
        private readonly IPageMetaServices _pageMetaServices;
        private IMapper _mapper;
        public TeamController(ITeamServices teamServices, IPageMetaServices pageMetaServices, IMapper mapper)
        {
            _teamServices = teamServices;
            _pageMetaServices = pageMetaServices;
            _mapper = mapper;
        }
       
        // GET: Admin/Teams
        public ActionResult Index(int? page, string keyword)
        {
            TeamListVM pageListVM = GetElements(page, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(pageListVM);

        }

        private TeamListVM GetElements(int? page, string keyword)
        {
            var pageListVM = new TeamListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Team.PageSize
            };
            int totalCount;
            var pagelist = _teamServices.GetTeamdElements(pageListVM.PageIndex - 1, pageListVM.PageSize, pageListVM.Keyword, out totalCount);

            pageListVM.TotalCount = totalCount;
            pageListVM.Teams = new StaticPagedList<Team>(pagelist, pageListVM.PageIndex, pageListVM.PageSize, pageListVM.TotalCount); ;
            return pageListVM;
        }
        [HttpPost]
        public JsonResult TeamSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/TeamSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("TeamSize").SetValue(pageSize);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        // GET: Admin/Teams/Create
        public ActionResult Create()
        {
            TeamIM team = new TeamIM()
            {
                Active = true
            };
            return PartialView("_Create", team);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Create(TeamIM team)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var newTeam = _mapper.Map<TeamIM, Team>(team);              

                var result = _teamServices.Create(newTeam);
               


                int count;
                int pageSize = SettingsManager.Team.PageSize;
                var list = _teamServices.GetTeamdElements(0, pageSize, string.Empty, out count);

                AR.Data = RenderPartialViewToString("_TeamList", list);

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Team));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        // GET: Admin/Teams/Edit/5
        public ActionResult Edit(int id)
        {

            var page = _teamServices.GetById(id);
            if (page == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
                // return HttpNotFound();
            }

            var editTeam = _mapper.Map<Team, TeamIM>(page);           

            return PartialView("_Edit", editTeam);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TeamIM page)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var old = _teamServices.GetById(page.Id);
                var editTeam = _mapper.Map(page, old);

                

                _teamServices.Update(editTeam);

              

                // var pageVM = _mapper.Map<TeamVM>(editTeam);

                AR.Id = page.Id;
                AR.Data = RenderPartialViewToString("_TeamItem", editTeam);

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Team));
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
            var page = _teamServices.GetById(id);
            if (page == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Team));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                page.Active = !page.Active;
                _teamServices.Update(page);

                AR.Data = RenderPartialViewToString("_TeamItem", page);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Team));
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
            var page = _teamServices.GetById(id);
            if (page != null)
            {
                _teamServices.Delete(page);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Team));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Team));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }





        //[HttpPost]
        //public JsonResult CreateIndex()
        //{

        //    try
        //    {
        //        var list = _teamServices.GetActiveElements().Select(m => new SearchData
        //        {
        //            Id = $"PAGE{m.Id}",
        //            Name = m.Post,
        //            Description = StringHelper.StripTagsCharArray(m.Description),
        //            Url = $"{SettingsManager.Site.SiteDomainName}/teams/{m.SeoName}"
        //        }).ToList();

        //        GoLucene.AddUpdateLuceneIndex(list);

        //        AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Team));
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