using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TZGCMS.Infrastructure.Configs;

namespace TZGCMS.Model.Admin.ViewModel.FileManager
{
    public class DirectoryVM
    {
        public string Name { get; set; }
        public string DirPath { get; set; }
        public bool  HasChildren { get; set; }
    }

    public class FileVM
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FilePath { get; set; }
        public double FileSize { get; set; }
        public string ImgUrl
        {
            get
            {
                return ".jpg.png.gif".Contains(this.Extension.ToLower()) ? this.FilePath : string.Format("{0}/{1}.png", SettingsManager.File.ExtensionDir, this.Extension); ;
            }
        }
    }

    public sealed class FileHelper
    {
        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryVM> GetDirectories(string rootPath, string webPath)
        {
         //   !Directory.EnumerateFiles(filePath).Any() && !Directory.EnumerateDirectories(filePath).Any();

            return new DirectoryInfo(rootPath).GetDirectories()
                .Where(dir => !dir.Name.StartsWith("_")).Select(dir => new DirectoryVM
                {
                    Name = dir.Name,
                    DirPath = string.Format("{0}/{1}", webPath, dir.Name),
                    HasChildren = Directory.EnumerateDirectories(Path.Combine(rootPath, dir.Name)).Any()
                });
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="webPath"></param>
        /// <returns></returns>
        public static IEnumerable<FileVM> GetFileList(string rootPath, string webPath)
        {
            return new DirectoryInfo(rootPath).GetFiles()
                .Where(dir => !dir.Name.StartsWith("_")).Select(f => new FileVM
                {
                    Name = f.Name,
                    Extension = f.Extension.Replace(".", ""),
                    CreatedDate = f.CreationTime,
                    FilePath = string.Format("{0}/{1}", webPath, f.Name),
                    FileSize = f.Length / 1024
                });
        }
    }
}
