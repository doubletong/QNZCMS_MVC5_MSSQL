using AutoMapper.QueryableExtensions;
using QNZ.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Data.Enums;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Model;
using TZGCMS.Model.Front.InputModel.Emails;
using TZGCMS.Model.Front.ViewModel;
using TZGCMS.Resources.Front;
using TZGCMS.Service.Emails;

namespace TZGCMS.SiteWeb.Controllers
{
    public class ContactController : BaseController
    {

        TZGCMS.Infrastructure.Email.IEmailService _emailService;
        private readonly IQNZDbContext _db;

        public ContactController(TZGCMS.Infrastructure.Email.IEmailService emailService, IQNZDbContext db)
        {
            _emailService = emailService;

            _db = db;
        }
        // GET: Contact
        public async Task<ActionResult> Index()
        {
            var vm = await _db.Outlets.Where(d => d.Active).OrderByDescending(d => d.Importance).ProjectTo<OutletVM>().ToListAsync();


            var url = Request.RawUrl;
            ViewBag.PageMeta = await _db.PageMetas.FirstOrDefaultAsync(d => d.ModelType == (short)ModelType.MENU && d.ObjectId == url);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SendEmail(EmailIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            if (Session["SigCaptcha"] != null && Session["SigCaptcha"].ToString().ToLower() != vm.CaptchaText.ToLower())
            {
                ModelState.AddModelError(string.Empty, "验证码不正确!");
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
                // return View(model);
            }

            var template = await _db.EmailTemplateSets.FirstOrDefaultAsync(d => d.TemplateNo == "T003");
            // _templateService.GetEmailTemplateByTemplateNo("T003");
            if (template == null)
            {
                AR.Setfailure(string.Format(Messages.NoEmailTemplate, "T003"));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


            vm.Subject = SettingsManager.Site.SiteName + "联系邮件";
            var emailBody = ReplaceTemplate(template.Body);
            emailBody = emailBody.Replace("{Name}", vm.Name);
            emailBody = emailBody.Replace("{Phone}", vm.Phone);
            emailBody = emailBody.Replace("{Email}", vm.Email);
            emailBody = emailBody.Replace("{Message}", vm.Body);

            var emailAccount = await _db.EmailAccountSets.FindAsync(template.EmailAccountId);
            //_accountService.GetById(template.EmailAccountId);

            try
            {


                EmailSet email = new EmailSet
                {
                    Body = emailBody,
                    Subject = vm.Subject,
                    MailTo = vm.Email,
                    MailCc = string.Empty,
                    Active = true,
                    CreatedBy = vm.Name,
                    CreatedDate = DateTime.Now
                };

                _db.EmailSets.Add(email);
                await _db.SaveChangesAsync();
                // _emailListService.Create(email);

                _emailService.SendMail(vm.Name, vm.Email, SettingsManager.Site.MailTo, string.Empty,
                  vm.Subject, emailBody, emailAccount.Smtpserver, emailAccount.Email, SettingsManager.Site.SiteName,
                  emailAccount.UserName, EncryptionHelper.Decrypt(emailAccount.Password), (int)emailAccount.Port, emailAccount.EnableSsl);


                AR.SetSuccess(Messages.EmailSentSuccess);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }

        public string ReplaceTemplate(string body)
        {
            body = body.Replace("{SiteName}", SettingsManager.Site.SiteName);
            body = body.Replace("{SiteURL}", SettingsManager.Site.SiteDomainName);

            return body;
        }
    }
}