using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Z.Util
{
    /// <summary>
    /// CData
    /// </summary>
    public class CDATA : IXmlSerializable
    {
        private string text = string.Empty;

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public CDATA()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text"></param>
        public CDATA(string text)
        {
            this.text = text;
        }

        #endregion

        /// <summary>
        /// Value
        /// </summary>
        public string Value
        {
            get { return text; }
        }

        #region IXmlSerializable Members

        /// <summary>
        /// 写入XML
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            WriteXml(this.text, writer);
        }

        /// <summary>
        /// 写入XML
        /// </summary>
        /// <param name="s"></param>
        /// <param name="writer"></param>
        void WriteXml(string s, System.Xml.XmlWriter writer)
        {
            if (string.IsNullOrEmpty(s) == false)
            {
                int index = s.IndexOf("]]>");

                if (index >= 0)
                {
                    WriteXml(s.Substring(0, index + 2), writer);
                    WriteXml(s.Substring(index + 2), writer);
                }
                else
                    writer.WriteCData(s);
            }
        }


        /// <summary>
        /// GetSchema
        /// </summary>
        /// <returns></returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            //MSDN dice "This member supports the .NET Framework infrastructure 
            //and is not intended to be used directly from your code."
            return null;
        }

        /// <summary>
        /// ReadXml
        /// </summary>
        /// <param name="reader"></param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            //(@update 04-nov-2005, vedi errata korrige)

            this.text = reader.ReadString();
            //reader.Read()     
        }
        #endregion

        #region 重载运算符

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="address">服务器地址</param>
        /// <returns></returns>
        public static implicit operator CDATA(string address)
        {
            return new CDATA(address);
        }

        #endregion
    }
}
