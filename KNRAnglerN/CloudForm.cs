using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        Matrix<double> mp, mx, my;
        byte[] pixels;
        float[,] zbuf;
        float[] zb;
        List<float[]> points = new List<float[]>();
        List<float[]> pointsTrans = new List<float[]>();
        List<float[]> pointsScreen = new List<float[]>();
        List<byte[]> pointsRGB = new List<byte[]>();
        int pointsCount;

        private void button1_Click(object sender, EventArgs e)
        {
            if (instance.picDepthMap.Image != null && instance.pictureBox1.Image != null)
            {
                Bitmap d = new Bitmap(instance.picDepthMap.Image);
                Bitmap c = new Bitmap(instance.pictureBox1.Image);
                cloud.GenerateFromDepthMap(d, c, 91.49284f, 60, 2, 20);
            }
            /* Bitmap b = new Bitmap("d.png");
             Bitmap bb = new Bitmap("dc.png");
             cloud.GenerateFromDepthMap(b, bb, 90f, 60, 2, 20);*/
            tmrGenerate.Enabled = true;
        }

        private void tmrGenerate_Tick(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        public CloudForm(MainForm mainForm)
        {
            InitializeComponent();
            this.instance = mainForm;
            DoubleBuffered = true;
        }

        public void ClearPoints() { }
        public void AddPoints(float[][] points) { }

        void AddPoint(float x, float y, float z, int r, int g, int b)
        {
            if (x*x+y*y+z*z < 12) return;
            points.Add(new float[] { x, y, z });
            pointsRGB.Add(new byte[] { (byte)r, (byte)g, (byte)b });
            pointsTrans.Add(new float[] { 0, 0, 0 });
            pointsScreen.Add(new float[] { 0, 0, 0 });
        }

        private void CloudForm_Load(object sender, EventArgs e)
        {
            Random r = new Random(2);
            for (int i = 0; i < Math.Pow(10, 6.5); i++)
            {
              //  AddPoint(r.Next(-10, 11) / 8f, r.Next(-10, 11) / 8f, r.Next(-10, 11) / 8f, 255, 255, 255);
                AddPoint((float)r.NextDouble()*10, (float)r.NextDouble() * 10, (float)r.NextDouble() * 10, r.Next(10,200), r.Next(10, 200), r.Next(10, 200));
            }


            for (int i = 0; i < 200; i++)
            {
                AddPoint(0, 0, i / 50f, 0, 0, 255);
                AddPoint(0, i / 50f, 0, 0, 255, 0);
                AddPoint(i / 50f, 0, 0, 255, 0 ,0);
            }
            /*
            int n = 400;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double de = j * Math.PI * 2 / n;
                    double ro = i * Math.PI * 2 / n;
                    float x = (float)( Math.Cos(de) * Math.Sin(ro));
                    float y = 1+(float)( Math.Sin(de) * Math.Sin(ro));
                    float z = (float)(  Math.Cos(ro));
                    if(z<0)AddPoint(x, y, z, 250, 250, 200);
                    else AddPoint(x, y, z, 250 / 2, 250 / 2, 200 / 2);
                }
            }*/
            zbuf = new float[picRender.Width, picRender.Height];
            zb = new float[picRender.Width * picRender.Height * 3];
            lblNum.Text = (points.Count/1000000.0).ToString();
            pointsCount = points.Count;
            /* double[,] pro = {  {  2/(right-left), 0, 0, -(right+left)/(right-left) },
                                      {0, 2/(top-bottom), 0, -(top+bottom)/(top-bottom) },
                                      { 0, 0, -2/(far - near), -(far + near)/(far - near)},
                                      { 0, 0, 0, 1 }};
             mp = Matrix<double>.Build.DenseOfArray(pro);*/
            cloud = new Cloud(picRender.Width, picRender.Height);
            //  cloud.Generate((int)Math.Pow(10, 6.5), 5f);
            cloud.Clear();
        }

        private void tmrRender_Tick(object sender, EventArgs e)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
           /* if (instance.picDepthMap.Image != null && instance.pictureBox1.Image != null)
            {
                Bitmap d = new Bitmap(instance.picDepthMap.Image);
                Bitmap c = new Bitmap(instance.pictureBox1.Image);
                cloud.GenerateFromDepthMap(d, c, 90, 60, 2, 20);
            }*/
            /* for (int i = 0; i < picRender.Width; i++)
                 for (int j = 0; j < picRender.Height; j++)
                     zbuf[i, j] = 9999f;*/

            /*  var bit = new Bitmap(picRender.Width, picRender.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

              ox = trbTranslateX.Value;
              oy = trbTranslateY.Value;
              rx = trbRotationX.Value / 180f * (float)Math.PI;
              ry = trbRotationY.Value / 180f * (float)Math.PI;

              BitmapData bitmapData = bit.LockBits(new Rectangle(0, 0, bit.Width, bit.Height), ImageLockMode.ReadWrite, bit.PixelFormat);

              int bytesPerPixel = Bitmap.GetPixelFormatSize(bit.PixelFormat) / 8;
              int byteCount = bitmapData.Stride * bit.Height;
              if(pixels == null)pixels = new byte[byteCount];
              IntPtr ptrFirstPixel = bitmapData.Scan0;
              //Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
              int heightInPixels = bitmapData.Height;
              int widthInBytes = bitmapData.Width * bytesPerPixel;
              int bitHeight = bit.Height;
              int bitWidth = bit.Width;
              int bitmapDataStride = bitmapData.Stride;
              unsafe
              {
                  for (int i = 0; i < byteCount; i += 3)
                  {
                      pixels[i] = 0;
                      pixels[i + 1] = 0;
                      pixels[i + 2] = 0;
                      zb[i] = 999f;
                  }

                  float C = (float)Math.Cos(rx);
                  float S = (float)Math.Sin(rx);
                  float c = (float)Math.Cos(ry);
                  float s = (float)Math.Sin(ry);
                  float sS = s*S;
                  float cS = c*S;
                  float sC = s*C;
                  float cC = c*C;

                  var vp = new double[4];
                  var vf = new double[4];

                  float trbTranslateZValue = trbTranslateZ.Value / 10f;

                  //Parallel.For(0, pointsCount, new ParallelOptions { MaxDegreeOfParallelism = 4},i =>
                  for (int i = 0; i < pointsCount; i++)
                  {
                      float[] p = points[i];
                      float[] ps = pointsTrans[i];
                      //float[] ps = pointsScreen[i];

                      ps[0] = p[0] * c + p[2] * s;
                      ps[1] = p[0] * sS + p[1] * C - p[2] * cS;
                      ps[2] = trbTranslateZValue - (-p[0] * sC + p[1] * S + p[2] * cC);
                      //pt[3] = 1;
                      
                      byte[] pRGB = pointsRGB[i];
                      int x = (int)(ox + ps[0] * sx);
                      int y = bitHeight - (int)(oy + ps[1] * sy);
                      // DrawPixel(x, y, pRGB[0], pRGB[1], pRGB[2], 1f, ps[2]);

                      if (x < 0 || y < 0 || x >= bitWidth || y >= bitHeight) continue;
                      int idx = y * bitmapDataStride + x * 3;
                      if (zb[idx] < ps[2]) continue;
                      zb[idx] = ps[2];
                      pixels[idx] = pRGB[2];
                      pixels[idx + 1] = pRGB[1];
                      pixels[idx + 2] = pRGB[0];
                  }
                  //);
              }
              void DrawPixel(int x, int y, byte r, byte g, byte b, float op, float z)
              {
                  if (x < 0 || y < 0 || x >= bitWidth || y >= bitHeight) return;
                  /* op = op > 1f ? 1f : op < 0f ? 0 : op;
                   int idx = y * bitmapData.Stride + x * 3;
                   pixels[idx] = (byte)(b * op + pixels[idx] * (1f - op));
                   pixels[idx + 1] = (byte)(g * op + pixels[idx + 1] * (1f - op));
                   pixels[idx + 2] = (byte)(r * op + pixels[idx + 2] * (1f - op));* /
                  int idx = y * bitmapDataStride + x * 3;
                  if (zb[idx] < z) return;
                  zb[idx] = z;
                  pixels[idx] = b;
                  pixels[idx + 1] = g;
                  pixels[idx + 2] = r;
              }
              void DrawRect(int x, int y, byte r, byte g, byte b, float op, float z)
              {
                  DrawPixel(x, y, r, g, b, op, z);
                 /* DrawPixel(x - 1, y, r, g, b, op, z);
                  DrawPixel(x + 1, y, r, g, b, op, z);
                  DrawPixel(x, y - 1, r, g, b, op, z);
                  DrawPixel(x, y + 1, r, g, b, op, z);
                  DrawPixel(x + 1, y + 1, r, g, b, op, z);
                  DrawPixel(x - 1, y + 1, r, g, b, op, z);
                  DrawPixel(x + 1, y - 1, r, g, b, op, z);
                  DrawPixel(x - 1, y - 1, r, g, b, op, z);* /
              }
              void DrawZbuffer()
              {
                  for (int i = 0; i < byteCount; i += 3)
                  {
                      HsvToRgb(map(zb[i], -10, 10, 0, 270), 1.0, zb[i] > 99 ? 0 : 1, out int r, out int g, out int b);
                      pixels[i] = (byte)b;
                      pixels[i + 1] = (byte)g;
                      pixels[i + 2] = (byte)r;
                  }
              }

              //DrawZbuffer();
              Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
              bit.UnlockBits(bitmapData);
              st.Stop();*/

            Bitmap bit = cloud.Render(trbRotationX.Value / 180f * (float)Math.PI,
                trbRotationY.Value / 180f * (float)Math.PI,
                trbTranslateX.Value,
                trbTranslateY.Value,
                trbScale.Value / 3f,
                trbScale.Value / 3f);
            picRender.Invoke(new Action(() =>
             {
                 Image img = picRender.Image;
                 picRender.Image = bit;
                 if (img != null) img.Dispose();
             }));
            
            Text = st.ElapsedMilliseconds.ToString();
        }
        Cloud cloud;
        float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}
