﻿using TZGCMS.SiteWeb.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Model.Admin.InputModel;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model;
using System.Collections;
using TZGCMS.Infrastructure.Cache;

namespace TZGCMS.SiteWeb.Areas.Admin.Controllers
{

    [SIGAuth]
    public class InfoController : BaseController
    {
        private readonly ICacheService _cacheService;
        public InfoController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        [HttpGet]
        public ViewResult Company()
        {
            var cfe = SettingsManager.Company;

            var vm = new CompanyIM
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
                Email = cfe.Email,
                Email2 = cfe.Email2,

                Facebook = cfe.Facebook,
                LinkedIn = cfe.LinkedIn,
                Youtube = cfe.Youtube,
                Oicq = cfe.Oicq,
                OicqTwo = cfe.OicqTwo,
                SinaWeibo = cfe.SinaWeibo,
                WeiXing = cfe.WeiXing,
                WeiXingCode = cfe.WeiXingCode,
                WeiXing2 = cfe.WeiXing2,
                WeiXingCode2 = cfe.WeiXingCode2

            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Company(CompanyIM vm)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = GetModelErrorMessage();

                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            try
            {
                var xmlFile = Server.MapPath("~/Config/CompanySettings.config");
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
                item.Element("Email").SetValue(vm.Email);
                item.Element("Email2").SetValue(vm.Email2 ?? "");

                item.Element("Facebook").SetValue(vm.Facebook ?? "");
                item.Element("LinkedIn").SetValue(vm.LinkedIn ?? "");
                item.Element("Youtube").SetValue(vm.Youtube ?? "");

                item.Element("Oicq").SetValue(vm.Oicq ?? "");
                item.Element("OicqTwo").SetValue(vm.OicqTwo ?? "");
                item.Element("SinaWeibo").SetValue(vm.SinaWeibo ?? "");
                item.Element("WeiXing").SetValue(vm.WeiXing ?? "");
                item.Element("WeiXingCode").SetValue(vm.WeiXingCode ?? "");
                item.Element("WeiXing2").SetValue(vm.WeiXing2 ?? "");
                item.Element("WeiXingCode2").SetValue(vm.WeiXingCode2 ?? "");
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
                  
                    GoogleAnalyticsID = info.GoogleAnalyticsID,
                    LoginLogo = info.LoginLogo,
                    DashboardLogo = info.DashboardLogo,
                    MailTo = info.MailTo
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
                },
                SiteCloseSet = new SiteCloseIM
                {
                    IsClose = info.IsClose,
                    CloseInfo = info.CloseInfo,
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
                item.Element("DashboardLogo").SetValue(vm.DashboardLogo ?? "");
                item.Element("LoginLogo").SetValue(vm.LoginLogo ?? "");
                item.Element("MailTo").SetValue(vm.MailTo ?? "");
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
        public JsonResult CloseSite(SiteCloseIM vm)
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
             
                item.Element("IsClose").SetValue(vm.IsClose);
                item.Element("CloseInfo").SetValue(vm.CloseInfo ?? "");
            
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
        public ActionResult CacheSet()
        {
            var cfe = SettingsManager.Site;

            var vm = new SiteCacheIM
            {
                CacheDuration = cfe.CacheDuration,
                EnableCaching = cfe.EnableCaching               
            };

            return View(vm);
          
        }

        [HttpPost]
        public JsonResult CacheSet(SiteCacheIM vm)
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

                item.Element("EnableCaching").SetValue(vm.EnableCaching);
                item.Element("CacheDuration").SetValue(vm.CacheDuration);

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
        [ValidateAntiForgeryToken]
        public JsonResult ClearCaches()
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = GetModelErrorMessage();
                AR.Setfailure(errorMessage);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

            try
            {
                _cacheService.Invalidate("Menu");

                return Json(AR, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {

                AR.Setfailure(ex.Message);
                return Json(AR, JsonRequestBehavior.DenyGet);
            }

        }
    }
}