using System;
using System.Collections;
using System.Linq;
using System.Web;

namespace Z.OAuth
{
    /// <summary>
    /// Provides an internal structure to sort the query parameter
    /// </summary>
    public class QueryParameter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="needSign"></param>
        public QueryParameter(string name, object value, bool needSign = true)
        {
            Name = name;
            Value = value;
            NeedSign = needSign;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// 是否参与签名
        /// </summary>
        public bool NeedSign { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GenerateEncodedUrlParameter()
        {
            return String.Format("{0}={1}", HttpUtility.UrlEncode(Name), HttpUtility.UrlEncode(Convert.ToString(Value)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GenerateJsonItem()
        {
            if (Value is string)
            {
                return String.Format("\"{0}\":\"{1}\"", Name, Value);
            }
            else if (Value.GetType().IsArray)
            {
                var values = (Value as IEnumerable).Cast<object>().Select(v => String.Format("\"{0}\"", v));

                return String.Format("\"{0}\":[{1}]", Name, String.Join(",", values));
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
