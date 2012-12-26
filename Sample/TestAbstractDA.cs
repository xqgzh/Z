using System;
using System.Collections.Generic;
using System.Text;
using Z.DA;
using System.Data;

namespace Sample
{
    public class TestAbstractDA : AbstractDA
    {
        public TestAbstractDA() : base("TestDB") { }

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
        /// 测试存储过程调用
        /// </summary>
        public StoreProcedureList<TestUser> TestStoreProcedureList(string ProcedureName, params object[] values)
        {
            StoreProcedureList<TestUser> info = ExecuteProcedureList<TestUser>(
                ProcedureName, 
                EntityTools.ChangeType<TestUser>,
                values);

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


        /// <summary>
        /// 测试返回一个字符串列表
        /// </summary>
        /// <param name="ProcedureName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public List<string> TestListString()
        {
            return ExecuteList<string>(EntityTools.ChangeType, "select UserName from T_TEST");
        }
    }
}
