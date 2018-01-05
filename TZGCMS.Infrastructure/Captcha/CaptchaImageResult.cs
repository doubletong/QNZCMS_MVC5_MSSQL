using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TZGCMS.Infrastructure.Captcha
{
    public class CaptchaImageResult : ActionResult
    {
        /// <summary>
        /// 生成随机码001
        /// </summary>
        /// <returns></returns>
        private string GenerateRandomCode()
        {
            Random r = new Random();
            string s = "";
            for (int j = 0; j < 5; j++)
            {
                int i = r.Next(3);
                int ch;
                switch (i)
                {
                    case 1:
                        ch = r.Next(0, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    default:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
                r.NextDouble();
                r.Next(100, 1999);
            }
            return s;
        }
        /// <summary>
        /// 生成随机码002
        /// </summary>
        /// <returns></returns>
        public string GetCaptchaString(int length)
        {
            int intZero = '1';
            int intNine = '9';
            int intA = 'A';
            int intZ = 'Z';
            int intCount = 0;
            int intRandomNumber = 0;
            string strCaptchaString = "";

            Random random = new Random(System.DateTime.Now.Millisecond);

            while (intCount < length)
            {
                intRandomNumber = random.Next(intZero, intZ);
                if (((intRandomNumber >= intZero) && (intRandomNumber <= intNine) || (intRandomNumber >= intA) && (intRandomNumber <= intZ)))
                {
                    strCaptchaString = strCaptchaString + (char)intRandomNumber;
                    intCount = intCount + 1;
                }
            }
            return strCaptchaString;
        }

        public override void ExecuteResult(ControllerContext context)
        {

          
            string randomString = GenerateRandomCode();
            context.HttpContext.Session["SigCaptcha"] = randomString;     
         
             RandomImage ci = new RandomImage(context.HttpContext.Session["SigCaptcha"].ToString(),120, 38);          

             HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "image/jpeg";
            ci.Image.Save(response.OutputStream, ImageFormat.Jpeg);
            ci.Dispose();

        }

    }
}