using System;
using System.Collections.Generic;
using System.Text;
using Z.Ex;

namespace Z.Log
{
    /// <summary>
    /// 日志实体类
    /// </summary>
    public class LogInfo
    {
        /// <summary>
        /// 替代null值的字符串
        /// </summary>
        protected const string NULL_STRING = "<NULL>";

        /// <summary>
        /// 实体格式
        /// </summary>
        protected string Format { get; set; }
        /// <summary>
        /// 实体的参数
        /// </summary>
        protected List<string> Args = new List<string>();

        /// <summary>
        /// 日志实体类构造函数
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public LogInfo(string format, params object[] args)
        {
            this.Format = format;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    Args.Add(NULL_STRING);
                }
                else
                {
                    if (args[i] is Exception)
                    {
                        Args.Add(ExceptionFormatter.FormatException(args[i] as Exception));
                    }
                    else
                    {
                        Args.Add(args[i].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameter">string 类型的参数</param>
        /// <returns>当前LogInfo实例</returns>
        public LogInfo AppendParameter(string parameter)
        {
            if (parameter == null)
            {
                Args.Add(NULL_STRING);
            }
            else
            {
                Args.Add(parameter);
            }
            return this;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameter">object 类型的参数</param>
        /// <returns>当前LogInfo实例</returns>
        public LogInfo AppendParameter(object parameter)
        {
            if (parameter == null)
            {
                Args.Add(NULL_STRING);
            }
            else
            {
                Args.Add(parameter.ToString());
            }
            return this;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameter">Exception 类型的参数</param>
        /// <returns>当前LogInfo实例</returns>
        public LogInfo AppendParameter(Exception parameter)
        {
            if (parameter == null)
            {
                Args.Add(NULL_STRING);
            }
            else
            {
                Args.Add(ExceptionFormatter.FormatException(parameter));
            }
            return this;
        }

        /// <summary>
        /// 返回格式化的log item
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(Format, Args.ToArray());
        }
    }
}
