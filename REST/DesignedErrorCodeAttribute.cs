using System;
using System.Collections.Generic;
using System.Text;

namespace Z.Rest
{
    /// <summary>
    /// 用于声明接口处对所抛出业务异常的约束
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DesignedErrorCodeAttribute : Attribute
    {
        /// <summary>
        /// 声明该attribute的方法或处于声明该attribute的类中的方法所能跑出异常的errorcode集合
        /// </summary>
        public int[] Codes { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="codes"></param>
        public DesignedErrorCodeAttribute(params int[] codes)
        {
            Codes = codes;
        }
    }
}
