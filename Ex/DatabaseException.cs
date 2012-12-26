using System;
using System.Collections.Generic;
using System.Text;
using Z.Util;

namespace Z.Ex
{
    /// <summary>
    /// 数据库访问异常 
    /// </summary>
    public class DatabaseException : Exceptionbase
    {
        /// <summary>
        /// 发生异常的SQL语句
        /// </summary>
        public string SQL;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        /// <param name="sql"></param>
        public DatabaseException(string msg, Exception ex, string sql)
            : base(msg, ex)
        {
            SQL = sql;
        }

        /// <summary>
        /// 将数据填充到XML流中
        /// </summary>
        /// <param name="sbXml"></param>
        public override void ToXmlElements(StringBuilder sbXml)
        {
            base.ToXmlElements(sbXml);

            sbXml.Append(StringTools.GetNameValue("SQL", SQL));

        }
    }
}
