using System;
using System.Diagnostics;
using System.Text;
using log4net;
using log4net.Config;
using Z.Ex;
using Z.Util;

namespace Z.Log
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class Logger
    {
        private const string NULL_STRING = "<NULL>";
        private ILog logger;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static Logger()
        {
            try
            {
                XmlConfigurator.ConfigureAndWatch(FileTools.FindFile("log4net.config"));
            }
            catch (Exception)
            {
                System.Diagnostics.Trace.TraceError("load log4net configuration file failed.");
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">日志名称</param>
        public Logger(string name)
        {
            logger = LogManager.GetLogger(name);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">日志类型</param>
        public Logger(Type type)
        {
            logger = LogManager.GetLogger(type);
        }

        #region IsEnabled
        /// <summary>
        /// 是否记录Debug日志
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return logger.IsDebugEnabled; }
        }
        /// <summary>
        /// 是否记录Info日志
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return logger.IsInfoEnabled; }
        }
        /// <summary>
        /// 是否记录Warn日志
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return logger.IsWarnEnabled; }
        }
        /// <summary>
        /// 是否记录Error日志
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return logger.IsErrorEnabled; }
        }

        #endregion

        #region Debug
        /// <summary>
        /// 写入Debug日志
        /// </summary>
        /// <param name="message">日志</param>
        public void Debug(string message)
        {
            logger.Debug(message);
            Trace.TraceInformation("Debug:" + message);
        }
        /// <summary>
        /// 写入Debug日志
        /// </summary>
        /// <param name="info">日志实体</param>
        public void Debug(LogInfo info)
        {
            logger.Debug(info.ToString());
            Trace.TraceInformation("Debug:" + info.ToString());
        }
        /// <summary>
        /// 写入Debug日志
        /// </summary>
        /// <param name="parameters">日志参数</param>
        public void Debug(params object[] parameters)
        {
            if (logger.IsDebugEnabled)
            {
                StringBuilder sb = new StringBuilder();
                foreach (object obj in parameters)
                {
                    if (obj != null)
                    {
                        sb.Append(obj.ToString());
                        sb.Append("    ");
                    }
                }
                logger.Debug(sb.ToString());
            }
        }
        #endregion

        #region Info
        /// <summary>
        /// 写入Info日志
        /// </summary>
        /// <param name="message">日志</param>
        public void Info(string message)
        {
            logger.Info(message);
            Trace.TraceInformation("Info:" + message);
        }
        /// <summary>
        /// 写入Info日志
        /// </summary>
        /// <param name="info">日志实体</param>
        public void Info(LogInfo info)
        {
            logger.Info(info.ToString());
        }
        /// <summary>
        /// 写入Info日志
        /// </summary>
        /// <param name="parameters">日志参数数组</param>
        public void Info(params object[] parameters)
        {
            if (logger.IsInfoEnabled)
            {
                StringBuilder sb = new StringBuilder();
                foreach (object obj in parameters)
                {
                    if (obj != null)
                    {
                        sb.Append(obj.ToString());
                        sb.Append("    ");
                    }
                }
                logger.Info(sb.ToString());
            }
        }
        #endregion

        #region Warning
        /// <summary>
        /// 写入Warn日志
        /// </summary>
        /// <param name="message">日志</param>
        public void Warn(string message)
        {
            logger.Warn(message);
            Trace.TraceWarning("Warn:" + message);
        }
        /// <summary>
        /// 写入Warn日志
        /// </summary>        
        public void Warn(LogInfo info)
        {
            logger.Warn(info.ToString());
        }
        /// <summary>
        /// 写入Warn日志
        /// </summary>
        /// <param name="parameters">日志参数数组</param>
        public void Warn(params object[] parameters)
        {
            if (logger.IsWarnEnabled)
            {
                StringBuilder sb = new StringBuilder();
                foreach (object obj in parameters)
                {
                    if (obj != null)
                    {
                        sb.Append(obj.ToString());
                        sb.Append("    ");
                    }
                }
                logger.Warn(sb.ToString());
            }
        }
        #endregion

        #region Error
        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="message">日志</param>
        public void Error(string message)
        {
            logger.Error(message);
            Trace.TraceError("Error:" + message);
        }
        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="info">日志实体</param>
        public void Error(LogInfo info)
        {
            logger.Error(info.ToString());
        }

        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="ex">异常</param>
        public void Error(Exception ex)
        {
            logger.Error(ExceptionFormatter.FormatException(ex));
        }

        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="message">其它日志信息</param>
        public void Error(Exception ex, string message)
        {
            logger.Error(message + Environment.NewLine + ExceptionFormatter.FormatException(ex));
        }

        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="parameters">日志参数数组</param>
        public void Error(Exception ex, params object[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object obj in parameters)
            {
                if (obj != null)
                {
                    sb.Append(obj.ToString());
                    sb.Append("    ");
                }
                else
                {
                    sb.Append(NULL_STRING);
                    sb.Append("    ");
                }
            }
            logger.Error(sb.ToString() + Environment.NewLine + ExceptionFormatter.FormatException(ex));
        }
        #endregion

    }
}
