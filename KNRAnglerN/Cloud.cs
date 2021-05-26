using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace KNRAnglerN
{
    class Cloud
    {
        private float[][] points; 
        private float[][] pointsTransformed; 
        private float[][] pointsScreen; 
        private byte[][] pointsColor;
        private float[] zbuffer;
        private int width, height;
        private byte[] pixels;
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
                points[i] = new[] { x, y, z };
                pointsTransformed[i] = new float[3];
                pointsScreen[i] = new float[3];
                pointsColor[i] = new[] { (byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256) };
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
                    
                    float d = depthBytes[didx] / 255f;//0.01 40
                    
                    float h_fov = hFov / 180f * (float)Math.PI;
                    float v_fov = vFov / 180f * (float)Math.PI;
                    float htan = (float)(Math.Tan(h_fov / 2f));
                    float vtan = (float)(Math.Tan(v_fov / 2f));

                     d = 1f - d;
                        d *= 39.99f;
                        d += 0.01f;
                    //d *= (20f - 0.4f);
                   // d += 0.4f;
                    float x0 = (float)(j) / colorWidth* 2 - 1f;
                    float y0 = (float)(i) / colorHeight*2 - 1f;

                    float x = d * (x0) * htan;
                    float y = d * (y0) *vtan;
                    float z = -d;
                    if(d == 0f || d > 39.00f) newPoints[counter] = new float[] { 100, 100, 100 };
                    else newPoints[counter] = new[] { x, y, z };
                    newColors[counter++] = new[] { colorBytes[idx + 2], colorBytes[idx + 1], colorBytes[idx] };
                   
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
            newPoints = new float[8000][];
            newColors = new byte[8000][];

            
            for (int i = 0; i < 2000; i++)
            {
                float d = i / 2000f * 39.99f + 0.01f;
                float h_fov = hFov / 180f * (float)Math.PI;
                float v_fov = vFov / 180f * (float)Math.PI;
                double de = 0.5 * h_fov;
                double ro = 0.5 * v_fov;
                
                //d *= 0.1f;
                float x = (float)(Math.Tan(de)) * d;
                float y = (float)(Math.Tan(ro)) * d;
                float z = -d;

                newPoints[counter] = new[] { x, y, z };
                newColors[counter++] = new byte[] { 255, 255, 255 };
            }
            for(int i = 0; i < 2000; i++)
            {
                float d = i / 2000f * 39.99f + 0.01f;
                float h_fov = hFov / 180f * (float)Math.PI;
                float v_fov = vFov / 180f * (float)Math.PI;
                double de = -0.5 * h_fov;
                double ro = 0.5 * v_fov;
                
                //d *= 0.1f;
                float x = (float)(Math.Tan(de)) * d;
                float y = (float)(Math.Tan(ro)) * d;
                float z = -d;

                newPoints[counter] = new[] { x, y, z };
                newColors[counter++] = new byte[] { 255, 255, 255 };
            }
            for(int i = 0; i < 2000; i++)
            {
                float d = i / 2000f * 39.99f + 0.01f;
                float h_fov = hFov / 180f * (float)Math.PI;
                float v_fov = vFov / 180f * (float)Math.PI;
                double de = 0.5 * h_fov;
                double ro = -0.5 * v_fov;
                
                //d *= 0.1f;
                float x = (float)(Math.Tan(de)) * d;
                float y = (float)(Math.Tan(ro)) * d;
                float z = -d;

                newPoints[counter] = new[] { x, y, z };
                newColors[counter++] = new byte[] { 255, 255, 255 };
            }
            for(int i = 0; i < 2000; i++)
            {
                float d = i / 2000f * 39.99f+0.01f;
                float h_fov = hFov / 180f * (float)Math.PI;
                float v_fov = vFov / 180f * (float)Math.PI;
                double de = -0.5 * h_fov;
                double ro = -0.5 * v_fov;
                
                //d *= 0.1f;
                float x = (float)(Math.Tan(de)) * d;
                float y = (float)(Math.Tan(ro)) * d;
                float z = -d;

                newPoints[counter] = new[] { x, y, z };
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
            Bitmap bit = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bit.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bit.PixelFormat);

            IntPtr ptrFirstPixel = bitmapData.Scan0;
            int bitmapDataStride = bitmapData.Stride;
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
            //Random r = new Random();
            int pointsCount = points.Length, x, y, idx;
            float ptx, pty, ptz;
            for (int i = (int)(DateTime.Now.Ticks % 10); i < pointsCount; i+=10)
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

                if (x-1 < 0 || y < 0 || x-1 >= width || y >= height) continue;
                idx = y * bitmapDataStride + (x-1) * 3;
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
