using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.WeiXin.Model
{
    /// <summary>
    /// 微信错误访问的情况 
    /// </summary>
    public class ErrorMsg
    {
        /// <summary>
        /// 错误编号
        /// </summary>
        [DataMember(Name = "errcode")]
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误提示消息
        /// </summary>
        [DataMember(Name = "errmsg")]
        public string ErrMsg { get; set; }
    }

    public class MassSendErrorMsg
    {
        /// <summary>
        /// 错误编号
        /// </summary>
        [DataMember(Name = "errcode")]
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误提示消息
        /// </summary>
        [DataMember(Name = "errmsg")]
        public string ErrMsg { get; set; }
        [DataMember(Name = "msg_id")]
        public string MsgId { get; set; }
        [DataMember(Name = "msg_data_id")]
        public string MsgDataId { get; set; }
    }


    //    {
    //    "errcode":0,
    //    "errmsg":"send job submission success",
    //    "msg_id":34182, 
    //    "msg_data_id": 206227730
    //    }
}
