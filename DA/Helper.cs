using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Z.DA.DB;
using Z.Ex;
using System.Configuration;
using System.Collections;

namespace Z.DA
{
    class Helper
    {
        private IDbConnection MyConnection;
        private IDatabase MyDatabase;

        #region 构造函数

        #region 构造函数， 通过App.Config中的ConnectionSection中获取连接字符串和提供商, 创建Connetion并打开

        /// <summary>
        /// 构造函数， 通过App.Config中的ConnectionSection中获取连接字符串和提供商, 创建Connetion并打开
        /// </summary>
        /// <param name="ConnectionName"></param>
        public Helper(string ConnectionName)
        {
            ConnectionStringSettings Sec = ConfigurationManager.ConnectionStrings[ConnectionName];
            if (Sec == null)
                throw new ConfigurationErrorsException("在配置文件中找不到数据库节：" + ConnectionName);

            SelectDatabase(
                Sec.ProviderName,
                Sec.ConnectionString);
        }

        #endregion

        #region 构造函数， 直接传入连接字符串和提供商, 创建Connetion并打开

        /// <summary>
        /// 构造函数， 直接传入连接字符串和提供商, 创建Connetion并打开
        /// </summary>
        /// <param name="ProviderName"></param>
        /// <param name="ConnectionString"></param>
        public Helper(string ProviderName, string ConnectionString)
        {
            SelectDatabase(ProviderName, ConnectionString);
        }

        #endregion

        #region 传入一个Helper2实例, 复用现有的Helper2, 无需打开Connection

        /// <summary>
        /// 传入一个Helper2实例, 复用现有的Helper2, 无需打开Connection
        /// </summary>
        /// <param name="MyHelper"></param>
        public Helper(Helper MyHelper)
        {
            MyConnection = MyHelper.MyConnection;
            MyDatabase = MyHelper.MyDatabase;
        }

        #endregion

        #region 选择数据库实例，并打开连接

        /// <summary>
        /// 选择数据库实例，并打开连接
        /// </summary>
        /// <param name="ProviderName"></param>
        /// <param name="ConnectionString"></param>
        private void SelectDatabase(string ProviderName, string ConnectionString)
        {
            switch (ProviderName.ToLower())
            {
                case "mysql":
                    MyDatabase = new MySQL();
                    break;
                case "sqlserver":
                case "sql":
                    MyDatabase = new SqlServer();
                    break;
                default:
                    throw new Exception("数据库类型不支持!");
            }

            MyConnection = MyDatabase.CreateConnection(ConnectionString);

            MyConnection.Open();
        }

        #endregion

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// ExecuteDataSet
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="SQL"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int ExecuteDataSet(IDbCommand cmd, IDataAdapter adapter, string SQL, DataSet ds, params IDataParameter[] ParaList)
        {
            try
            {
                BuildCommand(cmd, SQL, ParaList);

                return adapter.Fill(ds);
            }
            catch (DatabaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
        }

        #endregion

        #region ExecuteNoneQuery

        /// <summary>
        /// 执行一个SQL语句， 无返回值
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int ExecuteNoneQuery(IDbCommand cmd, string SQL, params IDataParameter[] ParaList)
        {
            try
            {
                BuildCommand(cmd, SQL, ParaList);
                
                return cmd.ExecuteNonQuery();
            }
            catch (DatabaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }

        }

        #endregion

        #region ExecuteEntity<TValue>

        /// <summary>
        /// 返回一个 TValue类型的实体对象, 由 GetEntity 负责解析
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="ConnectionString"></param>
        /// <param name="Extracter"></param>
        /// <param name="SQL"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public TValue ExecuteEntity<TValue>(IDbCommand cmd, Converter<IDataRecord, TValue> Extracter, string SQL, params IDataParameter[] ParaList) where TValue : new()
        {
            try
            {
                TValue t = default(TValue);

                BuildCommand(cmd, SQL, ParaList);
                
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            if (reader.Read())
                            {
                                t = Extracter(reader);
                            }
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                

                Extracter = null;

                return t;
            }
            catch (DatabaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
        }

        #endregion

        #region ExecuteList<TValue>

        /// <summary>
        /// 返回一个 TValue类型的List对象, 由 GetEntity 负责解析
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="ConnectionString"></param>
        /// <param name="Extracter"></param>
        /// <param name="SQL"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public List<TValue> ExecuteList<TValue>(IDbCommand cmd, Converter<IDataRecord, TValue> Extracter, string SQL, params IDataParameter[] ParaList)
        {
            try
            {
                List<TValue> list = new List<TValue>();

                BuildCommand(cmd, SQL, ParaList);

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TValue t = Extracter(reader);

                        if (t != null)
                            list.Add(t);
                    }
                }

                return list;
            }
            catch (DatabaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
        }

        #endregion

        #region 私有方法, 构建 IDbCommand

        /// <summary>
        /// 内部函数, 从SQL语句中解析参数, 加入cmd.Parameter中
        /// </summary>
        /// <param name="values"></param>
        /// <param name="SQL"></param>
        /// <param name="cmd"></param>
        private void BuildCommand(IDbCommand cmd, string SQL, params object[] values)
        {
            //解决误传的问题
            if (values.Length > 0 && values[0] is IList)
            {
                IList list = values[0] as IList;

                object[] objList = new object[list.Count];

                for (int i = 0; i < objList.Length; i++)
                {
                    objList[i] = list[i];
                }

                BuildCommand(cmd, SQL, objList);
                return;
            }

            if (SQL.IndexOf("{0}") < 0)
            {
                cmd.CommandText = SQL;
                return;
            }

            StringBuilder sbSQL = new StringBuilder(SQL);

            for (int i = 0, j = values.Length; i < j; i++)
            {
                IDbDataParameter p = cmd.CreateParameter();
                p.ParameterName = MyDatabase.FormaterParameterName(i);
                p.Value = values[i];

                if (p.Value == null)
                    p.Value = System.DBNull.Value;
                cmd.Parameters.Add(p);

                sbSQL.Replace("{" + i + "}", p.ParameterName);
            }

            cmd.CommandText = sbSQL.ToString();

            sbSQL = null;
        }

        #endregion

    }
}
