using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinConfigManager.C;
using WinConfigManager.WinControls;

namespace WinConfigManager
{
    public partial class WinForm : Form
    {
        public WinForm()
        {
            InitializeComponent();
            InitConfigs();
        }

        private void WinForm_Load(object sender, EventArgs e)
        {

        }

        void InitConfigs()
        {
            LoadConfigNodes(ManagerConfig.Instance, btnServerConfigs);

            toolStrip1.ResumeLayout(true);
        }

        void LoadConfigNodes(ConfigEntitySection configs, ToolStripDropDownItem root)
        {
            foreach (ConfigEntitySection section in configs.Sections)
            {
                SectionMenu node = new SectionMenu(section);

                LoadConfigNodes(section, node);
                root.DropDownItems.Add(node);
            }

            foreach (ConfigFileEntity config in configs.List)
            {
                ConfigMenu node = new ConfigMenu(config);
                root.DropDownItems.Add(node);
            }
        }
    }
}
