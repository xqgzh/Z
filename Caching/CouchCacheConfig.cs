using System;
using System.Xml.Serialization;

namespace Z.Caching
{

    /// <summary>
    /// Couch Cache配置类
    /// </summary>
    [Serializable]
    [XmlRoot("CouchCacheConfig")]
    public class CouchCacheConfig
    {  

        /// <summary>
        /// BucketName
        /// </summary>
        [XmlElement("Bucket")]
        public string BucketName { get; set; }

        /// <summary>
        /// BucketPassword
        /// </summary>
        [XmlElement("BucketPassword")]
        public string BucketPassword { get; set; }

        /// <summary>
        /// Uri
        /// </summary>
        [XmlElement("Uri")]
        public string Uri { get; set; }

    }
}
