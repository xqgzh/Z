using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinConfigManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FrmConfig fConfig = new FrmConfig();

            Application.Run(fConfig);
            //FrmOpen fOpen = new FrmOpen();

            //if (fOpen.ShowDialog() == DialogResult.OK)
            //{
            //    FrmConfig fConfig = new FrmConfig();
            //    fConfig.txtRemoteAddress.Text = fOpen.GetRemoteAddress();
            //    fConfig.txtRemotePass.Text = fOpen.txtRemotePass.Text;
            //    fConfig.txtRemoteName.Text = fOpen.txtRemoteName.Text;
            //    fConfig.txtRemoteFile.Text = fOpen.txtRemoteFile.Text;

            //    fConfig.RemoteConfigObj = fOpen.RemoteConfigObj;

            //    Application.Run(fConfig);
            //}
        }
    }
}
