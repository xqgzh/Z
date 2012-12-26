using System;
using System.Collections.Generic;
using System.Text;

namespace Z.Ex
{
    /// <summary>
    /// 异常基类
    /// </summary>
    public abstract class Exceptionbase : Exception
    {
        /// <summary>
        /// 是否格式化堆栈信息
        /// </summary>
        internal protected bool IsFormatStack = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Exceptionbase()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msg"></param>
        public Exceptionbase(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public Exceptionbase(string msg, Exception ex)
            : base(msg, ex)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sbXml"></param>
        public virtual void ToXmlElements(StringBuilder sbXml)
        {
        }
    }
}
