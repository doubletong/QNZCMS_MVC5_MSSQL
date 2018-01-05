﻿using SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Model.Admin.InputModel;
using TZGCMS.Model.Admin.ViewModel;

namespace SiteWeb.Areas.Admin.Controllers
{

    [SIGAuth]
    public class InfoController : BaseController
    {
        ILoggingService _logger;
        public InfoController(ILoggingService logger)
        {           
            _logger = logger;
        }
        public ViewResult Index()
        {
          
            var cfe = SettingsManager.Contact;
            var sle = SettingsManager.Social;

            CompanyInfoIM com = new CompanyInfoIM
            {
                Contact = new ContactIM
                {
                    CompanyName = cfe.CompanyName,
                    CompanyShortName = cfe.CompanyShortName,
                    Address = cfe.Address,
                    Coordinate = cfe.Coordinate,
                    ContactMan = cfe.ContactMan,
                    Fax = cfe.Fax,
                    Phone = cfe.Phone,
                    ZipCode = cfe.ZipCode,
                    Mobile = cfe.Mobile,
                    MailTo = cfe.MailTo,
                    MailCC = cfe.MailCC,
                },
                Social = new SocialIM
                {
                    Oicq = sle.Oicq,
                    OicqTwo = sle.OicqTwo,
                    SinaWeibo = sle.SinaWeibo,
                    WeiXing = sle.WeiXing,
                    WeiXingCode = sle.WeiXingCode
                }
            };



            return View(com);
        }


        [HttpPost]
        public ActionResult Contact(ContactIM vm)
        {
            _logger.Info("创建数据库备份：");
            if (!ModelState.IsValid)
            {
                var errorMessage = GetModelErrorMessage();

                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            try
            {
                var xmlFile = Server.MapPath("~/Config/ContactSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("CompanyName").SetValue(vm.CompanyName);
                item.Element("CompanyShortName").SetValue(vm.CompanyShortName ?? "");
                item.Element("Address").SetValue(vm.Address);
                item.Element("Coordinate").SetValue(vm.Coordinate ?? "");
                item.Element("ContactMan").SetValue(vm.ContactMan);
                item.Element("Fax").SetValue(vm.Fax ?? "");
                item.Element("Phone").SetValue(vm.Phone);
                item.Element("ZipCode").SetValue(vm.ZipCode ?? "");
                item.Element("Mobile").SetValue(vm.Mobile ?? "");
                item.Element("MailTo").SetValue(vm.MailTo);
                item.Element("MailCC").SetValue(vm.MailCC ?? "");
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            //TempData["message"] = "保存成功！";          

        }


        [HttpPost]
        public JsonResult Social(SocialIM vm)
        {
            if (!ModelState.IsValid)
            {

                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/SocialSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("Oicq").SetValue(vm.Oicq ?? "");
                item.Element("OicqTwo").SetValue(vm.OicqTwo ?? "");
                item.Element("SinaWeibo").SetValue(vm.SinaWeibo ?? "");
                item.Element("WeiXing").SetValue(vm.WeiXing ?? "");
                item.Element("WeiXingCode").SetValue(vm.WeiXingCode ?? "");

                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }


        }





        public ViewResult Site()
        {

            var info = SettingsManager.Site;
            var sle = SettingsManager.Article;
            var blog = SettingsManager.Blog;
            var caseSet = SettingsManager.Case;
            var productSet = SettingsManager.Product;
            var weixinSet = SettingsManager.WeiXin;
            var videoSet = SettingsManager.Video;

            ModuleSetIM vm = new ModuleSetIM
            {
               
                SiteInfo = new SiteInfoIM
                {
                    SiteName = info.SiteName,
                    SiteDomainName = info.SiteDomainName,
                    WebNumber = info.WebNumber,
                    BaiduSiteID = info.BaiduSiteID,
                    IsClose = info.IsClose,
                    CloseInfo = info.CloseInfo,
                    GoogleAnalyticsID = info.GoogleAnalyticsID,
                    LoginLogo = info.LoginLogo,
                    DashboardLogo = info.DashboardLogo,
                },
                ProductSet = new ProductSetIM
                {
                    EnableCaching = productSet.EnableCaching,
                    CacheDuration = productSet.CacheDuration,
                    FrontPageSize = productSet.FrontPageSize,
                    ThumbHeight = productSet.ThumbHeight,
                    ThumbWidth = productSet.ThumbWidth,
                    ImageHeight = productSet.ImageHeight,
                    ImageWidth = productSet.ImageWidth,
                    CategoryImageWidth = productSet.CategoryImageWidth,
                    CategoryImageHeight = productSet.CategoryImageHeight
                },
                ArticleSet = new ArticleSetIM
                {
                    EnableCaching = sle.EnableCaching,
                    CacheDuration = sle.CacheDuration,
                    FrontPageSize = sle.FrontPageSize,
                    ThumbHeight = sle.ThumbHeight,
                    ThumbWidth = sle.ThumbWidth,
                    ImageHeight = sle.ImageHeight,
                    ImageWidth = sle.ImageWidth

                },
                PostSet = new PostSetIM
                {
                    EnableCaching = blog.EnableCaching,
                    CacheDuration = blog.CacheDuration,
                    FrontPageSize = blog.FrontPageSize,
                    ThumbHeight = blog.ThumbHeight,
                    ThumbWidth = blog.ThumbWidth
                },
                CaseSet = new CaseSetIM
                {
                    EnableCaching = caseSet.EnableCaching,
                    CacheDuration = caseSet.CacheDuration,
                    FrontPageSize = caseSet.FrontPageSize,
                    ThumbHeight = caseSet.ThumbHeight,
                    ThumbWidth = caseSet.ThumbWidth
                },
                PhotoSet = new PhotoSetIM
                {
                    EnableCaching = caseSet.EnableCaching,
                    CacheDuration = caseSet.CacheDuration,
                    ThumbHeight = caseSet.ThumbHeight,
                    ThumbWidth = caseSet.ThumbWidth
                },
                WeiXinSet = new WeiXinSetIM()
                {
                    Token = weixinSet.Token,
                    AppId = weixinSet.AppId,
                    AppSecret = weixinSet.AppSecret,
                    EncodingAESKey = weixinSet.EncodingAESKey
                },
                VideoSet = new VideoSetIM()
                {
                    FrontPageSize = videoSet.FrontPageSize,
                    ThumbHeight = videoSet.ThumbHeight,
                    ThumbWidth = videoSet.ThumbWidth,
                    Timer = videoSet.Timer
                }
            };


            return View(vm);
        }

        [HttpPost]
        public JsonResult VideoSet(VideoSetIM im)
        {
            if (!ModelState.IsValid)
            {

                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/VideoSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("FrontPageSize").SetValue(im.FrontPageSize);
                item.Element("ThumbHeight").SetValue(im.ThumbHeight);
                item.Element("ThumbWidth").SetValue(im.ThumbWidth);
                item.Element("Timer").SetValue(im.Timer);
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult Article(ArticleSetIM im)
        {
            if (!ModelState.IsValid)
            {

                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/ArticleSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("FrontPageSize").SetValue(im.FrontPageSize);
                item.Element("ThumbHeight").SetValue(im.ThumbHeight);
                item.Element("ThumbWidth").SetValue(im.ThumbWidth);
                item.Element("ImageHeight").SetValue(im.ImageHeight);
                item.Element("ImageWidth").SetValue(im.ImageWidth);
                item.Element("EnableCaching").SetValue(im.EnableCaching);
                item.Element("CacheDuration").SetValue(im.CacheDuration);

                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult PhotoSet(PhotoSetIM im)
        {
            if (!ModelState.IsValid)
            {

                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/PhotoSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("EnableCaching").SetValue(im.EnableCaching);
                item.Element("CacheDuration").SetValue(im.CacheDuration);
                item.Element("ThumbHeight").SetValue(im.ThumbHeight);
                item.Element("ThumbWidth").SetValue(im.ThumbWidth);


                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }
        [HttpPost]
        public JsonResult Blog(PostSetIM im)
        {
            if (!ModelState.IsValid)
            {

                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/BlogSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("FrontPageSize").SetValue(im.FrontPageSize);
                item.Element("EnableCaching").SetValue(im.EnableCaching);
                item.Element("CacheDuration").SetValue(im.CacheDuration);
                item.Element("ThumbHeight").SetValue(im.ThumbHeight);
                item.Element("ThumbWidth").SetValue(im.ThumbWidth);


                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult ProductSet(ProductSetIM im)
        {
            if (!ModelState.IsValid)
            {

                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/ProductSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("FrontPageSize").SetValue(im.FrontPageSize);
                item.Element("EnableCaching").SetValue(im.EnableCaching);
                item.Element("CacheDuration").SetValue(im.CacheDuration);
                item.Element("ThumbHeight").SetValue(im.ThumbHeight);
                item.Element("ThumbWidth").SetValue(im.ThumbWidth);
                item.Element("ImageHeight").SetValue(im.ImageHeight);
                item.Element("ImageWidth").SetValue(im.ImageWidth);
                item.Element("CategoryImageWidth").SetValue(im.CategoryImageWidth);
                item.Element("CategoryImageHeight").SetValue(im.CategoryImageHeight);

                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult WeiXinSet(WeiXinSetIM im)
        {
            if (!ModelState.IsValid)
            {

                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/WeiXinSettings.config");
                var doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                if (item != null)
                {
                    item.Element("Token")?.SetValue(im.Token);
                    item.Element("AppId")?.SetValue(im.AppId);
                    item.Element("AppSecret")?.SetValue(im.AppSecret);
                    item.Element("EncodingAESKey")?.SetValue(im.EncodingAESKey);
                  
                }

                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult CaseSet(CaseSetIM im)
        {
            if (!ModelState.IsValid)
            {

                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/CaseSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("FrontPageSize").SetValue(im.FrontPageSize);
                item.Element("EnableCaching").SetValue(im.EnableCaching);
                item.Element("CacheDuration").SetValue(im.CacheDuration);
                item.Element("ThumbHeight").SetValue(im.ThumbHeight);
                item.Element("ThumbWidth").SetValue(im.ThumbWidth);


                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult EditSite(SiteInfoIM vm)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                var xmlFile = Server.MapPath("~/Config/GlobalSettings.config");
                XDocument doc = XDocument.Load(xmlFile);

                var item = doc.Descendants("Settings").FirstOrDefault();
                item.Element("SiteName").SetValue(vm.SiteName ?? "");
                item.Element("SiteDomainName").SetValue(vm.SiteDomainName ?? "");
                item.Element("WebNumber").SetValue(vm.WebNumber ?? "");
                item.Element("BaiduSiteID").SetValue(vm.BaiduSiteID ?? "");
                item.Element("GoogleAnalyticsID").SetValue(vm.GoogleAnalyticsID ?? "");
                item.Element("IsClose").SetValue(vm.IsClose);
                item.Element("CloseInfo").SetValue(vm.CloseInfo ?? "");
                item.Element("DashboardLogo").SetValue(vm.DashboardLogo ?? "");
                item.Element("LoginLogo").SetValue(vm.LoginLogo ?? "");
                doc.Save(xmlFile);

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {

                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }


        // GET: /bbi_Admin/Info/EmailSet     邮箱服务设置

        //public ViewResult EmailSet()
        //{
        //    var smtp = SettingsManager.SMTP;
        //    EmailSetVM emailSetVM = new EmailSetVM
        //    {
        //        From = smtp.From,
        //        SmtpServer = smtp.SmtpServer,
        //        Port = smtp.Port,
        //        UserName = smtp.UserName,
        //        Password = smtp.Password,
        //        EnableSsl = smtp.EnableSsl
        //    };
        //    return View(emailSetVM);
        //}



        //[HttpPost]
        //public JsonResult EditEmailSet(EmailSetVM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errorMessage = GetModelErrorMessage();

        //        AR.Setfailure(errorMessage);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //    try
        //    {
        //        var xmlFile = Server.MapPath("~/Config/SMTPSettings.config");
        //        XDocument doc = XDocument.Load(xmlFile);
        //        var item = doc.Descendants("Settings").FirstOrDefault();

        //        item.Element("From").SetValue(vm.From ?? "");
        //        item.Element("SmtpServer").SetValue(vm.SmtpServer ?? "");
        //        item.Element("Port").SetValue(vm.Port);
        //        item.Element("UserName").SetValue(vm.UserName ?? "");

        //        if (!string.IsNullOrEmpty(vm.Password))
        //        {
        //            var pw =  EncryptionHelper.Encrypt(vm.Password);
        //            item.Element("Password").SetValue(pw);
        //        }

        //        item.Element("EnableSsl").SetValue(vm.EnableSsl);

        //        doc.Save(xmlFile);

        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        AR.Setfailure(ex.Message);
        //        return Json(AR, JsonRequestBehavior.DenyGet);
        //    }

        //}

    }
}