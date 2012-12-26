using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Z.C
{
    /// <summary>
    /// 消息队列监听配置项
    /// </summary>
    [Serializable]
    public class MessageQueueConfig
    {
        /// <summary>
        /// 消息队列地址
        /// </summary>
        [XmlAttribute]
        public string Address { get; set; }

        /// <summary>
        /// 监听线程数量, 默认1个线程
        /// </summary>
        [XmlAttribute]
        public int ThreadCount = 5;

        /// <summary>
        /// 监听超时设置(秒)
        /// </summary>
        [XmlAttribute]
        public int TimeoutSecond = 3;

        /// <summary>
        /// 两次监听之间的间隔时间(毫秒)
        /// </summary>
        [XmlAttribute]
        public int SleepMilliSecond = 100;

        /// <summary>
        /// 如果执行失败, 是否需要重试
        /// </summary>
        [XmlAttribute]
        public int RetryCount = 0;

        /// <summary>
        /// 路由字段名称
        /// </summary>
        public string DispatchName;

        /// <summary>
        /// 路由字段值
        /// </summary>
        public string[] DispatchValues = new string[]{};

        /// <summary>
        /// 如果执行失败, 转发到另外的MQ队列
        /// </summary>
        [XmlAttribute]
        public string RetryAddress { get; set; }

        /// <summary>
        /// 路由配置
        /// </summary>
        [XmlElement("Dispatch")]
        public MessageQueueConfig[] DispatchConfigs = new MessageQueueConfig[]{};

        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageQueueConfig() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="address"></param>
        /// <param name="threadCount"></param>
        /// <param name="timeoutSecond"></param>
        public MessageQueueConfig(string address, int threadCount, int timeoutSecond)
        {
            Address = address;
            ThreadCount = threadCount;
            TimeoutSecond = timeoutSecond;
        }


        /// <summary>
        /// 构造函数, 默认获取超时时间
        /// </summary>
        /// <param name="address"></param>
        /// <param name="threadCount"></param>
        public MessageQueueConfig(string address, int threadCount)
        {
            Address = address;
            ThreadCount = threadCount;
        }
    }
}
