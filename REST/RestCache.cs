using System;
using System.Collections.Generic;
using System.Reflection;
using Z.Caching;
using Z.Log;
using Z.Util;

namespace Z.Rest
{   
    /// <summary>
    /// Rest缓存
    /// </summary>
    public class RestCache
    {
        private readonly static Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string RestPrefix = "REST_";
        private const string CACHE_TYPE_MEMCACHED = "memcached";
        private const string CACHE_TYPE_ASPNET = "asp.net";
        private const string CACHE_TYPE_COUCH = "couchcache";
        private static RestCacheConfiguration configuration;
        private static ICacheManager cacheManager;

        /// <summary>
        /// 读取当前应用下的restCache.config文件初始化缓存
        /// </summary>
        static RestCache()
        {
            try
            {
                var configFile = FileTools.FindFile("restCache.config");
                if (configFile == null)
                {
                    Enable = false;
                    logger.Error("Cannot find the configuration for RestCache");
                    configuration = new RestCacheConfiguration();
                    configuration.CacheItems = new List<CacheItem>();
                }
                else
                {
                    configuration = Z.C.AppConfigHandler.GetConfig<RestCacheConfiguration>(configFile.FullName);
                    if (configuration.CacheType == CACHE_TYPE_MEMCACHED)
                    {
                        Enable = true;
                        cacheManager = new MemcachedManager(configuration.PoolSet);
                    }
                    else if (configuration.CacheType == CACHE_TYPE_ASPNET)
                    {
                        Enable = true;
                        cacheManager = new DotNetCacheManager();
                    }
                    else if (configuration.CacheType == CACHE_TYPE_COUCH)
                    {
                        Enable = true;
                        cacheManager = new CouchCacheManager();
                    }
                }
            }
            catch (Exception e)
            {
                Enable = false;
                logger.Error(e);
                configuration = new RestCacheConfiguration();
                configuration.CacheItems = new List<CacheItem>();
            }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RestCache()
        {

        }

        /// <summary>
        /// 注册缓存信息
        /// </summary>
        /// <param name="info"></param>
        public void Register(RestMethodInfo info)
        {
            #region 生成缓存项
            if (RestCache.Enable)
            {
                CacheItem config = configuration.CacheItems.Find((item) => { return info.ActionId == item.ActionId; });
                if (config != null)
                {
                    info.ExpireSeconds = config.ExpireSeconds;
                    info.IsCached = config.IsCached;
                }
            }
            #endregion
        }

        /// <summary>
        ///  获取缓存的资源信息
        /// </summary>
        /// <param name="cacheKey">缓存key</param>
        /// <returns></returns>
        public byte[] GetResource(string cacheKey)
        {
            if (!Enable) return null;

            try
            {
                object result = cacheManager.Get(cacheKey);
                byte[] value = result as byte[];
                return value;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "获取缓存失败" + cacheKey);
            }

            return null;
        }

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public static bool Enable { get; private set; }

        /// <summary>
        /// 缓存资源
        /// </summary>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="bytes">需缓存的内容</param>
        /// <param name="ExpireSeconds">过期时间(秒)</param>
        public void CacheResource(string cacheKey, byte[] bytes, int ExpireSeconds)
        {
            if (!Enable) return;
            try
            {
                DateTime expiryDate = DateTime.Now.AddSeconds(ExpireSeconds);
                cacheManager.Set(cacheKey, bytes, expiryDate);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "设置缓存失败" + cacheKey);
            }
        }

        /// <summary>
        /// 获取该returncode对应的返回值是否被缓存
        /// </summary>
        /// <param name="returnCode"></param>
        /// <returns></returns>
        public bool IsIgnore(int returnCode)
        {
            return configuration.IgnoreReturnCodes.Exists((item) => { return item == returnCode; });
        }
    }
}
