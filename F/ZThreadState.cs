using System;
using System.Collections.Generic;
using System.Text;

namespace Z.F
{
    /// <summary>
    /// 抽象线程状态
    /// </summary>
    public enum ZThreadState
    {
        /// <summary>
        /// 初始化
        /// </summary>
        INIT = 1,

        /// <summary>
        /// 参数已经准备好
        /// </summary>
        READY = 2,

        /// <summary>
        /// 正在启动
        /// </summary>
        START = 3,

        /// <summary>
        /// 正在睡眠状态
        /// </summary>
        SLEEP = 4,

        /// <summary>
        /// 正在执行状态
        /// </summary>
        RUNNING = 5,

        /// <summary>
        /// 正在停止状态
        /// </summary>
        STOP
    }
}
