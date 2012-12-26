using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Z.Caching;

namespace Z.Rest
{
    /// <summary>
    /// Rest缓存配置
    /// </summary>
    [Serializable]
    [XmlRoot("restCacheConfiguration")]
    public class RestCacheConfiguration
    {
        /// <summary>
        /// 缓存类型 目前支持memcached 和 asp.net两种
        /// </summary>
        [XmlElement("cacheType")]
        public string CacheType { get; set; }

        /// <summary>
        /// 忽略的返回值列表
        /// </summary>
        [XmlArray("ignoreReturnCodes")]
        [XmlArrayItem("code")]
        public List<int> IgnoreReturnCodes { get; set; }

        /// <summary>
        /// 缓存池
        /// </summary>
        [XmlArray("poolSet")]
        [XmlArrayItem("cachePool")]
        public List<CachePool> PoolSet { get; set; }

        /// <summary>
        /// 缓存命中
        /// </summary>
        [XmlArray("cacheItems")]
        [XmlArrayItem("cacheItem")]
        public List<CacheItem> CacheItems { get; set; }
    }

    /// <summary>
    /// Rest缓存项
    /// </summary>
    [Serializable]
    [XmlRoot("cacheItem")]
    public class CacheItem
    {
        /// <summary>
        /// action标识符
        /// </summary>
        [XmlIgnore]
        public string ActionIdentity { get; set; }

        /// <summary>
        /// action Id
        /// </summary>
        [XmlElement("actionId")]
        public int ActionId { get; set; }

        /// <summary>
        /// 该接口是否缓存
        /// </summary>
        [XmlElement("isCached")]
        public bool IsCached { get; set; }

        /// <summary>
        /// 用于生成缓存key的相关参数列表
        /// </summary>
        [XmlIgnore]
        public List<CacheKey> Keys { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [XmlElement("expireSeconds")]
        public int ExpireSeconds { get; set; }
    }
    
    /// <summary>
    /// 缓存键
    /// </summary>
    [Serializable]
    [XmlRoot("cacheItem")]
    public class CacheKey
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// string类型的值
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// 参数所在位置
        /// </summary>
        public int Order { get; set; }
    }
}
