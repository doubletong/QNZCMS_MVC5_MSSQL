using System;
using System.Web;

namespace TZGCMS.Infrastructure.Logging
{
    public class LogUtility
    {
        
        public static string BuildExceptionMessage(Exception x)
        {

            Exception logException = x;
            if (x.InnerException != null)
            {
                logException = x.InnerException;
            }

            string strErrorMsg = Environment.NewLine + "虚拟路径：" + HttpContext.Current?.Request.Path;

            // Get the QueryString along with the Virtual Path
            strErrorMsg += Environment.NewLine + "原始Url：" + HttpContext.Current?.Request.RawUrl;

            // Get the error message
            strErrorMsg += Environment.NewLine + "异常消息：" + logException.Message;

            // Source of the message
            strErrorMsg += Environment.NewLine + "源：" + logException.Source;

            // Stack Trace of the error
            strErrorMsg += Environment.NewLine + "Stack Trace :" + logException.StackTrace;

            // Method where the error occurred
            strErrorMsg += Environment.NewLine + "TargetSite :" + logException.TargetSite;
            return strErrorMsg;

        }
 

    }   // End Class

}
