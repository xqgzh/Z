using System;
using System.Collections.Generic;
using System.Text;
using Z.DA;
using System.Data;

namespace Sample
{
    class TestAbstractConnection : AbstractConnection
    {
        public TestAbstractConnection() : base("TestDBMySql") { }

        public void DeleteAll()
        {
            ExecuteNoneQuery("delete from T_TEST");
        }

        public int Insert(string UserName, string UserPass)
        {
            string sql = "Insert into T_TEST(UserName, UserPass) values({0},{1}); Select @@IDENTITY;";

            return ExecuteScalar<int>(sql, UserName, UserPass);
        }

        public bool Update(int ID, string UserName, string UserPass)
        {
            string sql = "Update T_TEST Set UserName = {1}, UserPass = {2} where ID= {0};";

            return ExecuteNoneQuery(sql, ID, UserName, UserPass) > 0;
        }

        public TestUser SelectUserByUserId(int ID)
        {
            return ExecuteEntity<TestUser>(
                EntityTools.ChangeType<TestUser>,
                "select ID, UserName, UserPass from T_TEST where ID = {0}", ID);
        }

        public List<TestUser> SelectAll()
        {
            return ExecuteList<TestUser>("select * from T_TEST");
        }

        /// <summary>
        /// 测试存储过程调用
        /// </summary>
        public StoreProcedureInfo TestStoreProcedure(string ProcedureName, params object[] values)
        {
            StoreProcedureInfo info = ExecuteProcedureDataSet(ProcedureName, values);

            return info;
        }

        /// <summary>
        /// 测试存储过程不返回任何值
        /// </summary>
        /// <param name="ProcedureName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int TestStoreProcedureNonQuery(string ProcedureName, params object[] values)
        {
            return ExecuteProcedureNonQuery(ProcedureName, values);
        }

        public void TestWarning()
        {
            

            ExecuteNoneQuery("DROP TABLE IF EXISTS test");
            ExecuteNoneQuery("CREATE TABLE test (id INT, dt DATETIME)");
            ExecuteNoneQuery("INSERT INTO test VALUES (1, NOW())");
            ExecuteNoneQuery("INSERT INTO test VALUES (2, NOW())");
            ExecuteNoneQuery("INSERT INTO test VALUES (3, NOW())");

            DataSet ds = ExecuteDataSet("SELECT * FROM test WHERE dt = '" + DateTime.Now + "'");
            DataSet ds2 = ExecuteDataSet("SELECT * FROM test WHERE dt = '" + DateTime.Now + "'");
        }
    }
}
