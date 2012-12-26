using System;
using System.Reflection;
using Couchbase;
using Couchbase.Configuration;
using Z.Log;
using Z.Util;

namespace Z.Caching
{
    /// <summary>
    /// 使用CouchDB的缓存
    /// </summary>
    public class CouchCacheManager : ICacheManager
    {
        private static ICouchbaseClientConfiguration config;
        private readonly static Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 构造函数
        /// </summary>
        public CouchCacheManager()
        {
            InitConfig();
        }

        private static void InitConfig()
        {
            try
            {
                var cc = Z.C.AppConfigHandler.GetConfig<CouchCacheConfig>(FileTools.FindFile("couchCache.config").FullName);
                config = new CouchbaseClientConfiguration()
                {
                    Bucket = cc.BucketName,
                    BucketPassword = cc.BucketPassword,
                };
                config.Urls.Add(new Uri(cc.Uri));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new Exception("CouchCache configuration exception.", ex);
            }

        }

        bool ICacheManager.Set(string key, object value, DateTime expiry)
        {
            bool bRet = false;
            using (var client = new CouchbaseClient(config))
            {
                bRet = client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value, expiry);
            }
            return bRet;
        }

        bool ICacheManager.Replace(string key, object value, DateTime expiry)
        {
            bool bRet = false;
            using (var client = new CouchbaseClient(config))
            {
                bRet = client.Store(Enyim.Caching.Memcached.StoreMode.Replace, key, value, expiry);
            }
            return bRet;
        }

        object ICacheManager.Get(string key)
        {
            object ret = null;
            using (var client = new CouchbaseClient(config))
            {
                if (!client.TryGet(key, out ret))
                {
                    ret = null;
                }
            }
            return ret;
        }

        bool ICacheManager.Delete(string key)
        {
            bool bRet = false;
            using (var client = new CouchbaseClient(config))
            {
                bRet = client.Remove(key);
            }
            return bRet;
        }

        bool ICacheManager.Flush()
        {
            //CouchDB 不要清空cache
            return false;
            //throw new NotImplementedException();
        }
    }
}
