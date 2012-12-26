using System;
using System.Collections.Generic;
using System.Text;
using Z.DA;
using System.Data;

namespace Sample
{
    public class TestAbstractTable : AbstractTable
    {
        public TestAbstractTable() : base("TestDBMySQL", "T_TEST") { }

        public void DeleteAll()
        {
            ExecuteNoneQuery("TRUNCATE TABLE T_TEST;");
        }

        public TestUser SelectUserByUserId(int ID)
        {
            return ExecuteEntity<TestUser>(
                EntityTools.ChangeType<TestUser>,
                "select ID, UserName, UserPass from T_TEST where ID = {0}", ID);
        }

        public DataSet SelectDataSetByUserId(int ID)
        {
            return ExecuteDataSet("select ID, UserName, UserPass from T_TEST where ID = {0}", ID);
        }


        public List<TestUser> SelectAll()
        {
            return ExecuteList<TestUser>("select * from T_TEST");
        }
    }
}
