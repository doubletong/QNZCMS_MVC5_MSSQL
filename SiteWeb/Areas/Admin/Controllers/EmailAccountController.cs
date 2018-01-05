using AutoMapper;
using PagedList;
using System;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Linq;
using SiteWeb.Filters;
using TZGCMS.Service.Emails;
using TZGCMS.Model.Admin.ViewModel.Emails;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Data.Entity.Emails;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.InputModel.Emails;
using TZGCMS.Resources.Admin;

namespace SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class EmailAccountController : BaseController
    {
        private readonly IEmailAccountServices _emailAccountService;
        private readonly TZGCMS.Infrastructure.Email.IEmailService _emailService;
        private readonly IMapper _mapper;

        public EmailAccountController(
            IEmailAccountServices emailAccountService,
            TZGCMS.Infrastructure.Email.IEmailService emailService, 
            IMapper mapper)
        {
            _emailAccountService = emailAccountService;
            _emailService = emailService;
            _mapper = mapper;

        }
        // GET: Admin/EmailAccount

        #region EmailAccount

        public ActionResult Index(int? page, string keyword)
        {
            EmailAccountListVM emailAccountListVM = GetElements(page, keyword);
         
            ViewBag.PageSizes = new SelectList(Site.PageSizes());
            return View(emailAccountListVM);

        }

        private EmailAccountListVM GetElements(int? page, string keyword)
        {
            var emailAccountListVM = new EmailAccountListVM()
            {
                Keyword = keyword,
                PageIndex = (page ?? 1),
                PageSize = SettingsManager.EmailAccount.PageSize
            };
            int totalCount;
            var emailAccountlist = _emailAccountService.GetPagedElements(emailAccountListVM.PageIndex-1, emailAccountListVM.PageSize, emailAccountListVM.Keyword, out totalCount);
          
            emailAccountListVM.TotalCount = totalCount;
            emailAccountListVM.EmailAccounts = new StaticPagedList<EmailAccount>(emailAccountlist, emailAccountListVM.PageIndex, emailAccountListVM.PageSize, emailAccountListVM.TotalCount); ;
            return emailAccountListVM;
        }


        [HttpPost]
        public JsonResult PageSizeSet(int pageSize)
        {
            try
            {
                var xmlFile = Server.MapPath("~/Config/EmailAccountSettings.config");
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
        public ActionResult Add()
        {
            var emailAccount = new EmailAccountIM();          
            emailAccount.Port = 25;

            return PartialView("_Add", emailAccount);
        }



        [HttpPost]
        public JsonResult Add(EmailAccountIM vm)
        {
            vm.IsDefault = false;

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            if(!string.IsNullOrEmpty(vm.Password))
            {
                vm.Password = EncryptionHelper.Encrypt(vm.Password);
            }
            var emailAccount = _mapper.Map<EmailAccountIM, EmailAccount>(vm);
            emailAccount.CreatedBy = Site.CurrentUserName;
            emailAccount.CreatedDate = DateTime.Now;
            _emailAccountService.Create(emailAccount);

           

            int count;
            int pageSize = SettingsManager.EmailAccount.PageSize;
            var list = _emailAccountService.GetPagedElements(0, pageSize, string.Empty,out count);
         //   List<EmailAccountVM> emailAccountList = _mapper.Map<List<EmailAccount>, List<EmailAccountVM>>(list);
            AR.Data = RenderPartialViewToString("_EmailAccountList", list);

            AR.SetSuccess(String.Format(Messages.AlertCreateSuccess, EntityNames.EmailAccount));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        [HttpGet]
        public ActionResult Test(int id)
        {

            EmailAccount emailAccount = _emailAccountService.GetById(id);

            if (emailAccount == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var vm = new TestEmailIM()
            {
                AccountId = emailAccount.Id               
            };        

            return PartialView("_Test", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Test(TestEmailIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            try
            {
                var emailAccount = _emailAccountService.GetById(vm.AccountId);
                var emailAccountIM = _mapper.Map<EmailAccountIM>(emailAccount);

                if (!string.IsNullOrEmpty(emailAccountIM.Password))
                {
                    var pw = EncryptionHelper.Decrypt(emailAccountIM.Password);
                    emailAccountIM.Password = pw;
                }

                _emailService.SendMail(SettingsManager.Site.SiteName, vm.TestEmail, vm.TestEmail, string.Empty,
                    "测试", "测试邮件", emailAccountIM.SmtpServer, emailAccountIM.Email, string.Empty, emailAccountIM.UserName, 
                    emailAccountIM.Password, emailAccountIM.Port, emailAccountIM.EnableSsl);

                AR.SetSuccess(String.Format(Messages.AlertSendSuccess, EntityNames.EmailAccount));
                return Json(AR, JsonRequestBehavior.DenyGet);

            }catch(Exception er)
            {
                AR.Setfailure(er.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
           

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {

            EmailAccount emailAccount = _emailAccountService.GetById(id);

            if (emailAccount == null)
            {
                AR.Setfailure(Messages.HttpNotFound);
                return Json(AR, JsonRequestBehavior.AllowGet);
            }

            var vm = _mapper.Map<EmailAccountIM>(emailAccount);

            if (!string.IsNullOrEmpty(vm.Password))
            {
                vm.Password = EncryptionHelper.Decrypt(vm.Password);
            }

            return PartialView("_Edit", vm);
        }

      


        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Edit(EmailAccountIM vm)
        {

            if (!ModelState.IsValid)
            {
                AR.Setfailure(GetModelErrorMessage());
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            var emailAccount = _emailAccountService.GetById(vm.Id);
         
            emailAccount.Email = vm.Email;
            emailAccount.Port = vm.Port;
            emailAccount.UserName = vm.UserName;
            emailAccount.Smtpserver = vm.SmtpServer;
            emailAccount.EnableSsl = vm.EnableSsl;
            emailAccount.UpdatedBy = Site.CurrentUserName;
            emailAccount.UpdatedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(vm.Password))
            {
                var pw = EncryptionHelper.Encrypt(vm.Password);
                emailAccount.Password = pw;
            }
        
            _emailAccountService.Update(emailAccount);
            
           // var emailAccountVM = _mapper.Map<EmailAccountVM>(emailAccount);

            AR.Id = emailAccount.Id;
            AR.Data = RenderPartialViewToString("_EmailAccountItem", emailAccount);

            AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.EmailAccount));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }

        // DELETE: /User/DeleteSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {

            EmailAccount emailAccount = _emailAccountService.GetById(id);
                     

            _emailAccountService.Delete(emailAccount);
            AR.SetSuccess(String.Format(Messages.AlertDeleteSuccess, EntityNames.EmailAccount));
            return Json(AR, JsonRequestBehavior.DenyGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsDefault(int aid,int pageIndex,string keyword)
        {
            try
            { 

                var emailAccount = _emailAccountService.SetDefault(aid);
                if (emailAccount == null)
                {
                    AR.Setfailure(Messages.AlertUpdateFailure);
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }

                //var vm = _mapper.Map<EmailAccountVM>(emailAccount);
                //AR.Data = RenderPartialViewToString("_EmailAccountItem", vm);

                int count;
                int pageSize = SettingsManager.EmailAccount.PageSize;
                var list = _emailAccountService.GetPagedElements(pageIndex-1, pageSize, keyword, out count);

                // List<EmailAccountVM> emailAccountList = _mapper.Map<List<EmailAccount>, List<EmailAccountVM>>(list);
                AR.Data = RenderPartialViewToString("_EmailAccountList", list);

                AR.SetSuccess(String.Format(Messages.AlertUpdateSuccess, EntityNames.EmailAccount));
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        #endregion
    }
}