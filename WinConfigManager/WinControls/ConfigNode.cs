using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinConfigManager.WinControls
{
    class ConfigNode : TreeNode
    {
        public ConfigFileEntity SelectedConfig;

        public ConfigNode(ConfigFileEntity config)
        {
            ImageIndex = 2;

            Text = config.Name;

            if (string.IsNullOrEmpty(Text))
            {
                UriBuilder u = new UriBuilder(config.RemoteAddress);

                Text = string.Format("{0}[{1}]", u.Host, config.RemoteName);
            }
            SelectedConfig = config;
        }
    }

    class ConfigMenu : ToolStripButton
    {
        public ConfigFileEntity SelectedConfig;

        public ConfigMenu(ConfigFileEntity config) : base(config.Name)
        {
            base.AutoSize = true;
            Text = config.Name;

            if (string.IsNullOrEmpty(Text))
            {
                UriBuilder u = new UriBuilder(config.RemoteAddress);

                Text = string.Format("{0}[{1}]", u.Host, config.RemoteName);
            }

            SelectedConfig = config;
            DisplayStyle = ToolStripItemDisplayStyle.Text;

            base.Width = 200;
        }
    }
}
