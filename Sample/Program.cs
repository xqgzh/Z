using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Collections;
using System.Transactions;
using Z.DA;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using Z.Ex;
using Sample.C;
using System.Runtime.Remoting;
using Z.Util;
using Z.C;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(AppConfig.Instance.IsLog);
                AppConfigHandler.GetConfig<AppConfig>("AppConfig.xml");
                AppConfigHandler.EnableRemoteConfig(10008, "56yhgfrt");
                
                //AppConfigHandler.SaveConfig<AppConfig>("AppConfig.xml", true, AppConfig.Instance);


                string s = string.Empty;

                while (true)
                {
                    try
                    {
                        Console.WriteLine();
                        Console.WriteLine("如何使用 Z.DA 进行数据库访问");
                        Console.WriteLine("1. 基本数据库操作(从AbstractDA派生)");
                        Console.WriteLine("2. 基本数据库操作(基本表操作)");
                        Console.WriteLine("3. 基本数据库操作(执行存储过程)");
                        Console.WriteLine("4. 基本数据库操作(实现一个长连接， 避免每次执行重复创建和销毁Connection)");
                        Console.WriteLine("5. 基本数据库操作(实现事务)");
                        Console.WriteLine("6. 基本数据库操作(打印内部缓存SQL数据)");
                        Console.WriteLine("7. 基本数据库操作(返回泛型列表的存储过程)");
                        Console.WriteLine("8. 基本数据库操作(返回存储过程, 不返回任何值)");
                        Console.WriteLine("9. 基本数据库操作(返回一个字符串数组)");
                        Console.WriteLine("0, q, x. 退出");
                        Console.WriteLine("c. 清屏");
                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("选择样例:");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        s = Console.ReadLine().ToLower();

                        Console.ForegroundColor = ConsoleColor.Green;

                        switch (s)
                        {
                            case "1":
                                HowTO_1();
                                break;
                            case "2":
                                HowTO_2();
                                break;
                            case "3":
                                HowTO_3();
                                break;
                            case "4":
                                HowTO_4();
                                break;
                            case "5":
                                HowTO_5();
                                break;
                            case "6":
                                HowTO_6();
                                break;
                            case "7":
                                HowTO_7();
                                break;
                            case "8":
                                HowTO_8();
                                break;
                            case "9":
                                HowTO_9();
                                break;
                            case "0":
                            case "q":
                            case "x":
                                Console.ForegroundColor = ConsoleColor.Gray;
                                return;
                            case "c":
                                Console.Clear();
                                break;
                            default:
                                Console.Clear();
                                break;
                        }

                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        #region HowTO: 1. 基本数据库操作(从AbstractDA派生)

        /// <summary>
        /// HowTO: 1. 基本数据库操作(从AbstractDA派生)
        /// </summary>
        static void HowTO_1()
        {
            Console.WriteLine("HowTO: 1. 从AbstractDA派生的数据库操作");
            TestAbstractDA da = new TestAbstractDA();

            da.DeleteAll();

            int UserID = da.Insert("测试1", "测试1");

            TestUser u = da.SelectUserByUserId(UserID);

            Console.WriteLine("插入T_TEST:  ID={0},UserName={1}, UserPass={2}", u.ID, u.UserName, u.UserPass);

            da.Update(u.ID, "修改之后的用户名", "修改之后的用户名");

            u = da.SelectUserByUserId(UserID);

            Console.WriteLine("更新T_TEST:  ID={0},UserName={1}, UserPass={2}", u.ID, u.UserName, u.UserPass);
        }

        #endregion

        #region HowTO: 2. 基本数据库操作(基本表操作)

        /// <summary>
        /// HowTO: 2. 基本数据库操作(基本表操作)
        /// </summary>
        static void HowTO_2()
        {
            Console.WriteLine("HowTO: 2. 直接使用AbstractTable进行表操作");

            TestAbstractTable da = new TestAbstractTable();

            //清空表
            da.DeleteAll();

            //插入表
            int UserID = da.Insert("基本表操作", "基本表操作");

            //查询结果
            TestUser u = da.SelectUserByUserId(UserID);

            Console.WriteLine("Insert(values):  ID={0},UserName={1}, UserPass={2}", u.ID, u.UserName, u.UserPass);

            u.UserName = "Update<T>修改示例";
            u.UserPass = "Update<T>修改示例";


            //Update<T>示例, 直接通过对象进行更新
            da.Update<TestUser>(u);

            //查询DataSet
            DataSet ds = da.SelectDataSetByUserId(UserID);

            //Update<T>示例, 直接通过对象进行更新
            if (ds.Tables[0].Rows.Count > 0)
            {
                da.Update<DataRow>(ds.Tables[0].Rows[0]);
            }


            //查询结果
            u = da.SelectUserByUserId(UserID);

            Console.WriteLine("Update<T>结果:  ID={0},UserName={1}, UserPass={2}", u.ID, u.UserName, u.UserPass);

            //Update(values)
            da.Update(u.ID, "参数化修改示例", "参数化修改示例");

            u = da.SelectUserByUserId(UserID);
            Console.WriteLine("Update(values)结果:  ID={0},UserName={1}, UserPass={2}", u.ID, u.UserName, u.UserPass);

            //Delete(主键)
            da.Delete(u.ID);

            u.UserName = "泛型插入";
            u.UserPass = "泛型插入";

            //Insert<T>
            UserID = da.Insert<TestUser>(u);

            //查询
            u = da.SelectUserByUserId(UserID);
            Console.WriteLine("Inser<T>结果:  ID={0},UserName={1}, UserPass={2}", u.ID, u.UserName, u.UserPass);

        }

        #endregion

        #region HowTO: 3. 基本数据库操作(执行存储过程)

        /// <summary>
        /// HowTO: 3. 基本数据库操作(执行存储过程)
        /// </summary>
        static void HowTO_3()
        {
            Console.WriteLine("HowTO: 3. 调用存储过程, 返回DataSet");

            TestAbstractDA da = new TestAbstractDA();

            StoreProcedureInfo info = da.TestStoreProcedure("p_test", 2,DateTime.Now);

            Console.WriteLine("存储过程p_test的返回值:{0}", info.ReturnCode);

            foreach (string s in info.OutputParameters.Keys)
            {
                Console.WriteLine("\t输出参数:{0}:{1}", s, info.OutputParameters[s]);
            }

            if (info.DataSet != null && info.DataSet.Tables.Count > 0)
            {
                Console.WriteLine("\tDataSet:");
                foreach (DataColumn c in info.DataSet.Tables[0].Columns)
                {
                    Console.Write("\t{0}", c.ColumnName);
                }
                Console.WriteLine();

                foreach (DataRow r in info.DataSet.Tables[0].Rows)
                {
                    foreach (DataColumn c in info.DataSet.Tables[0].Columns)
                    {
                        Console.Write("\t{0}", r[c]);
                    }
                    Console.WriteLine();
                }
            }
        }

        #endregion

        #region HowTO: 4. 基本数据库操作(实现一个长连接， 避免每次执行重复创建和销毁Connection)

        /// <summary>
        /// HowTO: 4. 基本数据库操作(实现一个长连接， 避免每次执行重复创建和销毁Connection)
        /// </summary>
        static void HowTO_4()
        {
            Console.WriteLine("HowTO: 4. 长连接，避免每次执行重复创建和销毁Connection");

            //长连接必须尽量确保销毁
            using (TestAbstractConnection da = new TestAbstractConnection())
            {
                da.DeleteAll();

                int UserID = da.Insert("测试1", "测试1");

                TestUser u = da.SelectUserByUserId(UserID);

                Console.WriteLine("插入T_TEST:  ID={0},UserName={1}, UserPass={2}", u.ID, u.UserName, u.UserPass);

                da.Update(u.ID, "修改之后的用户名", "修改之后的用户名");

                u = da.SelectUserByUserId(UserID);

                Console.WriteLine("更新T_TEST:  ID={0},UserName={1}, UserPass={2}", u.ID, u.UserName, u.UserPass);                
            }
        }

        #endregion

        #region HowTO: 5. Transaction,  支持两个不同数据库连接之间的事务

        /// <summary>
        /// HowTO: 5. Transaction,  支持两个不同数据库连接之间的事务
        /// </summary>
        static void HowTO_5()
        {
            Console.WriteLine("HowTO: 5. Transaction,  支持两个不同数据库连接之间的事务");

            TestAbstractDA da1 = new TestAbstractDA();
            TestAbstractTable da2 = new TestAbstractTable();

            try
            {

                using (TransScope scope = new TransScope(da1, da2))
                {
                    da1.DeleteAll();
                    da2.DeleteAll();

                    int id1 = da1.Insert("1", "1");
                    int id2 = da2.Insert("2", "2");
                    da1.Insert("3", "3");
                    da2.Insert("4", "4");

                    Console.WriteLine("da1:");
                    foreach (TestUser u in da1.SelectAll())
                    {
                        Console.WriteLine("\tID={0},UserName={1},UserPass={2}", u.ID, u.UserName, u.UserPass);
                    }

                    Console.WriteLine("da2:");
                    foreach (TestUser u in da2.SelectAll())
                    {
                        Console.WriteLine("\tID={0},UserName={1},UserPass={2}", u.ID, u.UserName, u.UserPass);
                    }

                    da1.Update(id1, "修改", "修改");
                    da2.Update(id2, "修改", "修改");

                    Console.WriteLine("da1修改后:");
                    foreach (TestUser u in da1.SelectAll())
                    {
                        Console.WriteLine("\tID={0},UserName={1},UserPass={2}", u.ID, u.UserName, u.UserPass);
                    }

                    Console.WriteLine("da1修改后:");
                    foreach (TestUser u in da2.SelectAll())
                    {
                        Console.WriteLine("\tID={0},UserName={1},UserPass={2}", u.ID, u.UserName, u.UserPass);
                    }


                    scope.Commit();
                }
            }
            catch (DatabaseException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("SQL:" + ex.SQL);
            }
        }

        #endregion

        #region HowTO: 6. 基本数据库操作(打印内部缓存SQL数据)

        /// <summary>
        /// 6. 基本数据库操作(打印内部缓存SQL数据)
        /// </summary>
        static void HowTO_6()
        {
            Console.WriteLine("HowTO: 6. 基本数据库操作(打印内部缓存SQL数据)");

            string s = DatabaseHelper.ShowCache();

            Console.WriteLine(s);
        }

        #endregion

        #region HowTO: 7. 基本数据库操作(执行存储过程返回泛型列表)

        /// <summary>
        /// HowTO: 7. 基本数据库操作(执行存储过程返回泛型列表)
        /// </summary>
        static void HowTO_7()
        {
            Console.WriteLine("HowTO: 7. 执行存储过程返回泛型列表");

            TestAbstractDA da = new TestAbstractDA();

            StoreProcedureList<TestUser> info = da.TestStoreProcedureList("p_test", 2);

            Console.WriteLine("存储过程p_test的返回值:{0}", info.ReturnCode);

            foreach (string s in info.OutputParameters.Keys)
            {
                Console.WriteLine("\t输出参数:{0}:{1}", s, info.OutputParameters[s]);
            }

            foreach (TestUser u in info.List)
            {
                Console.WriteLine("\tID={0}, UserName={1}, UserPass={2}",
                    u.ID, u.UserName, u.UserPass);
            }
        }

        #endregion

        #region HowTO: 8. 基本数据库操作(执行存储过程不返回任何值)

        /// <summary>
        /// HowTO: 8. 基本数据库操作(执行存储过程不返回任何值)
        /// </summary>
        static void HowTO_8()
        {
            Console.WriteLine("HowTO: 8. 执行存储过程不返回任何值");

            using (TestAbstractConnection da = new TestAbstractConnection())
            {
                da.TestWarning();
            }
        }

        #endregion

        #region HowTO: 9. 基本数据库操作(返回一个字符串列表)

        /// <summary>
        /// HowTO: 9. 基本数据库操作(返回一个字符串列表)
        /// </summary>
        static void HowTO_9()
        {
            Console.WriteLine("HowTO: 9. 返回一个字符串列表");

            using (TestAbstractDA da = new TestAbstractDA())
            {
                List<string> list = da.TestListString();

                list.ForEach(delegate(string s) { Console.WriteLine(s); });


            }

        }

        #endregion


    }
}
