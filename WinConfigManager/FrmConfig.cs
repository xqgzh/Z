using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Z.C;
using System.IO;
using WinConfigManager.C;
using System.Xml;
using WinConfigManager.WinControls;

namespace WinConfigManager
{
    public partial class FrmConfig : Form
    {
        internal RemoteConfig RemoteConfigObj = null;
        ConfigFileEntity CurrentConfig = null;

        public FrmConfig()
        {
            InitializeComponent();

        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            InitConfigs();
            //RefreshConfigXml();
        }

        void InitConfigs()
        {
            trConfigs.Nodes.Clear();
            trConfigs.Nodes.Add("Root");

            LoadConfigNodes(ManagerConfig.Instance, trConfigs.Nodes[0]);

            trConfigs.ExpandAll();
        }

        void LoadConfigNodes(ConfigEntitySection configs, TreeNode root)
        {
            foreach (ConfigEntitySection section in configs.Sections)
            {
                SectionNode node = new SectionNode(section);

                node.ContextMenuStrip = contextMenuStrip2;
                LoadConfigNodes(section, node);
                node.ImageIndex = 0;
                root.Nodes.Add(node);
            }

            foreach (ConfigFileEntity config in configs.List)
            {
                ConfigNode node = new ConfigNode(config);
                node.ContextMenuStrip = contextMenuStrip1;
                node.ImageIndex = 2;
                root.Nodes.Add(node);
            }
        }

        string GetTxtConfigXml()
        {
            return txtConfigXml.Text;
        }


        void RefreshConfigXml()
        {
            string s = RemoteConfigObj.GetConfig(txtRemoteName.Text, txtRemotePass.Text).Trim();
            string x = GetTxtConfigXml();

            if (string.IsNullOrEmpty(x) == false && s != x)
            {
                if (MessageBox.Show("本地内容与远程内容不符, 是否覆盖?", "内容不一致", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            txtConfigXml.Text = s;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshConfigXml();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ConfigFileEntity cfe = new ConfigFileEntity();

                cfe.RemoteAddress = txtRemoteAddress.Text;
                cfe.RemotePass = txtRemotePass.Text;
                cfe.RemoteName = txtRemoteName.Text;
                cfe.RemoteFile = txtRemoteFile.Text;

                cfe.Content = txtConfigXml.Text;

                MessageBox.Show(saveFileDialog1.FileName);

                File.WriteAllText(saveFileDialog1.FileName, Z.Util.XmlTools.ToXml(cfe));

                btnSave.Enabled = false;
            }
        }

        private void txtConfigXml_TextChanged(object sender, EventArgs e)
        {
            if (btnSave.Enabled == false)
                btnSave.Enabled = true;
        }

        void SaveRemoteConfig()
        {
            if (!RemoteConfigObj.TestConfig(txtRemoteName.Text, txtConfigXml.Text, txtRemotePass.Text))
            {
                MessageBox.Show("配置信息与远程文件不符", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(RemoteConfigObj.SaveConfig(txtRemoteName.Text, txtConfigXml.Text, txtRemoteFile.Text, txtRemotePass.Text));
            }
        }

        private void btnSaveRemote_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtConfigXml.Text))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();

                    

                    doc.LoadXml(txtConfigXml.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                SaveRemoteConfig();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            FrmOpen fOpen = new FrmOpen();

            if (fOpen.ShowDialog() == DialogResult.OK)
            {
                txtRemoteAddress.Text = fOpen.GetRemoteAddress();
                txtRemotePass.Text = fOpen.txtRemotePass.Text;
                txtRemoteName.Text = fOpen.txtRemoteName.Text;
                txtRemoteFile.Text = fOpen.txtRemoteFile.Text;

                RemoteConfigObj = fOpen.RemoteConfigObj;

                RefreshConfigXml();
            }
        }

        private void btnViewServerVersions_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(CurrentConfig.RemoteAddress))
                {
                    return;
                }

                RemoteConfigObj = Activator.GetObject(typeof(Z.C.RemoteConfig), CurrentConfig.RemoteAddress) as RemoteConfig;

                //检查是否能够正常使用
                string strConfig = RemoteConfigObj.GetServerVersions().Trim();

                FrmServerVersion fServerVersion = new FrmServerVersion(strConfig);

                fServerVersion.ShowDialog();


            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void trConfigs_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void trConfigs_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void trConfigs_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is SectionNode)
            {
                grpBox.Enabled = false;
            }
            else
            {
                grpBox.Enabled = true;
            }
        }

        private void toolOpenConfig_Click(object sender, EventArgs e)
        {
            if (trConfigs.SelectedNode != null && trConfigs.SelectedNode is ConfigNode)
            {
                
                ConfigNode config = trConfigs.SelectedNode as ConfigNode;

                if (backgroundWorker1.IsBusy == false)
                    backgroundWorker1.RunWorkerAsync(config.SelectedConfig);
            }
        }

        void OpenConfig(ConfigFileEntity config)
        {
            try
            {
                if (string.IsNullOrEmpty(config.RemoteAddress))
                {
                    return;
                }

                RemoteConfigObj = Activator.GetObject(typeof(Z.C.RemoteConfig), config.RemoteAddress) as RemoteConfig;

                //检查是否能够正常使用
                string strConfig = RemoteConfigObj.GetConfig(config.RemoteName, config.RemotePass).Trim();

                txtConfigXml.Text = strConfig;

            }
            catch (Exception ex)
            {
                MessageBox.Show(this,  ex.Message);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            CurrentConfig = e.Argument as ConfigFileEntity;
            OpenConfig(e.Argument as ConfigFileEntity);
        }

        private void btnEditStatus_Click(object sender, EventArgs e)
        {
            btnEditStatus.Checked = !btnEditStatus.Checked;

            if (btnEditStatus.Checked)
            {
                txtConfigXml.ReadOnly = false;
                txtConfigXml.BackColor = Color.White;
            }
            else
            {
                txtConfigXml.ReadOnly = true;
                txtConfigXml.BackColor = SystemColors.Control;
            }
        }

    }
}
