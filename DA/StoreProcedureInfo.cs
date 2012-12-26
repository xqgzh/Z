using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Z.DA
{
    /// <summary>
    /// 存储过程的返回值数据
    /// </summary>
    [Serializable]
    public class StoreProcedureInfo
    {
        /// <summary>
        /// 输出参数字典
        /// </summary>
        public Dictionary<string, object> OutputParameters = new Dictionary<string, object>();

        /// <summary>
        /// 返回值
        /// </summary>
        public int ReturnCode;

        /// <summary>
        /// 在存储过程中Select的结果集
        /// </summary>
        public DataSet DataSet;
    }
    
    /// <summary>
    /// 基于泛型的存储过程返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class StoreProcedureList<T>
    {
        /// <summary>
        /// 输出参数字典
        /// </summary>
        public Dictionary<string, object> OutputParameters = new Dictionary<string, object>();

        /// <summary>
        /// 返回值
        /// </summary>
        public int ReturnCode;

        /// <summary>
        /// 在存储过程中Select的结果集
        /// </summary>
        public List<T> List = new List<T>();
    }
}
