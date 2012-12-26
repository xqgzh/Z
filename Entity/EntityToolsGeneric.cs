using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data;
using System.Web;
using System.Collections;
using Z.DA;


namespace Z.Entity
{
    /// <summary>
    /// 实体访问工具, 对指定实体类型T实现按照属性名称进行存取的接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityTools<T>
    {
        #region 方法

        ///// <summary>
        ///// 获取指定属性的值 (string name, bool IgnoreCase)
        ///// </summary>
        //public static readonly Func<T, string, bool, object> GetValue;

        private static readonly Func<T, int, object> GetValue_NEW;

        /// <summary>
        /// 设置指定属性的值 (string name, bool IgnoreCase, object value)
        /// </summary>
        public static readonly Func<T, string, bool, object,bool> SetValue;

        /// <summary>
        /// 获取指定属性的值, 结果转换为字符串 (string name, bool IgnoreCase)
        /// </summary>
        public static readonly Func<T, string, bool, string> GetValueString;

        /// <summary>
        /// 设置指定属性的值, 输入为字符串 (string name, bool IgnoreCase, string value)
        /// </summary>
        public static readonly Func<T, string, bool, string, bool> SetValueString;

        #endregion

        #region 属性

        /// <summary>
        /// 将T类型的DataTable结构
        /// </summary>
        public static DataTable Table;

        /// <summary>
        /// 属性数量
        /// </summary>
        public static int PropertyCount;

        /// <summary>
        /// 字段数量
        /// </summary>
        public static int FieldCount;

        /// <summary>
        /// 属性或者字段的总数量
        /// </summary>
        public static int FieldOrPropertyCount;

        /// <summary>
        /// 属性或者字段的名称列表
        /// </summary>
        public static readonly string[] FeildOrPropertys;

        /// <summary>
        /// 属性名称列表
        /// </summary>
        public static readonly string[] Propertys;

        /// <summary>
        /// 字段名称列表
        /// </summary>
        public static readonly string[] Fields;

        #endregion

        #region 静态构造函数

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static EntityTools()
        {
            EntityToolsInternal.CalculateFieldPropertys<T>(ref FieldCount, ref PropertyCount, ref Fields, ref Propertys, ref FeildOrPropertys);

            FieldOrPropertyCount = PropertyCount + FieldCount;

            Table = EntityTools.ToDataTable(typeof(T));
            //GetValue = EntityToolsInternal.Func_GetValue<T, object>();
            GetValue_NEW = EntityToolsInternal.Func_GetValue2<T, object>();
            SetValue = EntityToolsInternal.Func_SetValue<T, object>();
            GetValueString = EntityToolsInternal.Func_GetValue<T, string>();
            SetValueString = EntityToolsInternal.Func_SetValue<T, string>();
        }

        public static object GetValue(T obj, string name, bool IgnoreCase)
        {
            if (obj is IDataObject)
                return (obj as IDataObject).GetValue(name, IgnoreCase);

            int HashCode = IgnoreCase ? 
                StringLowerTable.GetLowerHashCode(name.ToLower()) :
                StringLowerTable.GetLowerHashCode(name);

            return GetValue_NEW(obj, HashCode);
        }

        #endregion

        #region From

        /// <summary>
        /// 从IDataRecord中解析对象属性
        /// </summary>
        /// <param name="record"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FromIDataRecord(IDataRecord record, T target)
        {
            int totalCount = 0;

            for (int i = 0; i < record.FieldCount; i++)
            {
                string name = record.GetName(i);

                if (string.IsNullOrEmpty(name) || record.IsDBNull(i))
                    continue;

                if (SetValue(target, name, true, record.GetValue(i))) totalCount++;
            }

            return totalCount;
        }

        /// <summary>
        /// 从DataRow中解析对象属性
        /// </summary>
        /// <param name="record"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FromDataRow(DataRow record, T target)
        {
            int totalCount = 0;


            for (int i = 0, j = record.Table.Columns.Count; i < j; i++)
            {
                string name = record.Table.Columns[i].ColumnName;

                if (string.IsNullOrEmpty(name) || record.IsNull(i))
                    continue;

                if (SetValue(target, name, true, record[i])) totalCount++;
            }

            return totalCount;
        }

        /// <summary>
        /// 从HttpRequest(POST, GET, Server-Varable, Cookies)中解析对象属性, 返回结果
        /// </summary>
        /// <param name="request"></param>
        /// <param name="target"></param>
        /// <returns>解析到的结果数量</returns>
        public static int FromHttpRequest(HttpRequest request, T target)
        {
            int totalCount = 0;

            for (int i = 0, j = request.Params.Count; i < j; i++)
            {
                string name = request.Params.GetKey(i);

                string values = request.Params.Get(i);

                if (SetValueString(target, name, true, values)) totalCount++;
            }

            return totalCount;
        }

        /// <summary>
        /// 从HttpRequest的POST集合中解析对象属性, 返回结果
        /// </summary>
        /// <param name="request"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FromHttpRequestPost(HttpRequest request, T target)
        {
            int totalCount = 0;

            for (int i = 0, j = request.Form.AllKeys.Length; i < j; i++)
            {
                string name = request.Form.GetKey(i);

                string values = request.Form.Get(i);

                if (SetValueString(target, name, true, values)) totalCount++;
            }

            return totalCount;
        }

        /// <summary>
        /// 从HttpRequest的QueryString集合中解析对象属性, 返回结果
        /// </summary>
        /// <param name="request"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FromHttpRequestGet(HttpRequest request, T target)
        {
            int totalCount = 0;

            for (int i = 0, j = request.QueryString.AllKeys.Length; i < j; i++)
            {
                string name = request.QueryString.GetKey(i);

                string values = request.QueryString.Get(i);

                if (SetValueString(target, name, true, values)) totalCount++;
            }

            return totalCount;
        }

        /// <summary>
        /// 从HttpRequest的Cookies集合中解析对象属性, 返回结果
        /// </summary>
        /// <param name="request"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FromHttpRequestCookies(HttpRequest request, T target)
        {
            int totalCount = 0;

            for (int i = 0, j = request.Cookies.AllKeys.Length; i < j; i++)
            {
                var cookie = request.Cookies.Get(i);
                string name = cookie.Name;

                string value = cookie.Value;

                if (SetValueString(target, name, true, value)) totalCount++;
            }

            return totalCount;
        }

        /// <summary>
        /// 从DbDataParameterList中解析对象属性,
        /// 只解析Direction!=ParameterDirection.Input的参数
        /// </summary>
        /// <param name="DbDataParameterList">IDbDataParameter列表</param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FromIDbParameterList(IList DbDataParameterList, T target)
        {
            int totalCount = 0;

            foreach (IDbDataParameter p in DbDataParameterList)
            {
                if (p.Direction == ParameterDirection.Input)
                    continue;

                //Direction!=ParameterDirection.Input
                string pName = EntityToolsInternal.GetParameterName(p);

                if (SetValue(target, pName, true, p.Value))
                    totalCount++;
            }

            return totalCount;
        }

        #endregion

        #region To

        /// <summary>
        /// 从IDataRecord中解析对象属性
        /// </summary>
        /// <param name="target"></param>
        /// <param name="DbDataParameterList">IDbDataParameter列表</param>
        /// <returns></returns>
        public static int ToIDbParameterList(T target, IList DbDataParameterList)
        {
            int totalCount = 0;

            if (target is DataRow)
            {
                EntityTools.DataRow2DbParameterList(target as DataRow, DbDataParameterList);
                return totalCount;
            }


            foreach (IDbDataParameter p in DbDataParameterList)
            {
                if (p.Direction != ParameterDirection.Input)
                    continue;

                string pName = EntityToolsInternal.GetParameterName(p);

                object o = GetValue(target, pName, false);

                if (o == null)
                    p.Value = DBNull.Value;
                else
                    p.Value = o;
            }

            return totalCount;
        }

        #endregion
    }

}
