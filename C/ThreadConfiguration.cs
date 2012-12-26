using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Z.C
{
    /// <summary>
    /// 线程配置项
    /// </summary>
    [Serializable]
    public class ThreadConfiguration
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ThreadConfiguration() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="threadCount">线程数量</param>
        public ThreadConfiguration(int threadCount) 
        {
            ThreadCount = threadCount;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="threadCount">线程数量</param>
        /// <param name="executeTimeSecond">每次执行间隔时间</param>
        public ThreadConfiguration(int threadCount, int executeTimeSecond)
        {
            ThreadCount = threadCount;
            ExecuteTimeSecond = executeTimeSecond;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="threadCount">线程数量</param>
        /// <param name="executeTimeSecond">每次执行间隔时间</param>
        /// <param name="sleepTimeMillisecond">轮空睡眠时间</param>
        public ThreadConfiguration(int threadCount, int executeTimeSecond, int sleepTimeMillisecond)
        {
            ThreadCount = threadCount;
            ExecuteTimeSecond = executeTimeSecond;
            SleepTimeMillisecond = sleepTimeMillisecond;
        }

        #endregion

        /// <summary>
        /// 线程数量
        /// </summary>
        [XmlAttribute]
        public int ThreadCount = 1;

        /// <summary>
        /// 每次轮空睡眠时间, 默认500毫秒
        /// </summary>
        [XmlAttribute]
        public int SleepTimeMillisecond = 500;

        /// <summary>
        /// 每次执行间隔时间, 默认600秒, 10分钟
        /// </summary>
        [XmlAttribute]
        public int ExecuteTimeSecond = 600;
    }
}
