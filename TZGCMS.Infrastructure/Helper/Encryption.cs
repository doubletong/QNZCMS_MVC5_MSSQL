using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TZGCMS.Infrastructure
{
    public sealed class Encryption
    {
        public static string GetSwcSHA1(string value)
        {
            SHA1 algorithm = SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
        }

        ///   <summary>   
        ///   使用SHA1算法加密一个字符串    
        ///   </summary>   
        ///   <param name="str">要加密的字符串</param>
        ///   <returns>加密后的字符串</returns>
        public static string GetSha1Str(string str)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            var data = Encoding.Default.GetBytes(str);
            var ret = sha.ComputeHash(data);
            return ret.Aggregate("", (current, t) => current + t.ToString("X2"));
        }
    }
}
