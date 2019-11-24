using AutoMapper;
using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using PagedList;
using QNZ.Data;
using TZGCMS.Model;
using System.Data.Entity;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class OutletController :QNZBaseController
    {
        private IQNZDbContext _db;
        private IMapper _mapper;
        public OutletController(IMapper mapper, IQNZDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }


        //private readonly IOutletServices _teamServices;
        //private readonly IPageMetaServices _pageMetaServices;
        //private IMapper _mapper;
        //public OutletController(IOutletServices teamServices, IPageMetaServices pageMetaServices, IMapper mapper)
        //{
        //    _teamServices = teamServices;
        //    _pageMetaServices = pageMetaServices;
        //    _mapper = mapper;
        //}
       
        // GET: Admin/Outlets
        public async Task<ActionResult> Index(int? page, string keyword)
        {
            OutletListVM pageListVM = await GetElementsAsync(page, keyword);
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(pageListVM);

        }

        private async Task<OutletListVM> GetElementsAsync(int? page, string keyword)
        {
            var vm = new OutletListVM()
            {
                Keyword = keyword,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.Outlet.PageSize
            };

            var query = _db.Outlets.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Name.Contains(keyword) || d.Address.Contains(keyword));
            }



            var list = await query.AsNoTracking().OrderByDescending(d => d.Importance)
                .Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ProjectTo<OutletVM>().ToListAsync();
                //_teamServices.GetOutletdElements(pageListVM.PageIndex - 1, pageListVM.PageSize, pageListVM.Keyword, out totalCount);

            vm.TotalCount = await query.CountAsync();
            vm.Outlets = new StaticPagedList<OutletVM>(list, vm.PageIndex, vm.PageSize, vm.TotalCount); ;
            return vm;
        }
        [HttpPost]
        public JsonResult OutletSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/OutletSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("OutletSize").SetValue(pageSize);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }


        // GET: Admin/Outlets/Create
        //public ActionResult Create()
        //{
        //    OutletIM team = new OutletIM()
        //    {
        //        Active = true
        //    };
        //    return PartialView("_Create", team);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //public JsonResult Create(OutletIM team)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        AR.Setfailure(GetModelErrorMessage());
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    try
        //    {
        //        var newOutlet = _mapper.Map<OutletIM, QNZ.Data.Outlet>(team);
        //        var result = _teamServices.Create(newOutlet);  

        //        int count;
        //        int pageSize = SettingsManager.Outlet.PageSize;
        //        var list = _teamServices.GetOutletdElements(0, pageSize, string.Empty, out count);

        //        AR.Data = RenderPartialViewToString("_OutletList", list);

        //        AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Outlet));
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }
        //    catch (Exception er)
        //    {
        //        AR.Setfailure(er.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}

        // GET: Admin/Outlets/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            OutletIM im = new OutletIM()
            {
                Active = true
            };
            if (id == null)
            {
                return View(im);
            }
            else
            {
                var model = await _db.Outlets.FindAsync(id);
                if (model == null)
                {
                    //AR.Setfailure(Messages.HttpNotFound);
                    //return Json(AR, JsonRequestBehavior.DenyGet);
                    return HttpNotFound();
                }

                im = _mapper.Map<Outlet, OutletIM>(model);

                return View(im);
            }

         

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(OutletIM im)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                if (im.Id > 0)
                {
                    var model = await _db.Outlets.FindAsync(im.Id);
                    model = _mapper.Map(im, model);

                    _db.Entry(model).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    //_teamServices.Update(editOutlet);
                    // var pageVM = _mapper.Map<OutletVM>(editOutlet);

                    //AR.Id = im.Id;
                    //AR.Data = RenderPartialViewToString("_OutletItem", model);

                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Outlet));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var newOutlet = _mapper.Map<OutletIM, Outlet>(im);
                    newOutlet.CreatedBy = Site.CurrentUserName;
                    newOutlet.CreatedDate = DateTime.Now;

                    _db.Outlets.Add(newOutlet);
                    await _db.SaveChangesAsync();

                    //int count;
                    //int pageSize = SettingsManager.Outlet.PageSize;
                    //var list = _teamServices.GetOutletdElements(0, pageSize, string.Empty, out count);

                    //AR.Data = RenderPartialViewToString("_OutletList", list);

                    AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Outlet));
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
        public async Task<JsonResult> IsActive(int id)
        {
            var model = await _db.Outlets.FindAsync(id);
            if (model == null)
            {
                AR.Setfailure(String.Format(Messages.AlertUpdateSuccess, EntityNames.Outlet));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                model.Active = !model.Active;
                _db.Entry(model).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                // _teamServices.Update(model);

                AR.Data = RenderPartialViewToString("_OutletItem", model);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Outlet));
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
            var model = await _db.Outlets.FindAsync(id);
            if (model != null)
            {
                _db.Outlets.Remove(model);

                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Outlet));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Outlet));
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

        //        AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Outlet));
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