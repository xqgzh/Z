using System.Reflection;
using Z.Log;
using Z.Rest;
using Z.Util;

namespace Z.Caching
{
    /// <summary>
    /// cache工厂
    /// </summary>
    public class CacheFactory
    {
        private readonly static Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string CACHE_TYPE_MEMCACHED = "memcached";
        private const string CACHE_TYPE_ASPNET = "asp.net";
        private const string CACHE_TYPE_COUCH = "couchcache";
        private static ICacheManager cacheManager = null;

        /// <summary>
        /// 创建CacheManager实例
        /// </summary>
        /// <returns></returns>
        public static ICacheManager CreateCacheManager()
        {
            if (cacheManager != null)
                return cacheManager;
            else
            {
                var configFile = FileTools.FindFile("restCache.config");
                if (configFile == null)
                {
                    logger.Error("Cannot find the configuration for RestCache");
                    cacheManager = new DotNetCacheManager();
                }
                else
                {
                    var configuration = Z.C.AppConfigHandler.GetConfig<RestCacheConfiguration>(configFile.FullName);
                    if (configuration.CacheType == CACHE_TYPE_MEMCACHED)
                    {
                        cacheManager = new MemcachedManager(configuration.PoolSet);
                    }
                    else if (configuration.CacheType == CACHE_TYPE_ASPNET)
                    {
                        cacheManager = new DotNetCacheManager();
                    }
                    else if (configuration.CacheType == CACHE_TYPE_COUCH)
                    {
                        cacheManager = new CouchCacheManager();
                    }
                }
            }
            return cacheManager;
        }
    }
}
