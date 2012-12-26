using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Memcached.ClientLibrary;
using Z.Log;
using Z.Util;

namespace Z.Caching
{
    /// <summary>
    /// 缓冲管理类
    /// </summary>
    public class MemcachedManager : ICacheManager
    {
        private readonly static Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        private static MemcachedClient defaultClient;
        private static IDictionary<string, MemcachedClient> cacheClients = new Dictionary<string, MemcachedClient>();

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MemcachedManager()
        {
            try
            {
                CacheConfiguration cc = Z.C.AppConfigHandler.GetConfig<CacheConfiguration>(FileTools.FindFile("cache.config").FullName);
                init(cc.PoolSet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new Exception("Memcached configuration exception.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pools"></param>
        public MemcachedManager(List<CachePool> pools)
        {
            init(pools);
        }

        private void init(List<CachePool> pools)
        {
            foreach (CachePool p in pools)
            {
                if (cacheClients.ContainsKey(p.Name))
                {
                    throw new Exception("Duplicate configuration for the pool name  : " + p.Name);

                }

                MemcachedClient client = new MemcachedClient();
                //TODO:考虑增加是否压缩的配置选项
                client.EnableCompression = false;
                SockIOPool pool = null;

                if (p.Name.Equals("default instance") || string.IsNullOrEmpty(p.Name))
                {
                    pool = SockIOPool.GetInstance();
                    defaultClient = client;
                }
                else
                {
                    pool = SockIOPool.GetInstance(p.Name);
                    client.PoolName = p.Name;
                }

                pool.SetServers(p.Servers);
                pool.SetWeights(p.Weights);
                pool.InitConnections = p.InitConnections;
                pool.MinConnections = p.MinConnections;
                pool.MaxConnections = p.MaxConnections;
                pool.MaxIdle = p.MaxIdle;
                pool.SocketConnectTimeout = p.SocketConnectTimeout;
                pool.SocketTimeout = p.SocketTimeout;
                pool.MaintenanceSleep = p.MaintenanceSleep;
                pool.Failover = p.Failover;
                pool.Nagle = p.Nagle;
                pool.Initialize();

                cacheClients.Add(p.Name, client);
            }

            if (defaultClient == null)
            {
                throw new Exception("Missing the configuration for default cache pool");
            }
        }

        #region access default pool
        /// <summary>
        /// 判断键是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            return defaultClient.KeyExists(key);
        }

        /// <seealso cref="Z.Caching.ICacheManager.Set"/>
        public bool Set(string key, object value, DateTime expiry)
        {
            return defaultClient.Set(key, value, expiry);
        }

        /// <seealso cref="Z.Caching.ICacheManager.Replace"/>
        public bool Replace(string key, object value, DateTime expiry)
        {
            return defaultClient.Replace(key, value, expiry);
        }

        /// <seealso cref="Z.Caching.ICacheManager.Get"/>
        public object Get(string key)
        {
            return defaultClient.Get(key);
        }

        /// <summary>
        /// 获取所有键值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public Hashtable GetAll(string[] keys)
        {
            return defaultClient.GetMultiple(keys);
        }

        /// <seealso cref="Z.Caching.ICacheManager.Delete"/>
        public bool Delete(string key)
        {
            return defaultClient.Delete(key);
        }

        /// <seealso cref="Z.Caching.ICacheManager.Flush"/>
        public bool Flush()
        {
            return defaultClient.FlushAll();
        }

        /// <summary>
        /// 关闭缓存
        /// </summary>
        public void Close()
        {
            Close("default instance");
        }
        #endregion

        #region access named pool
        /// <summary>
        /// 缓存池是否存在指定Key
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool KeyExists(string poolName, string key)
        {
            return cacheClients[poolName].KeyExists(key);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public bool Set(string poolName, string key, object value, DateTime expiry)
        {
            return cacheClients[poolName].Set(key, value, expiry);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间</param>
        /// <returns></returns>
        public bool Replace(string poolName, string key, object value, DateTime expiry)
        {
            return cacheClients[poolName].Replace(key, value, expiry);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <param name="key">缓存Key</param>      
        /// <returns></returns>
        public object Get(string poolName, string key)
        {
            return cacheClients[poolName].Get(key);
        }

        /// <summary>
        /// 获取所有指定缓存
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <param name="keys">缓存Key数组</param>      
        /// <returns></returns>
        public Hashtable GetAll(string poolName, string[] keys)
        {
            return cacheClients[poolName].GetMultiple(keys);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <param name="key">缓存Key</param>      
        /// <returns></returns>
        public bool Delete(string poolName, string key)
        {
            return cacheClients[poolName].Delete(key);
        }

        /// <summary>
        /// 设置所有缓存失效
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <returns></returns>
        public bool Flush(string poolName)
        {
            return cacheClients[poolName].FlushAll();
        }

        /// <summary>
        /// 关闭缓存服务器
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        public void Close(string poolName)
        {
            SockIOPool.GetInstance(poolName).Shutdown();
        }
        #endregion
    }

}
