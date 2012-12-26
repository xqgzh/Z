using System;

namespace Z.Rest
{
    /// <summary>
    /// Rest 方法属性实体
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RestAPIMethodAttribute : System.Attribute
    {
        /// <summary>
        /// 方法ID
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 方法标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 记录返回值的最大长度  
        /// -1 : 不记录
        /// 0  : 完全记录
        /// others : 记录长度
        /// </summary>
        public int MaxLogLength { get; set; }

        /// <summary>
        /// 方法需要的安全级别
        /// </summary>
        public int[] SecurityLevel { get; set; }

        /// <summary>
        /// 默认的呈现器
        /// </summary>
        public string DefaultHandler { get; set; }

        /// <summary>
        /// 访问控制策略
        /// 
        /// 0:不使用任何访问控制
        /// 1:要求验证访问链接标识(valid参数或者session flag)
        /// 2:编辑session flag
        /// </summary>
        public int AccessPolicy { get; set; }

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

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RestAPIMethodAttribute()
        {

        }
        /// <summary>
        /// Rest 方法属性构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="actionId"></param>
        /// <param name="securityType">需要的认证级别</param>
        public RestAPIMethodAttribute(string name, int actionId, params int[] securityType)
        {
            this.Name = name;
            this.ActionId = actionId;
            this.SecurityLevel = securityType;
        }
    }


}