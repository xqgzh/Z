using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Z.C
{
    /// <summary>
    /// 计划定时执行的时间表
    /// </summary>
    [Serializable]
    public class ScheduleConfiguration
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ScheduleConfiguration() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="threadCount"></param>
        /// <param name="timeFomrat"></param>
        /// <param name="executeTime"></param>
        public ScheduleConfiguration(int threadCount, string timeFomrat, string executeTime)
        {
            ThreadCount = threadCount;
            TimeFomrat = timeFomrat;
            ExecuteTime = executeTime;
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
        /// 时间格式化字符串, 用于计算当前时间是否符合执行时机
        /// </summary>
        [XmlAttribute]
        public string TimeFomrat { get; set; }

        /// <summary>
        /// 当前时间经过TimeFomrat格式化之后的结果与ExecuteTime相符合, 就表示是触发实际
        /// </summary>
        [XmlAttribute]
        public string ExecuteTime;
    }
}
