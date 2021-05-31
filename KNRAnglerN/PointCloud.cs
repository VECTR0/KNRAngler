using System;
using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace KNRAnglerN
{
    public class PointCloud
    {
        public struct Point
        {
            public float x, y, z;
            public byte r, g, b, a;
            public bool enabled;
        }
        
        private Point[] _points;
        private float[] _zBuffer;
        public volatile bool initializedCalulation = false;
        public volatile bool initializedRendering = false;
        private byte[] _frameBuffer;
        private byte[] _colorBytes, _depthBytes;
        
        private void InitializeCalculation(int width, int height)
        {
            //points = new Point[height * width];
            _points = new Point[(int)1E6];
            initializedCalulation = true;
        }
        
        private void InitializeRendering(int width, int height)
        {
            _frameBuffer = new byte[width * height * 3];
            _zBuffer = new float[_frameBuffer.Length];
            initializedRendering = true;
        }

        public void Calculate(Bitmap depth, Bitmap color, float hFov, float vFov, float minDistance, float maxDistance)
        {
            if (depth.Width != color.Width || depth.Height != color.Height) return;
            if (!initializedCalulation)
            {
                InitializeCalculation(depth.Width, depth.Height);
                CreateArrayFromBitmap(depth, ref _depthBytes, out int depthBpp2);
                CreateArrayFromBitmap(color, ref _colorBytes, out int colorBpp2);
            }

            ReadBitmapToArray(depth, ref _depthBytes, out int depthBpp);
            ReadBitmapToArray(color, ref _colorBytes, out int colorBpp);
            if (colorBpp != depthBpp) throw new Exception("Wrong BPP for color and depth");

            unsafe
            {
                int depthHeight = depth.Height;
                int depthWidth = depth.Width;
                int colorHeight = color.Height;
                int colorWidth = color.Width;
                int counter = 0;
                float h_fov = hFov / 180f * (float) Math.PI;
                float v_fov = vFov / 180f * (float) Math.PI;
                float hTan = (float) (Math.Tan(h_fov / 2f));
                float vTan = (float) (Math.Tan(v_fov / 2f));

                // float hTan = (float) (Math.Tan(hFov * Math.PI / 180f / 2f));
                // float vTan = (float) (Math.Tan(vFov * Math.PI / 180f / 2f));
                for (int i = 0; i < colorHeight; i++)
                {
                    for (int j = 0; j < colorWidth; j++)
                    {
                        int idx = ((colorHeight - i - 1) * colorWidth + j) * colorBpp;
                        int dx = (int) ((float) j * colorWidth / depthWidth);
                        int dy = (int) ((float) (colorHeight - i - 1) * colorHeight / depthHeight);
                        int didx = ((dy) * depthWidth + dx) * depthBpp;

                        // int idx = (i * depthWidth + j) * depthBpp;
                        float d = (1f - _depthBytes[didx] / 255f) * 19.7f + 0.3f; //0.01 40 depth

                        ref Point p = ref _points[counter];
                        if (d > 19) p.enabled = false;
                        else
                        {
                            float x0 = (float) (j) / colorWidth * 2 - 1f;
                            float y0 = (float) (i) / colorHeight * 2 - 1f;

                            p.enabled = true;
                            p.x = d * (x0) * hTan;
                            p.y = d * (y0) * vTan;
                            p.z = -d;
                            p.r = _colorBytes[idx + 2];
                            p.g = _colorBytes[idx + 1];
                            p.b = _colorBytes[idx];
                        }
                        counter++;
                    }
                }

                for (int i = 0; i < 2000; i++)
                {
                    float d = i / 2000f * 19.7f + 0.3f;
                    double de = 0.5 * h_fov;
                    double ro = 0.5 * v_fov;
                    float x = (float) (Math.Tan(de)) * d;
                    float y = (float) (Math.Tan(ro)) * d;
                    float z = -d;
                    _points[counter].enabled = true;
                    _points[counter].x = x;
                    _points[counter].y = y;
                    _points[counter].z = z;
                    _points[counter].r = 155; //colorBytes[idx + 2];
                    _points[counter].g = 255; //colorBytes[idx + 1];
                    _points[counter].b = 155; //colorBytes[idx];
                    counter++;
                }

                for (int i = 0; i < 2000; i++)
                {
                    float d = i / 2000f * 19.7f + 0.3f;
                    double de = -0.5 * h_fov;
                    double ro = 0.5 * v_fov;
                    float x = (float) (Math.Tan(de)) * d;
                    float y = (float) (Math.Tan(ro)) * d;
                    float z = -d;
                    _points[counter].enabled = true;
                    _points[counter].x = x;
                    _points[counter].y = y;
                    _points[counter].z = z;
                    _points[counter].r = 155; //colorBytes[idx + 2];
                    _points[counter].g = 255; //colorBytes[idx + 1];
                    _points[counter].b = 155; //colorBytes[idx];
                    counter++;
                }

                for (int i = 0; i < 2000; i++)
                {
                    float d = i / 2000f * 19.7f + 0.3f;
                    double de = 0.5 * h_fov;
                    double ro = -0.5 * v_fov;
                    float x = (float) (Math.Tan(de)) * d;
                    float y = (float) (Math.Tan(ro)) * d;
                    float z = -d;
                    _points[counter].enabled = true;
                    _points[counter].x = x;
                    _points[counter].y = y;
                    _points[counter].z = z;
                    _points[counter].r = 155; //colorBytes[idx + 2];
                    _points[counter].g = 255; //colorBytes[idx + 1];
                    _points[counter].b = 155; //colorBytes[idx];
                    counter++;
                }

                for (int i = 0; i < 2000; i++)
                {
                    float d = i / 2000f * 19.7f + 0.3f;
                    double de = -0.5 * h_fov;
                    double ro = -0.5 * v_fov;
                    float x = (float) (Math.Tan(de)) * d;
                    float y = (float) (Math.Tan(ro)) * d;
                    float z = -d;
                    _points[counter].enabled = true;
                    _points[counter].x = x;
                    _points[counter].y = y;
                    _points[counter].z = z;
                    _points[counter].r = 155; //colorBytes[idx + 2];
                    _points[counter].g = 255; //colorBytes[idx + 1];
                    _points[counter].b = 155; //colorBytes[idx];
                    counter++;
                }
            }
        }

        private void CreateArrayFromBitmap(Bitmap processedBitmap, ref byte[] data, out int bytesPerPixel)
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadOnly, processedBitmap.PixelFormat);
            bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * processedBitmap.Height;
            data = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, data, 0, byteCount);
            processedBitmap.UnlockBits(bitmapData);
        }

        private void ReadBitmapToArray(Bitmap bitmap, ref byte[] bytes, out int bytesPerPixel)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * bitmap.Height;
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, bytes, 0, byteCount);
            bitmap.UnlockBits(bitmapData);
        }

        public Bitmap Render(int width, int height, float rx, float ry, float ox, float oy, float sx, float sy, bool drawZBuffer)
        {
            if (!initializedRendering) InitializeRendering(width, height);
            Bitmap bit = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bit.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                bit.PixelFormat);

            IntPtr ptrFirstPixel = bitmapData.Scan0;
            int bitmapDataStride = bitmapData.Stride;

            int byteCount = width * 3 * height;

            for (int i = 0; i < byteCount; i += 3)
            {
                _frameBuffer[i] = 0;
                _frameBuffer[i + 1] = 0;
                _frameBuffer[i + 2] = 0;
                _zBuffer[i] = 999f;
            }

            float C = (float) Math.Cos(rx);
            float S = (float) Math.Sin(rx);
            float c = (float) Math.Cos(ry);
            float s = (float) Math.Sin(ry);
            float sS = s * S;
            float cS = c * S;
            float sC = s * C;
            float cC = c * C;
            unsafe
            {
                for (int i = 0; i < _points.Length; i++)
                {
                    ref Point p = ref _points[i];
                    if (!p.enabled) continue;
                    float ptx = p.x * c + p.z * s;
                    float pty = p.x * sS + p.y * C - p.z * cS;
                    float ptz = p.x * sC - p.y * S - p.z * cC;


                    int x = (int) (ox + ptx * sx);
                    int y = height - (int) (oy + pty * sy);

                    if (x < 1 || y < 1 || x >= width - 1 || y >= height - 1) continue;
                    int idx = y * bitmapDataStride + x * 3;
                    if (_zBuffer[idx] > ptz)
                    {
                        _zBuffer[idx] = ptz;
                        _frameBuffer[idx] = p.b;
                        _frameBuffer[idx + 1] = p.g;
                        _frameBuffer[idx + 2] = p.r;
                    }
/*
                    idx += 3;
                    if (zBuffer[idx] > ptz)
                    {
                        zBuffer[idx] = ptz;
                        frameBuffer[idx] = p.b;
                        frameBuffer[idx + 1] = p.g;
                        frameBuffer[idx + 2] = p.r;
                    }
                    idx += bitmapDataStride - 3;
                    if (zBuffer[idx] > ptz)
                    {
                        zBuffer[idx] = ptz;
                        frameBuffer[idx] = p.b;
                        frameBuffer[idx + 1] = p.g;
                        frameBuffer[idx + 2] = p.r;
                    }
                    idx += 3;
                    if (zBuffer[idx] > ptz)
                    {
                        zBuffer[idx] = ptz;
                        frameBuffer[idx] = p.b;
                        frameBuffer[idx + 1] = p.g;
                        frameBuffer[idx + 2] = p.r;
                    }*/
                }

                if (drawZBuffer)
                {
                    for (int i = 0; i < byteCount; i += 3)
                    {
                        HsvToRgb(MapRange(_zBuffer[i], -10, 10, 0, 270),
                            1.0,
                            //_zBuffer[i] > 99 ? 0 : 1,
                            Clamp(MapRange(_zBuffer[i], 0, 40, 1, 0), 0, 1),
                            out int r, out int g, out int b);
                        _frameBuffer[i] = (byte) b;
                        _frameBuffer[i + 1] = (byte) g;
                        _frameBuffer[i + 2] = (byte) r;
                    }
                }
            }

            Marshal.Copy(_frameBuffer, 0, ptrFirstPixel, byteCount);
            bit.UnlockBits(bitmapData);
            return bit;
        }

        private static float Clamp(float x, float min, float max) => x < min ? min : x > max ? max : x;
        
        private static float MapRange(float x, float in_min, float in_max, float out_min, float out_max) => (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;

        private static void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
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

        static int Clamp(int x) => x < 0 ? 0 : x > 255 ? 255 : x;
    }
}