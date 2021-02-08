using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KNRAnglerN
{
    public partial class MainForm : Form
    {
        public Sensors sens = new Sensors();

        public const string ver = "7.2";
        public readonly ConsoleForm consoleForm;
        public readonly SettingsForm settingsForm;
        public OkonClient okonClient;
        public int requestedVideoFeedFrames = 0;
        public int requestedDepthMapFrames = 0;
        public int framesNum = 0;
        public int ping = 0;
        DateTime framesLastCheck = DateTime.Now;
        private object lock_ = new object();

        public enum Packet : byte
        {
            SET_MTR = 0xA0,
            GET_SENS = 0xB0,
            GET_DEPTH = 0xB1,
            GET_DEPTH_BYTES = 0xB2,
            GET_VIDEO_BYTES = 0xB3,
            SET_SIM = 0xC0,
            ACK = 0xC1,
            GET_ORIEN = 0xC2,
            SET_ORIEN = 0xC3,
            REC_STRT = 0xD0,
            REC_ST = 0xD1,
            REC_RST = 0xD2,
            GET_REC = 0xD3,
            PING = 0xC5,
            GET_DETE = 0xDE
        }

        [Flags]
        private enum Flag
        {
            None = 0,
            SERVER_ECHO = 1,
            DO_NOT_LOG_PACKET = 2,
            TEST = 128
        }
        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            consoleForm = new ConsoleForm(this);
            settingsForm = new SettingsForm(this);
            Text = "KNR Wędkarz - Okoń Sim control v" + ver + " by Vectro 2021";
        }

        public class Info : OkonClient.IInfo
        {
            public MainForm mainFormInstance;
            public Info(MainForm instance) => this.mainFormInstance = instance;
            public void YeetLog(string info)
            {
                if (mainFormInstance.settingsForm.chkYeetLog.Checked)
                    mainFormInstance.consoleForm.txtConsole.AppendText("[LOG]: " + info + Environment.NewLine);
            }
            public void YeetException(Exception exp)
            {
                if (exp.Message.ToLower().Contains("aborted")) return;
                mainFormInstance.consoleForm.txtConsole.AppendText("[ERR]: " + exp.Message + Environment.NewLine);
                mainFormInstance.consoleForm.txtConsole.AppendText("[ERR]: " + exp.StackTrace + Environment.NewLine);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //  this.BeginInvoke((Action)(() => MessageBox.Show("Warning!\nDevelopment build, some components may not work"))); 
            pictureBox1.Controls.Add(picHUD);
            picHUD.BackColor = Color.Transparent;
        }

        public void HandleReceivedPacket(object o, OkonClient.PacketEventArgs e)
        {
            int maxLength = 0;
            foreach (var s in Enum.GetNames(typeof(Packet))) maxLength = Math.Max(maxLength, s.Length);
            switch ((Packet)e.packetType)
            {
                case Packet.GET_DEPTH:
                    consoleForm.Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + Encoding.ASCII.GetString(e.packetData, 0, e.packetData.Length);
                    var json = Utf8Json.JsonSerializer.Deserialize<dynamic>(Encoding.ASCII.GetString(e.packetData));
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(json["depth"])))
                    {
                        picDepthMap.Invoke(new Action(() =>
                         {
                             Image img = picDepthMap.Image;
                             picDepthMap.Image = Image.FromStream(ms);
                             if (img != null) img.Dispose();
                         }));
                    }
                    break;
                case Packet.GET_DEPTH_BYTES:
                    requestedDepthMapFrames--;
                    //Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + "size: " + e.packetData.Length +"B";
                    using (var ms = new MemoryStream(e.packetData))
                    {
                        picDepthMap.Invoke(new Action(() =>
                         {
                             Image img = picDepthMap.Image;
                             picDepthMap.Image = Image.FromStream(ms);
                             if (img != null) img.Dispose();
                         }));
                        framesNum++;
                    }
                    break;
                case Packet.GET_VIDEO_BYTES:
                    requestedVideoFeedFrames--;
                    //Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + "size: " + e.packetData.Length +"B";
                    using (var ms = new MemoryStream(e.packetData))
                    {
                        pictureBox1.Invoke(new Action(() =>
                        {
                            Image img = pictureBox1.Image;
                            pictureBox1.Image = Image.FromStream(ms);
                            if (img != null) img.Dispose();
                        }));
                    }
                    break;
                case Packet.GET_SENS:
                    sens = Utf8Json.JsonSerializer.Deserialize<Sensors>(e.packetData);
                    break;
                case Packet.PING:
                    json = Utf8Json.JsonSerializer.Deserialize<dynamic>(Encoding.ASCII.GetString(e.packetData));
                    ping = (int)json["ping"];
                    break;
                default:
                    consoleForm.Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + Encoding.ASCII.GetString(e.packetData, 0, e.packetData.Length);
                    break;
            }
        }

        private void tmrFrameRate_Tick(object sender, EventArgs e)
        {
            if (okonClient != null && okonClient.IsConnected() && settingsForm.chkVideoFeed.Checked)
            {
                if (requestedVideoFeedFrames < 2)
                    try
                    {
                        okonClient.SendString((byte)Packet.GET_VIDEO_BYTES, (byte)Flag.DO_NOT_LOG_PACKET, "");
                        requestedVideoFeedFrames++;
                    }
                    catch
                    {
                        consoleForm.Log = "Error, packet not sent";
                        settingsForm.chkVideoFeed.Checked = false;
                    }
                if (requestedDepthMapFrames < 2)
                    try
                    {
                        okonClient.SendString((byte)Packet.GET_DEPTH_BYTES, (byte)Flag.DO_NOT_LOG_PACKET, "");
                        requestedDepthMapFrames++;
                    }
                    catch
                    {
                        consoleForm.Log = "Error, packet not sent";
                        settingsForm.chkVideoFeed.Checked = false;
                    }
            }
        }
        bool[] control = { false, false, false, false, false, false };
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (okonClient == null || !okonClient.IsConnected() || !settingsForm.chkManualControl.Checked) return;

            if (e.KeyCode == Keys.W) control[0] = true;
            if (e.KeyCode == Keys.A) control[1] = true;
            if (e.KeyCode == Keys.S) control[2] = true;
            if (e.KeyCode == Keys.D) control[3] = true;
            if (e.KeyCode == Keys.Q) control[4] = true;
            if (e.KeyCode == Keys.E) control[5] = true;
            UpdateManualControl();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (okonClient == null || !okonClient.IsConnected() || !settingsForm.chkManualControl.Checked) return;
            if (e.KeyCode == Keys.W) control[0] = false;
            if (e.KeyCode == Keys.A) control[1] = false;
            if (e.KeyCode == Keys.S) control[2] = false;
            if (e.KeyCode == Keys.D) control[3] = false;
            if (e.KeyCode == Keys.Q) control[4] = false;
            if (e.KeyCode == Keys.E) control[5] = false;
            UpdateManualControl();
        }

        private void UpdateManualControl()
        {//W A S D  Q E
            float vel = 0, dir = 0, fr = 0, ba = 0;
            if (control[0] && !control[2]) vel = 1f;
            if (!control[0] && control[2]) vel = -1f;
            if (control[1] && !control[3]) dir = -1f;
            if (!control[1] && control[3]) dir = 1f;
            if (control[4] && !control[5]) { fr = 1f; ba = -0.7f; }
            if (!control[4] && control[5]) { fr = -0.7f; ba = 1f; }
            float l = Math.Max(Math.Min((1 * dir + 1 * vel), 1), -1);
            float r = Math.Max(Math.Min((-1 * dir + 1 * vel), 1), -1);
            try
            {
                okonClient.SendString((byte)Packet.SET_MTR, (byte)Flag.DO_NOT_LOG_PACKET, "{\"FL\":" + fr.ToString().Replace(',', '.') + ",\"FR\":" + fr.ToString().Replace(',', '.') + ",\"ML\":" + l.ToString().Replace(',', '.') + ",\"MR\":" + r.ToString().Replace(',', '.') + ",\"B\":" + ba.ToString().Replace(',', '.') + "}");
            }
            catch
            {
                settingsForm.chkManualControl.Checked = false;
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (settingsForm.Visible) settingsForm.Hide();
            else settingsForm.Show();
        }

        private void btnShowConsole_Click(object sender, EventArgs e)
        {
            if (consoleForm.Visible) consoleForm.Hide();
            else consoleForm.Show();
        }

        private void tmrFramerate_Tick_1(object sender, EventArgs e)
        {
            lblFrameRate.Text = Math.Round((1000.0 * framesNum / (DateTime.Now.Subtract(framesLastCheck).TotalMilliseconds))).ToString() + "FPS ping " + ping + "ms";

            framesLastCheck = DateTime.Now;
            framesNum = 0;
            if (okonClient != null && okonClient.IsConnected())
            {
                try
                {
                    long time = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond);
                    okonClient.SendString((byte)Packet.PING, (byte)Flag.DO_NOT_LOG_PACKET, "{\"timestamp\":" + time + ",\"ping\":0}");
                }
                catch { }
            }

        }

        double time = 0;
        private void tmrHUD_Tick(object sender, EventArgs e)
        {
            //int start = DateTime.Now.Millisecond;
            int W = pictureBox1.Width;
            int H = pictureBox1.Height;
            Bitmap b = new Bitmap(W, H);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, W, H);
                //g.DrawLine(Pens.Green, 0, H / 2, W, H / 2);
                // g.DrawLine(Pens.Green, W/2, 0, W/2, H);
                Pen green = new Pen(Brushes.Red, 3);
                Pen debug = new Pen(Brushes.Aquamarine, 1);


                {//HEADING\
                    float w = 0.5f;
                    float h = 0.1f;
                    float top = 0.05f;


                    float anchorX = W/2 - W * w / 2;
                    float anchorY = H*top;

                    //g.DrawRectangle(debug, anchorX, anchorY, W * w, H * h);
                    g.DrawLine(green, anchorX, anchorY + H*h, anchorX + W*w, anchorY+H*h);

                    float textBoxH = 0.7f;
                    float fontHeight = H * h * textBoxH;
                    Font f = new Font("Inconsolata", fontHeight * 0.7f, FontStyle.Bold);

                    float rulerH = 0.3f;
                    float fontSmallH = 0.8f;
                    float fontSmallHeight = H * h * (1f - rulerH) * fontSmallH;
                    Font fsmall = new Font("Inconsolata", fontSmallHeight * 0.7f, FontStyle.Bold);

                    float hdg = ((sens.gyro.y%360f)+360f)%360f;
                    float hfov = 91.5f;
                    float lineY = anchorY + H * h;
                    float smallLine = 0.7f;
                    float bigLine = 1f;
                    for(float angle = 0f; angle < 360; angle += 5)
                    {
                        float x = W/2+(angle -hdg)/ hfov * W;
                        if (x > W/2+180f/hfov*W) x -= 360f/hfov*W;
                        if (x < W/2-180f/hfov*W) x += 360f/hfov*W;
                        if (x < anchorX || x > anchorX + W * w) continue;
                        string str = angle.ToString();
                        float lineH = smallLine/2;
                        if (angle % 10 == 0) lineH = smallLine;
                        if (angle % 90 == 0) lineH = bigLine;

                        if (angle % 10 != 0) str = "";
                        if(angle == 0) str = "N";
                        if(angle == 45) str = "NE";
                        if(angle == 90) str = "E";
                        if(angle == 135) str = "SE";
                        if(angle == 180) str = "S";
                        if(angle == 225) str = "SW";
                        if(angle == 270) str = "W";
                        if(angle == 315) str = "NW";


                        g.DrawLine(green, x, lineY, x, lineY - H * h * rulerH * lineH);
                        g.DrawString(str, fsmall, Brushes.Red, x - g.MeasureString(str, fsmall).Width / 2, lineY - H * h * rulerH * lineH - g.MeasureString(str, fsmall).Height);


                    }


                    float fontWidth = g.MeasureString("000", f).Width / 3;
                    float textBoxW = 0.1f;
                    float textBoxX = W / 2 - fontWidth * 3 / 2;
                    float textBoxY = anchorY;
                    g.DrawLine(green, W / 2, lineY, W / 2- fontWidth * 3/4, lineY - H * h + fontHeight);
                    g.DrawLine(green, W / 2, lineY, W / 2+ fontWidth * 3 / 4, lineY - H * h + fontHeight);
                    g.FillRectangle(Brushes.Black, textBoxX, textBoxY, fontWidth * 3, fontHeight);
                    g.DrawRectangle(green, textBoxX, textBoxY, fontWidth * 3, fontHeight);
                    g.DrawString(((int)hdg).ToString("000"), f, Brushes.Red, textBoxX, textBoxY);

                }



                {//ALTITUDE
                    float alt = sens.baro.pressure / 1000f / 9.81f;
                    float w = 0.1f;
                    float h = 0.7f;
                    float right = 0.01f;
                    float fontHeight = 30;
                    Font f = new Font("Inconsolata", fontHeight * 0.6f, FontStyle.Bold);


                    float anchorX = W - W * w - W * right;
                    float anchorY = H / 2 - H * h / 2;
                    //g.DrawRectangle(green, anchorX, anchorY, W * w, H * h);
                    g.DrawLine(green, anchorX, anchorY, anchorX, anchorY + H * h);
                    DrawX(anchorX, anchorX);

                    float sizePerMeter = 1f * h;
                    float step = .1f;
                    for (float depth = 0; depth < alt + h / 2 / sizePerMeter; depth += step)
                    {
                        if (depth < alt - h / 2 / sizePerMeter) continue;
                        float y = H / 2 + H * (depth - alt) * sizePerMeter;
                        g.DrawLine(green, anchorX, y, anchorX + 10, y);
                        g.DrawString(depth.ToString("0.0"), f, Brushes.Red, anchorX + 10, y - g.MeasureString(depth.ToString(), f).Height / 2f);

                    }

                    g.DrawLine(green, anchorX, anchorY + H * h / 2, anchorX+9, anchorY + H * h / 2 + fontHeight/4);
                    g.DrawLine(green, anchorX, anchorY + H * h / 2, anchorX+9, anchorY + H * h / 2 - fontHeight/4);
                    g.FillRectangle(Brushes.Black, anchorX + 9, H / 2 - fontHeight / 2, g.MeasureString("0.00", f).Width, fontHeight);
                    g.DrawRectangle(green, anchorX + 9, H / 2 - fontHeight / 2, g.MeasureString("0.00", f).Width, fontHeight);
                    g.DrawString(alt.ToString("0.0"), f, Brushes.Red, anchorX + 9, H / 2 - 12);

                }

                {//LADDER
                    float roll = sens.gyro.z;
                    float pitch = sens.gyro.x;
                    float cos = (float)+Math.Cos(ToRadians(roll));
                    float sin = (float)-Math.Sin(ToRadians(roll));
                    DrawX(W / 2, H / 2);



                    PointF GetRotated(PointF p_) => new PointF(W / 2 + p_.X * cos - p_.Y * sin, H / 2 + p_.X * sin + p_.Y * cos);
                    void DrawLadderStep(float value_, float width_)
                    {
                        g.DrawLine(green, GetRotated(new PointF(-width_ / 2, value_)), GetRotated(new PointF(width_ / 2, value_)));
                    }
                }

                {//ROLL
                    float roll = sens.gyro.z;
                    float bottom = 0.1f;
                    float r = 0.5f - bottom; // vertical
                    float centerX = W / 2;
                    float centerY = H / 2;
                    float fontHeight = 30;
                    Font f = new Font("Inconsolata", fontHeight * 0.6f, FontStyle.Bold);

                    float angleVisible = 90;
                    g.DrawArc(green, W / 2 - H * r, H / 2 - H * r, 2 * H * r, 2 * H * r, 90 - angleVisible / 2, angleVisible);

                    float bigLine = 30;
                    float smallLine = bigLine * 0.6f;
                    for (float angle = -180; angle < 180; angle += 10)
                    {
                        if ((270 - roll + angle + 360) % 360 < 270 - angleVisible / 2) continue;
                        if ((270 - roll + angle + 360) % 360 > 270 + angleVisible / 2) continue;
                        float lineLen = angle % 90 == 0 ? bigLine : smallLine;
                        if (angle < 0)
                        {
                            DrawLineOnArc(centerX, centerY, 270 + roll - angle, H * r + lineLen * 0.0f, H * r + lineLen * 0.2f);
                            DrawLineOnArc(centerX, centerY, 270 + roll - angle, H * r + lineLen * 0.4f, H * r + lineLen * 0.6f);
                            DrawLineOnArc(centerX, centerY, 270 + roll - angle, H * r + lineLen * 0.8f, H * r + lineLen * 1.0f);
                        }
                        else if (angle == 0) DrawLineOnArcBold(centerX, centerY, 270 + roll - angle, H * r - lineLen / 4, H * r + lineLen);
                        else DrawLineOnArc(centerX, centerY, 270 + roll - angle, H * r, H * r + lineLen);
                        float stringX = W / 2 + (float)Math.Cos(ToRadians(270 + roll - angle)) * (H * r + fontHeight / 2 + bigLine);
                        float stringY = H / 2 - (float)Math.Sin(ToRadians(270 + roll - angle)) * (H * r + fontHeight / 2 + bigLine);
                        g.DrawString(Math.Abs(angle).ToString(), f, Brushes.Red, stringX - g.MeasureString(Math.Abs(angle).ToString(), f).Width / 2, stringY - g.MeasureString(Math.Abs(angle).ToString(), f).Height / 2);

                    }

                    g.FillRectangle(Brushes.Black, W / 2 - g.MeasureString("-180", f).Width / 2, H - 40, g.MeasureString("-180", f).Width, 30);
                    g.DrawRectangle(green, W / 2 - g.MeasureString("-180", f).Width / 2, H - 40, g.MeasureString("-180", f).Width, 30);
                    if (roll > 180) roll -= 360f;
                    g.DrawString(roll.ToString(" 000;-000"), f, Brushes.Red, W / 2 - g.MeasureString("-180", f).Width / 2, H - 40);

                }

                {//BATTERY
                    float w = 0.2f;
                    float h = 0.05f;
                    float fill = (sens.baro.pressure % 4000)/4000;
                    float x = 0.01f;
                    float y = 0.2f;
                    float anchorX = W * x;
                    float anchorY = H * y;

                    g.DrawRectangle(green, anchorX, anchorY, W * w, H * h);
                    g.FillRectangle(Brushes.Red, anchorX, anchorY, W * w*fill, H * h);

                    float textBoxH = 0.05f;
                    float fontHeight = H * textBoxH;
                    Font f = new Font("Inconsolata", fontHeight * 0.7f, FontStyle.Bold);
                    g.DrawString("BAT 14,3V", f, Brushes.Red, anchorX, anchorY - g.MeasureString("-180", f).Height);

                }

                {//WARN
                    float x = 0.01f;
                    float y = 0.5f;
                    float anchorX = W * x;
                    float anchorY = H * y;
                    string str = "";
                    str += "TRPD ARMED" + Environment.NewLine;
                    if ((sens.baro.pressure % 4000) / 4000 > 0.9f) str += "OVERHEAT" + Environment.NewLine;
                    if((sens.baro.pressure % 4000) / 4000 < 0.1f) str += "LOW BAT" + Environment.NewLine;
                    if(sens.baro.pressure / 1000f / 9.81f > 1.5)str += "MAX PRESSURE" + Environment.NewLine;
                    float textBoxH = 0.04f;
                    float fontHeight = H * textBoxH;
                    Font f = new Font("Inconsolata", fontHeight * 0.7f, FontStyle.Bold);
                    g.DrawString(str, f, Brushes.Red, anchorX, anchorY - g.MeasureString("-180", f).Height);

                }


                double ToRadians(float a_) => (float)(a_ * Math.PI / 180.0);
                void DrawX(float x, float y) { g.DrawEllipse(Pens.Magenta, x - 4, y - 4, 8, 8); }
                void DrawLineOnArc(float x_, float y_, float a_, float s_, float e_)
                {
                    a_ *= (float)Math.PI / 180.0f;
                    g.DrawLine(green, x_ + (float)(Math.Cos(a_) * s_), y_ - (float)(Math.Sin(a_) * s_), x_ + (float)(Math.Cos(a_) * (e_)), y_ - (float)(Math.Sin(a_) * e_));
                }
                void DrawLineOnArcBold(float x_, float y_, float a_, float s_, float e_)
                {
                    Pen newPen = new Pen(green.Brush, green.Width * 2);
                    newPen.Width = green.Width * 2;
                    a_ *= (float)Math.PI / 180.0f;
                    g.DrawLine(newPen, x_ + (float)(Math.Cos(a_) * s_), y_ - (float)(Math.Sin(a_) * s_), x_ + (float)(Math.Cos(a_) * (e_)), y_ - (float)(Math.Sin(a_) * e_));
                }
            }



            b.MakeTransparent();
            time += 0.1;

            picHUD.Invoke(new Action(() =>
            {
                Image img = picHUD.Image;
                picHUD.Image = b;
                if (img != null) img.Dispose();
            }));

            //int elapsed = DateTime.Now.Millisecond - start;
            //Text = elapsed.ToString() ;

            if (okonClient != null && okonClient.IsConnected()) okonClient.SendString((byte)Packet.GET_SENS, (byte)Flag.DO_NOT_LOG_PACKET, "");
        }
    }
    public class Sensors
    {
        public Gyro gyro = new Gyro();
        public Rot_speed rot_speed = new Rot_speed();
        public Accel accel = new Accel();
        public Angular_accel angular_accel = new Angular_accel();
        public Baro baro = new Baro();
    }

    public class Gyro
    {
        public float x, y, z;
    }

    public class Accel
    {
        public float x, y, z;
    }

    public class Rot_speed
    {
        public float x, y, z;
    }

    public class Angular_accel
    {
        public float x, y, z;
    }

    public class Baro
    {
        public float pressure;
    }
}

