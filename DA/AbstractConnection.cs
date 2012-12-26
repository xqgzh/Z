using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Z.DA.DB;
using System.Data.Common;
using Z.Ex;
using System.Collections;
using System.Threading;

namespace Z.DA
{
    /// <summary>
    /// 数据访问基类， 从<c>AbstractDA</c>派生， 通过配置实现一个长连接， 
    /// 当创建此类的实例时将自动打开一个数据库连接， 在对象生命周期内都使用此连接执行数据库操作， 
    /// 通过IDispose接口实现连接关闭及销毁
    /// </summary>
    public class AbstractConnection :  AbstractDA
    {
        #region 构造函数

        #region 构造函数， 通过App.Config中的ConnectionSection中获取连接字符串和提供商, 创建Connetion并打开

        /// <summary>
        /// 构造函数， 通过App.Config中的ConnectionSection中获取连接字符串和提供商, 创建Connetion并打开
        /// </summary>
        /// <param name="ConnectionName"></param>
        public AbstractConnection(string ConnectionName) : base(ConnectionName)
        {
            KeepAlive = true;
            CreateConnection();
            IsConnectionOwner = true;
        }

        #endregion

        #region 构造函数， 直接传入连接字符串和提供商, 创建Connetion并打开

        /// <summary>
        /// 构造函数， 直接传入连接字符串和提供商, 创建Connetion并打开
        /// </summary>
        /// <param name="ProviderName"></param>
        /// <param name="ConnectionString"></param>
        public AbstractConnection(string ProviderName, string ConnectionString) : base(ProviderName, ConnectionString)
        {
            KeepAlive = true;
            CreateConnection();
            IsConnectionOwner = true;
        }

        #endregion

        #region 构造函数， 传入一个AbstractConnection实例, 复用现有的AbstractConnection, 无需打开Connection

        /// <summary>
        /// 传入一个AbstractConnection实例, 复用现有的AbstractConnection, 无需打开Connection
        /// </summary>
        /// <param name="Instance">AbstractConnection实例</param>
        public AbstractConnection(AbstractConnection Instance)
        {
            KeepAlive = true;
            MyConnectionString = Instance.MyConnectionString;
            ProviderName = Instance.ProviderName;

            MyConnection = Instance.MyConnection;
            MyDatabase = Instance.MyDatabase;
            MyTransaction = Instance.MyTransaction;

            IsConnectionOwner = false;
        }

        #endregion

        #endregion

        #region 线程连接池(已弃用)

        ///// <summary>
        ///// 虚函数实现
        ///// </summary>
        ///// <returns></returns>
        //internal override IDbConnection CreateConnection()
        //{
        //    if (MyConnection == null)
        //    {
        //        //遍历当前线程连接池中是否已存在， 如果已有则直接赋值， 否则打开一个新的连接并且加入到当前线程连接池中
        //        if (DatabaseCache.ThreadConnectionPool.ContainsKey(MyConnectionString))
        //        {
        //            MyConnection = DatabaseCache.ThreadConnectionPool[MyConnectionString];
        //        }
        //        else
        //        {
        //            //不存在， 则创建
        //            MyConnection = MyDatabase.CreateConnection(MyConnectionString);
        //            MyConnection.Open();
        //            IsConnectionOwner = true;

        //            DatabaseCache.ThreadConnectionPool.Add(MyConnectionString, MyConnection);
        //        }
        //    }

        //    if (MyConnection.State == ConnectionState.Closed)
        //    {
        //        MyConnection.Open();
        //        IsConnectionOwner = true;
        //    }

        //    return MyConnection;
        //}

        #endregion
    }
}
