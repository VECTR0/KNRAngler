using System.ComponentModel;
using System.Windows.Forms;

namespace KNRAnglerN
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.btnStartVideo = new System.Windows.Forms.Button();
            this.btnConnectVideo = new System.Windows.Forms.Button();
            this.txtJsonPort = new System.Windows.Forms.TextBox();
            this.txtVideoPort = new System.Windows.Forms.TextBox();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.chkYeetLog = new System.Windows.Forms.CheckBox();
            this.chkManualControl = new System.Windows.Forms.CheckBox();
            this.chkVideoFeed = new System.Windows.Forms.CheckBox();
            this.btnConnectJson = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDepth = new System.Windows.Forms.CheckBox();
            this.pnlSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartVideo
            // 
            this.btnStartVideo.Enabled = false;
            this.btnStartVideo.Location = new System.Drawing.Point(269, 31);
            this.btnStartVideo.Name = "btnStartVideo";
            this.btnStartVideo.Size = new System.Drawing.Size(90, 23);
            this.btnStartVideo.TabIndex = 34;
            this.btnStartVideo.Text = "Start Video";
            this.btnStartVideo.UseVisualStyleBackColor = true;
            this.btnStartVideo.Click += new System.EventHandler(this.btnStartVideo_Click);
            // 
            // btnConnectVideo
            // 
            this.btnConnectVideo.Enabled = false;
            this.btnConnectVideo.Location = new System.Drawing.Point(173, 31);
            this.btnConnectVideo.Name = "btnConnectVideo";
            this.btnConnectVideo.Size = new System.Drawing.Size(90, 23);
            this.btnConnectVideo.TabIndex = 33;
            this.btnConnectVideo.Text = "Connect Video";
            this.btnConnectVideo.UseVisualStyleBackColor = true;
            // 
            // txtJsonPort
            // 
            this.txtJsonPort.Location = new System.Drawing.Point(77, 59);
            this.txtJsonPort.Name = "txtJsonPort";
            this.txtJsonPort.Size = new System.Drawing.Size(90, 20);
            this.txtJsonPort.TabIndex = 30;
            this.txtJsonPort.Text = "44210";
            // 
            // txtVideoPort
            // 
            this.txtVideoPort.Enabled = false;
            this.txtVideoPort.Location = new System.Drawing.Point(77, 33);
            this.txtVideoPort.Name = "txtVideoPort";
            this.txtVideoPort.Size = new System.Drawing.Size(90, 20);
            this.txtVideoPort.TabIndex = 31;
            this.txtVideoPort.Text = "44209";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(77, 7);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(186, 20);
            this.txtIp.TabIndex = 32;
            this.txtIp.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Simulation IP";
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.chkDepth);
            this.pnlSettings.Controls.Add(this.chkYeetLog);
            this.pnlSettings.Controls.Add(this.chkManualControl);
            this.pnlSettings.Controls.Add(this.chkVideoFeed);
            this.pnlSettings.Controls.Add(this.btnConnectJson);
            this.pnlSettings.Controls.Add(this.txtVideoPort);
            this.pnlSettings.Controls.Add(this.label3);
            this.pnlSettings.Controls.Add(this.label2);
            this.pnlSettings.Controls.Add(this.label1);
            this.pnlSettings.Controls.Add(this.txtIp);
            this.pnlSettings.Controls.Add(this.txtJsonPort);
            this.pnlSettings.Controls.Add(this.btnStartVideo);
            this.pnlSettings.Controls.Add(this.btnConnectVideo);
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(365, 174);
            this.pnlSettings.TabIndex = 36;
            // 
            // chkYeetLog
            // 
            this.chkYeetLog.AutoSize = true;
            this.chkYeetLog.BackColor = System.Drawing.SystemColors.Control;
            this.chkYeetLog.Location = new System.Drawing.Point(7, 131);
            this.chkYeetLog.Name = "chkYeetLog";
            this.chkYeetLog.Size = new System.Drawing.Size(177, 17);
            this.chkYeetLog.TabIndex = 38;
            this.chkYeetLog.Text = "Yeet networking logs to console";
            this.chkYeetLog.UseVisualStyleBackColor = false;
            // 
            // chkManualControl
            // 
            this.chkManualControl.AutoSize = true;
            this.chkManualControl.BackColor = System.Drawing.SystemColors.Control;
            this.chkManualControl.Checked = true;
            this.chkManualControl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkManualControl.Location = new System.Drawing.Point(7, 108);
            this.chkManualControl.Name = "chkManualControl";
            this.chkManualControl.Size = new System.Drawing.Size(141, 17);
            this.chkManualControl.TabIndex = 38;
            this.chkManualControl.Text = "Enable keyboard control";
            this.chkManualControl.UseVisualStyleBackColor = false;
            // 
            // chkVideoFeed
            // 
            this.chkVideoFeed.AutoSize = true;
            this.chkVideoFeed.Checked = true;
            this.chkVideoFeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVideoFeed.Location = new System.Drawing.Point(7, 85);
            this.chkVideoFeed.Name = "chkVideoFeed";
            this.chkVideoFeed.Size = new System.Drawing.Size(112, 17);
            this.chkVideoFeed.TabIndex = 39;
            this.chkVideoFeed.Text = "Enable video feed";
            this.chkVideoFeed.UseVisualStyleBackColor = true;
            // 
            // btnConnectJson
            // 
            this.btnConnectJson.Location = new System.Drawing.Point(173, 57);
            this.btnConnectJson.Name = "btnConnectJson";
            this.btnConnectJson.Size = new System.Drawing.Size(90, 23);
            this.btnConnectJson.TabIndex = 37;
            this.btnConnectJson.Text = "Connect WAPI";
            this.btnConnectJson.UseVisualStyleBackColor = true;
            this.btnConnectJson.Click += new System.EventHandler(this.btnConnectJson_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "WAPI Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Video Port";
            // 
            // chkDepth
            // 
            this.chkDepth.AutoSize = true;
            this.chkDepth.BackColor = System.Drawing.SystemColors.Control;
            this.chkDepth.Location = new System.Drawing.Point(6, 154);
            this.chkDepth.Name = "chkDepth";
            this.chkDepth.Size = new System.Drawing.Size(106, 17);
            this.chkDepth.TabIndex = 38;
            this.chkDepth.Text = "Show depth map";
            this.chkDepth.UseVisualStyleBackColor = false;
            this.chkDepth.CheckedChanged += new System.EventHandler(this.chkDepth_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 175);
            this.Controls.Add(this.pnlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public Button btnStartVideo;
        public Button btnConnectVideo;
        public TextBox txtJsonPort;
        public TextBox txtVideoPort;
        public TextBox txtIp;
        private Label label1;
        private Panel pnlSettings;
        private Label label3;
        private Label label2;
        public Button btnConnectJson;
        public CheckBox chkManualControl;
        public CheckBox chkVideoFeed;
        public CheckBox chkYeetLog;
        public CheckBox chkDepth;
    }
}