using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Z.Ex;

namespace Z.DA
{
    /// <summary>
    /// 事务支持类
    /// </summary>
    public class TransScope : IDisposable
    {
        #region 内部属性

        /// <summary>
        /// AbstractDA列表，用于确定哪些DA类要参与事务执行
        /// </summary>
        List<AbstractDA> AbstractDAList = new List<AbstractDA>();

        /// <summary>
        /// 事务列表， 用于协调不同数据库连接的事务
        /// </summary>
        Dictionary<string, IDbTransaction> TransactionList = new Dictionary<string, IDbTransaction>();

        /// <summary>
        /// 需要关闭的数据库连接列表
        /// </summary>
        List<IDbConnection> NeedCloseConnection = new List<IDbConnection>();

        private bool IsDispose = false;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数， 传入所有需要执行事务的AbstractDA类, 默认事务隔离级别是IsolationLevel.RepeatableRead
        /// </summary>
        /// <param name="list"></param>
        public TransScope(params AbstractDA[] list) : this(IsolationLevel.RepeatableRead, list) {}

        /// <summary>
        /// 构造函数， 传入所有需要执行事务的AbstractDA类
        /// </summary>
        /// <param name="TransLevel">事务隔离级别</param>
        /// <param name="list"></param>
        public TransScope(IsolationLevel TransLevel, params AbstractDA[] list)
        {
            //寻找当前可用的连接， 加入到Transaction中
            foreach (AbstractDA da in list)
            {
                if (!TransactionList.ContainsKey(da.MyConnectionString))
                {
                    if (da.MyConnection != null && da.MyConnection.State == ConnectionState.Open)
                    {
                        IDbTransaction trans = da.MyConnection.BeginTransaction(TransLevel);

                        TransactionList.Add(da.MyConnectionString, trans);
                    }
                }
            }

            foreach (AbstractDA da in list)
            {
                //当前连接字符串还没有创建连接
                if (!TransactionList.ContainsKey(da.MyConnectionString))
                {
                    IDbConnection connection = da.MyDatabase.CreateConnection(da.MyConnectionString);
                    connection.Open();

                    NeedCloseConnection.Add(connection);
                    TransactionList.Add(
                        da.MyConnectionString,
                        connection.BeginTransaction(TransLevel));
                }

                da.MyTransaction = TransactionList[da.MyConnectionString];

                AbstractDAList.Add(da);
            }
        }

        #endregion

        #region 提交事务

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (!IsDispose)
            {
                try
                {
                    foreach (IDbTransaction trans in TransactionList.Values)
                    {
                        trans.Commit();
                        trans.Dispose();
                    }

                    foreach (IDbConnection conn in NeedCloseConnection)
                    {
                        conn.Close();
                        conn.Dispose();
                    }

                    foreach (AbstractDA da in AbstractDAList)
                    {
                        da.MyTransaction = null;
                    }

                    AbstractDAList.Clear();
                    TransactionList.Clear();
                }
                finally
                {
                    IsDispose = true;
                }
            }
            else
            {

            }
        }

        #endregion

        #region IDisposable接口

        /// <summary>
        /// 主动销毁事务
        /// </summary>
        public void Dispose()
        {
            if (IsDispose == false)
            {
                //默认回滚
                foreach (IDbTransaction trans in TransactionList.Values)
                {
                    trans.Rollback();
                    trans.Dispose();
                }

                foreach (IDbConnection conn in NeedCloseConnection)
                {
                    conn.Close();
                    conn.Dispose();
                }

                foreach (AbstractDA da in AbstractDAList)
                {
                    da.MyTransaction = null;
                }

                IsDispose = true;
            }
        }

        #endregion
    }
}
