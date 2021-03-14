namespace KNRAnglerN
{
    partial class CloudForm
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
            this.picRender = new System.Windows.Forms.PictureBox();
            this.tmrRender = new System.Windows.Forms.Timer(this.components);
            this.trbTranslateY = new System.Windows.Forms.TrackBar();
            this.trbTranslateX = new System.Windows.Forms.TrackBar();
            this.trbRotationX = new System.Windows.Forms.TrackBar();
            this.trbTranslateZ = new System.Windows.Forms.TrackBar();
            this.trbRotationY = new System.Windows.Forms.TrackBar();
            this.lblNum = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.trbScale = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.picRender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbTranslateY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbTranslateX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbRotationX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbTranslateZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbRotationY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbScale)).BeginInit();
            this.SuspendLayout();
            // 
            // picRender
            // 
            this.picRender.Location = new System.Drawing.Point(12, 12);
            this.picRender.Name = "picRender";
            this.picRender.Size = new System.Drawing.Size(776, 426);
            this.picRender.TabIndex = 0;
            this.picRender.TabStop = false;
            // 
            // tmrRender
            // 
            this.tmrRender.Enabled = true;
            this.tmrRender.Interval = 50;
            this.tmrRender.Tick += new System.EventHandler(this.tmrRender_Tick);
            // 
            // trbTranslateY
            // 
            this.trbTranslateY.Location = new System.Drawing.Point(12, 409);
            this.trbTranslateY.Maximum = 400;
            this.trbTranslateY.Minimum = -400;
            this.trbTranslateY.Name = "trbTranslateY";
            this.trbTranslateY.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trbTranslateY.Size = new System.Drawing.Size(45, 155);
            this.trbTranslateY.TabIndex = 1;
            this.trbTranslateY.Value = -100;
            // 
            // trbTranslateX
            // 
            this.trbTranslateX.LargeChange = 30;
            this.trbTranslateX.Location = new System.Drawing.Point(81, 444);
            this.trbTranslateX.Maximum = 400;
            this.trbTranslateX.Minimum = -400;
            this.trbTranslateX.Name = "trbTranslateX";
            this.trbTranslateX.Size = new System.Drawing.Size(102, 45);
            this.trbTranslateX.SmallChange = 10;
            this.trbTranslateX.TabIndex = 2;
            this.trbTranslateX.Value = -100;
            // 
            // trbRotationX
            // 
            this.trbRotationX.Location = new System.Drawing.Point(794, 12);
            this.trbRotationX.Maximum = 360;
            this.trbRotationX.Name = "trbRotationX";
            this.trbRotationX.Size = new System.Drawing.Size(104, 45);
            this.trbRotationX.TabIndex = 3;
            // 
            // trbTranslateZ
            // 
            this.trbTranslateZ.LargeChange = 30;
            this.trbTranslateZ.Location = new System.Drawing.Point(189, 444);
            this.trbTranslateZ.Maximum = 100;
            this.trbTranslateZ.Minimum = -100;
            this.trbTranslateZ.Name = "trbTranslateZ";
            this.trbTranslateZ.Size = new System.Drawing.Size(102, 45);
            this.trbTranslateZ.SmallChange = 10;
            this.trbTranslateZ.TabIndex = 2;
            // 
            // trbRotationY
            // 
            this.trbRotationY.Location = new System.Drawing.Point(794, 63);
            this.trbRotationY.Maximum = 360;
            this.trbRotationY.Name = "trbRotationY";
            this.trbRotationY.Size = new System.Drawing.Size(104, 45);
            this.trbRotationY.TabIndex = 3;
            // 
            // lblNum
            // 
            this.lblNum.AutoSize = true;
            this.lblNum.Location = new System.Drawing.Point(12, 12);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(35, 13);
            this.lblNum.TabIndex = 4;
            this.lblNum.Text = "label1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(130, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // trbScale
            // 
            this.trbScale.LargeChange = 30;
            this.trbScale.Location = new System.Drawing.Point(482, 2);
            this.trbScale.Maximum = 100;
            this.trbScale.Minimum = 1;
            this.trbScale.Name = "trbScale";
            this.trbScale.Size = new System.Drawing.Size(200, 45);
            this.trbScale.SmallChange = 10;
            this.trbScale.TabIndex = 2;
            this.trbScale.Value = 50;
            // 
            // CloudForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 563);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblNum);
            this.Controls.Add(this.trbRotationY);
            this.Controls.Add(this.trbRotationX);
            this.Controls.Add(this.trbScale);
            this.Controls.Add(this.trbTranslateZ);
            this.Controls.Add(this.trbTranslateX);
            this.Controls.Add(this.trbTranslateY);
            this.Controls.Add(this.picRender);
            this.Name = "CloudForm";
            this.Text = "CloudForm";
            this.Load += new System.EventHandler(this.CloudForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picRender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbTranslateY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbTranslateX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbRotationX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbTranslateZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbRotationY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picRender;
        private System.Windows.Forms.Timer tmrRender;
        private System.Windows.Forms.TrackBar trbTranslateY;
        private System.Windows.Forms.TrackBar trbTranslateX;
        private System.Windows.Forms.TrackBar trbRotationX;
        private System.Windows.Forms.TrackBar trbTranslateZ;
        private System.Windows.Forms.TrackBar trbRotationY;
        private System.Windows.Forms.Label lblNum;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TrackBar trbScale;
    }
}