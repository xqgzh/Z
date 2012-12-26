using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.C;
using System.Xml.Serialization;

namespace Z.Access
{
    /// <summary>
    /// 访问日志配置文件
    /// </summary>
    public class AccessConfig
    {
        #region 读取/保存操作

        /// <summary>
        /// 网关配置类单例
        /// </summary>
        public static AccessConfig Instance
        {
            get
            {
                return AppConfigHandler.GetConfig<AccessConfig>("Access.config", true);
            }
        }

        #endregion

        /// <summary>
        /// 缺省的Log服务器
        /// </summary>
        public MessageQueueConfig DefaultLogServer = new MessageQueueConfig("FormatName:DIRECT=TCP:192.168.72.65\\Private$\\AccessLogMQ", 1);

        /// <summary>
        /// MQ路由
        /// </summary>
        [XmlElement("LogServer")]
        public AssessMQ[] LogServers = new AssessMQ[] { };
    }
}
