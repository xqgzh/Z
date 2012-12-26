using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Z.Util
{
    /// <summary>
    /// 提供普通String中
    /// 未能提供的常用功能,更多功能可以参考Microsoft.VisualBasic命名空间下的Strings类
    /// </summary>
    public static class StringTools
    {
        #region 获取字符经过指定编码后的字节长度

        /// <summary>
        /// 获取字符经过指定编码后的字节长度
        /// 对于需要保存数据库的字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static int ByteLength(string str, Encoding encoder)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] b = encoder.GetBytes(str);
                return b.Length;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region 获得字符串以一定字符分割后得到的数组

        /// <summary>
        /// 获得字符串以一定字符分割后得到的数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] Split(string str, string split)
        {
            return str.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region 得到指定名称和值的赋值字符串

        /// <summary>
        /// 得到指定名称和值的赋值字符串
        /// </summary>
        /// <param name="Name">属性名称</param>
        /// <param name="Value"></param>
        /// <returns></returns>
        /// <author>龚正</author>
        public static string GetNameValue(string Name, object Value)
        {
            if (Value != null)
                return string.Format("<{0}>{1}</{0}>", Name, System.Web.HttpUtility.HtmlEncode(Value.ToString()));
            else
                return string.Format("<{0}/>", Name);
        }

        #endregion

        #region 将字符串按照JavaScript的escape函数的方式对字符串进行编码

        /// <summary>
        /// 将字符串按照JavaScript的escape函数的方式对字符串进行编码，
        /// 以便于在C#中编码的字符串在JavaScript中也能正确解码
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static string JSEscape(string strIn)
        {
            string tmp = "";
            foreach (char c in strIn)
            {
                int i = Convert.ToInt32(c);
                if (i > 127)
                    tmp += "%u" + i.ToString("X4");
                else
                {
                    if (char.IsLetterOrDigit(c))
                        tmp += c;
                    else
                        tmp += "%" + i.ToString("X2");
                }
            }
            return tmp;
        }

        #endregion

        #region 根据 指定的 表名 和 字段名 将所有的行累加, 按照指定的规则 累加为字符串

        /// <summary>
        /// 根据 指定的 表名 和 字段名 将所有的行累加, 按照指定的规则 累加为字符串
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <returns></returns>
        public static string JoinString(DataSet ds, string TableName, string ColumnName, string Prefix, string Suffix, string Spliter)
        {
            if (!ds.Tables.Contains(TableName))
                return "";

            if (!ds.Tables[TableName].Columns.Contains(ColumnName))
                return "";

            return JoinString(ds.Tables[TableName], ColumnName, Prefix, Suffix, Spliter);
        }

        /// <summary>
        /// 根据 指定的 表名 和 字段名 将所有的行累加, 按照指定的规则 累加为字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ColumnName"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <returns></returns>
        public static string JoinString(DataTable dt, string ColumnName, string Prefix, string Suffix, string Spliter)
        {
            if (!dt.Columns.Contains(ColumnName))
                return "";

            return JoinString(dt.Rows, ColumnName, Prefix, Suffix, Spliter);

        }

        /// <summary>
        /// 根据 指定的 表名 和 字段名 将所有的行累加, 按照指定的规则 累加为字符串
        /// </summary>
        /// <param name="Columns"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <param name="IsAppendAutoIncrement">是否附加自增量变量</param>
        /// <returns></returns>
        public static string JoinString(DataColumnCollection Columns, string Prefix, string Suffix, string Spliter, bool IsAppendAutoIncrement)
        {
            if (Columns.Count <= 0)
                return string.Empty;

            StringBuilder strJoin = new StringBuilder();

            int i = 0;
            int j = Columns.Count - 1;
            for (; i < j; i++)
            {
                DataColumn c = Columns[i];

                if (c.AutoIncrement && IsAppendAutoIncrement == false)
                    continue;

                strJoin.Append(Prefix);
                strJoin.Append(c.ColumnName);
                strJoin.Append(Suffix);
                strJoin.Append(Spliter);
            }

            strJoin.Append(Prefix);
            strJoin.Append(Columns[i].ColumnName);
            strJoin.Append(Suffix);

            return strJoin.ToString();
        }

        /// <summary>
        /// 根据 指定的 表名 和 字段名 将所有的行累加, 按照指定的规则 累加为字符串
        /// </summary>
        /// <param name="Columns"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <param name="IsAppendAutoIncrement">是否附加自增量变量</param>
        /// <returns></returns>
        public static string JoinString(DataColumn[] Columns, string Prefix, string Suffix, string Spliter, bool IsAppendAutoIncrement)
        {
            if (Columns.Length <= 0)
                return string.Empty;

            StringBuilder strJoin = new StringBuilder();

            int i = 0;
            int j = Columns.Length - 1;
            for (; i < j; i++)
            {
                DataColumn c = Columns[i];

                if (c.AutoIncrement && IsAppendAutoIncrement == false)
                    continue;

                strJoin.Append(Prefix);
                strJoin.Append(c.ColumnName);
                strJoin.Append(Suffix);
                strJoin.Append(Spliter);
            }

            strJoin.Append(Prefix);
            strJoin.Append(Columns[i].ColumnName);
            strJoin.Append(Suffix);

            return strJoin.ToString();
        }

        /// <summary>
        /// 根据 指定的 表名 和 字段名 将所有的行累加, 按照指定的规则 累加为字符串
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="ColumnName"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <returns></returns>
        public static string JoinString(DataRowCollection rs, string ColumnName, string Prefix, string Suffix, string Spliter)
        {
            if (rs.Count <= 0)
                return "";

            if (!rs[0].Table.Columns.Contains(ColumnName))
                return "";

            string strJoin = "";

            foreach (DataRow r in rs)
            {
                if (!r.IsNull(ColumnName))
                {
                    string strTemp = Prefix;

                    strTemp += r[ColumnName].ToString();

                    strTemp += Suffix;

                    strJoin += strTemp + Spliter;
                }

            }

            return strJoin;
        }

        /// <summary>
        /// 根据 指定的 表名 和 字段名 将所有的行累加, 按照指定的规则 累加为字符串
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="ColumnName"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <returns></returns>
        public static string JoinString(DataRow[] rs, string ColumnName, string Prefix, string Suffix, string Spliter)
        {
            if (rs.Length <= 0)
                return "";

            if (!rs[0].Table.Columns.Contains(ColumnName))
                return "";

            string strJoin = "";

            foreach (DataRow r in rs)
            {
                if (!r.IsNull(ColumnName))
                {
                    string strTemp = Prefix;

                    strTemp += r[ColumnName].ToString();

                    strTemp += Suffix;

                    strJoin += strTemp + Spliter;
                }

            }

            if (strJoin.EndsWith(Spliter))
                strJoin = strJoin.Substring(0, strJoin.Length - Spliter.Length);

            return strJoin;
        }

        #endregion

        #region 根据 指定的 类型 将类型数组按照指定的规则 累加为字符串

        /// <summary>
        /// 根据 指定的 类型 将类型数组按照指定的规则 累加为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <returns></returns>
        public static string JoinString<T>(T[] ts, string Prefix, string Suffix, string Spliter)
        {
            StringBuilder sb = new StringBuilder();

            if (ts == null || ts.Length == 0)
                return string.Empty;

            for (int i = 0; i < ts.Length - 1; i++)
            {
                sb.Append(string.Format("{0}{1}{2}{3}",
                    Prefix, ts[i].ToString(), Suffix, Spliter));
            }

            sb.Append(string.Format("{0}{1}{2}",
                Prefix, ts[ts.Length - 1].ToString(), Suffix));

            return sb.ToString();
        }

        /// <summary>
        /// 根据 指定的 类型 将类型数组按照指定的规则 累加为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <returns></returns>
        public static string JoinString<T>(List<T> ts, string Prefix, string Suffix, string Spliter)
        {
            StringBuilder sb = new StringBuilder();

            if (ts == null || ts.Count == 0)
                return string.Empty;

            for (int i = 0; i < ts.Count - 1; i++)
            {
                sb.Append(string.Format("{0}{1}{2}{3}",
                    Prefix, ts[i].ToString(), Suffix, Spliter));
            }

            sb.Append(string.Format("{0}{1}{2}",
                Prefix, ts[ts.Count - 1].ToString(), Suffix));

            return sb.ToString();
        }

        /// <summary>
        /// 根据 指定的 类型 将类型数组按照指定的规则 累加为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="Prefix"></param>
        /// <param name="Suffix"></param>
        /// <param name="Spliter"></param>
        /// <returns></returns>
        public static string JoinString<T>(IEnumerable<T> ts, string Prefix, string Suffix, string Spliter)
        {
            StringBuilder sb = new StringBuilder();

            if (ts == null)
                return string.Empty;

            foreach (T t in ts)
            {
                sb.Append(string.Format("{0}{1}{2}{3}",
                 Prefix, t.ToString(), Suffix, Spliter));
            }

            if (sb.ToString().EndsWith(Suffix + Spliter))
            {
                string s = Suffix + Spliter;

                sb.Remove(sb.Length - s.Length, s.Length);
            }

            return sb.ToString();
        }

        #endregion

        #region 判断字符串是否为数字

        /// <summary>
        /// 判断字符串是否为数字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNumber(string source)
        {
            Regex reg = new Regex(@"^\d+$", RegexOptions.IgnoreCase);
            return reg.IsMatch(source);
        }

        #endregion

        #region 判断字符串是否为带小数的数字

        /// <summary>
        /// 判断字符串是否为带小数的数字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDecimal(string source)
        {
            if (source == "")
                return false;

            Regex reg = new Regex(@"^\d*(\.\d*)?$", RegexOptions.IgnoreCase);
            return reg.IsMatch(source);
        }


        #endregion

        #region 判断字符串是否符合正则表达式要求（不区分大小写）

        /// <summary>
        /// 判断字符串是否符合正则表达式要求（不区分大小写）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool IsValide(string source, string regex)
        {
            if (source == "")
                return false;

            Regex reg = new Regex(regex, RegexOptions.IgnoreCase);
            return reg.IsMatch(source);
        }

        #endregion

        #region 格式化过滤 WebForm 输入框中的非法字符

        /// <summary>
        /// 格式化过滤 WebForm 输入框中的非法字符
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string InputText(string inputString)
        {
            StringBuilder retVal = new StringBuilder();
            if ((inputString != null) && (inputString != String.Empty))
            {
                inputString = inputString.Trim();
                for (int i = 0; i < inputString.Length; i++)
                {
                    switch (inputString[i])
                    {
                        case '"':
                            retVal.Append("&quot;");
                            break;
                        case '<':
                            retVal.Append("&lt;");
                            break;
                        case '>':
                            retVal.Append("&gt;");
                            break;
                        default:
                            retVal.Append(inputString[i]);
                            break;
                    }
                }
                retVal.Replace("'", " ");
            }
            return retVal.ToString().Trim();
        }

        #endregion

        #region 分隔字符串的处理方法　（　添加　／　删除 ）
        /// <summary>
        /// 向 分隔列字符串中加入新的项,新添加的项不会重复
        /// </summary>
        /// <param name="source">原字符串</param>
        /// <param name="split">分隔符</param>
        /// <param name="item">新加的项</param>
        /// <returns></returns>
        public static string AddSplitItem(string source, string split, string item)
        {
            if (source.IndexOf(item) >= 0)
                return source;
            if (source == "")
            {
                return item;
            }
            else
            {
                return source + split + item;
            }
        }

        /// <summary>
        /// 从 分隔列字符串中删除项
        /// </summary>
        /// <param name="source">原字符串</param>
        /// <param name="split">分隔符</param>
        /// <param name="item">新加的项</param>
        /// <returns></returns>
        public static string RemoveSplitItem(string source, string split, string item)
        {
            if (source.IndexOf(item) == -1)
            {
                return source;
            }
            else
            {
                //剁掉首尾
                if (source.IndexOf(split) == 0)
                {
                    source = source.Remove(0, split.Length);
                }
                if (source.LastIndexOf(split) == source.Length - split.Length)
                    source = source.Remove(source.Length - split.Length, split.Length);

                source = source.Replace(item, "");
                source = source.Replace(split + split, split);

                return source;
            }
        }

        #endregion

        #region 剔除 Like 模糊查询中的非法Bug字符 (　Sql Server 版本 )
        /// <summary>
        /// 剔除 Like 模糊查询中的非法Bug字符 (　Sql Server 版本 )
        /// </summary>
        /// <param name="sqlStr">SQL 查询字符串</param>
        /// <returns>返回过滤后的合格字符</returns>
        public static string ToLikeSql(string sqlStr)
        {
            if (sqlStr == null) return "";
            StringBuilder b = new StringBuilder(sqlStr);
            b.Replace("'", "''");
            b.Replace("[", "[[]");
            b.Replace("%", "[%]");
            b.Replace("_", "[_]");
            return b.ToString();
        }
        #endregion

        #region 格式化Sql语句中的Where or /Where and 现象

        /// <summary>
        /// 格式化Sql语句可用
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static string FormatSql(string strSql)
        {
            if (strSql.EndsWith("where"))
                strSql = strSql.Replace("where", "");
            if (strSql.IndexOf("where and") > 0)
                strSql = strSql.Replace("where and", "where");
            if (strSql.IndexOf("where or") > 0)
                strSql = strSql.Replace("where or", "where");
            return strSql;
        }

        #endregion

        #region 删除行尾指定的内容

        /// <summary>
        /// 删除行尾指定的字符串
        /// </summary>
        /// <param name="sbIn"></param>
        /// <param name="end">要删除的行尾的字符串</param>
        public static void TrimEnd(StringBuilder sbIn, string end)
        {
            if (sbIn.ToString().EndsWith(end))
                sbIn.Remove(sbIn.Length - end.Length, end.Length);
        }

        #endregion

        

        #region 将指定的对象序列化为二进制的BASE64编码字符串

        /// <summary>
        /// 将指定的对象序列化为二进制的BASE64编码字符串
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToObjectBase64String(object o)
        {
            string s = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(ms, o);
                s = Convert.ToBase64String(ms.ToArray());

                ms.Close();
            }

            return s;
        }

        /// <summary>
        /// 将指定的对象转换为Xml文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T FromObjectBase64String<T>(string s) where T : class
        {
            byte[] bytes = Convert.FromBase64String(s);
            object o = null;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                o = (T)bf.Deserialize(ms);
                ms.Close();
            }

            return o as T;
        }

        #endregion

        #region 确定某字符串是否存在

        /// <summary>
        /// 确定某字符串是否存在
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strFinds"></param>
        /// <returns></returns>
        public static string Conterns(string strA, string[] strFinds)
        {
            string strFind = string.Format(",{0},", strA);
            foreach (string s in strFinds)
            {
                if (strFind.IndexOf(',' + s + ',') >= 0)
                    return s;
            }

            return "";
        }

        #endregion

        #region 将DataSet转换为TXT格式

        /// <summary>
        /// 将DataSet转换为TXT格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static StringBuilder ToStringBuilder(DataTable dt)
        {
            int RowCount = dt.Rows.Count;
            int ColCount = dt.Columns.Count;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < RowCount; i++)
            {
                DataRow r = dt.Rows[i];
                for (int j = 0; j < ColCount; j++)
                {
                    sb.AppendFormat("{0}\t", r[dt.Columns[j]]);
                }

                sb.AppendLine();
            }

            return sb;
        }

        #endregion

        /// <summary>
        /// 判断输入字符中，是否包含中文
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ContainChineseString(string s)
        {
            Regex rx = new Regex("^[\u4e00-\u9fa5]$");
            for (int i = 0; i < s.Length; i++)
            {
                if ( rx.IsMatch(s[i].ToString()))
                    return true ;
            }
            return false;
        }
    }
}
