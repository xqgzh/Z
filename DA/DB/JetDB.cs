using System;
using System.Data;
using System.Data.OleDb;

namespace Z.DA.DB
{
    /// <summary>
    /// AccessDB访问类
    /// </summary>
    class JetDB : IDatabase
    {
        public System.Data.IDbConnection CreateConnection(string ConnectionString)
        {
            return new OleDbConnection(ConnectionString);
        }

        public System.Data.IDbDataAdapter CreateAdapter(System.Data.IDbCommand cmd)
        {
            return new OleDbDataAdapter((OleDbCommand)cmd);
        }

        public string FormaterParameterName(object o)
        {
            return string.Format("@Z{0}", o);
        }

        public void ExtractStoreProcedureParameters(System.Data.IDbCommand cmd)
        {
            throw new NotImplementedException();
        }

        public string ParameterPrefix
        {
            get { return "@"; }
        }

        public void ExtractTableParameters(string TableName, System.Data.IDbDataAdapter adapter, out DatabaseCache InsertCache, out DatabaseCache DeleteCache, out DatabaseCache UpdateCache, out DatabaseCache IsExistCache, out System.Data.DataTable dt)
        {
            adapter.SelectCommand.CommandText = "select top 1 * from " + TableName;

            DataSet ds = new DataSet();

            dt = adapter.FillSchema(ds, SchemaType.Source)[0];

            dt.TableName = TableName;

            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter as OleDbDataAdapter);

            builder.ConflictOption = ConflictOption.OverwriteChanges;
            //builder.SetAllValues = false;
            OleDbCommand InsertCmd = builder.GetInsertCommand(true);
            builder.ConflictOption = ConflictOption.OverwriteChanges; InsertCache = new DatabaseCache(InsertCmd.CommandText, InsertCmd.Parameters);
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

            OleDbCommand UpdateCmd = builder.GetUpdateCommand(true);
            UpdateCache = new DatabaseCache(UpdateCmd.CommandText, UpdateCmd.Parameters);
            UpdateCache.CurrentTable = dt;

            OleDbCommand DeleteCmd = builder.GetDeleteCommand(true);
            DeleteCache = new DatabaseCache(DeleteCmd.CommandText, DeleteCmd.Parameters);
            DeleteCache.CurrentTable = dt;

            IsExistCache = new DatabaseCache(DeleteCmd.CommandText, DeleteCmd.Parameters);
            IsExistCache.CurrentTable = dt;
            IsExistCache.SQL = IsExistCache.SQL.Replace("DELETE FROM [" + TableName + "]", "Select count(1) from [" + TableName + "] with(nolock) ");
        }
    }
}
