using AutoMapper;
using PagedList;
using System;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Linq;
using TZGCMS.SiteWeb.Filters;
using TZGCMS.Service.Emails;
using TZGCMS.Model.Admin.ViewModel.Emails;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Resources.Admin;
using TZGCMS.Model.Admin.InputModel.Emails;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class EmailController : BaseController
    {
        private readonly IEmailServices _emailServices;
        private readonly TZGCMS.Infrastructure.Email.IEmailService _emailingService;
        private readonly IEmailAccountServices _emailAccountServices;
        private readonly IMapper _mapper;

        public EmailController(IEmailServices emailServices , IEmailAccountServices emailAccountServices, TZGCMS.Infrastructure.Email.IEmailService emailingService, IMapper mapper)
        {
            _emailServices = emailServices;
            _emailingService = emailingService;
            _emailAccountServices = emailAccountServices;
            _mapper = mapper;

        }
        // GET: Admin/Email

        #region Email

        public ActionResult Index(int? page, string keyword)
        {
            EmailListVM emailListVM = GetElements(page, keyword,true);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(emailListVM);

        }

        public ActionResult Trash(int? page, string keyword)
        {
            EmailListVM emailListVM = GetElements(page, keyword, false);

            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(emailListVM);

        }

        private EmailListVM GetElements(int? page, string keyword,bool active)
        {
            var emailListVM = new EmailListVM();

            emailListVM.Keyword = keyword;
            emailListVM.PageIndex = (page ?? 1);
            emailListVM.PageSize = SettingsManager.Email.PageSize;
            int totalCount;
            var emaillist = _emailServices.GetPagedElements(emailListVM.PageIndex - 1, emailListVM.PageSize, emailListVM.Keyword, active, out totalCount);
           // var emailVMList = _mapper.Map<List<Email>, List<EmailVM>>(emaillist);
            emailListVM.TotalCount = totalCount;
            emailListVM.Emails = new StaticPagedList<Email>(emaillist, emailListVM.PageIndex, emailListVM.PageSize, emailListVM.TotalCount); ;
            return emailListVM;
        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/EmailSettings.config");
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
        public ActionResult Reply(int id)
        {

            Email email = _emailServices.GetById(id);
            if (email == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            EmailIM vm = new EmailIM()
            {
                Subject = $"回复：{email.Subject}",
                MailTo = email.MailTo,
                Body = $"<p></p><blockquote>{email.Body}</blockquote>"
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]

        public JsonResult Reply(EmailIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var smtp = _emailAccountServices.GetElements().Where(m => m.IsDefault).FirstOrDefault();

            if (smtp == null)
            {
                AR.SetWarning(Messages.SetDefaultEmailAccount);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            try
            {
                _emailingService.SendMail(SettingsManager.Site.SiteName, SettingsManager.Site.MailTo, vm.MailTo, string.Empty, vm.Subject, vm.Body,
                    smtp.Smtpserver, smtp.Email, SettingsManager.Site.SiteName, smtp.UserName, EncryptionHelper.Decrypt(smtp.Password), smtp.Port, smtp.EnableSsl);

                AR.SetSuccess(String.Format(Messages.AlertSendSuccess, EntityNames.Email));
                return Json(AR, JsonRequestBehavior.DenyGet);

            }
            catch(Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            //var result =  _emailService.SendMail(SettingsManager.Site.SiteName, vm.MailTo, vm.Subject, vm.Body);
            
        }

        // DELETE: /User/DeleteSite
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult Delete(int id)
        //{

        //    Email vCategory = _emailService.GetById(id);


        //    _emailService.Delete(vCategory);
        //    AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Email));
        //    return Json(AR, JsonRequestBehavior.DenyGet);

        //}
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult BulkRemove(int[] ids)
        {
            bool result = _emailServices.DeleteEmails(ids);

            if (result)
            {
                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Email));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Email));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult BulkTrash(int[] ids)
        {
           bool result = _emailServices.IsTrashEmails(ids,false);

            if (result)
            {
                AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.Email));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertDeleteFailure, EntityNames.Email));
            return Json(AR, JsonRequestBehavior.DenyGet);           
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult BulkRestore(int[] ids)
        {
            bool result = _emailServices.IsTrashEmails(ids,true);

            if (result)
            {
                AR.SetSuccess(String.Format(Messages.AlertRestoreSuccess, EntityNames.Email));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            AR.Setfailure(String.Format(Messages.AlertRestoreFailure, EntityNames.Email));
            return Json(AR, JsonRequestBehavior.DenyGet);
        }

       
        public ActionResult Detail(int id)
        {
            var email = _emailServices.GetById(id);
            email.Readed = true;
            _emailServices.Update(email);

            if (email == null)
                return HttpNotFound();

         //   EmailVM vm = _mapper.Map<EmailVM>(email);

            return View(email);
        }


        #endregion
    }
}