using System;
using System.Threading;
using Z.C;

namespace Z.F
{
    /// <summary>
    /// 定时执行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractSchedule<T> : AbstractThread where T: ScheduleConfiguration
    {
        /// <summary>
        /// 日程计划配置项
        /// </summary>
        protected abstract T MyConfig { get; }

        /// <summary>
        /// 是否正在执行
        /// </summary>
        bool IsExecuting = false;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ThreadName"></param>
        public AbstractSchedule(string ThreadName)
            : base(ThreadName)
        {
            
        }

        /// <summary>
        /// 线程轮询
        /// </summary>
        protected override void Execute()
        {
            ThisLogger.Debug(InternalThread.Name + "正在执行");

            while (!IsStop)
            {
                State = ZThreadState.SLEEP;

                bool CurrentExecute = CheckNeedRun();

                if (CurrentExecute == true && IsExecuting == false)
                {
                    IsExecuting = true;

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
                    }
                }
                else
                {
                    if(CurrentExecute == false)
                        IsExecuting = false;
                    Thread.Sleep(MyConfig.SleepTimeMillisecond);
                }
            }

            ThisLogger.Debug(Thread.CurrentThread.Name + "已停止");
            State = ZThreadState.STOP;
        }

        private bool CheckNeedRun()
        {
            string s = MyConfig.TimeFomrat;

            switch (MyConfig.TimeFomrat)
            {
                case "s":
                case "m":
                case "H":
                case "h":
                case "M":
                    s += s;
                    break;
            }

            string TimeToRun = DateTime.Now.ToString(s);

            switch (MyConfig.TimeFomrat)
            {
                case "s":
                case "m":
                case "H":
                case "h":
                case "M":
                    TimeToRun = TimeToRun.Substring(1, 1);
                    break;
            }

            bool CurrentExecute = (string.Compare(TimeToRun, MyConfig.ExecuteTime) == 0);
            return CurrentExecute;
        }
    }
}
