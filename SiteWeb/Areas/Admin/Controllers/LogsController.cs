using AutoMapper;
using PagedList;
using SiteWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TZGCMS.Data.Entity.Logs;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Model.Admin.ViewModel;
using TZGCMS.Model.Admin.ViewModel.Log;
using TZGCMS.Service.Systems;

namespace SiteWeb.Areas.Admin.Controllers
{
    [SIGAuth]
    public class LogsController : BaseController
    {
        private readonly ILogServices _logServices;
        private readonly IMapper _mapper;

        public LogsController(ILogServices logServices, IMapper mapper)
        {
            _logServices = logServices;
            _mapper = mapper;

        }
       


        public ActionResult Index(int? page, DateTime? startDate, DateTime? expireDate, string level)
        {
            var logSearchVM = new LogSearchVM()
            {
                StartDate = startDate,
                ExpireDate = expireDate,
                Level = level,

                PageIndex = (page ?? 1),
                PageSize = SettingsManager.Log.PageSize
            };
            int totalCount;
            var logs = _logServices.SearchLogs(logSearchVM.PageIndex-1, logSearchVM.PageSize, startDate,
                expireDate, level, out totalCount);
            logSearchVM.TotalUserCount = totalCount;

            logSearchVM.Logs = _mapper.Map<IEnumerable<LogVM>>(logs);

            var logsAsIPagedList = new StaticPagedList<LogVM>(logSearchVM.Logs, logSearchVM.PageIndex, logSearchVM.PageSize, logSearchVM.TotalUserCount);
            ViewBag.OnePageOfLogs = logsAsIPagedList;

            return View(logSearchVM);

        }



        [HttpPost]
        public JsonResult Delete(string id)
        {
            if (id == "all")
            {
                bool count = _logServices.RemoveAll();
                if (count)
                {
                    AR.SetSuccess("已清空所有日志");
                    return Json(AR, JsonRequestBehavior.DenyGet);
                }
            }
            else
            {

                int logId;
                Int32.TryParse(id, out logId);
                _logServices.Delete(logId);

                AR.SetSuccess("已成功删除日志");
                return Json(AR, JsonRequestBehavior.DenyGet);

            }

            AR.Setfailure("删除日志失败");
            return Json(AR, JsonRequestBehavior.DenyGet);
        }

    }
}