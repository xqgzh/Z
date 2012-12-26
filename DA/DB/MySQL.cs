using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Z.Util;

namespace Z.DA.DB
{
    class MySQL : IDatabase
    {
        public string ParameterPrefix
        {
            get { return "?"; }
        }

        public IDbConnection CreateConnection(string ConnectionString)
        {
            return new MySqlConnection(ConnectionString);
        }

        public IDbDataAdapter CreateAdapter(IDbCommand cmd)
        {
            return new MySqlDataAdapter(cmd as MySqlCommand);
        }

        public string FormaterParameterName(object o)
        {
            return string.Format("?Z{0}", o);
        }

        public void ExtractStoreProcedureParameters(IDbCommand cmd)
        {
            MySqlCommandBuilder.DeriveParameters(cmd as MySqlCommand);
        }

        public void ExtractTableParameters(string TableName, IDbDataAdapter adapter,
    out DatabaseCache InsertCache,
    out DatabaseCache DeleteCache,
    out DatabaseCache UpdateCache,
    out DatabaseCache IsExistCache,
    out DataTable dt
    )
        {
            MySqlDataAdapter adap = adapter as MySqlDataAdapter;

            adap.FillLoadOption = LoadOption.OverwriteChanges;
            adapter.SelectCommand.CommandText = "select * from " + TableName;

            DataSet ds = new DataSet();

            dt = adapter.FillSchema(ds, SchemaType.Source)[0];

            dt.TableName = TableName;

            InsertCache = new DatabaseCache(string.Empty, new IDataParameter[dt.Columns.Count]);
            DeleteCache = new DatabaseCache(string.Empty, new IDataParameter[dt.PrimaryKey.Length]);
            UpdateCache = new DatabaseCache(string.Empty, new IDataParameter[dt.Columns.Count]);
            IsExistCache = new DatabaseCache(string.Empty, new IDataParameter[dt.PrimaryKey.Length]);
            InsertCache.SQL = "insert into " + TableName + "(";
            DeleteCache.SQL = "delete from " + TableName + " where ";
            UpdateCache.SQL = "Update " + TableName + " set ";
            IsExistCache.SQL = "select count(1) from " + TableName + " where ";

            InsertCache.SQL += StringTools.JoinString(dt.Columns, string.Empty, string.Empty, ",", false);
            InsertCache.SQL += ") values(";
            InsertCache.SQL += StringTools.JoinString(dt.Columns, "?", string.Empty, ",", false);
            InsertCache.SQL += ");";

            Dictionary<string, DataColumn> PrimaryKeys = new Dictionary<string,DataColumn>();

            foreach (DataColumn c in dt.PrimaryKey)
            {
                if (c.AutoIncrement)
                    InsertCache.IsHaveAutoIncrement = true;

                PrimaryKeys.Add(c.ColumnName, c);
            }

            string s = " 1=1 ";

            foreach (DataColumn c in dt.Columns)
            {
                if (!PrimaryKeys.ContainsKey(c.ColumnName))
                {
                    UpdateCache.SQL += c.ColumnName + "=?" + c.ColumnName + ",";
                }
                else
                {
                    s += " And " + c.ColumnName + "=?" + c.ColumnName;
                }
            }

            if (UpdateCache.SQL.EndsWith(","))
                UpdateCache.SQL = UpdateCache.SQL.Remove(UpdateCache.SQL.Length - 1);

            UpdateCache.SQL += " Where ";

            DeleteCache.SQL += s;
            UpdateCache.SQL += s;
            IsExistCache.SQL += s;

            if (InsertCache.IsHaveAutoIncrement == true)
            {
                InsertCache.SQL += "select last_insert_id();";
            }

            InsertCache.Parameters = new IDataParameter[InsertCache.IsHaveAutoIncrement?dt.Columns.Count - 1:dt.Columns.Count];
            DeleteCache.Parameters = new IDataParameter[PrimaryKeys.Count];
            UpdateCache.Parameters = new IDataParameter[dt.Columns.Count];
            IsExistCache.Parameters = new IDataParameter[PrimaryKeys.Count];

            int InsertIndex = 0 , DeleteIndex = 0, UpdateIndex = 0;

            foreach (DataColumn c in dt.Columns)
            {
                IDataParameter p = adapter.SelectCommand.CreateParameter();
                p.ParameterName = "?" + c.ColumnName;
                p.SourceColumn = c.ColumnName;

                if (!c.AutoIncrement)
                {
                    InsertCache.Parameters[InsertIndex++] = p;
                }

                if (PrimaryKeys.ContainsKey(c.ColumnName))
                {
                    DeleteCache.Parameters[DeleteIndex] = p;
                    IsExistCache.Parameters[DeleteIndex] = p;
                    DeleteIndex++;
                }

                UpdateCache.Parameters[UpdateIndex] = p;
                UpdateIndex++;
            }

            InsertCache.CurrentTable = UpdateCache.CurrentTable = DeleteCache.CurrentTable = IsExistCache.CurrentTable = dt;
        }

    }
}
