using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z.Log;
using System.Reflection;
using System.Messaging;
using Z.Util;
using Z.C;

namespace Z.Access
{
    /// <summary>
    /// 访问日志记录器
    /// </summary>
    public class AccessLogger
    {
        private readonly static Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 记录访问日志

        /// <summary>
        /// 记录访问日志
        /// </summary>
        /// <param name="log"></param>
        public void Log(AccessInfo log)
        {
            try
            {
                MessageQueueConfig config = GetConfig(log);

                MessageQueueTools.SendMessage(
                    config.Address,
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
        MessageQueueConfig GetConfig(AccessInfo log)
        {
            foreach (AssessMQ mq in AccessConfig.Instance.LogServers)
            {
                foreach (var action in mq.Actions)
                {
                    if (string.Compare(log.Action, action) == 0)
                    {
                        return mq;
                    }
                }
            }

            return AccessConfig.Instance.DefaultLogServer;
        }

        #endregion
    }
}
