using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Collections;
using Z.Util;

namespace Z.Entity
{
    /// <summary>
    /// 实体对象转换工具
    /// </summary>
    public static class EntityTools
    {
        #region IDataRecord -> 字符串

        /// <summary>
        /// IDataRecord -> 字符串
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static string ChangeType(IDataRecord dr)
        {
            if (dr == null || dr.FieldCount == 0 || dr.IsDBNull(0))
                return string.Empty;

            return Convert.ToString(dr[0]);
        }

        #endregion

        #region IDataRecord -> T泛型

        /// <summary>
        /// IDataRecord -> T泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ChangeType<T>(IDataRecord dr) where T : new()
        {
            T target = new T();
            Entity.EntityTools<T>.FromIDataRecord(dr, target);
            return target;
        }

        #endregion

        #region DataRow => DataParameterList

        /// <summary>
        /// DataRow => DataParameterList
        /// </summary>
        /// <param name="r"></param>
        /// <param name="ParameterList"></param>
        public static void DataRow2DbParameterList(DataRow r, IList ParameterList)
        {
            DataTable table = r.Table;

            foreach (IDbDataParameter p in ParameterList)
            {
                if (p.Direction != ParameterDirection.Input)
                    continue;

                string pName = p.ParameterName.Substring(1);

                if (!string.IsNullOrEmpty(p.SourceColumn))
                {
                    pName = p.SourceColumn;
                }

                if (pName.StartsWith("Original_"))
                    pName = pName.Replace("Original_", string.Empty);

                if (table.Columns.Contains(pName))
                {
                    if (r[pName] == null)
                        p.Value = DBNull.Value;
                    else
                        p.Value = r[pName];
                }
            }
        }

        #endregion

        #region ToDataTable

        /// <summary>
        /// Type => DataTable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(Type type)
        {
            DataTable dt = new DataTable(type.Name);

            MemberInfo[] memberList = type.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);

            foreach (var member in memberList)
            {
                if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                {
                    DataColumn c = new DataColumn();
                    c.ColumnName = member.Name;

                    if (member.MemberType == MemberTypes.Property)
                        c.DataType = (member as PropertyInfo).PropertyType;
                    else if (member.MemberType == MemberTypes.Field)
                        c.DataType = (member as FieldInfo).FieldType;

                    dt.Columns.Add(c);
                }

                dt.AcceptChanges();
            }

            return dt;
        }

        #endregion
    }
}
