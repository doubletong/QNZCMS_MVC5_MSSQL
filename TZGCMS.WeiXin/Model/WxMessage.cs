using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.WeiXin.Model
{
    public class WxMessage
    {
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public long CreateTime { get; set; }

        public string Content { get; set; }
        public string MsgType { get; set; }
        public string EventName { get; set; }
        public string EventKey { get; set; }
    }


}
