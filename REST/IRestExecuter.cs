using System.Web;
using System;

namespace Z.Rest
{
    /// <summary>
    /// 处理Rest请求
    /// </summary>
    public interface IRestExecuter
    {
        /// <summary>
        /// 解析请求中的参数，并将解析结果放入线程对象中。返回调用业务方法需要的参数列表。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="manager"></param>
        string[] ParseParameter(HttpRequest request, RestAPIManager manager);

        /// <summary>
        /// 执行具体逻辑
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="parameters"></param>
        void Execute(RestAPIManager manager, string[] parameters);
    }
}
