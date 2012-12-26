using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Z.DA.DB
{
    interface IDatabase
    {
        IDbConnection CreateConnection(string ConnectionString);
        IDbDataAdapter CreateAdapter(IDbCommand cmd);
        string FormaterParameterName(object o);

        void ExtractStoreProcedureParameters(IDbCommand cmd);

        string ParameterPrefix { get;}

        void ExtractTableParameters(string TableName, IDbDataAdapter adapter,
            out DatabaseCache InsertCache,
            out DatabaseCache DeleteCache,
            out DatabaseCache UpdateCache,
            out DatabaseCache IsExistCache,
            out DataTable dt
            );
    }
}
