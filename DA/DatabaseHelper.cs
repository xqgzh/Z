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
    /// <summary>
    /// 数据库访问工具， 与SQLHelper的访问方法一致
    /// </summary>
    public static class DatabaseHelper
    {
        #region ExecuteDataSet

        /// <summary>
        /// 执行SQL语句, 返回一个DataSet对象
        /// </summary>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(string ProviderName, string ConnectionString, string SQL, params object[] values)
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteDataSet(SQL, values);
        }


        /// <summary>
        /// 执行SQL语句, 返回一个DataSet对象
        /// </summary>
        /// <param name="config">数据库连接配置项</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(DatabaseConfiguration config, string SQL, params object[] values)
        {
            return new AbstractDA(config.ProviderName, config.ConnectionString).ExecuteDataSet(SQL, values);
        }

        #endregion

        #region ExecuteNoneQuery

        /// <summary>
        /// 重载:执行一个SQL语句, 返回影响的行数量
        /// </summary>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static int ExecuteNoneQuery(string ProviderName, string ConnectionString, string SQL, params object[] values)
        {
                return new AbstractDA(ProviderName, ConnectionString).ExecuteNoneQuery(SQL, values);
        }


        /// <summary>
        /// 重载:执行一个SQL语句, 返回影响的行数量
        /// </summary>
        /// <param name="config">数据库连接配置项</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static int ExecuteNoneQuery(DatabaseConfiguration config, string SQL, params object[] values)
        {
            return new AbstractDA(config.ProviderName, config.ConnectionString).ExecuteNoneQuery(SQL, values);
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 返回 T 类型的单值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(string ProviderName, string ConnectionString, string SQL, params object[] values)
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteScalar<T>(SQL, values);
        }

        #endregion

        #region ExecuteEntity

        /// <summary>
        /// ExecuteEntity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="Extracter">对象解析器</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static T ExecuteEntity<T>(string ProviderName, string ConnectionString, Converter<IDataRecord, T> Extracter, string SQL, params object[] values)
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteEntity<T>(Extracter, SQL, values);
        }

        #endregion

        #region ExecuteList

        /// <summary>
        /// 返回一个T类型的List对象, 由 Extracter 负责解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="Extracter">T对象解析器</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static List<T> ExecuteList<T>(string ProviderName, string ConnectionString, Converter<IDataRecord, T> Extracter, string SQL, params object[] values)
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteList<T>(Extracter, SQL, values);
        }

        #endregion

        #region ExecuteProcedureNonQuery

        /// <summary>
        /// 执行存储过程， 只返回存储过程返回值
        /// </summary>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入值列表</param>
        /// <returns>存储过程返回值</returns>
        public static int ExecuteProcedureNonQuery(string ProviderName, string ConnectionString, string ProcedureName, params object[] values)
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteProcedureNonQuery(ProcedureName, values);
        }

        #endregion

        #region ExecuteProcedure

        /// <summary>
        /// 执行存储过程， 不返回DataSet
        /// </summary>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static StoreProcedureInfo ExecuteProcedure(string ProviderName, string ConnectionString, string ProcedureName, params object[] values)
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteProcedure(ProcedureName, values);
        }


        #endregion

        #region ExecuteProcedureDataSet

        /// <summary>
        /// 执行存储过程， 不返回DataSet
        /// </summary>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static StoreProcedureInfo ExecuteProcedureDataSet(string ProviderName, string ConnectionString, string ProcedureName, params object[] values)
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteProcedureDataSet(ProcedureName, values);
        }

        #endregion

        #region ExecuteProcedureList<T>

        /// <summary>
        /// 执行存储过程， 返回类型为 T 的List列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="values">传入的值列表, 按照存储过程in类型参数的顺序赋值，跳过out参数</param>
        /// <returns></returns>
        public static StoreProcedureList<T> ExecuteProcedureList<T>(string ProviderName, string ConnectionString, string ProcedureName, params object[] values) where T : new()
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteProcedureList<T>(ProcedureName, EntityTools.ChangeType<T>, values);
        }

        /// <summary>
        /// 执行存储过程， 返回类型为 T 的List列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="Extracter">对象解析器</param>
        /// <param name="values">传入的值列表, 按照存储过程in类型参数的顺序赋值，跳过out参数</param>
        /// <returns></returns>
        public static StoreProcedureList<T> ExecuteProcedureList<T>(string ProviderName, string ConnectionString, string ProcedureName, Converter<IDataRecord, T> Extracter, params object[] values)
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteProcedureList<T>(ProcedureName, Extracter, values);
        }

        #endregion


        #region 重载方法

        /// <summary>
        /// 返回一个T类型的对象, 使用内置的EntityTools.ChangeType解析器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static T ExecuteEntity<T>(string ProviderName, string ConnectionString, string SQL, params object[] values) where T : new()
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteEntity<T>(EntityTools.ChangeType<T>, SQL, values);
        }

        /// <summary>
        /// 返回一个T类型的List对象, 使用内置的EntityTools.ChangeType解析器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="SQL">SQL参数化语句, 与string.format格式一致</param>
        /// <param name="values">传入值列表</param>
        /// <returns></returns>
        public static List<T> ExecuteList<T>(string ProviderName, string ConnectionString, string SQL, params object[] values) where T : new()
        {
            return new AbstractDA(ProviderName, ConnectionString).ExecuteList<T>(EntityTools.ChangeType<T>, SQL, values);
        }

        #endregion

        #region 显示缓存中的SQL语句

        /// <summary>
        /// 显示缓存中的SQL语句
        /// </summary>
        /// <returns></returns>
        public static string ShowCache()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string k in DatabaseCache.Instance.Keys)
            {
                sb.AppendFormat("[{0}]:{1}", k, DatabaseCache.Instance[k].SQL);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        #endregion

    }
}
