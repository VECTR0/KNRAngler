﻿namespace KNRAnglerN
{
    partial class MainForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tmrGetVideo = new System.Windows.Forms.Timer(this.components);
            this.picDepthMap = new System.Windows.Forms.PictureBox();
            this.lblFrameRate = new System.Windows.Forms.Label();
            this.btnShowSettings = new System.Windows.Forms.Button();
            this.btnShowConsole = new System.Windows.Forms.Button();
            this.tmrFramerate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDepthMap)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(960, 540);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // tmrGetVideo
            // 
            this.tmrGetVideo.Enabled = true;
            this.tmrGetVideo.Interval = 33;
            this.tmrGetVideo.Tick += new System.EventHandler(this.tmrFrameRate_Tick);
            // 
            // picDepthMap
            // 
            this.picDepthMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDepthMap.Location = new System.Drawing.Point(352, 3);
            this.picDepthMap.Name = "picDepthMap";
            this.picDepthMap.Size = new System.Drawing.Size(256, 144);
            this.picDepthMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDepthMap.TabIndex = 33;
            this.picDepthMap.TabStop = false;
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
            // btnShowSettings
            // 
            this.btnShowSettings.Location = new System.Drawing.Point(881, 514);
            this.btnShowSettings.Name = "btnShowSettings";
            this.btnShowSettings.Size = new System.Drawing.Size(76, 23);
            this.btnShowSettings.TabIndex = 34;
            this.btnShowSettings.Text = "Settings";
            this.btnShowSettings.UseVisualStyleBackColor = true;
            this.btnShowSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnShowConsole
            // 
            this.btnShowConsole.Location = new System.Drawing.Point(799, 514);
            this.btnShowConsole.Name = "btnShowConsole";
            this.btnShowConsole.Size = new System.Drawing.Size(76, 23);
            this.btnShowConsole.TabIndex = 34;
            this.btnShowConsole.Text = "Console Show/Hide";
            this.btnShowConsole.UseVisualStyleBackColor = true;
            this.btnShowConsole.Click += new System.EventHandler(this.btnShowConsole_Click);
            // 
            // tmrFramerate
            // 
            this.tmrFramerate.Enabled = true;
            this.tmrFramerate.Interval = 1000;
            this.tmrFramerate.Tick += new System.EventHandler(this.tmrFramerate_Tick_1);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 540);
            this.Controls.Add(this.btnShowConsole);
            this.Controls.Add(this.btnShowSettings);
            this.Controls.Add(this.picDepthMap);
            this.Controls.Add(this.lblFrameRate);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDepthMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Timer tmrGetVideo;
        public System.Windows.Forms.PictureBox picDepthMap;
        public System.Windows.Forms.Label lblFrameRate;
        public System.Windows.Forms.Button btnShowSettings;
        public System.Windows.Forms.Button btnShowConsole;
        private System.Windows.Forms.Timer tmrFramerate;
    }
}
