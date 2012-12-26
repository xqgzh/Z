using System;
using System.Collections.Generic;
using System.Linq;

namespace Z.OAuth
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryParameterList : List<QueryParameter>
    {
        /// <summary>
        /// 
        /// </summary>
        public BodyDataFormat FormatMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QueryParameterList() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatMode"></param>
        public QueryParameterList(BodyDataFormat formatMode)
        {
            FormatMode = formatMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                var parameter = this.Where(p => p.Name == key);
                if (parameter == null || !parameter.Any())
                {
                    return null;
                }

                if (parameter.First().Value is string)
                {
                    return Convert.ToString(parameter.First().Value);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(FormatMode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QueryParameterList Parse(string value)
        {
            var result = new QueryParameterList();

            // 检查数据的类型
            if (value.Contains("{"))
            {
                result.FormatMode = BodyDataFormat.JSON;
                int startIndex = value.IndexOf('{') + 1;
                int endIndex = value.LastIndexOf('}') - 1;
                if (startIndex < 0 || endIndex < 0)
                {
                    throw new ArgumentException("value不是合法的JSON格式。");
                }
                value = value.Substring(startIndex, endIndex - startIndex + 1).Trim();

                foreach (var parameter in value.Split(','))
                {
                    var keyValuePair = parameter.Split(':');
                    if (keyValuePair.Length != 2)
                    {
                        throw new ArgumentException("value不是合法的JSON格式。");
                    }

                    result.Add(new QueryParameter(keyValuePair[0], keyValuePair[1]));
                }
            }
            else
            {
                result.FormatMode = BodyDataFormat.EncodedUrl;
                foreach (var parameter in value.Split('&'))
                {
                    var keyValuePair = parameter.Split('=');
                    if (keyValuePair.Length != 2)
                    {
                        throw new ArgumentException("value不是合法的EncodedURL格式。");
                    }

                    result.Add(new QueryParameter(keyValuePair[0], keyValuePair[1]));
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatMode"></param>
        /// <returns></returns>
        public string ToString(BodyDataFormat formatMode)
        {
            switch (formatMode)
            {
                case BodyDataFormat.EncodedUrl:
                    return FormatAsEncodedUrl();
                case BodyDataFormat.JSON:
                    return FormatAsJson();
                default:
                    return base.ToString();
            }
        }

        private string FormatAsEncodedUrl()
        {
            return String.Join("&", this.Select(i => i.GenerateEncodedUrlParameter()));
        }

        private string FormatAsJson()
        {
            return "{" + String.Join(",", this.Select(i => i.GenerateJsonItem())) + "}";
        }
    }
}
