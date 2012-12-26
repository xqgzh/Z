using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Z.C
{
    /// <summary>
    /// 服务器配置信息
    /// </summary>
    public class WebServerConfiguration
    {
        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static implicit operator WebServerConfiguration(string address)
        {
            return new WebServerConfiguration(address, 30);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebServerConfiguration() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="address">服务器地址</param>
        public WebServerConfiguration(string address)
        {
            Address = address;
            TimeoutSecond = 30;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="address">服务器地址</param>
        /// <param name="timeoutSecond">访问超时时间(秒)</param>
        public WebServerConfiguration(string address, int timeoutSecond)
        {
            Address = address;
            TimeoutSecond = timeoutSecond;
        }

        /// <summary>
        /// 服务器地址
        /// </summary>
        [XmlAttribute]
        public string Address;

        /// <summary>
        /// 访问超时时间(秒)
        /// </summary>
        [XmlAttribute]
        public int TimeoutSecond;

        /// <summary>
        /// 格式化地址信息
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public string AddressFormat(params object[] objs)
        {
            return string.Format(Address, objs);
        }
    }
}
