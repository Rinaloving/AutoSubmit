namespace AutoSubmit
{
    partial class AutoTask
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoTask));
            this.menuStrip1 = new CCWin.SkinControl.SkinMenuStrip();
            this.tmiStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiStop = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiClose = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBox1 = new CCWin.SkinControl.RtfRichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Arrow = System.Drawing.Color.Black;
            this.menuStrip1.Back = System.Drawing.Color.White;
            this.menuStrip1.BackRadius = 4;
            this.menuStrip1.BackRectangle = new System.Drawing.Rectangle(10, 10, 10, 10);
            this.menuStrip1.Base = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(200)))), ((int)(((byte)(254)))));
            this.menuStrip1.BaseFore = System.Drawing.Color.Black;
            this.menuStrip1.BaseForeAnamorphosis = false;
            this.menuStrip1.BaseForeAnamorphosisBorder = 4;
            this.menuStrip1.BaseForeAnamorphosisColor = System.Drawing.Color.White;
            this.menuStrip1.BaseHoverFore = System.Drawing.Color.White;
            this.menuStrip1.BaseItemAnamorphosis = true;
            this.menuStrip1.BaseItemBorder = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(148)))), ((int)(((byte)(212)))));
            this.menuStrip1.BaseItemBorderShow = true;
            this.menuStrip1.BaseItemDown = ((System.Drawing.Image)(resources.GetObject("menuStrip1.BaseItemDown")));
            this.menuStrip1.BaseItemHover = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(148)))), ((int)(((byte)(212)))));
            this.menuStrip1.BaseItemMouse = ((System.Drawing.Image)(resources.GetObject("menuStrip1.BaseItemMouse")));
            this.menuStrip1.BaseItemPressed = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(148)))), ((int)(((byte)(212)))));
            this.menuStrip1.BaseItemRadius = 4;
            this.menuStrip1.BaseItemRadiusStyle = CCWin.SkinClass.RoundStyle.All;
            this.menuStrip1.BaseItemSplitter = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(148)))), ((int)(((byte)(212)))));
            this.menuStrip1.DropDownImageSeparator = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.menuStrip1.Fore = System.Drawing.Color.Black;
            this.menuStrip1.HoverFore = System.Drawing.Color.White;
            this.menuStrip1.ItemAnamorphosis = true;
            this.menuStrip1.ItemBorder = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(148)))), ((int)(((byte)(212)))));
            this.menuStrip1.ItemBorderShow = true;
            this.menuStrip1.ItemHover = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(148)))), ((int)(((byte)(212)))));
            this.menuStrip1.ItemPressed = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(148)))), ((int)(((byte)(212)))));
            this.menuStrip1.ItemRadius = 4;
            this.menuStrip1.ItemRadiusStyle = CCWin.SkinClass.RoundStyle.All;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiStart,
            this.tmiStop,
            this.tmiClose});
            this.menuStrip1.Location = new System.Drawing.Point(4, 28);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RadiusStyle = CCWin.SkinClass.RoundStyle.All;
            this.menuStrip1.Size = new System.Drawing.Size(722, 24);
            this.menuStrip1.SkinAllColor = true;
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.TitleAnamorphosis = true;
            this.menuStrip1.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(236)))));
            this.menuStrip1.TitleRadius = 4;
            this.menuStrip1.TitleRadiusStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // tmiStart
            // 
            this.tmiStart.Image = ((System.Drawing.Image)(resources.GetObject("tmiStart.Image")));
            this.tmiStart.Name = "tmiStart";
            this.tmiStart.Size = new System.Drawing.Size(57, 20);
            this.tmiStart.Text = "启动";
            this.tmiStart.Click += new System.EventHandler(this.tmiStart_Click);
            // 
            // tmiStop
            // 
            this.tmiStop.Image = ((System.Drawing.Image)(resources.GetObject("tmiStop.Image")));
            this.tmiStop.Name = "tmiStop";
            this.tmiStop.Size = new System.Drawing.Size(57, 20);
            this.tmiStop.Text = "停止";
            this.tmiStop.Click += new System.EventHandler(this.tmiStop_Click);
            // 
            // tmiClose
            // 
            this.tmiClose.Image = ((System.Drawing.Image)(resources.GetObject("tmiClose.Image")));
            this.tmiClose.Name = "tmiClose";
            this.tmiClose.Size = new System.Drawing.Size(57, 20);
            this.tmiClose.Text = "退出";
            this.tmiClose.Click += new System.EventHandler(this.tmiClose_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.HiglightColor = CCWin.SkinControl.RtfRichTextBox.RtfColor.White;
            this.richTextBox1.Location = new System.Drawing.Point(8, 57);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(715, 253);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextColor = CCWin.SkinControl.RtfRichTextBox.RtfColor.Black;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "4县报文自动上报程序";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // AutoTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 317);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AutoTask";
            this.Text = "四县报文自动生成工具";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinMenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tmiStart;
        private System.Windows.Forms.ToolStripMenuItem tmiStop;
        private System.Windows.Forms.ToolStripMenuItem tmiClose;
        private CCWin.SkinControl.RtfRichTextBox richTextBox1;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

