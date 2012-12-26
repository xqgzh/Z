using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.C;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Z.Access
{
    /// <summary>
    /// 访问日志MQ服务器配置
    /// </summary>
    [DataContract]
    [Serializable]
    public class AssessMQ : MessageQueueConfig
    {
        /// <summary>
        /// MQ对应的行为日志表
        /// </summary>
        [DataMember]
        [XmlElement("Action")]
        public string[] Actions;
    }
}
