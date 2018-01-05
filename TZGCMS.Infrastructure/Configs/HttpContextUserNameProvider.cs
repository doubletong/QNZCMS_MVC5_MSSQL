using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TZGCMS.Infrastructure.Configs
{
    public class HttpContextUserNameProvider
    {
        //log4net 记录用户名
        public override string ToString()
        {
            var context = HttpContext.Current;
            if (context?.User != null && context.User.Identity.IsAuthenticated)
            {
                return context.User.Identity.Name;
            }
            return "";
        }
    }
}
