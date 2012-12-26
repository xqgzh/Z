using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinConfigManager.C;
using System.Windows.Forms;

namespace WinConfigManager.WinControls
{
    public class SectionNode : TreeNode
    {
        public ConfigEntitySection SelectedSection;

        public SectionNode(ConfigEntitySection section)
        {
            ImageIndex = 0;
            Text = section.Name;
            SelectedSection = section;
        }
    }

    public class SectionMenu : ToolStripDropDownButton
    {
        public ConfigEntitySection SelectedSection;

        public SectionMenu(ConfigEntitySection section) 
        {
            DisplayStyle = ToolStripItemDisplayStyle.Text;

            base.AutoSize = true;

            Text = section.Name;
            SelectedSection = section;
        }
    }
}
