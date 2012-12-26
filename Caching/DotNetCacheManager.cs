using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using Z.Log;

namespace Z.Caching
{
    /// <summary>
    /// 使用asp.net提供的缓存
    /// </summary>
    public class DotNetCacheManager : ICacheManager
    {
        private static readonly Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        private Cache cache;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DotNetCacheManager()
        {
            if (HttpContext.Current != null)
            {
                cache = HttpContext.Current.Cache;
            }
        }

        /// <seealso cref="Z.Caching.ICacheManager.Set"/>
        public bool Set(string key, object value, DateTime expiry)
        {
            if (cache == null) return false;
            bool ret = false;
            try
            {
                cache.Insert(key, value, null, expiry, Cache.NoSlidingExpiration);
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "设置缓存时异常 key=" + key);
            }
            return ret;
        }

        /// <seealso cref="Z.Caching.ICacheManager.Replace"/>
        public bool Replace(string key, object value, DateTime expiry)
        {
            if (cache == null) return false;
            bool ret = false;
            try
            {
                cache.Insert(key, value, null, expiry, Cache.NoSlidingExpiration);
                ret = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "替换缓存时异常 key=" + key);
            }
            return ret;
        }

        /// <seealso cref="Z.Caching.ICacheManager.Get"/>
        public object Get(string key)
        {
            if (cache == null) return null;
            object value = null;
            try
            {
                value = cache.Get(key);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "获取缓存异常 key=" + key);
            }
            return value;
        }

        /// <seealso cref="Z.Caching.ICacheManager.Delete"/>
        public bool Delete(string key)
        {
            if (cache == null) return false;
            bool ret = false;
            try
            {
                ret = cache.Remove(key) != null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "清除缓存项时异常 key=" + key);
            }
            return ret;
        }

        /// <seealso cref="Z.Caching.ICacheManager.Flush"/>
        public bool Flush()
        {
            if (cache == null) return false;
            try
            {
                List<string> keyList = new List<string>();
                foreach (DictionaryEntry de in cache)
                {
                    keyList.Add(Convert.ToString(de.Key));
                }
                lock (cache)
                {
                    foreach (string k in keyList)
                    {
                        cache.Remove(k);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "清除所有缓存项时异常");
                return false;
            }
        }
    }
}
