using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Xml;
using System.Xml.Serialization;
using Z.Log;
using Z.Util;
using System.Diagnostics;
using System.Text;

namespace Z.C
{
    /// <summary>
    /// 通用配置读取类
    /// </summary>
    public class AppConfigHandler : IConfigurationSectionHandler
    {
        #region 静态缓存实现

        private static readonly Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        internal static Dictionary<string, object> Dict;

        static AppConfigHandler()
        {
            Dict = new Dictionary<string, object>();
            
        }

        #endregion

        #region 实现 IConfigurationSectionHandler 接口方法， 勿直接调用

        /// <summary>
        /// 实现 IConfigurationSectionHandler 接口方法， 勿直接调用
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        [Obsolete("AppConfigHandler.Create实现 IConfigurationSectionHandler 接口方法， 勿直接调用", true)]
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            //XPathNavigator nav = section.CreateNavigator();
            //string typename = (string)nav.Evaluate("string(@type)");
            //Type t = Type.GetType(typename);
            //XmlSerializer ser = XmlTools.GetXmlSerializer(t);
            //return ser.Deserialize(new XmlNodeReader(section));
            return section;
        }

        #endregion

        #region 配置文件读取接口（带缓存实现）

        /// <summary>
        /// 从.NET标准配置文件中解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetConfig<T>() where T : class, new()
        {
            return GetConfigInternal<T>(typeof(T).Name, 0, string.Empty, string.Empty, false);
        }

        /// <summary>
        /// 从指定的XML文件中解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConfigFileName">必须是完整的文件路径</param>
        /// <returns></returns>
        public static T GetConfig<T>(string ConfigFileName) where T : class, new()
        {
            return GetConfigInternal<T>(typeof(T).Name, 1, ConfigFileName, string.Empty, false);
        }

        /// <summary>
        /// 从当前执行的路径中读取文件并且解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FileNameWithoutPath">文件的名称不包含路径</param>
        /// <param name="IsCheckParent">是否查询父路径下的文件</param>
        /// <returns></returns>
        public static T GetConfig<T>(string FileNameWithoutPath, bool IsCheckParent) where T : class, new()
        {
            return GetConfigInternal<T>(typeof(T).Name, 3, FileNameWithoutPath, string.Empty, IsCheckParent);
        }

        /// <summary>
        /// 从指定的XML文件的Section中解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConfigFileName"></param>
        /// <param name="XPath"></param>
        /// <returns></returns>
        public static T GetConfig<T>(string ConfigFileName, string XPath) where T : class, new()
        {
            return GetConfigInternal<T>(typeof(T).Name, 2, ConfigFileName, XPath, false);
        }

        #endregion

        #region 解析并缓冲配置对象

        /// <summary>
        /// 解析并缓冲配置对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConfigName"></param>
        /// <param name="FromType"></param>
        /// <param name="ConfigFileName"></param>
        /// <param name="XPath"></param>
        /// <param name="IsCheckParent"></param>
        /// <returns></returns>
        private static T GetConfigInternal<T>(string ConfigName, int FromType, string ConfigFileName, string XPath, bool IsCheckParent) where T : class, new()
        {
            T o = null;

            //TODO: 当前是采用配置对象的类型名称作为缓存键, 但是如果一个应用从两个不同的位置读取相同类型的缓存键时, 会产生冲突.
            if (Dict.ContainsKey(ConfigName))
                return Dict[ConfigName] as T;

            lock (Dict)
            {
                if (!Dict.ContainsKey(ConfigName))
                {
                    switch (FromType)
                    {
                        case 1:
                            o = FromConfigFile<T>(ConfigFileName);
                            break;
                        case 2:
                            o = FromXmlSection<T>(ConfigFileName, XPath);
                            break;
                        case 3:
                            o = FromConfigFile<T>(ConfigFileName, IsCheckParent);
                            break;
                        default:
                            o = FromAppConfig<T>(ConfigName);
                            break;
                    }


                    if (o == null)
                        o = new T();

                    Dict.Add(ConfigName, o);

                }
                else
                    o = Dict[ConfigName] as T;
            }

            return o;
        }

        #endregion

        #region 从不同来源解析配置对象

        /// <summary>
        /// 从.NET标准配置文件中解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConfigName"></param>
        /// <returns></returns>
        private static T FromAppConfig<T>(string ConfigName) where T : class, new()
        {
            XmlNode node = ConfigurationManager.GetSection(ConfigName) as XmlNode;

            
            if (node != null)
            {
                using (XmlNodeReader reader = new XmlNodeReader(node))
                {
                    XmlSerializer xs = XmlTools.GetXmlSerializer(typeof(T));

                    return xs.Deserialize(reader) as T;
                }
            }

            return null;
        }

        /// <summary>
        /// 从一个Xml中解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConfigFileName"></param>
        /// <returns></returns>
        private static T FromConfigFile<T>(string ConfigFileName) where T : class, new()
        {
            if (File.Exists(ConfigFileName))
            {
                using (XmlReader reader = XmlReader.Create(ConfigFileName))
                {
                    XmlSerializer xs = XmlTools.GetXmlSerializer(typeof(T));
                    return xs.Deserialize(reader) as T;

                }
            }

            return null;
        }

        private static T FromConfigFile<T>(string ConfigFileNameWithoutPath, bool IsCheckParent) where T : class, new()
        {
            //查找文件
            FileInfo file = FileTools.FindFile(ConfigFileNameWithoutPath);

            if (file == null || file.Exists == false)
            {
                if (IsCheckParent == true && FileTools.CurrentDirectory.Parent.Exists)//没有文件, 有父目录, 默认放到父目录下
                    file = new FileInfo(Path.Combine(FileTools.CurrentDirectory.Parent.FullName, ConfigFileNameWithoutPath));
                else//没有文件, 没有父目录, 默认放到当前目录下
                    file = new FileInfo(Path.Combine(FileTools.CurrentDirectory.FullName, ConfigFileNameWithoutPath));
            }

            if(file.Exists)
                return FromConfigFile<T>(file.FullName);

            return null;
        }

        /// <summary>
        /// 从一个Xml的Section中解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConfigFileName"></param>
        /// <param name="XPath"></param>
        /// <returns></returns>
        private static T FromXmlSection<T>(string ConfigFileName, string XPath) where T : class, new()
        {
            if (File.Exists(ConfigFileName))
            {
                XmlDocument xdoc = new XmlDocument();

                xdoc.Load(ConfigFileName);

                XmlNode node = xdoc.SelectSingleNode(XPath);

                if (node != null)
                {
                    using (XmlNodeReader reader = new XmlNodeReader(node))
                    {
                        XmlSerializer xs = XmlTools.GetXmlSerializer(typeof(T));

                        return xs.Deserialize(reader) as T;
                    }
                }
            }

            return null;
        }

        #endregion

        #region 保存配置信息

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FileNameWithoutPath"></param>
        /// <param name="IsCheckParent"></param>
        /// <param name="config"></param>
        public static void SaveConfig<T>(string FileNameWithoutPath, bool IsCheckParent, T config)
        {
            //查找文件
            FileInfo file = FileTools.FindFile(FileNameWithoutPath);

            if (file == null || file.Exists == false)
            {
                if (IsCheckParent == true && FileTools.CurrentDirectory.Parent.Exists)//没有文件, 有父目录, 默认放到父目录下
                    file = new FileInfo(Path.Combine(FileTools.CurrentDirectory.Parent.FullName, FileNameWithoutPath));
                else//没有文件, 没有父目录, 默认放到当前目录下
                    file = new FileInfo(Path.Combine(FileTools.CurrentDirectory.FullName, FileNameWithoutPath));
            }

            using (StreamWriter sw = new StreamWriter(file.FullName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));

                XmlSerializerNamespaces xsns = new XmlSerializerNamespaces();
                xsns.Add("", "");

                xs.Serialize(sw, config, xsns);
            }
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="FileNameWithoutPath"></param>
        /// <param name="IsCheckParent"></param>
        /// <param name="config"></param>
        internal static void SaveConfig(string FileNameWithoutPath, bool IsCheckParent, object config)
        {
            //查找文件
            FileInfo file = FileTools.FindFile(FileNameWithoutPath);

            if (file == null || file.Exists == false)
            {
                if (IsCheckParent == true && FileTools.CurrentDirectory.Parent.Exists)//没有文件, 有父目录, 默认放到父目录下
                    file = new FileInfo(Path.Combine(FileTools.CurrentDirectory.Parent.FullName, FileNameWithoutPath));
                else//没有文件, 没有父目录, 默认放到当前目录下
                    file = new FileInfo(Path.Combine(FileTools.CurrentDirectory.FullName, FileNameWithoutPath));
            }


            using (StreamWriter sw = new StreamWriter(file.FullName))
            {
                XmlSerializer xs = new XmlSerializer(config.GetType());

                XmlSerializerNamespaces xsns = new XmlSerializerNamespaces();
                xsns.Add("", "");

                xs.Serialize(sw, config, xsns);
            }
        }

        #endregion

        #region 允许远程控制配置文件

        internal static string RemotePassword = string.Empty;

        private static bool HasEnableaRemoteConfig = false;
        /// <summary>
        /// 允许远程控制配置文件
        /// </summary>
        /// <param name="TcpPort"></param>
        /// <param name="Password">远程访问密码</param>
        /// <returns></returns>        
        public static void  EnableRemoteConfig(int TcpPort, string Password)
        {

            if (TcpPort <= 0)
                return;
            if (HasEnableaRemoteConfig)
                return;

            if (string.IsNullOrEmpty(Password))
                return;

            RemotePassword = Password;
            try
            {
                System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(
                    new System.Runtime.Remoting.WellKnownServiceTypeEntry(typeof(RemoteConfig), "Z.Config", System.Runtime.Remoting.WellKnownObjectMode.SingleCall));
                TcpChannel channel = new TcpChannel(TcpPort);
                ChannelServices.RegisterChannel(channel, false); 
                HasEnableaRemoteConfig = true;
            }
            catch (Exception ex)
            {
                HasEnableaRemoteConfig = false;
                logger.Error(ex);  
            }     
        }
        #endregion
    }
}
