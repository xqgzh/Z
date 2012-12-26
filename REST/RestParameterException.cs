using System;
using System.Collections.Generic;
using System.Text;

namespace Z.Rest
{
    /// <summary>
    /// 输出参数异常
    /// </summary>
    public class RestParameterException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public RestParameterException(string name, string value, string type)
            : base("name : " + name + " value : " + value + " expect type : " + type)
        {

        }
    }
}