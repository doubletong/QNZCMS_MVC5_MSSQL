using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.WeiXin.Model
{
    public class OAuthToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }

    }

    
    public class OAuthUserInfo
    {
        [DataMember(Name = "openid")]
        public string Openid { get; set; }
        public string nickname { get; set; }
        public int sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        [DataMember(Name = "privilege",IsRequired = false)]
        public string[] Privilege { get; set; }
        //public string privilege { get; set; }  //报错暂时注释
        public string unionid { get; set; }

    }
}
