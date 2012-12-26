using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Z.DA
{
    internal class DatabaseCache
    {
        /// <summary>
        /// 静态语句， 用于缓存所有执行过的语句
        /// </summary>
        internal static Dictionary<string, DatabaseCache> Instance = new Dictionary<string, DatabaseCache>();

        /// <summary>
        /// 线程静态连接池
        /// </summary>
        [ThreadStatic]
        private static Dictionary<string, IDbConnection> connectionList;

        /// <summary>
        /// 线程静态连接池
        /// </summary>
        internal static Dictionary<string, IDbConnection> ThreadConnectionPool
        {
            get
            {
                if (connectionList == null)
                {
                    connectionList = new Dictionary<string, IDbConnection>();
                }

                return connectionList;
            }
        }

        public string SQL;
        public IDataParameter[] Parameters;
        public DataTable CurrentTable;
        public bool IsHaveAutoIncrement;

        public DatabaseCache(string text, IDataParameter[] ParameterList)
        {
            SQL = text;
            Parameters = ParameterList;
        }

        public DatabaseCache(string text, IDataParameterCollection ParameterList)
        {
            SQL = text;

            Parameters = new IDataParameter[ParameterList.Count];

            ParameterList.CopyTo(Parameters, 0);
        }
    }
}
