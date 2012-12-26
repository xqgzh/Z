using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Z.C;
using WinConfigManager.C;

namespace WinConfigManager
{
    public partial class FrmOpen : Form
    {
        public FrmOpen()
        {
            InitializeComponent();

        }

        internal RemoteConfig RemoteConfigObj = null;

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtRemoteAddress.Text))
                {
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                RemoteConfigObj = Activator.GetObject(typeof(Z.C.RemoteConfig), GetRemoteAddress()) as RemoteConfig;

                //检查是否能够正常使用
                RemoteConfigObj.GetConfig(txtRemoteName.Text, txtRemotePass.Text);

                if (txtRemoteAddress.SelectedItem is ConfigFileEntity)
                {

                }
                else
                {
                    //检查是否有重复的, 如果是重复的, 则重新赋值
                    ConfigFileEntity cfe = ManagerConfig.Instance.Find(txtRemoteAddress.Text);

                    if (cfe != null)
                    {
                        cfe.RemotePass = txtRemotePass.Text;
                        cfe.RemoteFile = txtRemoteFile.Text;
                    }
                    else
                    {
                        cfe = new ConfigFileEntity();

                        cfe.RemoteAddress = GetRemoteAddress();
                        cfe.RemotePass = txtRemotePass.Text;
                        cfe.RemoteFile = txtRemoteFile.Text;
                        cfe.RemoteName = txtRemoteName.Text;

                        ManagerConfig.Instance.List.Add(cfe);
                        ManagerConfig.Instance.List.Sort();
                    }
                    ManagerConfig.Save();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FrmOpen_Load(object sender, EventArgs e)
        {
            txtRemoteAddress.Items.Clear();
            ManagerConfig.Instance.List.Sort();
            foreach (ConfigFileEntity cfe in ManagerConfig.Instance.List)
            {
                txtRemoteAddress.Items.Add(cfe);
            }

        }

        private void txtRemoteAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtRemoteAddress.SelectedItem != null && txtRemoteAddress.SelectedItem is ConfigFileEntity)
            {
                ConfigFileEntity cfe = txtRemoteAddress.SelectedItem as ConfigFileEntity;

                txtRemotePass.Text = cfe.RemotePass;
                txtRemoteFile.Text = cfe.RemoteFile;
                txtRemoteName.Text = cfe.RemoteName;
            }
        }

        public string GetRemoteAddress()
        {
            int i = txtRemoteAddress.Text.IndexOf("[");
            if (i > 0)
                return txtRemoteAddress.Text.Substring(0, i);

            return txtRemoteAddress.Text;
        }

        
    }
}
