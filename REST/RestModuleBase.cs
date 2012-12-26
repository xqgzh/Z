using System;
using System.Web;

namespace Z.Rest
{
    /// <summary>
    /// 处理Rest请求的HttpModule基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RestModuleBase<T> : IHttpModule where T : IRestExecuter, new()
    {
        /// <summary>
        /// http module 处理逻辑
        /// </summary>
        protected static T executer = new T();

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
            context.AcquireRequestState += AcquireRequestState;
            context.PreRequestHandlerExecute += PreRequestHandlerExecute;
            context.EndRequest += EndRequest;
            context.Error += Error;
            
        }

        /// <summary>
        /// 获取http请求并解析所需参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void BeginRequest(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 处理session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AcquireRequestState(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 执行业务逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void PreRequestHandlerExecute(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 获取http返回并加入所需数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EndRequest(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 捕获执行过程中的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Error(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
