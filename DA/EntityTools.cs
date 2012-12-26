using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Collections;
using Z.Util;

namespace Z.DA
{
    /// <summary>
    /// 实体对象转换工具
    /// </summary>
    public static class EntityTools
    {
        #region 初始化

        internal static Dictionary<Type, Dictionary<string, MemberInfo>> TypeDictionary;
        static readonly string IConvertibleName;

        static EntityTools()
        {
            TypeDictionary = new Dictionary<Type, Dictionary<string, MemberInfo>>();
            IConvertibleName = typeof(IConvertible).Name;
        }

        #endregion

        #region 转换类型 IDataRecord -> T泛型

        /// <summary>
        /// 转换类型 IDataRecord -> 字符串
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static string ChangeType(IDataRecord dr)
        {
            if (dr == null || dr.FieldCount == 0 || dr.IsDBNull(0))
                return string.Empty;

            return Convert.ToString(dr[0]);
        }

        /// <summary>
        /// 转换类型 IDataRecord -> T泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ChangeType<T>(IDataRecord dr) where T : new()
        {
            Type type = typeof(T);

            if (IsIConvertible(type))
            {
                return (T)Convert.ChangeType(dr[0], type);
            }
         
            return ChangeTypeInternal<T>(0,dr, type);
        }

        /// <summary>
        /// 转换类型 IDataRecord -> T泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ChangeType<T>(DataRow dr) where T : new()
        {
            Type type = typeof(T);

            if (IsIConvertible(type))
            {
                return (T)Convert.ChangeType(dr[0], type);
            }

            return ChangeTypeInternal<T>(1, dr, type);
        }

        private static T ChangeTypeInternal<T>(int iType, object o, Type type) where T : new()
        {
            T item = new T();

            //龚正, 2010-06-20 新增IDataObject支持
            if (item is IDataObject)
            {
                if (iType == 0)
                    IDataRecord2IDataObject((IDataRecord)o, item as IDataObject);
                else if (iType == 1)
                    IDataRow2IDataObject((DataRow)o, item as IDataObject);
            }
            else
            {
                Dictionary<string, MemberInfo> FieldList = GetFieldListFromCache(type);

                if (iType == 0)
                    ExtractFromIRecord<T>((IDataRecord)o, FieldList, item);
                else if (iType == 1)
                    ExtractFromDataRow<T>((DataRow)o, FieldList, item);
            }
             
            return item;
        }

        internal static Dictionary<string, MemberInfo> GetFieldListFromCache(Type type)
        {
            Dictionary<string, MemberInfo> FieldList = null;

            if (!TypeDictionary.ContainsKey(type))
            {
                lock (TypeDictionary)
                {
                    if (!TypeDictionary.ContainsKey(type))
                    {
                        FieldList = GetFieldList(type);

                        TypeDictionary.Add(type, FieldList);
                    }
                }
            }
            return TypeDictionary[type];
        }

        private static void ExtractFromIRecord<T>(IDataRecord dr, Dictionary<string, MemberInfo> FieldList, T item) where T : new()
        {
            for (int i = 0, j = dr.FieldCount; i < j; i++)
            {
                if (!dr.IsDBNull(i))
                {
                    string name = dr.GetName(i);
                    if (FieldList.ContainsKey(name))
                    {
                        if (FieldList[name] is PropertyInfo)
                        {
                            SetValue<T>(FieldList[name] as PropertyInfo, item, dr[i]);
                        }
                        else
                        {
                            SetValue<T>(FieldList[name] as FieldInfo, item, dr[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将DataRecord中的内容填充到IDataObject对象中
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="item"></param>
        public static void IDataRecord2IDataObject(IDataRecord dr, IDataObject item)
        {
            for (int i = 0, j = dr.FieldCount; i < j; i++)
            {
                if (!dr.IsDBNull(i))
                {
                    string name = dr.GetName(i);
                    item.SetValue(name, dr[i]);
                }
            }
        }

        private static void IDataRow2IDataObject(DataRow dr, IDataObject item)
        {
            for (int i = 0, j = dr.Table.Columns.Count; i < j; i++)
            {
                if (!dr.IsNull(i))
                {
                    string name = dr.Table.Columns[i].ColumnName;

                    item.SetValue(name, dr[i]);
                }
            }
        }

        private static void ExtractFromDataRow<T>(DataRow dr, Dictionary<string, MemberInfo> FieldList, T item) where T : new()
        {
            for (int i = 0, j = dr.Table.Columns.Count; i < j; i++)
            {
                if (!dr.IsNull(i))
                {
                    string name = dr.Table.Columns[i].ColumnName;

                    if (FieldList.ContainsKey(name))
                    {
                        if (FieldList[name] is PropertyInfo)
                        {
                            SetValue<T>(FieldList[name] as PropertyInfo, item, dr[i]);
                        }
                        else
                        {
                            SetValue<T>(FieldList[name] as FieldInfo, item, dr[i]);
                        }
                    }
                }
            }
        }

        internal static Dictionary<string, MemberInfo> GetFieldList(Type type)
        {
            Dictionary<string, MemberInfo> FieldList = new Dictionary<string, MemberInfo>();

            FieldInfo[] list1 = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] list2 = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0, j = list1.Length; i < j; i++)
            {
                if(IsIConvertible(list1[i].FieldType))
                    FieldList.Add(list1[i].Name, list1[i]);
            }

            for (int i = 0, j = list2.Length; i < j; i++)
            {
                if (list2[i].CanWrite && IsIConvertible(list2[i].PropertyType))
                    FieldList.Add(list2[i].Name, list2[i]);
            }

            return FieldList;
        }

        /// <summary>
        /// 将对象的值转换到ParameterList列表中
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ParameterList"></param>
        public static void ChangeType(object o, IList ParameterList)
        {
            if (o is DataRow)
            {
                ChangeType(o as DataRow, ParameterList);
                return;
            }

            Dictionary<string, MemberInfo> FieldList = GetFieldListFromCache(o.GetType());

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

                foreach (string k in FieldList.Keys)
                {
                    if (string.Compare(k, pName, true) == 0)
                    {
                        object oo = null;
                        if (FieldList[k] is PropertyInfo)
                        {
                            oo = (FieldList[k] as PropertyInfo).GetValue(o, null);
                        }
                        else if (FieldList[k] is FieldInfo)
                        {

                            oo = (FieldList[k] as FieldInfo).GetValue(o);
                        }

                        if (oo == null)
                            p.Value = DBNull.Value;
                        else
                            p.Value = oo;
                    }
                }
            }
        }

        /// <summary>
        /// 将对象的值转换到ParameterList列表中
        /// </summary>
        /// <param name="r"></param>
        /// <param name="ParameterList"></param>
        public static void ChangeType(DataRow r, IList ParameterList)
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

        /// <summary>
        /// 将对象的值转换到ParameterList列表中
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ParameterList"></param>
        public static void ChangeType(IList ParameterList, object o)
        {
            Dictionary<string, MemberInfo> FieldList = GetFieldListFromCache(o.GetType());

            foreach (IDbDataParameter p in ParameterList)
            {
                if (p.Direction == ParameterDirection.Input)
                    continue;
                
                string pName = p.ParameterName.Substring(1);

                foreach (string k in FieldList.Keys)
                {
                    if (string.Compare(k, pName, true) == 0)
                    {
                        if (FieldList[k] is PropertyInfo)
                        {
                            (FieldList[k] as PropertyInfo).SetValue(o, p.Value, null);
                        }
                        else if (FieldList[k] is FieldInfo)
                        {

                            (FieldList[k] as FieldInfo).SetValue(o, p.Value);
                        }
                    }
                }
            }
        }

        #endregion

        #region 内部方法

        private static bool IsIConvertible(Type t)
        {
            return t.GetInterface(IConvertibleName) != null;
        }


        static void SetValue<T>(PropertyInfo pi, T item, object o)
        {
            if (o == null) return;

            if (IsIConvertible(pi.PropertyType))
            {
                if (pi.PropertyType.IsEnum)
                {
                    pi.SetValue(item, Enum.Parse(pi.PropertyType, o.ToString(), true), null);
                }
                else
                {
                    if (o is IConvertible)
                    {
                        pi.SetValue(item, Convert.ChangeType(o, pi.PropertyType), null);
                    }
                    else
                    {
                        pi.SetValue(item, Convert.ChangeType(o.ToString(), pi.PropertyType), null);
                    }
                }
            }
            else
                pi.SetValue(item, o, null);
        }

        static void SetValue<T>(FieldInfo pi, T item, object o)
        {
            if (o == null) return;

            if (IsIConvertible(pi.FieldType))
            {
                if (pi.FieldType.IsEnum)
                {
                    pi.SetValue(item, Enum.Parse(pi.FieldType, o.ToString(), true));
                }
                else
                {
                    if (o is IConvertible)
                    {
                        pi.SetValue(item, Convert.ChangeType(o, pi.FieldType));
                    }
                    else
                    {
                        pi.SetValue(item, Convert.ChangeType(o.ToString(), pi.FieldType));
                    }
                }
            }
            else
                pi.SetValue(item, o);
        }

        #endregion

    }
}
