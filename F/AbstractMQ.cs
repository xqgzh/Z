using System;
using System.Messaging;
using System.Threading;
using Z.C;
using Z.Log;
using Z.Util;

namespace Z.F
{
    /// <summary>
    /// 抽象消息队列调度类
    /// </summary>
    public abstract class AbstractMQ<T> : AbstractThread where T: MessageQueueConfig
    {
        static AbstractMQ()
        {
            //if(MessageQueue.EnableConnectionCache == false)
                //MessageQueue.EnableConnectionCache = true;
        }

        #region 运行时状态

        /// <summary>
        /// 配置信息
        /// </summary>
        protected abstract T MyConfig { get; }

        /// <summary>
        /// 当前收到的Message
        /// </summary>
        protected Message CurrentMessage;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ThreadName">线程名称</param>
        public AbstractMQ(string ThreadName)
            : base(ThreadName)
        {
        }

        #endregion

        #region Execute

        /// <summary>
        /// Execute
        /// </summary>
        protected override void Execute()
        {
            ThisLogger.Debug(InternalThread.Name + "已启动");
            State = ZThreadState.START;
            try
            {
                BinaryMessageFormatter formater = new BinaryMessageFormatter();

                while (!IsStop)
                {
                    State = ZThreadState.SLEEP;
                    CurrentMessage = null;
                    CurrentMessage = TryReceiveMesssage();

                    try
                    {
                        if (CurrentMessage != null)
                        {
                            State = ZThreadState.RUNNING;
                            CurrentMessage.Formatter = formater;

                            if (CurrentMessage.Body != null)
                            {
                                CurrentExecuteTime = DateTime.Now;
                                try
                                {
                                    Run(CurrentMessage.Body);
                                }
                                catch (Exception ex)
                                {
                                    ThisLogger.Error(ex, MyConfig.Address, CurrentMessage.Body);

                                    //失败重发, 根据配置执行转发或者重试计数
                                    ReSendMQ();
                                }
                                finally
                                {
                                    LastExecuteTime = CurrentExecuteTime;
                                }
                            }
                            else
                            {
                                if (IsStop) break;
                                Thread.Sleep(MyConfig.SleepMilliSecond);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ThisLogger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ThisLogger.Error(ex, MyConfig.Address);
            }

            ThisLogger.Debug(InternalThread.Name + "已停止");
            State = ZThreadState.STOP;
        }

        #endregion

        #region 私有方法

        #region TryReceiveMesssage

        /// <summary>
        /// 从队列中获取消息, 默认3秒超时
        /// </summary>
        /// <returns></returns>
        protected virtual Message TryReceiveMesssage()
        {
            try
            {
                MessageQueue mqServer = new MessageQueue(MyConfig.Address);

                return mqServer.Receive(TimeSpan.FromSeconds(MyConfig.TimeoutSecond));
            }
            catch (MessageQueueException mqe)
            {
                switch (mqe.MessageQueueErrorCode)
                {
                    case MessageQueueErrorCode.IOTimeout:
                        return null;
                    default:
                        if (Convert.ToInt32(mqe.MessageQueueErrorCode) == -2147023170)
                            return null;
                        break;
                    case MessageQueueErrorCode.QueueNotFound:
                        throw;
                }

                ThisLogger.Error(mqe, MyConfig.Address, mqe.ErrorCode, mqe.MessageQueueErrorCode);
                Thread.Sleep(1000);
                return null;
            }
        }

        #endregion

        #region RetryMQ

        /// <summary>
        /// 将获取到的消息重新发送到队列中, 根据配置, 可以转发到其他MQ队列, 也可以执行重试计数
        /// </summary>
        protected void ReSendMQ()
        {
            if (CurrentMessage == null)
                return;

            if (string.IsNullOrEmpty(MyConfig.RetryAddress) == false)
            {
                //失败转发
                try
                {
                    MessageQueueTools.SendMessage(MyConfig.RetryAddress, CurrentMessage);
                }
                catch (Exception ex)
                {
                    ThisLogger.Error(ex, MyConfig.RetryAddress, CurrentMessage.Body);
                }
            }
            else if (MyConfig.RetryCount > 0)
            {
                //失败重试
                int TryTimes = 0; 
                
                try
                {
                    if (string.IsNullOrEmpty(CurrentMessage.Label) || int.TryParse(CurrentMessage.Label, out TryTimes) == false)
                        TryTimes = 0;

                    if (TryTimes < MyConfig.RetryCount)
                    {
                        TryTimes = TryTimes + 1;

                        CurrentMessage.Label = Convert.ToString(TryTimes);

                        MessageQueueTools.SendMessage(MyConfig.Address, CurrentMessage);
                    }
                    else
                    {
                        ThisLogger.Warn(new LogInfo("{0}超过重试计数({1})", CurrentMessage.Body, TryTimes));
                    }
                }
                catch (Exception ex)
                {
                    ThisLogger.Error(ex, MyConfig.Address, CurrentMessage.Label);
                }
            }
        }

        #endregion

        #endregion
    }
}
