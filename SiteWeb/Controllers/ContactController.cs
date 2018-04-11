using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model.Front.InputModel.Emails;
using TZGCMS.Model.Front.ViewModel;
using TZGCMS.Resources.Front;
using TZGCMS.Service.Emails;
using TZGCMS.Service.PageMetas;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ContactController : BaseController
    {
        TZGCMS.Infrastructure.Email.IEmailService _emailService;
        private readonly IEmailTemplateServices _templateService;
        private readonly IEmailServices _emailListService;
        private readonly IEmailAccountServices _accountService;
        private readonly IPageMetaServices _pageMetaService;
        public ContactController(
            IEmailTemplateServices templateService,
            IEmailServices emailListService,
            IEmailAccountServices accountService,
            TZGCMS.Infrastructure.Email.IEmailService emailService, IPageMetaServices pageMetaService)
        {
            _emailService = emailService;
            _templateService = templateService;
            _emailListService = emailListService;
            _accountService = accountService;
            _pageMetaService = pageMetaService;
        }
        // GET: Contact
        public ActionResult Index()
        {
            var url = Request.RawUrl;
            ViewBag.PageMeta = _pageMetaService.GetPageMeta(ModelType.MENU, url);
            return View();
        }

        [HttpPost]
        public JsonResult SendEmail(EmailIM vm)
        {
            var template = _templateService.GetEmailTemplateByTemplateNo("T003");
            if (template == null)
            {
                AR.Setfailure(string.Format(Messages.NoEmailTemplate, "T003"));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


            vm.Subject = SettingsManager.Site.SiteName + "联系邮件";
            var emailBody = _templateService.ReplaceTemplate(template.Body);
            emailBody = emailBody.Replace("{Name}", vm.Name);
            emailBody = emailBody.Replace("{Phone}", vm.Phone);
            emailBody = emailBody.Replace("{Email}", vm.Email);
            emailBody = emailBody.Replace("{Message}", vm.Body);

            var emailAccount = _accountService.GetById(template.EmailAccountId);

            try
            {
                _emailService.SendMail(vm.Name, SettingsManager.Site.MailTo, SettingsManager.Site.MailTo, string.Empty,
                 vm.Subject, emailBody, emailAccount.Smtpserver, emailAccount.Email, SettingsManager.Site.SiteName,
                 emailAccount.UserName, EncryptionHelper.Decrypt(emailAccount.Password), (int)emailAccount.Port, emailAccount.EnableSsl);


                Email email = new Email
                {
                    Body = emailBody,
                    Subject = vm.Subject,
                    MailTo = vm.Email,
                    MailCc = string.Empty,
                    Active = true,
                    CreatedBy = vm.Name,
                    CreatedDate = DateTime.Now
                };

                _emailListService.Create(email);

                AR.SetSuccess(Messages.EmailSentSuccess);
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