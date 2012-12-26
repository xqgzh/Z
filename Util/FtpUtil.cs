using System.IO;
using System.Net;
using System.Reflection;
using Z.Log;

namespace Z.Util
{
    /// <summary>
    /// ftp工具集
    /// </summary>
    public static class FtpUtil
    {
        private static Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// ftp 上传文件
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="targetPath"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void Upload(Stream sourceStream, string targetPath, string userName, string password)
        {
            var request = FtpWebRequest.Create(targetPath) as FtpWebRequest;
            request.Credentials = new NetworkCredential(userName, password);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.ContentLength = sourceStream.Length;

            using (var stream = request.GetRequestStream())
            {
                var buffer = new byte[1024 * 4];
                int num;
                while ((num = sourceStream.Read(buffer, 0, 1024)) > 0)
                {
                    stream.Write(buffer, 0, num);
                }
            }

            // TODO: 需要调用GetResponse吗？
            var response = request.GetResponse() as FtpWebResponse;
        }

        /// <summary>
        /// 在FTP中建立文件夹，这个函数只能向下创建一层文件夹。不能直接创建多层文件夹。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>成功新建返回True</returns>
        public static bool CreateFolder(string path, string userName, string password)
        {
            var request = FtpWebRequest.Create(path) as FtpWebRequest;
            request.Credentials = new NetworkCredential(userName, password);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                request.GetResponse();

                return true;
            }
            catch (WebException e)
            {
                if (e.Message.Contains("550"))
                {
                    // Do nothing. 可能文件夹已经存在。
                    return true;
                }
                else
                {
                    logger.Error(e);
                    return false;
                }
            }
        }
    }
}
