using System;
using System.Collections.Generic;
using System.Text;
using Z.Util;

namespace Z.Ex
{
    /// <summary>
    /// 异常格式化工具
    /// </summary>
    public class ExceptionFormatter
    {
        /// <summary>
        /// 格式化异常
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string FormatException(Exception e)
        {
            StringBuilder sbXml = new StringBuilder();

            if (e is Exceptionbase)
            {
                Exceptionbase ee = e as Exceptionbase;
                sbXml.Append(StringTools.GetNameValue("ExName", e.GetType().Name));
                sbXml.Append(StringTools.GetNameValue("MSG", e.Message));

                if (ee.IsFormatStack)
                {
                    sbXml.AppendLine(StringTools.GetNameValue("StackTrace", System.Web.HttpUtility.HtmlEncode(e.StackTrace)));
                    sbXml.AppendLine(StringTools.GetNameValue("Source", System.Web.HttpUtility.HtmlEncode(e.Source)));
                    sbXml.AppendLine(StringTools.GetNameValue("TargetSite", e.TargetSite));
                }
            }
            else
            {
                sbXml.Append(StringTools.GetNameValue("ExName", e.GetType().Name));
                sbXml.Append(StringTools.GetNameValue("MSG", e.Message));
                sbXml.AppendLine(StringTools.GetNameValue("StackTrace", System.Web.HttpUtility.HtmlEncode(e.StackTrace)));
                sbXml.AppendLine(StringTools.GetNameValue("Source", System.Web.HttpUtility.HtmlEncode(e.Source)));
                sbXml.AppendLine(StringTools.GetNameValue("TargetSite", e.TargetSite));
            }

            if (e.InnerException != null)
            {
                sbXml.Append("<InnerException>");
                sbXml.Append(FormatException(e.InnerException));
                sbXml.Append("</InnerException>");
            }

            if (e is Exceptionbase)
            {
                (e as Exceptionbase).ToXmlElements(sbXml);
            }

            return sbXml.ToString();
        }
    }
}
