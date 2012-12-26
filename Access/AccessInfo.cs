using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Z.Access
{
    /// <summary>
    /// 访问日志实体信息
    /// </summary>
    [DataContract]
    [Serializable]
    public class AccessInfo
    {
        #region 服务端信息

        /// <summary>
        /// 服务器标识
        /// </summary>
        [DataMember]
        public string ServerIP { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        [DataMember]
        public string ServiceName { get; set; }

        /// <summary>
        /// 线程名(id)
        /// </summary>
        [DataMember]
        public string ThreadName { get; set; }

        /// <summary>
        /// 打印该条日志的logger名
        /// </summary>
        [DataMember]
        public string Logger { get; set; }

        #endregion

        #region 运行信息

        /// <summary>
        /// 当前请求的动作
        /// </summary>
        [DataMember]
        public string Action { get; set; }

        /// <summary>
        /// 访问开始时间
        /// </summary>
        [DataMember]
        public string StartTime { get; set; }

        /// <summary>
        /// 访问消耗时间
        /// </summary>
        [DataMember]
        public string CostTime { get; set; }

        /// <summary>
        /// 返回码
        /// </summary>
        [DataMember]
        public string ReturnCode { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// 请求记录
        /// </summary>
        [DataMember]
        public string Request { get; set; }

        /// <summary>
        /// 返回结果
        /// </summary>
        [DataMember]
        public string Response { get; set; }

        #endregion

        #region 客户端信息

        /// <summary>
        /// 客户端IP
        /// </summary>
        [DataMember]
        public string ClientIP { get; set; }

        /// <summary>
        /// 客户端唯一识别码
        /// </summary>
        [DataMember]
        public string ClientID { get; set; }

        /// <summary>
        /// 客户端产品编号
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 客户端产品版本
        /// </summary>
        [DataMember]
        public string ProductVer { get; set; }

        /// <summary>
        /// 渠道编号
        /// </summary>
        [DataMember]
        public string Channel { get; set; }

        /// <summary>
        /// UserAgent
        /// </summary>
        [DataMember]
        public string UserAgent { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [DataMember]
        public string UserAccount { get; set; }

        /// <summary>
        /// token或者sessionid
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        #endregion
    }
}
