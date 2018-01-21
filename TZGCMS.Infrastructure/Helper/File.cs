using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TZGCMS.Infrastructure.Helper
{
    public class File
    {
        public static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = null;
            try
            {
                fs = System.IO.File.OpenRead(fullFilePath);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                return bytes;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

        }

        private static readonly string[] _extensions = new[]
                                                          {
                                                              "zip", "rar", "pdf",
                                                              "jpg", "png", "gif", "psd",
                                                              "tiff", "xls", "xlsx", "doc", "docx"
                                                          };


        public static List<SelectListItem> GetExtensions()
        {
            var extensions = new List<SelectListItem>();
            foreach (string item in _extensions)
            {
                extensions.Add(new SelectListItem { Value = item, Text = item });
            }
            return extensions;
        }
    }
}
