using System;
using System.Xml.Serialization;

namespace Z.C
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FtpConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public FtpConfiguration()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftp"></param>
        /// <param name="access"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public FtpConfiguration(string ftp, string access, string username, string password)
        {
            FtpPath = ftp;
            AccessPath = access;
            Username = username;
            Password = password;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement]
        public string FtpPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement]
        public string AccessPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string Username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public string[] Args { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resetArgsAfterFormat"></param>
        /// <returns></returns>
        public string FormatFtpPath(bool resetArgsAfterFormat = true)
        {
            if (Args == null || Args.Length == 0)
                throw new InvalidProgramException();

            var result = String.Format(FtpPath, Args);

            if (resetArgsAfterFormat)
                Args = null;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resetArgsAfterFormat"></param>
        /// <returns></returns>
        public string FormatAccessPath(bool resetArgsAfterFormat = true)
        {
            if (Args == null || Args.Length == 0)
                throw new InvalidProgramException();

            var result = String.Format(AccessPath, Args);

            if (resetArgsAfterFormat)
                Args = null;

            return result;
        }
    }
}
