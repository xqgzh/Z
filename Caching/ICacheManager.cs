using System;

namespace Z.Caching
{
    /// <summary>
    /// 缓存管理器
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        bool Set(string key, object value, DateTime expiry);

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        bool Replace(string key, object value, DateTime expiry);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        bool Delete(string key);

        /// <summary>
        /// 设置所有缓存失效
        /// </summary>
        /// <returns></returns>
        bool Flush();
    }
}
