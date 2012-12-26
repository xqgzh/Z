using System.Messaging;

namespace Z.Util
{
    /// <summary>
    /// 消息工具
    /// </summary>
    public static class MessageQueueTools
    {
        static MessageQueueTools()
        {
            //if(MessageQueue.EnableConnectionCache == false)
                //MessageQueue.EnableConnectionCache = true;
        }

        #region 静态消息发送方法

        /// <summary>
        /// 静态消息发送方法
        /// </summary>
        /// <param name="mqServer">消息服务器</param>
        /// <param name="o">发送对象</param>
        /// <param name="label">标签</param>
        /// <param name="formater">格式化对象</param>
        public static void SendMessage(MessageQueue mqServer, object o, string label, IMessageFormatter formater)
        {
            Message msg = new Message(o, formater);
            msg.Recoverable = true;
            msg.Label = label;

            mqServer.Send(msg);
        }

        /// <summary>
        /// 静态消息发送方法
        /// </summary>
        /// <param name="MessageQueuePath">消息服务器地址</param>
        /// <param name="o">发送对象</param>
        /// <param name="label">标签</param>
        /// <param name="formater">格式化对象</param>
        public static void SendMessage(string MessageQueuePath, object o, string label, IMessageFormatter formater)
        {
            using (MessageQueue mq = new MessageQueue(MessageQueuePath))
            {
                mq.Formatter = formater;

                Message msg = new Message(o, formater);
                msg.Recoverable = true;
                msg.Label = label;

                mq.Send(msg);
            }
        }

        /// <summary>
        /// 静态消息发送方法
        /// </summary>
        /// <param name="MessageQueuePath">消息服务器地址</param>
        /// <param name="msg">消息体</param>
        public static void SendMessage(string MessageQueuePath, Message msg)
        {
            using (MessageQueue mq = new MessageQueue(MessageQueuePath))
            {
                if(msg.Formatter != null)
                    mq.Formatter = msg.Formatter;

                SendMessage(mq, msg);
            }
        }

        /// <summary>
        /// 静态消息发送方法
        /// </summary>
        /// <param name="mqServer">消息服务器</param>
        /// <param name="msg">消息体</param>
        public static void SendMessage(MessageQueue mqServer, Message msg)
        {
            msg.Recoverable = true;
            mqServer.Send(msg);
        }
        #endregion
    }
}
