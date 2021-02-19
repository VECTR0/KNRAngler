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
        HUD hud;

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
            hud = new HUD(pictureBox1.Width, pictureBox1.Height, 60, 91.5f);
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
            picHUD.Invoke(new Action(() =>
            {
                Image img = picHUD.Image;
                hud.Update(sens.gyro);
                hud.metersUnderWater = sens.baro.pressure / 4200 / 9.81f;
                picHUD.Image = hud.Generate();
                if (img != null) img.Dispose();
            }));

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

