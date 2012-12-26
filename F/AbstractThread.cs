using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Z.Log;
using System.Reflection;

namespace Z.F
{
    /// <summary>
    /// 抽象线程类
    /// </summary>
    public abstract class AbstractThread : IDisposable
    {
        /// <summary>
        /// 日志
        /// </summary>
        internal static readonly Logger ThisLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 内部线程
        /// </summary>
        protected Thread InternalThread;

        /// <summary>
        /// 上次运行时间
        /// </summary>
        protected DateTime LastExecuteTime;

        /// <summary>
        /// 当前的执行时间
        /// </summary>
        protected DateTime CurrentExecuteTime;

        /// <summary>
        /// 线程状态
        /// </summary>
        public ZThreadState State = ZThreadState.INIT;

        /// <summary>
        /// 线程停止信号
        /// </summary>
        public bool IsStop { get; set; }

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ThreadName">线程名称</param>
        public AbstractThread(string ThreadName)
        {
            InternalThread = new Thread(Execute);
            InternalThread.Name = ThreadName;
            State = ZThreadState.READY;
        }

        #endregion

        #region Start

        /// <summary>
        /// 启动线程
        /// </summary>
        public virtual void Start(bool IsStartImediately)
        {
            IsStop = false;

            if (State == ZThreadState.INIT)
                throw new ArgumentException(this.GetType().Name + "未初始化");

            State = ZThreadState.START;

            InternalThread.Start();
        }

        #endregion

        /// <summary>
        /// 抽象方法-线程执行
        /// </summary>
        protected abstract void Execute();

        /// <summary>
        /// 抽象方法-业务运行
        /// </summary>
        protected abstract void Run(object o);


        #region IDisposable Members

        /// <summary>
        /// 销毁线程
        /// </summary>
        public virtual void Dispose()
        {
            InternalThread = null;
        }

        #endregion


    }
}
