using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WinConfigManager
{
    public partial class FrmServerVersion : Form
    {
        public FrmServerVersion(string xml)
        {
            InitializeComponent();

            InitGrid(xml);
        }

        void InitGrid(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodes = doc.SelectNodes("//File");

            foreach (XmlNode node in nodes)
            {
                int rowIndex = dataGridView1.Rows.Add();
                DataGridViewRow row = dataGridView1.Rows[rowIndex];

                row.Cells["cFileName"].Value = node.Attributes["FileName"].Value;
                row.Cells["cFileVersion"].Value = node.Attributes["Version"].Value;
                row.Cells["cCreateTime"].Value = node.Attributes["CreateTime"].Value;
                row.Cells["cModifyTime"].Value = node.Attributes["LastWriteTime"].Value;
            }
        }

        private void FrmServerVersion_Load(object sender, EventArgs e)
        {

        }
    }
}
