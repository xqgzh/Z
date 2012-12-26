using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Z.Util;

namespace Z.Rest
{
    /// <summary>
    /// 验证是否是合法的返回值类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public delegate bool ValidateReturnType(Type type);

    /// <summary>
    /// Rest 处理类
    /// </summary>
    public class RestManager
    {
        /// <summary>
        /// Rest缓存
        /// </summary>
        private RestCache cache = new RestCache();

        /// <summary>
        /// API管理器
        /// </summary>
        private RestAPIManager manager = new RestAPIManager();

        /// <summary>
        /// 验证是否是合法的返回值类型
        /// </summary>
        private event ValidateReturnType IsValidReturnType;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isValidReturnType"></param>
        public RestManager(ValidateReturnType isValidReturnType)
        {
            IsValidReturnType = isValidReturnType;
        }

        /// <summary>
        /// 在rest管理中注册相应的rest类
        /// </summary>
        /// <param name="type">有Rest类属性标识的类</param>
        public virtual void Register(Type type)
        {
            RestAPIClassAttribute[] classAttributes = type.GetCustomAttributes(typeof(RestAPIClassAttribute), false) as RestAPIClassAttribute[];
            if (classAttributes.Length == 1)
            {
                DesignedErrorCodeAttribute[] cErrorCodeAttributes = type.GetCustomAttributes(typeof(DesignedErrorCodeAttribute), false) as DesignedErrorCodeAttribute[];

                foreach (MethodInfo info in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    RestAPIMethodAttribute[] apiAttributes = info.GetCustomAttributes(typeof(RestAPIMethodAttribute), false) as RestAPIMethodAttribute[];
                    if (apiAttributes.Length == 1)
                    {
                        DesignedErrorCodeAttribute[] mErrorCodeAttributes = info.GetCustomAttributes(typeof(DesignedErrorCodeAttribute), false) as DesignedErrorCodeAttribute[];

                        if (!IsValidReturnType(info.ReturnType)) 
                            throw new Exception("不支持的返回值格式");
                        RestMethodInfo restInfo = new RestMethodInfo();                        
                        restInfo.ActionName = classAttributes[0].ResourceName + "/" + apiAttributes[0].Name;
                        restInfo.ResourceName = classAttributes[0].ResourceName + "/" + apiAttributes[0].Name + "(" + apiAttributes[0].ActionId + ")";
                        restInfo.ActionId = apiAttributes[0].ActionId;
                        restInfo.Description = apiAttributes[0].Title;
                        restInfo.MethodName = classAttributes[0].ResourceName + "_" + apiAttributes[0].Name; ;
                        restInfo.SecurityLevel = apiAttributes[0].SecurityLevel;
                        restInfo.MaxLogLength = apiAttributes[0].MaxLogLength;
                        restInfo.DefaultHandler = apiAttributes[0].DefaultHandler;
                        restInfo.AccessPolicy = apiAttributes[0].AccessPolicy;
                        restInfo.IpSecurityCheck = apiAttributes[0].IpSecurityCheck;
                        restInfo.EnableOsapCheck = apiAttributes[0].EnableOsapCheck;
                        restInfo.IsObsolete = (info.GetCustomAttributes(typeof(ObsoleteAttribute), false) as ObsoleteAttribute[]).Length > 0;
                        restInfo.ProxyMethodInfo = info;
                        restInfo.ReturnType = info.ReturnType;
                        restInfo.ParameterInfos = new List<RestParameterInfo>();
                        #region 获取该接口可能抛出的异常的errorcode集合
                        List<int> errorCodes = new List<int>();
                        if (cErrorCodeAttributes.Length == 1 && cErrorCodeAttributes[0].Codes != null)
                        {
                            foreach (int i in cErrorCodeAttributes[0].Codes)
                            {
                                if (!errorCodes.Contains(i))
                                {
                                    errorCodes.Add(i);
                                }
                            }
                        }
                        if (mErrorCodeAttributes.Length == 1 && mErrorCodeAttributes[0].Codes != null)
                        {
                            foreach (int i in mErrorCodeAttributes[0].Codes)
                            {
                                if (!errorCodes.Contains(i))
                                {
                                    errorCodes.Add(i);
                                }
                            }
                        }
                        errorCodes.Sort();
                        restInfo.ErrorCodes = errorCodes.ToArray();
                        #endregion
                        manager.RestInfos.Add(restInfo.ActionId, restInfo);

                        object[] cacheAttributes = info.GetCustomAttributes(false);
                        CachedMethodAttribute cacheAttribute = null;

                        foreach (object o in cacheAttributes)
                        {
                            cacheAttribute = o as CachedMethodAttribute;

                            if (cacheAttribute != null)
                                break;
                        }



                        if (RestCache.Enable && cacheAttribute != null)
                        {
                            restInfo.IsCached = cacheAttribute.IsCached;
                            restInfo.ExpireSeconds = cacheAttribute.DefaultExpireSeconds;
                            restInfo.CacheMethodAttr = cacheAttribute;
                        }
                        else
                        {
                            restInfo.IsCached = false;
                        }

                        foreach (ParameterInfo parameter in info.GetParameters())
                        {
                            RestParameterInfo pInfo = new RestParameterInfo();
                            pInfo.ParameterName = parameter.Name;
                            pInfo.ParameterType = parameter.ParameterType;                            

                            CachedKeyAttribute[] keyAttributes = parameter.GetCustomAttributes(typeof(CachedKeyAttribute), false) as CachedKeyAttribute[];
                            RestParameterAttribute[] requiredAttributes = parameter.GetCustomAttributes(typeof(RestParameterAttribute), false) as RestParameterAttribute[];
                            if (keyAttributes.Length > 0)
                            {
                                CacheKey key = new CacheKey();
                            }
                            if (requiredAttributes.Length > 0)
                            {
                                if (requiredAttributes[0].DefaultValue != null)
                                {
                                    if (!pInfo.ParameterType.Equals(requiredAttributes[0].DefaultValue.GetType()))
                                    {
                                        throw new Exception(string.Format("错误的默认值设定 resouce:{0} parameter:{1}", restInfo.ResourceName, parameter.Name));
                                    }
                                    //设置默认的值
                                    pInfo.DefaultValue = requiredAttributes[0].DefaultValue;                                   
                                }
                                pInfo.Description = requiredAttributes[0].Description;
                                pInfo.IsRequired = requiredAttributes[0].IsRequired;
                            }
                            restInfo.ParameterInfos.Add(pInfo);
                        }
                        manager.Register(restInfo);
                        if (RestCache.Enable)
                            cache.Register(restInfo);                        
                    }
                }
            }
        }

        /// <summary>
        /// 注册结束进行初始化
        /// </summary>
        public void RegisterComplete()
        {
            manager.RegisterComplete();
        }

        /// <summary>
        /// 获取接口信息
        /// </summary>
        /// <returns></returns>
        public string GetRestMethodsInfo(string format)
        {
            string content = null;
            if (string.IsNullOrEmpty(format))
            {
                content = GetSimple();
            }
            else
            {
                switch (format.ToLower())
                {
                    case "xml": content = GetXml(); break;
                    case "html": content = GetHtml(); break; 
                    default : content = GetSimple(); break;
                }
            }
            return content;
        }

        private string GetSimple()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div>Server site version : ");
            sb.Append(FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(this.GetType()).Location).FileVersion);
            sb.Append("</div>");
            sb.Append("<table>");
            foreach (RestMethodInfo info in manager.RestInfos.Values)
            {
                StringBuilder sb2 = new StringBuilder();
                for (int i = 1; i <= info.ParameterInfos.Count; i++)
                {
                    sb2.Append("&amp;");
                    sb2.Append(info.ParameterInfos[i - 1].ParameterName);
                    sb2.Append("={");
                    sb2.Append(i);
                    sb2.Append("}");
                }
                string url2 = "Rest/" + info.ActionName + "?format=xml";
                sb.Append("<tr><td>");
                sb.Append("<a href='" + url2 + sb2.ToString() + "'>");
                sb.Append(info.Description);
                sb.Append("</a>");
                sb.Append("</td><td>Url : ");
                sb.Append("<a href='");
                sb.Append(url2 + sb2.ToString());
                sb.Append("'>");
                sb.Append(url2 + sb2.ToString());
                sb.Append("</a></td></tr>");
            }
            sb.Append("<table>");

            return sb.ToString();
        }

        private string GetXml()
        {
            StringBuilder sb = new StringBuilder("<?xml version='1.0' encoding='utf-8'?><RestMethods version='" + FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(this.GetType()).Location).FileVersion + "'>");
            foreach (RestMethodInfo info in manager.RestInfos.Values)
            {
                sb.Append(XmlTools.ToXml(info, false).Trim());
            }
            sb.Append("</RestMethods>");
            return sb.ToString();
        }

        private string GetHtml()
        {
            String xml = GetXml();
            return xml;
        }

        /// <summary>
        /// 获取API管理器
        /// </summary>
        public RestAPIManager APIManager
        {
            get { return manager; }
        }

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] GetCacheResource(string key)
        {
            return cache.GetResource(key);
        }

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resource"></param>
        /// <param name="seconds"></param>
        public void PutCacheResource(string key, byte[] resource, int seconds)
        {
            cache.CacheResource(key, resource, seconds);
        }
    }
}
