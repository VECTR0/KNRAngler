namespace KNRAnglerN
{
    partial class ConsoleForm
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
            this.btnSendJson = new System.Windows.Forms.Button();
            this.cmbPacket = new System.Windows.Forms.ComboBox();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSendJson
            // 
            this.btnSendJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendJson.Location = new System.Drawing.Point(547, 225);
            this.btnSendJson.Name = "btnSendJson";
            this.btnSendJson.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSendJson.Size = new System.Drawing.Size(90, 23);
            this.btnSendJson.TabIndex = 34;
            this.btnSendJson.Text = "Send";
            this.btnSendJson.UseVisualStyleBackColor = true;
            this.btnSendJson.Click += new System.EventHandler(this.btnSendJson_Click);
            // 
            // cmbPacket
            // 
            this.cmbPacket.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbPacket.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPacket.FormattingEnabled = true;
            this.cmbPacket.Location = new System.Drawing.Point(0, 225);
            this.cmbPacket.Name = "cmbPacket";
            this.cmbPacket.Size = new System.Drawing.Size(114, 21);
            this.cmbPacket.TabIndex = 33;
            this.cmbPacket.SelectedIndexChanged += new System.EventHandler(this.cmbPacket_SelectedIndexChanged);
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.Location = new System.Drawing.Point(0, 0);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsole.Size = new System.Drawing.Size(637, 222);
            this.txtConsole.TabIndex = 32;
            this.txtConsole.WordWrap = false;
            // 
            // txtJson
            // 
            this.txtJson.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJson.Location = new System.Drawing.Point(120, 226);
            this.txtJson.Name = "txtJson";
            this.txtJson.Size = new System.Drawing.Size(421, 20);
            this.txtJson.TabIndex = 36;
            this.txtJson.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtJson_KeyPress);
            // 
            // ConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 250);
            this.Controls.Add(this.btnSendJson);
            this.Controls.Add(this.cmbPacket);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.txtJson);
            this.Name = "ConsoleForm";
            this.Text = "Console";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
            this.Load += new System.EventHandler(this.ConsoleForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Button btnSendJson;
        public System.Windows.Forms.ComboBox cmbPacket;
        public System.Windows.Forms.TextBox txtConsole;
        public System.Windows.Forms.TextBox txtJson;
    }
}