using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Z.Util
{
    /// <summary>
    /// 文件工具
    /// </summary>
    public class FileTools
    {
        /// <summary>
        /// 默认上溯目录级别
        /// </summary>
        const int MaxDept = 4;

        static FileTools()
        {
            try
            {
                if (System.Web.HttpContext.Current == null)
                {
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath) == false)
                        CurrentDirectory = new DirectoryInfo(AppDomain.CurrentDomain.RelativeSearchPath);
                    else
                        CurrentDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                }
                else
                    CurrentDirectory = new DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath("~"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 在运行目录下寻找文件,找不到则返回空
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static FileInfo FindFile(string FileName)
        {
            return FindFile(CurrentDirectory, FileName, 0);
        }

        /// <summary>
        /// 在运行目录下寻找文件,找不到则向上级目录查找, 直到根目录,或者达到上溯级别限制
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="FileName"></param>
        /// <param name="Dept"></param>
        /// <returns></returns>
        public static FileInfo FindFile(DirectoryInfo dir,  string FileName, int Dept)
        {
            string FilePath = Path.Combine(dir.FullName, FileName);

            if (File.Exists(FilePath))
                return new FileInfo(FilePath);

            //如果当前目录找不到, 上溯级别超过, 则直接返回
            if (Dept < MaxDept && dir.Parent != null && dir.Parent.Exists == true)
                return FindFile(dir.Parent, FileName, Dept + 1);

            return null;
        }

        /// <summary>
        /// 获取应用程序的当前目录
        /// </summary>
        /// <returns></returns>
        public static readonly DirectoryInfo CurrentDirectory;
    }
}
