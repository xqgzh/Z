namespace WinConfigManager
{
    partial class FrmConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConfig));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.trConfigs = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolOpenConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.删除此节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.修改节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.grpBox = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtConfigXml = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnEditStatus = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnSaveRemote = new System.Windows.Forms.ToolStripButton();
            this.btnViewServerVersions = new System.Windows.Forms.ToolStripButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtRemoteFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRemoteName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRemotePass = new System.Windows.Forms.TextBox();
            this.txtRemoteAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.添加新应用ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.grpBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.trConfigs);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grpBox);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(972, 417);
            this.splitContainer1.SplitterDistance = 229;
            this.splitContainer1.TabIndex = 0;
            // 
            // trConfigs
            // 
            this.trConfigs.ContextMenuStrip = this.contextMenuStrip1;
            this.trConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trConfigs.FullRowSelect = true;
            this.trConfigs.HideSelection = false;
            this.trConfigs.HotTracking = true;
            this.trConfigs.ImageIndex = 2;
            this.trConfigs.ImageList = this.imageList1;
            this.trConfigs.LabelEdit = true;
            this.trConfigs.Location = new System.Drawing.Point(0, 25);
            this.trConfigs.Name = "trConfigs";
            this.trConfigs.SelectedImageIndex = 2;
            this.trConfigs.ShowNodeToolTips = true;
            this.trConfigs.Size = new System.Drawing.Size(229, 392);
            this.trConfigs.TabIndex = 1;
            this.trConfigs.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trConfigs_NodeMouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolOpenConfig,
            this.删除此节点ToolStripMenuItem,
            this.修改节点ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(131, 70);
            // 
            // toolOpenConfig
            // 
            this.toolOpenConfig.Name = "toolOpenConfig";
            this.toolOpenConfig.Size = new System.Drawing.Size(130, 22);
            this.toolOpenConfig.Text = "打开";
            this.toolOpenConfig.Click += new System.EventHandler(this.toolOpenConfig_Click);
            // 
            // 删除此节点ToolStripMenuItem
            // 
            this.删除此节点ToolStripMenuItem.Name = "删除此节点ToolStripMenuItem";
            this.删除此节点ToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.删除此节点ToolStripMenuItem.Text = "删除此配置";
            // 
            // 修改节点ToolStripMenuItem
            // 
            this.修改节点ToolStripMenuItem.Name = "修改节点ToolStripMenuItem";
            this.修改节点ToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.修改节点ToolStripMenuItem.Text = "修改配置";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Folder.ico");
            this.imageList1.Images.SetKeyName(1, "openHS.png");
            this.imageList1.Images.SetKeyName(2, "Computer.ico");
            // 
            // toolStrip2
            // 
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(229, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // grpBox
            // 
            this.grpBox.Controls.Add(this.groupBox2);
            this.grpBox.Controls.Add(this.toolStrip1);
            this.grpBox.Controls.Add(this.groupBox3);
            this.grpBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBox.Location = new System.Drawing.Point(0, 0);
            this.grpBox.Name = "grpBox";
            this.grpBox.Size = new System.Drawing.Size(739, 395);
            this.grpBox.TabIndex = 9;
            this.grpBox.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtConfigXml);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 133);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(733, 259);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            // 
            // txtConfigXml
            // 
            this.txtConfigXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConfigXml.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtConfigXml.Location = new System.Drawing.Point(3, 17);
            this.txtConfigXml.Name = "txtConfigXml";
            this.txtConfigXml.ReadOnly = true;
            this.txtConfigXml.Size = new System.Drawing.Size(727, 239);
            this.txtConfigXml.TabIndex = 0;
            this.txtConfigXml.Text = "";
            this.txtConfigXml.WordWrap = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnEditStatus,
            this.btnSave,
            this.toolStripSeparator1,
            this.btnRefresh,
            this.btnSaveRemote,
            this.btnViewServerVersions});
            this.toolStrip1.Location = new System.Drawing.Point(3, 108);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(733, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnEditStatus
            // 
            this.btnEditStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditStatus.Image = ((System.Drawing.Image)(resources.GetObject("btnEditStatus.Image")));
            this.btnEditStatus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditStatus.Name = "btnEditStatus";
            this.btnEditStatus.Size = new System.Drawing.Size(33, 22);
            this.btnEditStatus.Text = "编辑";
            this.btnEditStatus.Click += new System.EventHandler(this.btnEditStatus_Click);
            // 
            // btnSave
            // 
            this.btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 22);
            this.btnSave.Text = "保存到本地";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(49, 22);
            this.btnRefresh.Text = "刷新";
            // 
            // btnSaveRemote
            // 
            this.btnSaveRemote.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSaveRemote.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveRemote.Image")));
            this.btnSaveRemote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveRemote.Name = "btnSaveRemote";
            this.btnSaveRemote.Size = new System.Drawing.Size(85, 22);
            this.btnSaveRemote.Text = "保存到远程";
            // 
            // btnViewServerVersions
            // 
            this.btnViewServerVersions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnViewServerVersions.Image = ((System.Drawing.Image)(resources.GetObject("btnViewServerVersions.Image")));
            this.btnViewServerVersions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnViewServerVersions.Name = "btnViewServerVersions";
            this.btnViewServerVersions.Size = new System.Drawing.Size(133, 22);
            this.btnViewServerVersions.Text = "查看远程服务器版本";
            this.btnViewServerVersions.Click += new System.EventHandler(this.btnViewServerVersions_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtRemoteFile);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtRemoteName);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtRemotePass);
            this.groupBox3.Controls.Add(this.txtRemoteAddress);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(3, 17);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(733, 91);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            // 
            // txtRemoteFile
            // 
            this.txtRemoteFile.Location = new System.Drawing.Point(566, 55);
            this.txtRemoteFile.Name = "txtRemoteFile";
            this.txtRemoteFile.Size = new System.Drawing.Size(100, 21);
            this.txtRemoteFile.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(471, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "配置文件名称: ";
            // 
            // txtRemoteName
            // 
            this.txtRemoteName.Location = new System.Drawing.Point(324, 55);
            this.txtRemoteName.Name = "txtRemoteName";
            this.txtRemoteName.Size = new System.Drawing.Size(100, 21);
            this.txtRemoteName.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(257, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "配置名称: ";
            // 
            // txtRemotePass
            // 
            this.txtRemotePass.Location = new System.Drawing.Point(113, 55);
            this.txtRemotePass.Name = "txtRemotePass";
            this.txtRemotePass.Size = new System.Drawing.Size(100, 21);
            this.txtRemotePass.TabIndex = 2;
            // 
            // txtRemoteAddress
            // 
            this.txtRemoteAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemoteAddress.Location = new System.Drawing.Point(113, 18);
            this.txtRemoteAddress.Name = "txtRemoteAddress";
            this.txtRemoteAddress.Size = new System.Drawing.Size(590, 21);
            this.txtRemoteAddress.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "远程配置密码: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "远程配置地址: ";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 395);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(739, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "*.xml|*.xml";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加新应用ToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(131, 26);
            // 
            // 添加新应用ToolStripMenuItem
            // 
            this.添加新应用ToolStripMenuItem.Name = "添加新应用ToolStripMenuItem";
            this.添加新应用ToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.添加新应用ToolStripMenuItem.Text = "添加新应用";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // FrmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 417);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配置文件管理器";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmConfig_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.grpBox.ResumeLayout(false);
            this.grpBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TreeView trConfigs;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.GroupBox grpBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox txtConfigXml;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnSaveRemote;
        private System.Windows.Forms.ToolStripButton btnViewServerVersions;
        private System.Windows.Forms.GroupBox groupBox3;
        internal System.Windows.Forms.TextBox txtRemoteFile;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.TextBox txtRemoteName;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.TextBox txtRemotePass;
        internal System.Windows.Forms.TextBox txtRemoteAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除此节点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 修改节点ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 添加新应用ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolOpenConfig;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripButton btnEditStatus;

    }
}

