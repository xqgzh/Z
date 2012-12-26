using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WinConfigManager
{
    public class ConfigFileEntity : IComparable<ConfigFileEntity>
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string RemoteAddress;

        [XmlAttribute]
        public string RemotePass;

        [XmlAttribute]
        public string RemoteName;

        [XmlAttribute]
        public string RemoteFile;

        public string Content;

        public override string ToString()
        {
            return string.Format("{1}[{0}]", RemoteName, RemoteAddress);
        }

        #region IComparable<ConfigFileEntity> Members

        public int CompareTo(ConfigFileEntity other)
        {
            return string.Compare(this.ToString(), other.ToString());
        }

        #endregion
    }
}
