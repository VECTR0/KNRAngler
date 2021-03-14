using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KNRAnglerN
{
    class Cloud
    {
        private float[][] points = null; 
        private float[][] pointsTransformed = null; 
        private float[][] pointsScreen = null; 
        private byte[][] pointsColor = null;
        private float[] zbuffer = null;
        private int width, height;
        private byte[] pixels = null;
        private int byteCount;


        public Cloud(int width, int height)
        {
            this.width = width;
            this.height = height;
            byteCount = width * 3 * height;
            pixels = new byte[byteCount];
            zbuffer = new float[byteCount];
        }

        public void AddPoints(float[][] newPoints, byte[][] colors)
        {
            int oldLength = points.Length;
            int newLength = points.Length + newPoints.Length;
            if (points == null) points = new float[0][];
            if (pointsTransformed == null) pointsTransformed = new float[0][];
            if (pointsScreen == null) pointsScreen = new float[0][];
            if (pointsColor == null) pointsColor = new byte[0][];
            if (points.Length != newPoints.Length)
            {
                Array.Resize(ref points, newLength);
                Array.Resize(ref pointsTransformed, newLength);
                Array.Resize(ref pointsScreen, newLength);
                Array.Resize(ref pointsColor, newLength);
            }

            for(int i = oldLength; i < newLength; i++)
            {
                points[i] = newPoints[i - oldLength];
                pointsColor[i] = colors[i - oldLength];
            }
        }

        public void Clear()
        {
            points = new float[0][];
            pointsTransformed = new float[0][];
            pointsScreen = new float[0][];
            pointsColor = new byte[0][];
        }

        public void Generate(int n, float d)
        {
            points = new float[n][];
            pointsTransformed = new float[n][];
            pointsScreen = new float[n][];
            pointsColor = new byte[n][];

            Random r = new Random(2);
            for (int i = 0; i < n; i++)
            {
                float x = (float)(r.NextDouble() * 2 - 1*0) * d;
                float y = (float)(r.NextDouble() * 2 - 1 * 0) * d;
                float z = (float)(r.NextDouble() * 2 - 1 * 0) * d;
                points[i] = new float[] { x, y, z };
                pointsTransformed[i] = new float[3];
                pointsScreen[i] = new float[3];
                pointsColor[i] = new byte[] { (byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256) };
            }
        }

        public void GenerateFromDepthMap(Bitmap depth, Bitmap color, float hFov, float vFov, float minDistance, float maxDistance)
        {
            Clear();
           
            int depthHeight = depth.Height;
            int depthWidth = depth.Width;
            int colorHeight = color.Height;
            int colorWidth = color.Width;
            ReadBitmapToArray(depth, out byte[] depthBytes, out int depthBpp);
            ReadBitmapToArray(color, out byte[] colorBytes, out int colorBpp);

            int newPointsNum = colorWidth * colorHeight;
            float[][] newPoints = new float[newPointsNum][];
            byte[][] newColors = new byte[newPointsNum][];

            
            int counter = 0;
            for (int i = 0; i < colorHeight; i++)
            {
                for (int j = 0; j < colorWidth; j++)
                {
                    int idx = ((colorHeight - i - 1) * colorWidth + j) * colorBpp;
                    int dx = (int)((float)j * colorWidth / depthWidth);
                    int dy = (int)((float)(colorHeight - i - 1) * colorHeight / depthHeight);
                    int didx = ((dy) * depthWidth + dx) * depthBpp;

                    float d = (depthBytes[didx] / 255f);//0.01 40
                    /* d *= d;
                     d *= (40f-0.01f)/255f;
                     d *= 100f;
                     
                     float x = (float)(d / Math.Tan( (3.14 - h_fov)/2 + j * h_fov / colorWidth ));
                     float y = (float)(-d / Math.Tan( (3.14 - v_fov)/2 + i * v_fov / colorHeight ));
                     float z = (float)(d);*/
                    float h_fov = hFov / 180f * (float)Math.PI;
                    float v_fov = vFov / 180f * (float)Math.PI;
                    float htan = (float)(Math.Tan(h_fov / 2f));
                    float vtan = (float)(Math.Tan(v_fov / 2f));
                    double de = ((float)(j) / colorWidth-0.5f) * h_fov;
                    double ro = ((float)(i) / colorHeight-0.5f) * v_fov;
                    float near = 0.01f;

                    /* float x = (float)(Math.Cos(de)) * r;
                     float y = (float)(i * 0.01f);
                     float z = (float)(Math.Sin(de)) * r;*/

                    /*float x = (float)(j * 0.01f);
                    float y = (float)(Math.Cos(ro)) * r;
                    float z = (float)(Math.Sin(ro)) * r;*/
                    d = 1f - d;
                    d *= 39.9f;
                    d += 0.01f;
                    d /= 2f;
                    float x0 = (float)(j) / colorWidth* 2 - 1f;
                    float y0 = (float)(i) / colorHeight*2 - 1f;
                    //float x = (float)( x0 / d / htan);
                    //float y = (float)( y0 / d / vtan);
                    float x = (float)(d * Math.Tan(de));
                    float y = (float)(d * Math.Tan(ro));
                    float z = d;

                    newPoints[counter] = new float[] { x, y, z };
                    newColors[counter++] = new byte[] { colorBytes[idx + 2], colorBytes[idx + 1], colorBytes[idx] };
                }
            }
            newPoints[0][0] = 0;
            newPoints[0][1] = 0;
            newPoints[0][2] = 0;
            newColors[0][0] = 255;
            newColors[0][1] = 0;
            newColors[0][2] = 255;
            AddPoints(newPoints, newColors);

            counter = 0;
            newPoints = new float[4000][];
            newColors = new byte[4000][];

            for(int i = 0; i < 1000; i++)
            {
                float d = i * 0.1f;
                float h_fov = hFov / 180f * (float)Math.PI;
                float v_fov = vFov / 180f * (float)Math.PI;
                double de = 0.5 * h_fov;
                double ro = 0.5 * v_fov;
                
                //d *= 0.1f;
                float x = (float)(Math.Tan(de)) * d;
                float y = (float)(Math.Tan(ro)) * d;
                float z = -d;

                newPoints[counter] = new float[] { x, y, z };
                newColors[counter++] = new byte[] { 255, 255, 255 };
            }
            for(int i = 0; i < 1000; i++)
            {
                float d = i * 0.1f;
                float h_fov = hFov / 180f * (float)Math.PI;
                float v_fov = vFov / 180f * (float)Math.PI;
                double de = -0.5 * h_fov;
                double ro = 0.5 * v_fov;
                
                //d *= 0.1f;
                float x = (float)(Math.Tan(de)) * d;
                float y = (float)(Math.Tan(ro)) * d;
                float z = -d;

                newPoints[counter] = new float[] { x, y, z };
                newColors[counter++] = new byte[] { 255, 255, 255 };
            }
            for(int i = 0; i < 1000; i++)
            {
                float d = i * 0.1f;
                float h_fov = hFov / 180f * (float)Math.PI;
                float v_fov = vFov / 180f * (float)Math.PI;
                double de = 0.5 * h_fov;
                double ro = -0.5 * v_fov;
                
                //d *= 0.1f;
                float x = (float)(Math.Tan(de)) * d;
                float y = (float)(Math.Tan(ro)) * d;
                float z = -d;

                newPoints[counter] = new float[] { x, y, z };
                newColors[counter++] = new byte[] { 255, 255, 255 };
            }
            for(int i = 0; i < 1000; i++)
            {
                float d = i * 0.1f;
                float h_fov = hFov / 180f * (float)Math.PI;
                float v_fov = vFov / 180f * (float)Math.PI;
                double de = -0.5 * h_fov;
                double ro = -0.5 * v_fov;
                
                //d *= 0.1f;
                float x = (float)(Math.Tan(de)) * d;
                float y = (float)(Math.Tan(ro)) * d;
                float z = -d;

                newPoints[counter] = new float[] { x, y, z };
                newColors[counter++] = new byte[] { 255, 255, 255 };
            }

            AddPoints(newPoints, newColors);
        }

        private void ReadBitmapToArray(Bitmap processedBitmap, out byte[] data, out int bytesPerPixel)
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadOnly, processedBitmap.PixelFormat);
            bytesPerPixel = Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * processedBitmap.Height;
            data = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, data, 0, data.Length);
            processedBitmap.UnlockBits(bitmapData);
        }

        public Bitmap Render(float rx, float ry, float ox, float oy, float sx, float sy)
        {
            ox += width / 2;
            oy += height/ 2;
            Bitmap bit = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bit.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bit.PixelFormat);

            IntPtr ptrFirstPixel = bitmapData.Scan0;
            int bitmapDataStride = bitmapData.Stride;
            unsafe
            {
               for (int i = 0; i < byteCount; i += 3)
                {
                    pixels[i] = 0;
                    pixels[i + 1] = 0;
                    pixels[i + 2] = 0;
                    zbuffer[i] = 999f;
                }

                float C = (float)Math.Cos(rx);
                float S = (float)Math.Sin(rx);
                float c = (float)Math.Cos(ry);
                float s = (float)Math.Sin(ry);
                float sS = s * S;
                float cS = c * S;
                float sC = s * C;
                float cC = c * C;

                var vp = new double[4];
                var vf = new double[4];

                int pointsCount = points.Length, x, y, idx;
                float ptx, pty, ptz;
                for (int i = 0; i < pointsCount; i++)
                {
                    float[] p = points[i];
                   // float[] ps = pointsTransformed[i];
                    //float[] ps = pointsScreen[i];

                    ptx = p[0] * c + p[2] * s;
                    pty = p[0] * sS + p[1] * C - p[2] * cS;
                    ptz = p[0] * sC - p[1] * S - p[2] * cC;
                    //pt[3] = 1;
                    
                    byte[] pRGB = pointsColor[i];
                    x = (int)(ox + ptx * sx);
                    y = height - (int)(oy + pty * sy);

                    if (x < 0 || y < 0 || x >= width || y >= height) continue;
                    idx = y * bitmapDataStride + x * 3;
                    if (zbuffer[idx] < ptz) continue;
                    zbuffer[idx] = ptz;
                    pixels[idx] = pRGB[2];
                    pixels[idx + 1] = pRGB[1];
                    pixels[idx + 2] = pRGB[0];

                    if (x+1 < 0 || y < 0 || x+1 >= width || y >= height) continue;
                    idx = y * bitmapDataStride + (x+1) * 3;
                    if (zbuffer[idx] < ptz) continue;
                    zbuffer[idx] = ptz;
                    pixels[idx] = pRGB[2];
                    pixels[idx + 1] = pRGB[1];
                    pixels[idx + 2] = pRGB[0];

                    if (x-1 < 0 || y-1 < 0 || x-1 >= width || y-1 >= height) continue;
                    idx = (y-1) * bitmapDataStride + (x-1) * 3;
                    if (zbuffer[idx] < ptz) continue;
                    zbuffer[idx] = ptz;
                    pixels[idx] = pRGB[2];
                    pixels[idx + 1] = pRGB[1];
                    pixels[idx + 2] = pRGB[0];

                    if (x < 0 || y-1 < 0 || x >= width || y-1 >= height) continue;
                    idx = (y-1) * bitmapDataStride + x * 3;
                    if (zbuffer[idx] < ptz) continue;
                    zbuffer[idx] = ptz;
                    pixels[idx] = pRGB[2];
                    pixels[idx + 1] = pRGB[1];
                    pixels[idx + 2] = pRGB[0];
                }
            }
            
           /*? void DrawZbuffer()
            {
                for (int i = 0; i < byteCount; i += 3)
                {
                    HsvToRgb(map(zb[i], -10, 10, 0, 270), 1.0, zb[i] > 99 ? 0 : 1, out int r, out int g, out int b);
                    pixels[i] = (byte)b;
                    pixels[i + 1] = (byte)g;
                    pixels[i + 2] = (byte)r;
                }
            }*/
            //DrawZbuffer();

            Marshal.Copy(pixels, 0, ptrFirstPixel, byteCount);
            bit.UnlockBits(bitmapData); 
            return bit;
        }
    }
}
