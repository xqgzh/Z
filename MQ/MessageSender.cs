using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Z.C;
using Z.DA;
using Z.Log;
using Z.Util;

namespace Z.MQ
{
    /// <summary>
    /// 消息发送
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageSender<T> where T : IMesaageMatch, new()
    {
        private readonly static Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        private MessageQueueConfig Config;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public MessageSender(MessageQueueConfig config)
        {
            Config = config;
        }

        #region 记录访问日志

        /// <summary>
        /// 记录访问日志
        /// </summary>
        /// <param name="log"></param>
        public void Send(T log)
        {
            try
            {
                MessageQueueConfig mq = GetConfig(log);

                MessageQueueTools.SendMessage(
                    mq.Address,
                    log, string.Empty, new BinaryMessageFormatter());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion

        #region 获取配置信息

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        MessageQueueConfig GetConfig(T log)
        {
            foreach (MessageQueueConfig mq in Config.DispatchConfigs)
            {
                foreach (var dispatchValue in mq.DispatchValues)
                {
                    if(log.IsMatch(dispatchValue))
                    {
                        return mq;
                    }
                }
            }

            return Config;
        }

        private string GetValue(string name, T log)
        {
            return string.Empty;
        }

        #endregion
    }
}
