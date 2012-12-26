using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace Z.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public class RestMethodInfo
    {
        /// <summary>
        /// 未知资源
        /// </summary>
        public static readonly RestMethodInfo UnknownMethod = new RestMethodInfo();

        static RestMethodInfo()
        {
            UnknownMethod.ActionId = 0;
            UnknownMethod.ActionName = "Unknown";
            UnknownMethod.DefaultHandler = null;
            UnknownMethod.Description = "未知资源";
            UnknownMethod.ErrorCodes = new int[] { 9003};
            UnknownMethod.ExpireSeconds = 0;
            UnknownMethod.IsCached = false;
            UnknownMethod.MaxLogLength = 0;
            UnknownMethod.MethodName = "Unknown";
            UnknownMethod.ParameterInfos = null;
            UnknownMethod.ProxyMethodInfo = null;
            UnknownMethod.ResourceName = "Unknown";
            UnknownMethod.SecurityLevel = new int[]{0};
        }

        private Type returnType;
        /// <summary>
        /// 资源ID
        /// </summary>
        public int ActionId { get; set; }
        /// <summary>
        /// 资源名
        /// </summary>
        [XmlIgnore]
        public string ActionName { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 方法标题
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 返回值类型string表示
        /// </summary>
        [XmlElement("ReturnType")]
        public string ReturnTypeString { get; set; }
        /// <summary>
        /// 方法需要的安全级别
        /// </summary>
        public int[] SecurityLevel { get; set; }
        /// <summary>
        /// 记录返回值的最大长度  
        /// -1 : 不记录
        /// 0  : 完全记录
        /// others : 记录长度
        /// </summary>
        public int MaxLogLength { get; set; }
        /// <summary>
        /// 资源全名
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCached { get; set; }

        /// <summary>
        /// 自定义CacheKey, 在原有Key基础之上附加此函数的返回字符串
        /// </summary>
        public CachedMethodAttribute CacheMethodAttr { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public int ExpireSeconds { get; set; }
        /// <summary>
        /// 默认的呈现器
        /// </summary>
        public string DefaultHandler { get; set; }
        /// <summary>
        /// 返回值类型
        /// </summary>
        [XmlIgnore]
        public Type ReturnType { 
            get 
            { 
                return returnType; 
            } 
            
            set 
            { 
                returnType = value; 
                ReturnTypeString = value.FullName; 
            }
        }
        /// <summary>
        /// 参数类型
        /// </summary>
        public List<RestParameterInfo> ParameterInfos { get; set; }
        
        /// <summary>
        /// 该方法可能抛出的业务异常的errorcode集合
        /// </summary>
        public int[] ErrorCodes { get; set; }
        /// <summary>
        /// 访问控制策略
        /// 
        /// 0:不使用任何访问控制
        /// 1:要求验证访问链接标识(valid参数或者session flag)
        /// 2:编辑session flag
        /// </summary>
        public int AccessPolicy { get; set; }
        /// <summary>
        /// 是否不建议使用
        /// </summary>
        public bool IsObsolete { get; set; }
        /// <summary>
        /// 所代理的方法的信息
        /// </summary>
        [XmlIgnore]
        public MethodInfo ProxyMethodInfo { get; set; }

        /// <summary>
        /// IP访问许可
        /// false:不检查IP访问许可
        /// true：检查访问许可
        /// </summary>
        public bool IpSecurityCheck = false;

        /// <summary>
        /// 启用OSap的验证
        /// </summary>
        public bool EnableOsapCheck = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public class RestParameterInfo
    {
        private object defaultValue;
        private string stringValue;
        private Type parameterType;

        /// <summary>
        /// 是否必须
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 参数名
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数类型的string值
        /// </summary>
        [XmlElement("ParameterType")]
        public string ParameterTypeString { get; set; }

        /// <summary>
        /// 参数描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        [XmlIgnore]
        public Type ParameterType { get { return parameterType; } set { parameterType = value; ParameterTypeString = value.FullName; } }

        /// <summary>
        /// 默认值
        /// </summary>
        [XmlIgnore]
        public object DefaultValue
        {
            set
            {
                defaultValue = value;
                stringValue = value == null ? null : value.ToString();
            }
            get { return defaultValue; }
        }

        /// <summary>
        /// 默认值的字符串表示
        /// </summary>
        [XmlElement("DefaultValue")]
        public string StringValue
        {
            get { return stringValue; }
        }
    }
}
