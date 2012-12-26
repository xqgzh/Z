using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Z.DA;
using Z.Log;

namespace Z.Util
{
    /// <summary>
    /// IDataObject的相关函数
    /// </summary>
    public static class DataObjectTools
    {
        private static readonly Logger logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 合并IDataObject集合

        /// <summary>
        /// 合并IDataObject集合
        /// </summary>
        /// <typeparam name="T">IDataObject类型</typeparam>
        /// <param name="KeyName">键名称集合</param>
        /// <param name="A">集合A</param>
        /// <param name="B">集合B</param>
        /// <returns></returns>
        public static List<T> Combine<T>(string KeyName, IEnumerable A, IEnumerable B) where T : IDataObject
        {
            Dictionary<string, T> dict = new Dictionary<string, T>();

            CombineInternal<T>(KeyName, dict, A);

            CombineInternal<T>(KeyName, dict, B);

            List<T> list2 = new List<T>(dict.Values);

            return list2;
        }


        /// <summary>
        /// 合并IDataObject集合
        /// </summary>
        /// <typeparam name="T">IDataObject类型</typeparam>
        /// <param name="KeyName">键名称集合</param>
        /// <param name="A">集合A</param>
        /// <param name="B">集合B</param>
        /// <param name="C">集合C</param>
        /// <returns></returns>
        public static List<T> Combine<T>(string KeyName, IEnumerable A, IEnumerable B, IEnumerable C) where T : IDataObject
        {
            Dictionary<string, T> dict = new Dictionary<string, T>();

            CombineInternal<T>(KeyName, dict, A);

            CombineInternal<T>(KeyName, dict, B);

            CombineInternal<T>(KeyName, dict, C);

            List<T> list2 = new List<T>(dict.Values);

            return list2;
        }

        /// <summary>
        /// 合并IDataObject集合
        /// </summary>
        /// <typeparam name="T">IDataObject类型</typeparam>
        /// <param name="KeyName">键名称</param>
        /// <param name="A">集合A</param>
        /// <param name="list">IEnumerable类型集合</param>
        /// <returns></returns>
        public static List<T> Combine<T>(string KeyName, IEnumerable A, params IEnumerable[] list) where T : IDataObject
        {
            Dictionary<string, T> dict = new Dictionary<string, T>();

            CombineInternal<T>(KeyName, dict, A);

            foreach(IEnumerable i in list)
                CombineInternal<T>(KeyName, dict, i);

            List<T> list2 = new List<T>(dict.Values);

            return list2;
        }


        /// <summary>
        /// 内部方法, 加入一个集合到字典中
        /// </summary>
        /// <typeparam name="T">IDataObject类型</typeparam>
        /// <param name="KeyName">键名称, 以,结束</param>
        /// <param name="Dict">字典</param>
        /// <param name="A">集合A</param>
        internal static void CombineInternal<T>(string KeyName, Dictionary<string, T> Dict, IEnumerable A) where T : IDataObject
        {
            foreach (T a in A)
            {
                string[] keyNamelist = KeyName.Split(new char[]{','},  StringSplitOptions.RemoveEmptyEntries);

                string k = string.Empty;

                foreach (string keyName in keyNamelist)
                {
                    string s = Convert.ToString(a.GetValue(keyName));

                    k = string.Concat(k, ",", s);
                }
                
                if (!string.IsNullOrEmpty(k) && !Dict.ContainsKey(k))
                    Dict.Add(k, a);
            }
        }

        #endregion

        #region 合并指定Name的数组为字符串

        /// <summary>
        /// 合并指定Name的数组为字符串
        /// </summary>
        /// <param name="A">ILIST数组, 包含IDataObject元素</param>
        /// <param name="Name">字段名称</param>
        /// <param name="FromIndex">开始索引</param>
        /// <param name="ToIndex">结束索引</param>
        /// <param name="Prefix">前缀</param>
        /// <param name="Suffix">后缀</param>
        /// <param name="Spliter">间隔符号</param>
        /// <returns></returns>
        public static string Join(IList A, string Name, int FromIndex, int ToIndex, string Prefix, string Suffix, string Spliter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Prefix);
            int i = FromIndex;
            for (; i <= ToIndex; i++)
            {
                if (i >= A.Count)
                    break;

                IDataObject obj = A[i] as IDataObject;

                if (obj != null)
                {
                    object o = obj.GetValue(Name);

                    if (o != null)
                    {
                        sb.AppendFormat("{0},", o);
                    }
                }
            }

            if (sb[sb.Length - 1] == ',')
                sb.Remove(sb.Length - 1, 1);
           

            sb.Append(Suffix);

            return sb.ToString();
        }

        #endregion

        #region 转换类型

        /// <summary>
        /// 转换IDataObject对象类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objInput"></param>
        /// <returns></returns>
        public static T ChangeType<T>(IDataObject objInput) where T: IDataObject, new()
        {
            T t = new T();
            
            Dictionary<string, MemberInfo> oldType = EntityTools.GetFieldListFromCache(objInput.GetType());

            foreach (string s in oldType.Keys)
            {
                try
                {
                    object o = objInput.GetValue(s);

                    if(o != null)
                        t.SetValue(s, o);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "IDataObject转换类型错误");
                }
            }

            return t;
        }


        /// <summary>
        /// 在两个IDataObject对象之间传递数据
        /// </summary>
        /// <param name="objInput"></param>
        /// <param name="objOutput"></param>
        /// <returns></returns>
        public static void ChangeType(IDataObject objInput, IDataObject objOutput)
        {

            Dictionary<string, MemberInfo> oldType = EntityTools.GetFieldListFromCache(objInput.GetType());

            foreach (string s in oldType.Keys)
            {
                try
                {
                    object o = objInput.GetValue(s);

                    if (o != null)
                        objOutput.SetValue(s, o);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "IDataObject转换类型错误");
                }
            }
        }

        #endregion
    }
}
