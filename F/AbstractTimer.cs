using System;
using System.Collections.Generic;
using System.Text;
using Z.Log;
using System.Reflection;
using System.Threading;
using Z.C;

namespace Z.F
{
    /// <summary>
    /// 抽象线程类
    /// </summary>
    public abstract class AbstractTimer<T> : AbstractThread where T : ThreadConfiguration
    {
        #region 运行时状态

        /// <summary>
        /// 配置信息
        /// </summary>
        protected T MyConfig;

        /// <summary>
        /// 下次运行时间
        /// </summary>
        protected DateTime NextExecuteTime;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ThreadName">线程名称</param>
        /// <param name="config">配置</param>
        public AbstractTimer(string ThreadName, T config)
            : base(ThreadName)
        {
            MyConfig = config;
        }

        #endregion

        #region Start

        /// <summary>
        /// 启动线程
        /// </summary>
        public override void Start(bool IsStartImediately)
        {
            if (IsStartImediately)
                NextExecuteTime = DateTime.Now;
            else
                NextExecuteTime = DateTime.Now.AddSeconds(MyConfig.ExecuteTimeSecond);

            base.Start(IsStartImediately);
        }

        #endregion

        #region Execute

        /// <summary>
        /// Execute
        /// </summary>
        protected override void Execute()
        {
            ThisLogger.Debug(InternalThread.Name + "正在执行");

            while (!IsStop)
            {
                State = ZThreadState.SLEEP;

                if (DateTime.Now >= NextExecuteTime)
                {
                    State = ZThreadState.RUNNING;
                    //开始执行
                    CurrentExecuteTime = DateTime.Now;
                    try
                    {
                        Run(null);
                    }
                    catch (Exception ex)
                    {
                        ThisLogger.Error(ex);
                    }
                    finally
                    {
                        LastExecuteTime = CurrentExecuteTime;
                        NextExecuteTime = DateTime.Now.AddSeconds(MyConfig.ExecuteTimeSecond);
                    }
                }
                else
                {
                    Thread.Sleep(MyConfig.SleepTimeMillisecond);
                }
            }

            ThisLogger.Debug(Thread.CurrentThread.Name + "已停止");
            State = ZThreadState.STOP;
        }

        #endregion
    }
}
