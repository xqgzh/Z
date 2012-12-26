using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Z.DA.DB
{
    class SqlServer : IDatabase
    {
        public string ParameterPrefix
        {
            get { return "@"; }
        }

        public IDbConnection CreateConnection(string ConnectionString)
        {
            return new SqlConnection(ConnectionString);
        }

        public IDbDataAdapter CreateAdapter(IDbCommand cmd)
        {
            return new SqlDataAdapter((SqlCommand)cmd);
        }

        public string FormaterParameterName(object o)
        {
            return string.Format("@Z{0}", o);
        }

        public void ExtractStoreProcedureParameters(IDbCommand cmd)
        {
            SqlCommandBuilder.DeriveParameters(cmd as SqlCommand);
        }

        public void ExtractTableParameters(string TableName, IDbDataAdapter adapter, 
            out DatabaseCache InsertCache, 
            out DatabaseCache DeleteCache,
            out DatabaseCache UpdateCache,
            out DatabaseCache IsExistCache,
            out DataTable dt
            )
        {
            adapter.SelectCommand.CommandText = "select top 1 * from " + TableName;

            DataSet ds = new DataSet();

            dt = adapter.FillSchema(ds, SchemaType.Source)[0];

            dt.TableName = TableName;

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter as SqlDataAdapter);

            builder.ConflictOption = ConflictOption.OverwriteChanges;
            //builder.SetAllValues = false;
            SqlCommand InsertCmd = builder.GetInsertCommand(true);
            builder.ConflictOption = ConflictOption.OverwriteChanges;




            InsertCache = new DatabaseCache(InsertCmd.CommandText, InsertCmd.Parameters);
            InsertCache.CurrentTable = dt;

            foreach (DataColumn c in dt.Columns)
            {
                if (c.AutoIncrement)
                {
                    InsertCache.IsHaveAutoIncrement = true;
                    InsertCache.SQL += ";Select @@IDENTITY;";
                    break;
                }
            }
            
            SqlCommand UpdateCmd = builder.GetUpdateCommand(true);
            UpdateCache = new DatabaseCache(UpdateCmd.CommandText, UpdateCmd.Parameters);
            UpdateCache.CurrentTable = dt;

            SqlCommand DeleteCmd = builder.GetDeleteCommand(true);
            DeleteCache = new DatabaseCache(DeleteCmd.CommandText, DeleteCmd.Parameters);
            DeleteCache.CurrentTable = dt;

            IsExistCache = new DatabaseCache(DeleteCmd.CommandText, DeleteCmd.Parameters);
            IsExistCache.CurrentTable = dt;
            IsExistCache.SQL = IsExistCache.SQL.Replace("DELETE FROM [" + TableName + "]", "Select count(1) from [" + TableName + "] with(nolock) ");
        }

    }
}
