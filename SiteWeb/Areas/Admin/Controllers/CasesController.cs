using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;

using System;
using System.Xml.Linq;
using System.Text;
using TZGCMS.Service.PageMetas;

using TZGCMS.Infrastructure.Configs;

using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;

using TZGCMS.Data.Entity.PageMetas;
using TZGCMS.Data.Enums;
using TZGCMS.Resources.Admin;


using TZGCMS.SiteWeb.Filters;
using TZGCMS.Model;
using System.Data.Entity;
using TZGCMS.Data.Entity;
using TZGCMS.Model.Search;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class CasesController : BaseController
    {

        private readonly IMapper _mapper;

        public CasesController(IMapper mapper)
        {     
            _mapper = mapper;
        }
        #region Cases


        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Index(int? page, string Keyword)
        {

            var vm = new CaseListVM
            {
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Case.PageSize,
                Keyword = Keyword
            };

            var query = _db.Cases.AsQueryable();

            if (!string.IsNullOrEmpty(Keyword))
            {
                query = query.Where(d => d.Title.Contains(Keyword) || d.Body.Contains(Keyword));
            }

            vm.TotalCount = await query.CountAsync();


            var cases = await query.OrderByDescending(d=>d.Pubdate).ThenByDescending(d=>d.Id).Skip((vm.PageIndex - 1) * vm.PageSize).Take(vm.PageSize).ToListAsync();
                //_articleServices.GetPagedElements(vm.PageIndex-1, vm.PageSize,  vm.Keyword, (int)vm.CategoryId, out count);

  
            vm.Cases = new StaticPagedList<Case>(cases, vm.PageIndex, vm.PageSize, vm.TotalCount);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(vm);

        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/CaseSettings.config");
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
        public async System.Threading.Tasks.Task<ActionResult> Edit(int? id)
        {           

            if (id > 0)
            {
                var vCase = await _db.Cases.FindAsync(id);
                if (vCase == null)
                {
                    AR.Setfailure(Messages.HttpNotFound);
                    return Json(AR, JsonRequestBehavior.AllowGet);
                }

                var editCase = _mapper.Map<Case, CaseIM>(vCase);

                var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.CASE && d.ObjectId == editCase.Id.ToString());
                if (pageMeta != null)
                {
                    editCase.SEOTitle = pageMeta.Title;
                    editCase.Keywords = pageMeta.Keyword;
                    editCase.SEODescription = pageMeta.Description;
                }

                return View(editCase);

            }
            else
            {
                var vm = new CaseIM
                {
                    Active = true,
                    Pubdate = DateTime.Now
                };
                return View(vm);
            }
           
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<JsonResult> Edit(CaseIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            //try
            //{
                if (vm.Id > 0)
                {
                    var editCase = await _db.Cases.FindAsync(vm.Id);

                    editCase = _mapper.Map(vm, editCase);

                    _db.Entry(editCase).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    var pageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == ModelType.CASE && d.ObjectId == editCase.Id.ToString());         

                    pageMeta = pageMeta ?? new PageMeta();
                    pageMeta.ObjectId = vm.Id.ToString();
                    pageMeta.Title = string.IsNullOrEmpty(vm.Title) ? vm.Title : vm.Title;
                    pageMeta.Keyword = string.IsNullOrEmpty(vm.Keywords) ? vm.Title : vm.Keywords.Replace('，', ',');
                    pageMeta.Description = vm.SEODescription;
                    pageMeta.ModelType = ModelType.CASE;

                    if (pageMeta.Id > 0)
                    {
                        _db.Entry(pageMeta).State = EntityState.Modified;
                    }
                    else
                    {
                        _db.PageMetas.Add(pageMeta);
                    }
                    await _db.SaveChangesAsync();            



                    AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Case));
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var newCase = _mapper.Map<CaseIM, Case>(vm);

                    newCase = _db.Cases.Add(newCase);
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
                                ModelType = ModelType.CASE,
                                ObjectId = newCase.Id.ToString()
                            };
                            _db.PageMetas.Add(pm);
                            await _db.SaveChangesAsync();
                        }
                    }

                    AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.Case));
                    return Json(AR, JsonRequestBehavior.DenyGet);

                }
               
               

            //}
            //catch (Exception er)
            //{
            //    AR.Setfailure(er.Message);
            //    return Json(AR, JsonRequestBehavior.DenyGet);
            //}


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<JsonResult> Active(int id)
        {

            Case vCase = await _db.Cases.FindAsync(id);
          
            try
            {
                vCase.Active = !vCase.Active;

                _db.Entry(vCase).State = EntityState.Modified;
                await _db.SaveChangesAsync();
               
                
                AR.Data = RenderPartialViewToString("_CaseItem", vCase);
                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.Case));
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
        public async System.Threading.Tasks.Task<JsonResult> Delete(int id)
        {

            Case vCase = await _db.Cases.FindAsync(id);

            if (vCase == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _db.Cases.Remove(vCase);
            await _db.SaveChangesAsync();

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Case));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }
        
        /// <summary>
        /// 创建文章索引
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateIndex()
        {
            try
            {
                var cases = _db.Cases.Where(d => d.Active == true).ToList();
                var list = cases.Select(m => new SearchData
                {
                    Id = "CASE"+m.Id,
                    Name = m.Title,
                    Description = string.IsNullOrEmpty(m.Summary)? StringHelper.StripTagsCharArray(m.Body) : m.Summary,
                    ImageUrl = string.IsNullOrEmpty(m.Thumbnail)?string.Empty: m.Thumbnail,
                    Url =  "/cases/detail/" + m.Id,
                    CreatedDate = m.Pubdate
                }).ToList();
            
                LuceneHelper.AddUpdateLuceneIndex(list);
               
                AR.SetSuccess(String.Format(Messages.AlertActionSuccess, EntityNames.Case));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        #endregion


    }
}
