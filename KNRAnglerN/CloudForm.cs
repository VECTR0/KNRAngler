using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace KNRAnglerN
{
    public partial class CloudForm : Form
    {
        MainForm instance;
        float ox = 200, oy = 200;
        float sx = 20, sy = 20;
        float rx = 0, ry = 0;
        readonly double right = 1, left = 0, top = 1, bottom = 0, far = 1, near = 0;
        private PointCloud cloud;
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (instance.picDepthMap.Image != null && instance.pictureBox1.Image != null)
            {
                var d = new Bitmap(instance.picDepthMap.Image);
                var c = new Bitmap(instance.pictureBox1.Image);
                
                if(c.Width == d.Width && c.Width != null && c.Width > 0)cloud.Calculate(d, c, 91.49284f, 60, 2, 20);
                
                d.Dispose();
                c.Dispose();
            }
            tmrGenerate.Enabled = true;
        }

        private void tmrGenerate_Tick(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        public CloudForm(MainForm mainForm)
        {
            InitializeComponent();
            instance = mainForm;
            DoubleBuffered = true;
        }

        private void CloudForm_Load(object sender, EventArgs e)
        {
            cloud = new PointCloud();
        }

        private void tmrRender_Tick(object sender, EventArgs e)
        {
            if (cloud == null || !cloud.initializedCalulation) return;
            Stopwatch st = new Stopwatch();
            st.Start();
            Bitmap bit = cloud.Render(
                picRender.Width,
                picRender.Height,
                trbRotationX.Value / 180f * (float)Math.PI,
                trbRotationY.Value / 180f * (float)Math.PI,
                trbTranslateX.Value,
                trbTranslateY.Value,
                trbScale.Value / 3f,
                trbScale.Value / 3f,
                false);
            Text = st.ElapsedMilliseconds.ToString();
            picRender.Invoke(new Action(() =>
             {
                 Image img = picRender.Image;
                 picRender.Image = bit;
                 if (img != null) img.Dispose();
             }));
        }
    }
}
