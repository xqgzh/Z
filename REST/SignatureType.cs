using System;
using System.Collections.Generic;
using System.Text;

namespace Z.Rest
{
    /// <summary>
    /// 服务器端验证类型
    /// </summary>
    public enum SignatureType
    {
        /// <summary>
        /// 无需验证
        /// </summary>
        None, 
        
        /// <summary>
        /// 静态密钥签名
        /// </summary>
        StaticSHA1, 
        
        /// <summary>
        /// 动态密钥签名
        /// </summary>
        DynamicSHA1
    }
}
