using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Z.Caching
{
    /// <summary>
    /// 缓存配置类
    /// </summary>
    [Serializable]
    [XmlRoot("cacheConfiguration")]
    public class CacheConfiguration
    {
        /// <summary>
        /// 缓存池
        /// </summary>
        [XmlArray("poolSet")]
        [XmlArrayItem("cachePool")]
        public List<CachePool> PoolSet { get; set; }
    }
}
