using System;
using System.Xml.Serialization;

namespace Z.Caching
{
    /// <summary>
    /// 缓存池
    /// </summary>
    [Serializable]
    [XmlRoot("cachePool")]
    public class CachePool
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CachePool()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">缓存池名称</param>
        /// <param name="servers">缓存池服务器</param>
        /// <param name="weights">权重</param>
        /// <param name="initConnections">初始连接数</param>
        /// <param name="minConnections">最小连接数</param>
        /// <param name="maxConnections">最大连接数</param>
        /// <param name="maxIdle">最大空闲时间</param>
        /// <param name="socketConnectTimeout">网络连接超时时间</param>
        /// <param name="socketTimeout">网络超时时间</param>
        /// <param name="maintenanceSleep">维护线程休息时间</param>
        /// <param name="failover">是否失效转移</param>
        /// <param name="nagle">是否用nagle算法启动socket</param>
        public CachePool(string name, string[] servers, int[] weights, int initConnections,
            int minConnections, int maxConnections, long maxIdle, int socketConnectTimeout,
            int socketTimeout, int maintenanceSleep, bool failover, bool nagle)
        {
            Name = name;
            Servers = servers;
            Weights = weights;
            InitConnections = initConnections;
            MinConnections = minConnections;
            MaxConnections = maxConnections;
            MaxIdle = maxIdle;
            SocketConnectTimeout = socketConnectTimeout;
            SocketTimeout = socketTimeout;
            MaintenanceSleep = maintenanceSleep;
            Failover = failover;
            Nagle = nagle;
        }
        /// <summary>
        /// 缓存池名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 缓存池服务器
        /// </summary>
        [XmlArray("servers")]
        [XmlArrayItem("server")]
        public string[] Servers { get; set; }
        /// <summary>
        /// 当存在多台缓冲服务器时，本台服务器被选中的权重，默认为1
        /// </summary>
        [XmlArray("weights")]
        [XmlArrayItem("weight")]
        public int[] Weights { get; set; }
        /// <summary>
        /// 初始连接数
        /// </summary>
        [XmlElement("initConnections")]
        public int InitConnections { get; set; }
        /// <summary>
        /// 最小连接数
        /// </summary>
        [XmlElement("minConnections")]
        public int MinConnections { get; set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        [XmlElement("maxConnections")]
        public int MaxConnections { get; set; }
        /// <summary>
        /// 最大空闲时间
        /// </summary>
        [XmlElement("maxIdle")]
        public long MaxIdle { get; set; }
        /// <summary>
        /// 网络连接超时时间
        /// </summary>
        [XmlElement("socketConnectTimeout")]
        public int SocketConnectTimeout { get; set; }
        /// <summary>
        /// 网络超时时间
        /// </summary>
        [XmlElement("socketTimeout")]
        public int SocketTimeout { get; set; }
        /// <summary>
        /// 维护线程休息时间
        /// </summary>
        [XmlElement("maintenanceSleep")]
        public int MaintenanceSleep { get; set; }
        /// <summary>
        /// 是否失效转移
        /// </summary>
        [XmlElement("failover")]
        public bool Failover { get; set; }
        /// <summary>
        /// 是否用nagle算法启动socket
        /// </summary>
        [XmlElement("nagle")]
        public bool Nagle { get; set; }
    }
}
