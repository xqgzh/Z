using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Z.DA.DB;
using Z.Ex;

namespace Z.DA
{
    /// <summary>
    /// 数据访问基类， 通过定义此类的派生类可以实现对数据库的访问， 通过<c>TransScope</c>实现事务
    /// </summary>
    public class AbstractDA : IDisposable
    {
        /// <summary>
        /// 默认的最小时间
        /// </summary>
        public static DateTime MinDateTime = new DateTime(1900, 1, 1);

        #region 私有变量

        /// <summary>
        /// 数据库类型
        /// </summary>
        internal protected string ProviderName;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        internal protected string MyConnectionString;


        /// <summary>
        /// 数据库类型
        /// </summary>
        internal IDatabase MyDatabase;

        /// <summary>
        /// 连接超时时间
        /// </summary>
        internal protected int ExecTimeout = 3000;

        /// <summary>
        /// 是否保留连接， 在对象销毁时通过IDispose接口关闭
        /// </summary>
        internal protected bool KeepAlive = false;

        /// <summary>
        /// 数据库连接是否由当前实例产生
        /// </summary>
        internal bool IsConnectionOwner = false;

        /// <summary>
        /// 数据库连接
        /// </summary>
        internal IDbConnection MyConnection = null;

        /// <summary>
        /// 数据库事务
        /// </summary>
        internal IDbTransaction MyTransaction = null;

        #endregion

        #region 构造函数

        #region 构造函数， 通过App.Config中的ConnectionSection中获取连接字符串和提供商, 创建Connetion并打开

        /// <summary>
        /// 构造函数， 通过App.Config中的ConnectionSection中获取连接字符串和提供商, 创建Connetion并打开
        /// </summary>
        /// <param name="ConnectionName"></param>
        public AbstractDA(string ConnectionName)
        {
            ConnectionStringSettings Sec = ConfigurationManager.ConnectionStrings[ConnectionName];
            if (Sec == null)
                throw new ConfigurationErrorsException("在配置文件中找不到数据库节：" + ConnectionName);

            InitDatabase(Sec.ProviderName, Sec.ConnectionString);
        }

        #endregion

        #region 构造函数， 直接传入连接字符串和提供商, 创建Connetion并打开

        /// <summary>
        /// 构造函数， 直接传入连接字符串和提供商, 创建Connetion并打开
        /// </summary>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        public AbstractDA(string ProviderName, string ConnectionString)
        {
            InitDatabase(ProviderName, ConnectionString);
        }

        #endregion

        #region 选择数据库实例

        /// <summary>
        /// 选择数据库实例
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="ConnectionString"></param>
        private void InitDatabase(string providerName, string ConnectionString)
        {
            switch (providerName.ToLower())
            {
                case "mysql":
                    MyDatabase = new MySQL();
                    break;
                case "sqlserver":
                case "sql":
                case "system.data.sqlclient":
                    MyDatabase = new SqlServer();
                    break;
                case "jetdb":
                case "":
                    MyDatabase = new JetDB();
                    break;
                default:
                    throw new Exception("数据库类型不支持!");
            }

            MyConnectionString = ConnectionString;
            ProviderName = providerName;
        }

        #endregion

        internal AbstractDA() { }

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// 执行一个SQL语句, 返回DataSet
        /// </summary>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        internal protected virtual DataSet ExecuteDataSet(string SQL, params object[] values)
        {
            if ( values != null
                && SQL.IndexOf("{" + values.Length + "}") >= 0) throw new ArgumentException("参数不匹配");

            IDbConnection connection = CreateConnection();
            IDataAdapter adapter = null;

            try
            {
                DataSet ds = new DataSet();

                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    BuildCommandText(values, SQL, cmd);

                    adapter = MyDatabase.CreateAdapter(cmd);
                    adapter.Fill(ds);
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
            finally
            {
                if (adapter != null && adapter is IDisposable)
                    (adapter as IDisposable).Dispose();

                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }

        }

        #endregion

        #region ExecuteNoneQuery

        /// <summary>
        /// 执行一个SQL语句, 返回影响的行数量
        /// </summary>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        internal protected virtual int ExecuteNoneQuery(string SQL, params object[] values)
        {
            if (values != null
                && SQL.IndexOf("{" + values.Length + "}") >= 0) throw new ArgumentException("参数不匹配");

            int iRet = -1;

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    BuildCommandText(values, SQL, cmd);
                    iRet = cmd.ExecuteNonQuery();
                }
                return iRet;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #region ExecuteScalar<T>

        /// <summary>
        /// 执行一个SQL语句, 返回 T 类型的单值
        /// </summary>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        internal protected virtual T ExecuteScalar<T>(string SQL, params object[] values)
        {
            if ( values != null
                && SQL.IndexOf("{" + values.Length + "}") >= 0) throw new ArgumentException("参数与值的数量不匹配");

            IDbConnection connection = CreateConnection();

            try
            {
                object o = null;

                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    BuildCommandText(values, SQL, cmd);

                    o = cmd.ExecuteScalar();
                }

                if (o != null && o != DBNull.Value)
                    return (T)Convert.ChangeType(o, typeof(T));

                return default(T);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #region ExecuteEntity<T>

        /// <summary>
        /// 执行一个SQL语句, 返回一个 T类型的List对象, 由 Extracter 负责解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Extracter">T对象解析器</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        internal protected virtual T ExecuteEntity<T>(Converter<IDataRecord, T> Extracter, string SQL, params object[] values)
        {
            if ( values != null
                && SQL.IndexOf("{" + values.Length + "}") >= 0) throw new ArgumentException("参数与值的数量不匹配");

            if (Extracter == null) throw new DatabaseException("为{0}类型准备的解析器Extracter为空！", null, SQL);

            IDbConnection connection = CreateConnection();

            T t = default(T);

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;

                    BuildCommandText(values, SQL, cmd);

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            t = Extracter(reader);
                        }

                        reader.Close();
                    }
                }

                return t;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #region ExecuteList

        /// <summary>
        /// 执行一个SQL语句, 返回一个 T类型的List对象, 由 Extracter 负责解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Extracter"></param>
        /// <param name="SQL"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal protected virtual IEnumerable<T> ExecuteEnumerator<T>(Converter<IDataRecord, T> Extracter, string SQL, params object[] values)
        {
            if (values != null
                && SQL.IndexOf("{" + values.Length + "}") >= 0) throw new ArgumentException("参数与值的数量不匹配");

            if (Extracter == null)
                throw new DatabaseException("为{0}类型准备的解析器Extracter为空！", null, SQL);

            IDbConnection connection = CreateConnection();

            try
            {
                using (var cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;
                    BuildCommandText(values, SQL, cmd);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T t = Extracter(reader);

                            if (t != null)
                                yield return t;
                        }

                        reader.Close();
                    }
                }
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 执行一个SQL语句, 返回一个 T类型的List对象, 由 Extracter 负责解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Extracter"></param>
        /// <param name="SQL"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal protected virtual List<T> ExecuteList<T>(Converter<IDataRecord, T> Extracter, string SQL, params object[] values)
        {
            if (values!= null
                && SQL.IndexOf("{" + values.Length + "}") >= 0) throw new ArgumentException("参数与值的数量不匹配");

            if (Extracter == null)
                throw new DatabaseException("为{0}类型准备的解析器Extracter为空！", null, SQL);

            List<T> list = new List<T>();

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;

                    BuildCommandText(values, SQL, cmd);

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T t = Extracter(reader);

                            if (t != null)
                                list.Add(t);
                        }

                        reader.Close();
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 分页列表显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Extracter"></param>
        /// <param name="SQL"></param>
        /// <param name="TotalCount"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal protected virtual List<T> ExecuteListPaged<T>(Converter<IDataRecord, T> Extracter, string SQL, out int TotalCount, params object[] values)
        {
            if (values != null
                && SQL.IndexOf("{" + values.Length + "}") >= 0) throw new ArgumentException("参数与值的数量不匹配");

            if (Extracter == null)
                throw new DatabaseException("为{0}类型准备的解析器Extracter为空！", null, SQL);

            TotalCount = -999;

            List<T> list = new List<T>();

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;

                    BuildCommandText(values, SQL, cmd);

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (TotalCount == -999)
                            {
                                string fName = reader.GetName(reader.FieldCount - 1);

                                if (fName == "ZDA_TOTAL_COUNT")
                                {
                                    TotalCount = reader.GetInt32(reader.FieldCount - 1);
                                }
                            }
                            
                            T t = Extracter(reader);

                            if (t != null)
                                list.Add(t);
                        }

                        reader.Close();
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, SQL);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion

        #region ExecuteProcedure

        /// <summary>
        /// 执行一个存储过程， 返回 T 类型的单值
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入的值列表, 按照存储过程in类型参数的顺序赋值，跳过out参数</param>
        /// <returns>T 类型的单值</returns>
        internal protected virtual T ExecuteScalarProcedure<T>(string ProcedureName, params object[] values)
        {
            StoreProcedureInfo info = new StoreProcedureInfo();
            IDbDataParameter[] plist = GetProcedureParameters(ProcedureName, values);
            return ExecuteScalarProcedure<T>(ProcedureName, plist);           
        }

        /// <summary>
        /// 执行一个存储过程， 返回 T 类型的单值
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="ParameterList">传入的IDataParameter参数列表</param>
        /// <returns>T 类型的单值</returns>
        internal protected virtual T ExecuteScalarProcedure<T>(string ProcedureName, params IDbDataParameter[] ParameterList)
        {           
            IDbConnection connection = CreateConnection();

            try
            {
                object o = null;
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandText = ProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (IDataParameter p in ParameterList)
                        cmd.Parameters.Add(p);
                    o = cmd.ExecuteScalar();
                }

                if (o != null && o != DBNull.Value)
                    return (T)Convert.ChangeType(o, typeof(T));
                return default(T);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, ProcedureName);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }


        /// <summary>
        /// 执行一个存储过程， 不返回DataSet
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="ParameterList">传入的IDataParameter参数列表</param>
        /// <returns>存储过程返回值</returns>
        protected int ExecuteProcedure(string ProcedureName, params IDbDataParameter[] ParameterList)
        {
            int iRet = -1;

            IDbConnection connection = CreateConnection();

            try
            {

                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandText = ProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (IDataParameter p in ParameterList)
                        cmd.Parameters.Add(p);

                    cmd.ExecuteNonQuery();

                    foreach (IDataParameter p in cmd.Parameters)
                        if (p.Direction == ParameterDirection.ReturnValue)
                            iRet = Convert.ToInt32(p.Value);
                }

                return iRet;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, ProcedureName);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 执行一个存储过程， 不返回DataSet
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入的值列表, 按照存储过程in类型参数的顺序赋值，跳过out参数</param>
        /// <returns>存储过程返回对象</returns>
        internal protected StoreProcedureInfo ExecuteProcedure(string ProcedureName, params object[] values)
        {
            StoreProcedureInfo info = new StoreProcedureInfo();

            IDbDataParameter[] plist = GetProcedureParameters(ProcedureName, values);

            info.ReturnCode = ExecuteProcedure(ProcedureName, plist);

            foreach (IDataParameter p in plist)
            {
                if (p.Direction == ParameterDirection.InputOutput ||
                    p.Direction == ParameterDirection.Output)
                {
                    info.OutputParameters.Add(p.ParameterName.Replace(MyDatabase.ParameterPrefix,""), p.Value);
                }
            }

            return info;

        }


        #endregion

        #region ExecuteProcedureNoneQuery

        /// <summary>
        /// 执行存储过程， 只返回存储过程返回值
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入的参数列表</param>
        /// <returns>存储过程返回值</returns>        
        internal protected int ExecuteProcedureNonQuery(string ProcedureName, params object[] values)
        {
            int iRet = -1;

            IDataParameter[] ParameterList = GetProcedureParameters(ProcedureName, values);

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandText = ProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (IDataParameter p in ParameterList)
                        cmd.Parameters.Add(p);

                    cmd.ExecuteNonQuery();

                    foreach (IDataParameter p in ParameterList)
                    {
                        if (p.Direction == ParameterDirection.ReturnValue)
                            iRet = Convert.ToInt32(p.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, ProcedureName);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }

            }

            return iRet;
        }

        #endregion

        #region ExecuteProcedureDataSet

        /// <summary>
        /// 执行存储过程， 返回DataSet
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="ds">返回的DataSet</param>
        /// <param name="ParameterList">传入的IDataParameter参数列表</param>
        /// <returns>存储过程返回值</returns>        
        protected int ExecuteProcedureDataSet(string ProcedureName, out DataSet ds, params IDataParameter[] ParameterList)
        {
            ds = new DataSet();
            int iRet = -1;
            IDbDataAdapter adapter = null;

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandText = ProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (IDataParameter p in ParameterList)
                        cmd.Parameters.Add(p);

                    adapter = MyDatabase.CreateAdapter(cmd);

                    adapter.Fill(ds);

                    foreach (IDataParameter p in ParameterList)
                    {
                        if (p.Direction == ParameterDirection.ReturnValue)
                            iRet = Convert.ToInt32(p.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, ProcedureName);
            }
            finally
            {
                if (adapter != null && adapter is IDisposable)
                    (adapter as IDisposable).Dispose();

                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }

            }

            return iRet;
        }

        /// <summary>
        /// 执行存储过程， 返回DataSet
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入的值列表, 按照存储过程in类型参数的顺序赋值，跳过out参数</param>
        /// <returns>存储过程返回对象</returns>
        internal protected StoreProcedureInfo ExecuteProcedureDataSet(string ProcedureName, params object[] values)
        {
            StoreProcedureInfo info = new StoreProcedureInfo();

            IDataParameter[] plist = GetProcedureParameters(ProcedureName, values);

            info.ReturnCode = ExecuteProcedureDataSet(ProcedureName, out info.DataSet, plist);

            foreach (IDataParameter p in plist)
            {
                if (p.Direction == ParameterDirection.InputOutput ||
                    p.Direction == ParameterDirection.Output)
                {
                    info.OutputParameters.Add(p.ParameterName.Replace(MyDatabase.ParameterPrefix, ""), p.Value);
                }
            }

            return info;
        }

        #endregion

        #region ExecuteProcedureList<T>

        /// <summary>
        /// 执行存储过程， 返回类型为 T 的List列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入的值列表, 按照存储过程in类型参数的顺序赋值，跳过out参数</param>
        /// <returns></returns>
        public StoreProcedureList<T> ExecuteProcedureList<T>(string ProcedureName, params object[] values) where T: new()
        {
            return ExecuteProcedureList<T>(ProcedureName, EntityTools.ChangeType<T>, values);
        }
        
        /// <summary>
        /// 执行存储过程， 返回类型为 T 的List列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="Extracter">对象解析器</param>
        /// <param name="values">传入的值列表, 按照存储过程in类型参数的顺序赋值，跳过out参数</param>
        /// <returns></returns>
        public StoreProcedureList<T> ExecuteProcedureList<T>(string ProcedureName, Converter<IDataRecord, T> Extracter, params object[] values)
        {
            StoreProcedureList<T> info = new StoreProcedureList<T>();

            IDbDataParameter[] ParameterList = GetProcedureParameters(ProcedureName, values);

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandText = ProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    foreach (IDataParameter p in ParameterList)
                        cmd.Parameters.Add(p);

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T t = Extracter(reader);

                            if (t != null)
                                info.List.Add(t);
                        }
                    }


                    foreach (IDataParameter p in ParameterList)
                    {
                        if (p.Direction == ParameterDirection.ReturnValue)
                            info.ReturnCode = Convert.ToInt32(p.Value);

                        if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                        {
                            info.OutputParameters.Add(p.ParameterName.Substring(1), p.Value);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, ProcedureName);
            }
            finally
            {
                if (MyConnection != connection && MyTransaction == null)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }

            }

            return info;
        }

        #endregion

        #region GetProcedureParameters

        /// <summary>
        /// 解析存储过程参数
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="objs">如果传入值，则按照参数的Input类型，顺序赋值到Parameter中</param>
        /// <returns>IDataParameter数组</returns>
        protected IDbDataParameter[] GetProcedureParameters(string ProcedureName, params object[] objs)
        {
            string CacheKey = MyConnectionString + ":SP:" + ProcedureName;
            if (!DatabaseCache.Instance.ContainsKey(CacheKey))
            {
                lock (DatabaseCache.Instance)
                {
                    if (!DatabaseCache.Instance.ContainsKey(CacheKey))
                    {
                        IDbDataParameter[] ParaList = ExtractProcedureParameters(ProcedureName);

                        DatabaseCache MyCache = new DatabaseCache(ProcedureName, ParaList);

                        DatabaseCache.Instance.Add(CacheKey, MyCache);
                    }
                }
            }

            return CloneParameters(DatabaseCache.Instance[CacheKey].Parameters, objs);
        }

        #endregion

        #region GetDatabaseType

        /// <summary>
        /// ConnectionName
        /// </summary>
        /// <param name="ConnectionName"></param>
        /// <returns></returns>
        public static string GetDatabaseType(string ConnectionName)
        {
            ConnectionStringSettings Sec = ConfigurationManager.ConnectionStrings[ConnectionName];
            if (Sec == null)
                throw new ConfigurationErrorsException("在配置文件中找不到数据库节：" + ConnectionName);

            return Sec.ProviderName;
        }

        #endregion

        #region ImportData(导入大量数据到数据库)

        /// <summary>
        /// ImportData(导入大量数据到数据库)
        /// </summary>
        /// <param name="dt">导入的数据放入DataTable中, DataTable的Columns为默认列映射, DataTable.TableName为目标表名</param>
        /// <param name="TimeoutSecond">导入超时</param>
        /// <returns></returns>
        public bool ImportData(DataTable dt, int TimeoutSecond)
        {
            if (MyConnection is MySqlConnection)
            {
                throw new NotImplementedException("针对MySQL的批量数据导入尚未实现");
                //using (MySqlBulkLoader bulkLoader = new MySqlBulkLoader(MyConnection as MySqlConnection))
                //{
                //    foreach (DataColumn c in CurrentTable.Columns)
                //    {
                //        bulkLoader.Columns.Add(c.ColumnName);
                //        //bulkLoader.
                //    }
                //}
            }
            else if (MyConnection is SqlConnection)
            {
                using (SqlBulkCopy sqlBulk = new SqlBulkCopy(MyConnection as SqlConnection))
                {
                    sqlBulk.BatchSize = dt.Rows.Count;
                    sqlBulk.BulkCopyTimeout = TimeoutSecond;
                    sqlBulk.DestinationTableName = dt.TableName;

                    foreach (DataColumn c in dt.Columns)
                    {
                        sqlBulk.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                    }
                    
                    sqlBulk.WriteToServer(dt);
                }
            }

            return true;
        }

        #endregion

        #region TruncateTable 清空表

        /// <summary>
        /// 表名称
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool TruncateTable(string TableName)
        {
            ExecuteNoneQuery("truncate table " + TableName);

            return true;
        }

        #endregion

        #region 重载方法

        /// <summary>
        /// 返回一个T类型的对象, 使用内置的EntityTools.ChangeType解析器进行解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        protected virtual T ExecuteEntity<T>(string SQL, params object[] values) where T : new()
        {
            return ExecuteEntity<T>(EntityTools.ChangeType<T>, SQL, values);
        }

        /// <summary>
        /// 返回一个 T类型的List对象, 使用内置的EntityTools.ChangeType解析器进行解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        protected virtual List<T> ExecuteList<T>(string SQL, params object[] values) where T : new()
        {
            return ExecuteList<T>(EntityTools.ChangeType<T>, SQL, values);
        }

        /// <summary>
        /// 返回一个 T类型的List对象, 使用内置的EntityTools.ChangeType解析器进行解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> ExecuteEnumerator<T>(string SQL, params object[] values) where T : new()
        {
            return ExecuteEnumerator<T>(EntityTools.ChangeType<T>, SQL, values);
        }

        /// <summary>
        /// SQL 2005 专用的分页函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQL"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="OrderBy"></param>
        /// <param name="TotalRecords"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual List<T> ExecuteListPaged<T>(string SQL, int PageIndex, int PageSize, string OrderBy, out int TotalRecords, params object[] values) where T : new()
        {
            StringBuilder sb;
            CreatePagedSQL(SQL, PageIndex, PageSize, OrderBy, values, out sb);

            return ExecuteListPaged<T>(EntityTools.ChangeType<T>, sb.ToString(), out TotalRecords, values);
        }

        private static void CreatePagedSQL(string SQL, int PageIndex, int PageSize, string OrderBy, object[] values, out StringBuilder sb)
        {
            sb = new StringBuilder(SQL.Length + 100);

            int i = SQL.IndexOf(" from ", StringComparison.OrdinalIgnoreCase);

            if (i <= 0) throw new DatabaseException("分页SQL语法错误", null, SQL);

            sb.AppendFormat(
                "with temp_t as ({0}, ROW_NUMBER() OVER (ORDER BY {2}) AS ZDA_ROW_NUMBER {1}) SELECT *, (select count(1) from temp_t) as ZDA_TOTAL_COUNT FROM temp_t WHERE ZDA_ROW_NUMBER between {3} and {4}",
                SQL.Substring(0, i), SQL.Substring(i), OrderBy, 
                PageSize * (PageIndex-1)+ 1,
                PageSize * PageIndex);

        }

        /// <summary>
        /// 返回一个 T类型的List对象, 使用内置的EntityTools.ChangeType解析器进行解析
        /// </summary>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        protected virtual List<string> ExecuteList(string SQL, params object[] values)
        {
            return ExecuteList<string>(EntityTools.ChangeType, SQL, values);
        }

        #endregion

        #region 私有函数

        #region CreateConnection

        /// <summary>
        /// 虚函数实现
        /// </summary>
        /// <returns></returns>
        internal virtual IDbConnection CreateConnection()
        {
            //如果存在当前连接， 则使用当前连接， 如果不存在， 则创建
            if (MyTransaction != null)
                return MyTransaction.Connection;

            if (MyConnection != null)
            {
                if (MyConnection.State != ConnectionState.Open)
                    MyConnection.Open();

                IsConnectionOwner = true;
                return MyConnection;
            }
            else
            {
                IDbConnection conn = MyDatabase.CreateConnection(MyConnectionString);
                conn.Open();

                if (KeepAlive)
                {
                    MyConnection = conn;
                    IsConnectionOwner = true;
                }

                return conn;
            }
        }

        #endregion

        #region CreateCommand

        /// <summary>
        /// 创建一个IDbCommand， 如果设定了事务， 则给创建的IDbCommand设置事务
        /// </summary>
        /// <returns></returns>
        internal IDbCommand CreateCommand(IDbConnection connection, IDbTransaction transaction)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandTimeout = ExecTimeout;

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            return cmd;
        }

        #endregion

        #region ExtractParameters

        /// <summary>
        /// 解析存储过程的参数
        /// </summary>
        /// <param name="ProcedureName"></param>
        /// <returns></returns>
        internal IDbDataParameter[] ExtractProcedureParameters(string ProcedureName)
        {
            IDbDataParameter[] Parameters = null;

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = ProcedureName;

                    MyDatabase.ExtractStoreProcedureParameters(cmd);

                    Parameters = new IDbDataParameter[cmd.Parameters.Count];

                    cmd.Parameters.CopyTo(Parameters, 0);
                }
            }
            finally
            {
                if (MyConnection != connection)
                {
                    if (!KeepAlive)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }

            return Parameters;
        }

        #endregion

        #region BuildCommandText

        static Regex reg = new Regex(@"\{\d+\}");

        /// <summary>
        /// 从SQL语句中解析参数, 加入cmd.Parameter中
        /// </summary>
        /// <param name="values"></param>
        /// <param name="SQL"></param>
        /// <param name="cmd"></param>
        internal void BuildCommandText(IList values, string SQL, IDbCommand cmd)
        {
            if (values!= null && values.Count > 0 && values[0] is IList)
            {
                BuildCommandText(values[0] as IList, SQL, cmd);
                return;
            }

            string CacheKey = MyConnectionString + ":" + SQL;

            cmd.CommandType = CommandType.Text;

            //不带任何参数无需缓存
            if (reg.IsMatch(SQL) == false)
            {
                cmd.CommandText = SQL;
                return;
            }

            if (!DatabaseCache.Instance.ContainsKey(CacheKey))
            {
                lock (DatabaseCache.Instance)
                {
                    if (!DatabaseCache.Instance.ContainsKey(CacheKey))
                    {
                        StringBuilder sbSQL = new StringBuilder(SQL);

                        IDbDataParameter[] pList = new IDbDataParameter[values.Count];

                        for (int i = 0, j = values.Count; i < j; i++)
                        {
                            pList[i] = cmd.CreateParameter();
                            pList[i].ParameterName = MyDatabase.FormaterParameterName(i);
                            pList[i].Value = null;

                            sbSQL.Replace("{" + i + "}", MyDatabase.FormaterParameterName(i));
                        }

                        DatabaseCache.Instance.Add(
                            CacheKey,
                            new DatabaseCache(sbSQL.ToString(), pList));

                        sbSQL = null;
                    }
                }
            }

            DatabaseCache cache = DatabaseCache.Instance[CacheKey];

            cmd.CommandText = cache.SQL;

            AddParametersClone2Cmd(cmd, cache.Parameters, values);
        }

        #endregion

        #region AddParametersClone2Cmd

        /// <summary>
        /// 将已有的IDataParameter数组复制到IDbCommand中
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="Parameters"></param>
        /// <param name="values"></param>
        internal void AddParametersClone2Cmd(IDbCommand cmd, IList Parameters, IList values)
        {
            int valueIndex = 0;

            for (int i = 0; i < Parameters.Count; i++)
            {
                IDbDataParameter p = (Parameters[i] as ICloneable).Clone() as IDbDataParameter;

                if (values != null && values.Count > valueIndex)
                {
                    if (values[valueIndex] == null)
                        p.Value = DBNull.Value;
                    else
                    {
                        if (values[valueIndex] is DateTime)
                        {
                            DateTime dt = (DateTime)values[valueIndex];

                            if (DateTime.Compare(dt, DateTime.MinValue) == 0)
                                p.Value = AbstractDA.MinDateTime;
                            else
                                p.Value = dt;
                        }
                        else
                            p.Value = values[valueIndex];
                    }
                    valueIndex++;
                }
                else
                    p.Value = DBNull.Value;

                cmd.Parameters.Add(p);
            }
        }

        #endregion

        #region CloneParameters

        /// <summary>
        /// 复制传入的Parameter列表, 返回一个新的实例数组
        /// </summary>
        /// <param name="Parameters"></param>
        /// <param name="values">如果传入值列表values， 则按照顺序进行赋值</param>
        /// <returns></returns>
        internal IDbDataParameter[] CloneParameters(IList Parameters, IList values)
        {
            IDbDataParameter[] NewParameters = new IDbDataParameter[Parameters.Count];

            int j = 0;

            for (int i = 0; i < Parameters.Count; i++)
            {
                NewParameters[i] = (Parameters[i] as ICloneable).Clone() as IDbDataParameter;
                NewParameters[i].Value = DBNull.Value;

                if (NewParameters[i].Direction == ParameterDirection.Input && j < values.Count)
                {
                    if (values[j] != null)
                        NewParameters[i].Value = values[j];
                    j++;
                }
            }

            return NewParameters;
        }

        #endregion

        #endregion

        #region 销毁连接和事务

        /// <summary>
        /// 显式销毁
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 隐式销毁
        /// </summary>
        ~AbstractDA()
        {
            Dispose(false);
        }

        /// <summary>
        /// 销毁数据库连接
        /// </summary>
        /// <param name="IsDispose"></param>
        protected virtual void Dispose(bool IsDispose)
        {
            if (IsConnectionOwner)
            {
                if (MyConnection != null)
                {
                    if (MyConnection.State == ConnectionState.Open)
                    {
                        MyConnection.Close();
                        MyConnection.Dispose();
                    }
                }
            }
        }


        #endregion

        #region Internal 方法

        #region 直接数据库连接访问

        /// <summary>
        /// 直接数据库连接访问
        /// </summary>
        internal IDbConnection Connection
        {
            get
            {
                return MyConnection;
            }
            set
            {
                if (MyConnection != null && MyConnection.State == ConnectionState.Open)
                {
                    throw new DatabaseException("当前实例已有数据库连接被打开", null, string.Empty);
                }

                MyConnection = value;
                KeepAlive = true;
                IsConnectionOwner = false;
            }
        }

        #endregion

        #endregion

    }
}
