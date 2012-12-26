using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Z.Ex;
using System.Collections;

namespace Z.DA
{
    /// <summary>
    /// 抽象表访问类, 自动生成Insert, Delete, Update, IsExist代码, 提供针对表的基本操作
    /// </summary>
    public class AbstractTable : AbstractDA
    {
        #region 私有变量

        private DatabaseCache InsertCache;
        private DatabaseCache UpdateCache;
        private DatabaseCache DeleteCache;
        private DatabaseCache IsExistCache;

        /// <summary>
        /// 表名称
        /// </summary>
        protected string TableName;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ProviderName">mysql或者sqlserver， 不区分大小写</param>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="tableName">数据库表名称</param>
        public AbstractTable(string ProviderName, string ConnectionString, string tableName)
            : base(ProviderName, ConnectionString)
        {
            InitTable(tableName);
        }

        /// <summary>
        /// 简单表访问类
        /// </summary>
        /// <param name="ConnectionName">在app.config中定义的数据库连接名称</param>
        /// <param name="tableName">表名称</param>
        public AbstractTable(string ConnectionName, string tableName)
            : base(ConnectionName)
        {
            InitTable(tableName);
        }

        private void InitTable(string tableName)
        {
            TableName = tableName;
            string CacheKey = MyConnectionString + ":TBL:" + TableName;

            if (!DatabaseCache.Instance.ContainsKey(CacheKey + ":Insert"))
            {
                lock (DatabaseCache.Instance)
                {
                    if (!DatabaseCache.Instance.ContainsKey(CacheKey + ":Insert"))
                    {
                        using (IDbConnection conn = CreateConnection())
                        {
                            using (IDbCommand command = CreateCommand(conn, MyTransaction))
                            {
                                IDbDataAdapter adapter = MyDatabase.CreateAdapter(command);

                                try
                                {
                                    DatabaseCache insertCache, updateCache, deleteCache, isExistCache;
                                    DataTable TableSchema;

                                    MyDatabase.ExtractTableParameters(TableName,
                                        adapter,
                                        out insertCache,
                                        out deleteCache,
                                        out updateCache,
                                        out isExistCache,
                                        out TableSchema);

                                    DatabaseCache.Instance.Add(CacheKey + ":Insert", insertCache);
                                    DatabaseCache.Instance.Add(CacheKey + ":Update", updateCache);
                                    DatabaseCache.Instance.Add(CacheKey + ":Delete", deleteCache);
                                    DatabaseCache.Instance.Add(CacheKey + ":IsExist", isExistCache);
                                }
                                finally
                                {
                                    if (adapter is IDisposable)
                                        (adapter as IDisposable).Dispose();
                                }

                            }
                        }
                    }
                }
            }

            InsertCache = DatabaseCache.Instance[CacheKey + ":Insert"];
            UpdateCache = DatabaseCache.Instance[CacheKey + ":Update"];
            DeleteCache = DatabaseCache.Instance[CacheKey + ":Delete"];
            IsExistCache = DatabaseCache.Instance[CacheKey + ":IsExist"];
        }

        #endregion

        #region Insert

        /// <summary>
        /// 插入表，如果表中包含自增量主键，则返回主键编号， 否则返回1
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public int Insert(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("Insert操作不能传入空值");

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = InsertCache.SQL;

                    AddParametersClone2Cmd(cmd, InsertCache.Parameters, values);

                    if (InsertCache.IsHaveAutoIncrement)
                        return Convert.ToInt32(cmd.ExecuteScalar());

                    return cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, DeleteCache.SQL);
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

        #region Insert<T>

        /// <summary>
        /// 插入类型为T的对象到表中，如果表中包含自增量主键，则返回主键编号， 否则返回1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Insert<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException("Insert操作不能传入空值");

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = InsertCache.SQL;

                    AddParametersClone2Cmd(cmd, InsertCache.Parameters, null);

                    EntityTools.ChangeType(value, cmd.Parameters);

                    int iRet = 0;

                    if (InsertCache.IsHaveAutoIncrement)
                        iRet = Convert.ToInt32(cmd.ExecuteScalar());
                    else
                        iRet = cmd.ExecuteNonQuery();

                    EntityTools.ChangeType(cmd.Parameters, value);

                    return iRet;
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, DeleteCache.SQL);
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

        #region Delete

        /// <summary>
        /// 从表中根据主键删除一条记录， values顺序必须和主键顺序一致
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool Delete(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("Delete操作不能传入空值");

            if (values.Length != DeleteCache.CurrentTable.PrimaryKey.Length)
                throw new ArgumentNullException("Delete传入参数的主键数量不正确,无法正确删除");

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = DeleteCache.SQL;

                    AddParametersClone2Cmd(cmd, DeleteCache.Parameters, values);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, DeleteCache.SQL);
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

        #region Update

        /// <summary>
        /// 从表中根据主键更新一条记录， values顺序必须和表字段的排列顺序一致
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool Update(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("Update操作不能传入空值");

            if (values.Length < DeleteCache.CurrentTable.PrimaryKey.Length)
                throw new ArgumentNullException("Update传入参数的主键数量不正确,无法正确更新");


            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = UpdateCache.SQL;

                    AddParametersClone2Cmd(cmd, UpdateCache.Parameters, null);

                    DataRow r = UpdateCache.CurrentTable.NewRow();

                    int j = UpdateCache.CurrentTable.Columns.Count;
                    if(j > values.Length)
                        j = values.Length;

                    for (int i = 0; i < j; i++)
                    {
                        r[i] = values[i];
                    }

                    foreach (IDbDataParameter p in cmd.Parameters)
                    {
                        if (UpdateCache.CurrentTable.Columns.Contains(p.SourceColumn))
                        {
                            p.Value = r[p.SourceColumn];
                        }
                    }

                    return cmd.ExecuteNonQuery() > 0;

                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, UpdateCache.SQL);
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

        #region Update<T>

        /// <summary>
        /// 从表中根据主键更新一条记录， 从已知对象中转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Update<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException("Update操作不能传入空值");

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = UpdateCache.SQL;

                    AddParametersClone2Cmd(cmd, UpdateCache.Parameters, null);

                    EntityTools.ChangeType(value, cmd.Parameters);

                    return cmd.ExecuteNonQuery() > 0;

                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, UpdateCache.SQL);
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

        #region IsExist

        /// <summary>
        /// 从表中根据主键检查一条记录是否存在， values顺序必须和主键顺序一致
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool IsExist(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("IsExist操作不能传入空值");

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = IsExistCache.SQL;

                    AddParametersClone2Cmd(cmd, IsExistCache.Parameters, values);

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, IsExistCache.SQL);
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

        #region IsExist<T>

        /// <summary>
        /// 从表中根据主键检查一条记录是否存在， values顺序必须和主键顺序一致
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsExist<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException("IsExist操作不能传入空值");

            IDbConnection connection = CreateConnection();

            try
            {
                using (IDbCommand cmd = CreateCommand(connection, MyTransaction))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = IsExistCache.SQL;

                    AddParametersClone2Cmd(cmd, IsExistCache.Parameters, null);

                    EntityTools.ChangeType(value, cmd.Parameters);

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex, IsExistCache.SQL);
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
        
        #region 当前的表结构，不包含任何数据

        /// <summary>
        /// 当前的表结构，不包含任何数据
        /// </summary>
        protected DataTable CurrentTable
        {
            get
            {
                return InsertCache.CurrentTable.Clone();
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// 保存实体, 如果实体已存在, 则更新, 否则插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns>1-更新, 2-插入</returns>
        public virtual int Save<T>(T t)
        {
            if (IsExist<T>(t))
            {
                Update<T>(t);
                return 1;

            }


            return Insert<T>(t);
        }

        #endregion

        #region TableSysn<T>

        /// <summary>
        /// 根据类型为T的对象检查数据库中是否存在, 如果存在则更新, 不存在则插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">类型为T的对象数组</param>
        /// <returns></returns>
        public int TableSync<T>(IEnumerable list)
        {
            int iCount = 0;

            if (InsertCache.IsHaveAutoIncrement)
                throw new DatabaseException("无法同步带自增量的表", null, string.Empty);

            foreach (T t in list)
            {
                if (IsExist<T>(t)) 
                    Update<T>(t);
                else 
                    Insert<T>(t);

                iCount++;
            }

            return iCount;
        }

        #endregion

        
    }
}
