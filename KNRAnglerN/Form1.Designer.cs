namespace KNRAnglerN
{
    partial class Form1
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
            this.btnStartVideo = new System.Windows.Forms.Button();
            this.btnConnectVideo = new System.Windows.Forms.Button();
            this.txtJsonPort = new System.Windows.Forms.TextBox();
            this.txtVideoPort = new System.Windows.Forms.TextBox();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.btnConnectJson = new System.Windows.Forms.Button();
            this.btnSendJson = new System.Windows.Forms.Button();
            this.cmbPacket = new System.Windows.Forms.ComboBox();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tmrFrameRate = new System.Windows.Forms.Timer(this.components);
            this.picDepthMap = new System.Windows.Forms.PictureBox();
            this.chkHideHUD = new System.Windows.Forms.CheckBox();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.lblFrameRate = new System.Windows.Forms.Label();
            this.chkVideoFeed = new System.Windows.Forms.CheckBox();
            this.chkManualControl = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDepthMap)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartVideo
            // 
            this.btnStartVideo.Location = new System.Drawing.Point(108, 685);
            this.btnStartVideo.Name = "btnStartVideo";
            this.btnStartVideo.Size = new System.Drawing.Size(90, 23);
            this.btnStartVideo.TabIndex = 29;
            this.btnStartVideo.Text = "Start Video";
            this.btnStartVideo.UseVisualStyleBackColor = true;
            // 
            // btnConnectVideo
            // 
            this.btnConnectVideo.Location = new System.Drawing.Point(12, 685);
            this.btnConnectVideo.Name = "btnConnectVideo";
            this.btnConnectVideo.Size = new System.Drawing.Size(90, 23);
            this.btnConnectVideo.TabIndex = 28;
            this.btnConnectVideo.Text = "Connect Video";
            this.btnConnectVideo.UseVisualStyleBackColor = true;
            // 
            // txtJsonPort
            // 
            this.txtJsonPort.Location = new System.Drawing.Point(108, 659);
            this.txtJsonPort.Name = "txtJsonPort";
            this.txtJsonPort.Size = new System.Drawing.Size(90, 20);
            this.txtJsonPort.TabIndex = 24;
            this.txtJsonPort.Text = "44210";
            // 
            // txtVideoPort
            // 
            this.txtVideoPort.Location = new System.Drawing.Point(12, 659);
            this.txtVideoPort.Name = "txtVideoPort";
            this.txtVideoPort.Size = new System.Drawing.Size(90, 20);
            this.txtVideoPort.TabIndex = 25;
            this.txtVideoPort.Text = "44209";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(12, 633);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(186, 20);
            this.txtIp.TabIndex = 26;
            this.txtIp.Text = "127.0.0.1";
            // 
            // btnConnectJson
            // 
            this.btnConnectJson.Location = new System.Drawing.Point(564, 338);
            this.btnConnectJson.Name = "btnConnectJson";
            this.btnConnectJson.Size = new System.Drawing.Size(90, 23);
            this.btnConnectJson.TabIndex = 23;
            this.btnConnectJson.Text = "Connect JSON";
            this.btnConnectJson.UseVisualStyleBackColor = true;
            this.btnConnectJson.Click += new System.EventHandler(this.btnConnectJson_Click);
            // 
            // btnSendJson
            // 
            this.btnSendJson.Location = new System.Drawing.Point(1178, 338);
            this.btnSendJson.Name = "btnSendJson";
            this.btnSendJson.Size = new System.Drawing.Size(90, 23);
            this.btnSendJson.TabIndex = 22;
            this.btnSendJson.Text = "Send JSON";
            this.btnSendJson.UseVisualStyleBackColor = true;
            this.btnSendJson.Click += new System.EventHandler(this.btnSendJson_Click);
            // 
            // cmbPacket
            // 
            this.cmbPacket.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPacket.FormattingEnabled = true;
            this.cmbPacket.Location = new System.Drawing.Point(660, 339);
            this.cmbPacket.Name = "cmbPacket";
            this.cmbPacket.Size = new System.Drawing.Size(114, 21);
            this.cmbPacket.TabIndex = 21;
            this.cmbPacket.SelectedIndexChanged += new System.EventHandler(this.cmbPacket_SelectedIndexChanged);
            // 
            // txtConsole
            // 
            this.txtConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.Location = new System.Drawing.Point(564, 12);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsole.Size = new System.Drawing.Size(704, 321);
            this.txtConsole.TabIndex = 20;
            this.txtConsole.WordWrap = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1280, 720);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // tmrFrameRate
            // 
            this.tmrFrameRate.Enabled = true;
            this.tmrFrameRate.Interval = 33;
            this.tmrFrameRate.Tick += new System.EventHandler(this.tmrFrameRate_Tick);
            // 
            // picDepthMap
            // 
            this.picDepthMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDepthMap.Location = new System.Drawing.Point(302, 12);
            this.picDepthMap.Name = "picDepthMap";
            this.picDepthMap.Size = new System.Drawing.Size(256, 144);
            this.picDepthMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDepthMap.TabIndex = 33;
            this.picDepthMap.TabStop = false;
            // 
            // chkHideHUD
            // 
            this.chkHideHUD.AutoSize = true;
            this.chkHideHUD.Checked = true;
            this.chkHideHUD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHideHUD.Location = new System.Drawing.Point(1186, 691);
            this.chkHideHUD.Name = "chkHideHUD";
            this.chkHideHUD.Size = new System.Drawing.Size(82, 17);
            this.chkHideHUD.TabIndex = 32;
            this.chkHideHUD.Text = "HUD visible";
            this.chkHideHUD.UseVisualStyleBackColor = true;
            this.chkHideHUD.CheckedChanged += new System.EventHandler(this.chkHideHUD_CheckedChanged);
            // 
            // txtJson
            // 
            this.txtJson.Location = new System.Drawing.Point(780, 340);
            this.txtJson.Name = "txtJson";
            this.txtJson.Size = new System.Drawing.Size(392, 20);
            this.txtJson.TabIndex = 31;
            // 
            // lblFrameRate
            // 
            this.lblFrameRate.AutoSize = true;
            this.lblFrameRate.Location = new System.Drawing.Point(12, 9);
            this.lblFrameRate.Name = "lblFrameRate";
            this.lblFrameRate.Size = new System.Drawing.Size(33, 13);
            this.lblFrameRate.TabIndex = 30;
            this.lblFrameRate.Text = "0FPS";
            // 
            // chkVideoFeed
            // 
            this.chkVideoFeed.AutoSize = true;
            this.chkVideoFeed.Checked = true;
            this.chkVideoFeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVideoFeed.Location = new System.Drawing.Point(1090, 691);
            this.chkVideoFeed.Name = "chkVideoFeed";
            this.chkVideoFeed.Size = new System.Drawing.Size(77, 17);
            this.chkVideoFeed.TabIndex = 32;
            this.chkVideoFeed.Text = "Video feed";
            this.chkVideoFeed.UseVisualStyleBackColor = true;
            // 
            // chkManualControl
            // 
            this.chkManualControl.AutoSize = true;
            this.chkManualControl.BackColor = System.Drawing.SystemColors.Control;
            this.chkManualControl.Checked = true;
            this.chkManualControl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkManualControl.Location = new System.Drawing.Point(1007, 691);
            this.chkManualControl.Name = "chkManualControl";
            this.chkManualControl.Size = new System.Drawing.Size(79, 17);
            this.chkManualControl.TabIndex = 32;
            this.chkManualControl.Text = "Manual Ctrl";
            this.chkManualControl.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.btnStartVideo);
            this.Controls.Add(this.btnConnectVideo);
            this.Controls.Add(this.txtJsonPort);
            this.Controls.Add(this.txtVideoPort);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.btnConnectJson);
            this.Controls.Add(this.btnSendJson);
            this.Controls.Add(this.cmbPacket);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.picDepthMap);
            this.Controls.Add(this.chkManualControl);
            this.Controls.Add(this.chkVideoFeed);
            this.Controls.Add(this.chkHideHUD);
            this.Controls.Add(this.txtJson);
            this.Controls.Add(this.lblFrameRate);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "KNR Wędkarz - Okoń Sim control v6.2 by Vectro 2020";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDepthMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartVideo;
        private System.Windows.Forms.Button btnConnectVideo;
        private System.Windows.Forms.TextBox txtJsonPort;
        private System.Windows.Forms.TextBox txtVideoPort;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Button btnConnectJson;
        private System.Windows.Forms.Button btnSendJson;
        private System.Windows.Forms.ComboBox cmbPacket;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer tmrFrameRate;
        private System.Windows.Forms.PictureBox picDepthMap;
        private System.Windows.Forms.CheckBox chkHideHUD;
        private System.Windows.Forms.TextBox txtJson;
        private System.Windows.Forms.Label lblFrameRate;
        private System.Windows.Forms.CheckBox chkVideoFeed;
        private System.Windows.Forms.CheckBox chkManualControl;
    }
}

