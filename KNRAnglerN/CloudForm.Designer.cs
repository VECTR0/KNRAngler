using System.ComponentModel;
using System.Windows.Forms;

namespace KNRAnglerN
{
    partial class CloudForm
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
            this.components = new System.ComponentModel.Container();
            this.picRender = new System.Windows.Forms.PictureBox();
            this.tmrRender = new System.Windows.Forms.Timer(this.components);
            this.trbTranslateY = new System.Windows.Forms.TrackBar();
            this.trbTranslateX = new System.Windows.Forms.TrackBar();
            this.trbRotationX = new System.Windows.Forms.TrackBar();
            this.trbTranslateZ = new System.Windows.Forms.TrackBar();
            this.trbRotationY = new System.Windows.Forms.TrackBar();
            this.lblNum = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.trbScale = new System.Windows.Forms.TrackBar();
            this.tmrGenerate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize) (this.picRender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbTranslateY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbTranslateX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbRotationX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbTranslateZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbRotationY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbScale)).BeginInit();
            this.SuspendLayout();
            // 
            // picRender
            // 
            this.picRender.Location = new System.Drawing.Point(0, 0);
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
            this.trbTranslateY.Location = new System.Drawing.Point(12, 432);
            this.trbTranslateY.Maximum = 500;
            this.trbTranslateY.Minimum = -500;
            this.trbTranslateY.Name = "trbTranslateY";
            this.trbTranslateY.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trbTranslateY.Size = new System.Drawing.Size(45, 132);
            this.trbTranslateY.TabIndex = 1;
            this.trbTranslateY.Value = -100;
            // 
            // trbTranslateX
            // 
            this.trbTranslateX.LargeChange = 30;
            this.trbTranslateX.Location = new System.Drawing.Point(63, 442);
            this.trbTranslateX.Maximum = 500;
            this.trbTranslateX.Minimum = -500;
            this.trbTranslateX.Name = "trbTranslateX";
            this.trbTranslateX.Size = new System.Drawing.Size(102, 45);
            this.trbTranslateX.SmallChange = 10;
            this.trbTranslateX.TabIndex = 2;
            this.trbTranslateX.Value = -100;
            // 
            // trbRotationX
            // 
            this.trbRotationX.Location = new System.Drawing.Point(672, 432);
            this.trbRotationX.Maximum = 360;
            this.trbRotationX.Name = "trbRotationX";
            this.trbRotationX.Size = new System.Drawing.Size(104, 45);
            this.trbRotationX.TabIndex = 3;
            // 
            // trbTranslateZ
            // 
            this.trbTranslateZ.LargeChange = 30;
            this.trbTranslateZ.Location = new System.Drawing.Point(63, 519);
            this.trbTranslateZ.Maximum = 100;
            this.trbTranslateZ.Minimum = -100;
            this.trbTranslateZ.Name = "trbTranslateZ";
            this.trbTranslateZ.Size = new System.Drawing.Size(102, 45);
            this.trbTranslateZ.SmallChange = 10;
            this.trbTranslateZ.TabIndex = 2;
            // 
            // trbRotationY
            // 
            this.trbRotationY.Location = new System.Drawing.Point(672, 519);
            this.trbRotationY.Maximum = 360;
            this.trbRotationY.Name = "trbRotationY";
            this.trbRotationY.Size = new System.Drawing.Size(104, 45);
            this.trbRotationY.TabIndex = 3;
            // 
            // lblNum
            // 
            this.lblNum.AutoSize = true;
            this.lblNum.Location = new System.Drawing.Point(75, 483);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(59, 13);
            this.lblNum.TabIndex = 4;
            this.lblNum.Text = "Translation";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(171, 432);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 5;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.button1_Click);
            // 
            // trbScale
            // 
            this.trbScale.LargeChange = 30;
            this.trbScale.Location = new System.Drawing.Point(171, 468);
            this.trbScale.Maximum = 1000;
            this.trbScale.Minimum = 1;
            this.trbScale.Name = "trbScale";
            this.trbScale.Size = new System.Drawing.Size(495, 45);
            this.trbScale.SmallChange = 10;
            this.trbScale.TabIndex = 2;
            this.trbScale.Value = 50;
            // 
            // tmrGenerate
            // 
            this.tmrGenerate.Interval = 50;
            this.tmrGenerate.Tick += new System.EventHandler(this.tmrGenerate_Tick);
            // 
            // CloudForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 563);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.lblNum);
            this.Controls.Add(this.trbRotationY);
            this.Controls.Add(this.trbRotationX);
            this.Controls.Add(this.trbScale);
            this.Controls.Add(this.trbTranslateZ);
            this.Controls.Add(this.trbTranslateX);
            this.Controls.Add(this.trbTranslateY);
            this.Controls.Add(this.picRender);
            this.Name = "CloudForm";
            this.Text = "Cloud viewer";
            this.Load += new System.EventHandler(this.CloudForm_Load);
            ((System.ComponentModel.ISupportInitialize) (this.picRender)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbTranslateY)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbTranslateX)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbRotationX)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbTranslateZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbRotationY)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.trbScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox picRender;
        private System.Windows.Forms.Timer tmrRender;
        private TrackBar trbTranslateY;
        private TrackBar trbTranslateX;
        private TrackBar trbRotationX;
        private TrackBar trbTranslateZ;
        private TrackBar trbRotationY;
        private Label lblNum;
        private System.Windows.Forms.Button btnGenerate;
        private TrackBar trbScale;
        private System.Windows.Forms.Timer tmrGenerate;
    }
}