using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PagedList;
using System;
using System.Xml.Linq;
using SiteWeb.Filters;
using TZGCMS.Service.Emails;
using TZGCMS.Model.Admin.ViewModel.Emails;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.InputModel.Emails;
using TZGCMS.Resources.Admin;

namespace SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class EmailTemplateController : BaseController
    {
        private readonly IEmailAccountServices _accountService;
        private readonly IEmailTemplateServices _templateService;
      
        private readonly IMapper _mapper;

        public EmailTemplateController(IEmailAccountServices accountService, IEmailTemplateServices templateService,IMapper mapper)
        {
            _accountService = accountService;
            _templateService = templateService;      
            _mapper = mapper;


        }
        #region Email Templates


        [HttpGet]
        public ActionResult Index(int? page, int? accountId, string Keyword)
        {

            var templateListVM = new EmailTemplateListVM()
            {
                AccountId = accountId ?? 0,
                PageIndex = page ?? 1,
                PageSize = SettingsManager.EmailTemplate.PageSize,
                Keyword = Keyword
            };
            int count;
            var templates = _templateService.GetPagedElements(templateListVM.PageIndex-1, templateListVM.PageSize, templateListVM.Keyword, templateListVM.AccountId, out count);
            foreach(var item  in templates)
            {
                item.EmailAccount = _accountService.GetById(item.EmailAccountId);
            }
            // var templateDtos = _mapper.Map<IEnumerable<EmailTemplate>, IEnumerable<EmailTemplateVM>>(templates.AsEnumerable());
            // templateListVM.EmailTemplates = templateDtos;

            templateListVM.TotalCount = count;

            var accountList = _accountService.GetElements().OrderByDescending(c => c.Email).ToList();
            var categories = new SelectList(accountList, "Id", "Email");
            ViewBag.Categories = categories;

            templateListVM.EmailTemplates = new StaticPagedList<EmailTemplate>(templates, templateListVM.PageIndex, templateListVM.PageSize, templateListVM.TotalCount);
            //  ViewBag.OnePageOfEmailTemplates = templatesAsIPagedList;

            ViewBag.PageSizes = new SelectList(Site.PageSizes());

            return View(templateListVM);

        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/EmailTemplateSettings.config");
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


        public ActionResult Add()
        {
            var template = new EmailTemplateIM();

            var accounts = _accountService.GetElements().OrderByDescending(m => m.Email).ToList();
            var lCategorys = new SelectList(accounts, "Id", "Email");

            ViewBag.Categories = lCategorys;
            return View(template);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(EmailTemplateIM template)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {

                var newEmailTemplate = _mapper.Map<EmailTemplateIM, EmailTemplate>(template);
                newEmailTemplate.CreatedBy = Site.CurrentUserName;
                newEmailTemplate.CreatedDate = DateTime.Now;

                _templateService.Create(newEmailTemplate);

                AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.EmailTemplate));
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            var vEmailTemplate = _templateService.GetById(Id);
            if (vEmailTemplate == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }


            var editEmailTemplate = _mapper.Map<EmailTemplate, EmailTemplateIM>(vEmailTemplate);          

            var accounts = _accountService.GetElements().OrderByDescending(m => m.Email).ToList();
            var lCategorys = new SelectList(accounts, "Id", "Email");

            ViewBag.Categories = lCategorys;

            return View(editEmailTemplate);


        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(EmailTemplateIM vm)
        {
            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var editEmailTemplate = _templateService.GetById(vm.Id);
                editEmailTemplate.Subject = vm.Subject;
                editEmailTemplate.TemplateNo = vm.TemplateNo;
                editEmailTemplate.Body = vm.Body;
                editEmailTemplate.UpdatedBy = Site.CurrentUserName;
                editEmailTemplate.UpdatedDate = DateTime.Now;
                //var editEmailTemplate = _mapper.Map<EmailTemplateIM, EmailTemplate>(template);

                _templateService.Update(editEmailTemplate);

             

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.EmailTemplate));
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult Active(int id)
        //{

        //    EmailTemplate vEmailTemplate = _templateService.GetById(id);

        //    try
        //    {
        //        vEmailTemplate.Active = !vEmailTemplate.Active;
        //        _templateService.Update(vEmailTemplate);

        //        var vm = _mapper.Map<EmailTemplateVM>(vEmailTemplate);

        //        AR.Data = RenderPartialViewToString("_EmailTemplateItem", vm);
        //        AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.EmailTemplate));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        AR.Setfailure(ex.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult Recommend(int id)
        //{

        //    EmailTemplate vEmailTemplate = _templateService.GetById(id);

        //    try
        //    {
        //        vEmailTemplate.Recommend = !vEmailTemplate.Recommend;
        //        _templateService.Update(vEmailTemplate);

        //        var vm = _mapper.Map<EmailTemplateVM>(vEmailTemplate);
        //        AR.Data = RenderPartialViewToString("_EmailTemplateItem", vm);
        //        AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.EmailTemplate));
        //        return Json(AR, JsonRequestBehavior.DenyGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        AR.Setfailure(ex.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {

            EmailTemplate vEmailTemplate = _templateService.GetById(id);

            if (vEmailTemplate == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            _templateService.Delete(vEmailTemplate);

            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.EmailTemplate));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [AllowAnonymous]
        public JsonResult IsNoUnique(string TemplateNo, int? Id)
        {
            return !IsExist(TemplateNo, Id)
                ? Json(true, JsonRequestBehavior.AllowGet)
                : Json(false, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public bool IsExist(string TemplateNo, int? id)
        {
           
            return _templateService.IsExistTemplate(TemplateNo,id.Value);          

        }

        #endregion


    }
}
