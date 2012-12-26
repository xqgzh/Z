using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Z.DA
{
    /// <summary>
    /// 数据库配置项
    /// </summary>
    [Serializable]
    public class DatabaseConfiguration
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DatabaseConfiguration() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="connectionString"></param>
        public DatabaseConfiguration(string providerName, string connectionString) 
        {
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        private string providerName;

        /// <summary>
        /// 数据库类型
        /// </summary>
        [XmlAttribute]
        public string ProviderName
        {
            get
            {
                return providerName;
            }
            set
            {
                providerName = value;
            }
        }
        
        /// <summary>
        /// 连接字符串
        /// </summary>
        [XmlAttribute]
        public string ConnectionString;
    }
}
