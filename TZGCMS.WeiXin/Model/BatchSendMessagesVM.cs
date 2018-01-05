using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.WeiXin.Model
{
    public class BatchSendMessagesVM
    {
        [DataMember(Name = "touser")]
        public string[] ToUser { get; set; }
        [DataMember(Name = "msgtype")]
        public string MsgType { get; set; }
        [DataMember(Name = "text")]
        public Message Text { get; set; }
    }

    //{
    //    "touser":[
    //    "OPENID1",
    //    "OPENID2"
    //        ],
    //    "msgtype": "text",
    //    "text": { "content": "hello from boxer."}
    //}

    public class SingleSendMessagesVM
    {
        [DataMember(Name = "touser")]
        public string ToUser { get; set; }
        [DataMember(Name = "msgtype")]
        public string MsgType { get; set; }
        [DataMember(Name = "text")]
        public Message Text { get; set; }
    }

//  {
//    "touser":"OPENID",
//    "msgtype":"text",
//    "text":
//    {
//    "content":"Hello World"
//    }
//  }

    public class Message
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }


}
