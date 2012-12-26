using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WinConfigManager.C
{
    public class ManagerConfig : ConfigEntitySection
    {
        public static ManagerConfig Instance
        {
            get
            {
                return Z.C.AppConfigHandler.GetConfig<ManagerConfig>("ManageConfig.config", true);
            }
        }

        public static void Save()
        {
            Z.C.AppConfigHandler.SaveConfig<ManagerConfig>("ManageConfig.config", true, Instance);
        }

        public ConfigFileEntity Find(string s)
        {
            foreach (ConfigFileEntity cf in ManagerConfig.Instance.List)
            {
                if (string.Compare(cf.ToString(), s) == 0)
                {
                    return cf;
                }
            }

            return null;
        }
    }

    public class ConfigEntitySection
    {
        [XmlAttribute]
        public string Name;

        [XmlElement("Section")]
        public List<ConfigEntitySection> Sections = new List<ConfigEntitySection>();

        [XmlElement("Remote")]
        public List<ConfigFileEntity> List = new List<ConfigFileEntity>();
    }
}
