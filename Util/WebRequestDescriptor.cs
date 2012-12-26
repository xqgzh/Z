using System;

namespace Z.Util
{
    /// <summary>
    /// 一个Web请求的数据化描述，用于模拟Web请求。
    /// </summary>
    [Serializable]
    public class WebRequestDescriptor
    {
        /// <summary>
        /// 发送请求时使用的HTTP头中的Host值。只有.NET 4支持修改此属性。
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 请求的Web地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求Web地址的方式。一般为WebRequestMethods.Http.Post或WebRequestMethods.Http.Get
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 在Method是Post时，发送的Data。可用于String.Format。
        /// </summary>
        public string PostData { get; set; }

        /// <summary>
        /// HTTP请求头中的ContetnType。默认是用于POST的application/x-www-form-urlencoded
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// WebRequestDescriptor的默认构造函数
        /// </summary>
        /// <param name="method">Web请求的方式，一般为WebRequestMethods.Http.Post或WebRequestMethods.Http.Get</param>
        /// <param name="host">发送请求时使用的HTTP头中的Host值。</param>
        /// <param name="url">请求的Web地址</param>
        /// <param name="contentType">请求的Http头中的ContentType，默认为application/x-www-form-urlencoded</param>
        public WebRequestDescriptor(string method, string host, string url, string contentType = "application/x-www-form-urlencoded")
        {
            Method = method;
            Host = host;
            Url = url;
            ContentType = contentType;
            PostData = String.Empty;
        }
    }
}
