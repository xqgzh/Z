using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Z.MQ
{
    /// <summary>
    /// 消息匹配
    /// </summary>
    public interface IMesaageMatch
    {
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsMatch(string value);
    }
}
