using System;

namespace Z.Rest
{
    /// <summary>
    /// Rest类属性实体
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CachedMethodAttribute : System.Attribute
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public CachedMethodAttribute()
        {
        }
        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCached { get; set; }

        /// <summary>
        /// 默认过期时间
        /// </summary>
        public int DefaultExpireSeconds { get; set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isCached"></param>
        /// <param name="defaultExpireSeconds"></param>
        public CachedMethodAttribute(bool isCached, int defaultExpireSeconds)
        {
            this.IsCached = isCached;
            this.DefaultExpireSeconds = defaultExpireSeconds;
        }
    }
}
