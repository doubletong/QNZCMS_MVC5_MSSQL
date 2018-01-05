using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TZGCMS.Model.Admin.ViewModel.Log
{
    public class LogSearchVM
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalUserCount { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Level { get; set; }

        public IEnumerable<LogVM> Logs { get; set; }

        public List<SelectListItem> LevelItems
        {
            get { return LogHelper.LogLevelItems(); }

        }
    }

    public class LogHelper
    {
        public static List<SelectListItem> LogLevelItems()
        {
            var items = new List<SelectListItem>
            {
                new SelectListItem {Text = "所有", Value = "ALL", Selected = true},
                new SelectListItem {Text = "信息", Value = "INFO"},
                new SelectListItem {Text = "调试", Value = "DEBUG"},
                new SelectListItem {Text = "警告", Value = "WARN"},
                new SelectListItem {Text = "错误", Value = "ERROE"},
                new SelectListItem {Text = "失败", Value = "FATAL"}
            };

            return items;
        }
    }
}
