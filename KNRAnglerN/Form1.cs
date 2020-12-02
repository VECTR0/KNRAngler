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
    public partial class Form1 : Form
    {
        OkonClient okonClient;
        int requestedVideoFeedFrames = 0;
        int requestedDepthMapFrames = 0;

        public string Log { get { return "nothing"; } set { txtConsole.AppendText(value + Environment.NewLine); } }

        private enum Packet : byte
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

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public class Info : OkonClient.IInfo
        {
            public Form1 instance;
            public Info(Form1 instance) => this.instance = instance;
            public void YeetLog(string info)
            {
                //instance.txtConsole.AppendText("[LOG]: " + info + Environment.NewLine);
            }
            public void YeetException(Exception exp)
            {
                instance.txtConsole.AppendText("[ERR]: " + exp.Message + Environment.NewLine);
                instance.txtConsole.AppendText("[ERR]: " + exp.StackTrace + Environment.NewLine);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach(string s in Enum.GetNames(typeof(Packet))) cmbPacket.Items.Add(s);
            MessageBox.Show("Warning!\nDevelopment build, some components may not work");
            btnConnectJson.Focus();
        }

        public void HandleReceivedPacket(object o, OkonClient.PacketEventArgs e)
        {
            int maxLength = 0;
            foreach (var s in Enum.GetNames(typeof(Packet))) maxLength = Math.Max(maxLength, s.Length);
            switch ((Packet)e.packetType)
            {
                case Packet.GET_DEPTH:
                    Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + Encoding.ASCII.GetString(e.packetData, 0, e.packetData.Length);
                    var json = Utf8Json.JsonSerializer.Deserialize<dynamic>(Encoding.ASCII.GetString(e.packetData));
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(json["depth"]));
                    Image img = picDepthMap.Image;
                    picDepthMap.Image = Image.FromStream(ms);
                    if (img != null) img.Dispose();
                    break;
                case Packet.GET_DEPTH_BYTES:
                    requestedDepthMapFrames--;
                    //Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + "size: " + e.packetData.Length +"B";
                    ms = new MemoryStream(e.packetData);
                    img = picDepthMap.Image;
                    picDepthMap.Image = Image.FromStream(ms);
                    if (img != null) img.Dispose();
                    break;
                case Packet.GET_VIDEO_BYTES:
                    requestedVideoFeedFrames--;
                    //Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + "size: " + e.packetData.Length +"B";
                    ms = new MemoryStream(e.packetData);
                    img = pictureBox1.Image;
                    pictureBox1.Image = Image.FromStream(ms);
                    if (img != null) img.Dispose();
                    break;
                default:
                    Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + Encoding.ASCII.GetString(e.packetData, 0, e.packetData.Length);
                    break;
            }
        }

        private void btnConnectJson_Click(object sender, EventArgs e)
        {
            txtConsole.Clear();
            btnConnectJson.Enabled = false;
            if (okonClient != null)
            {
                if (okonClient.IsConnected())
                {
                    okonClient.Dispose();
                    okonClient = null;
                    btnConnectJson.Enabled = true;
                    btnConnectJson.Text = "Connect";
                    Log = "Disconnected";
                    return;
                }
                else okonClient.Dispose();
            }
            try {
                okonClient = new OkonClient(txtIp.Text, ushort.Parse(txtJsonPort.Text), new Info(this));
            } catch
            {
                Log = "Error, wrong parameters";
                btnConnectJson.Enabled = true;
                btnConnectJson.Text = "Connect";
                return;
            }
            okonClient.PacketReceived += HandleReceivedPacket;
            try
            {
                okonClient.Connect();
            }
            catch
            {
                okonClient.Dispose();
                okonClient = null;
                btnConnectJson.Enabled = true;
                btnConnectJson.Text = "Connect";
                return;
            }
            btnConnectJson.Text = "Disconnect";
            btnConnectJson.Enabled = true;
            Log = "Connected";
            requestedVideoFeedFrames = 0;
            requestedDepthMapFrames = 0;
            }

        private void btnSendJson_Click(object sender, EventArgs e)
        {
            int maxLength = 0;
            foreach (var s in Enum.GetNames(typeof(Packet))) maxLength = Math.Max(maxLength, s.Length);
            Log = "SENT [" + cmbPacket.Text.PadRight(maxLength) + "] " + txtJson.Text;
            try
            {
                okonClient.SendString((byte)Enum.Parse(typeof(Packet), cmbPacket.Text), txtJson.Text);
            }
            catch
            {
                Log = "Error, packet not sent";
            }
        }

        private void cmbPacket_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Enum.Parse(typeof(Packet), cmbPacket.Text))
            {
                case Packet.SET_MTR:
                    txtJson.Text = "{\"FL\":0.0,\"FR\":0.0,\"ML\":0.0,\"MR\":0.0,\"B\":0.0}";
                    break;
                case Packet.SET_ORIEN:
                    txtJson.Text = "{ \"rot\":{ \"x\":0,\"y\":0,\"z\":0},\"pos\":{ \"x\":0,\"y\":0,\"z\":0} }";
                    break;
                default:
                    txtJson.Text = "{}";
                    break;
            }
        }

        private void chkHideHUD_CheckedChanged(object sender, EventArgs e)
        {
            txtIp.Visible = chkHideHUD.Checked;
            txtVideoPort.Visible = chkHideHUD.Checked;
            txtJsonPort.Visible = chkHideHUD.Checked;
            btnConnectVideo.Visible = chkHideHUD.Checked;
            btnStartVideo.Visible = chkHideHUD.Checked;
            txtConsole.Visible = chkHideHUD.Checked;
            btnConnectJson.Visible = chkHideHUD.Checked;
            cmbPacket.Visible = chkHideHUD.Checked;
            txtJson.Visible = chkHideHUD.Checked;
            btnSendJson.Visible = chkHideHUD.Checked;
            //picDepthMap.Visible = chkHideHUD.Checked;
        }

        private void tmrFrameRate_Tick(object sender, EventArgs e)
        {
            if (okonClient != null && okonClient.IsConnected() && chkVideoFeed.Checked) {
                if (requestedVideoFeedFrames < 2)
                    try
                    {
                        okonClient.SendString((byte)Packet.GET_VIDEO_BYTES, "");
                        requestedVideoFeedFrames++;
                    }
                    catch
                    {
                        Log = "Error, packet not sent";
                        chkVideoFeed.Checked = false;
                    }
                if (requestedDepthMapFrames < 2)
                    try
                    {
                        okonClient.SendString((byte)Packet.GET_DEPTH_BYTES, "");
                        requestedDepthMapFrames++;
                    }
                    catch
                    {
                        Log = "Error, packet not sent";
                        chkVideoFeed.Checked = false;
                    }
            } 
        }
        bool[] control = { false, false, false, false, false, false};
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (okonClient == null || !okonClient.IsConnected() || !chkManualControl.Checked) return;

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
            if (okonClient == null || !okonClient.IsConnected() || !chkManualControl.Checked) return;
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
            if (!control[4] && control[5]) { fr = -0.7f;  ba = 1f; }
            float l = Math.Max(Math.Min((1 * dir + 1 * vel), 1), -1);
            float r = Math.Max(Math.Min((-1 * dir + 1 * vel), 1), -1);
            lblFrameRate.Text = "{\"FL\":" + fr.ToString().Replace(',', '.') + ",\"FR\":" + fr.ToString().Replace(',', '.') + ",\"ML\":" + l.ToString().Replace(',', '.') + ",\"MR\":" + r.ToString().Replace(',', '.') + ",\"B\":" + ba.ToString().Replace(',', '.') + "}";
            try
            {
                okonClient.SendString((byte)Packet.SET_MTR, "{\"FL\":" + fr.ToString().Replace(',','.') + ",\"FR\":" + fr.ToString().Replace(',', '.') + ",\"ML\":" + l.ToString().Replace(',', '.') + ",\"MR\":" + r.ToString().Replace(',', '.') + ",\"B\":" + ba.ToString().Replace(',', '.') + "}");
            }
            catch
            {
                chkManualControl.Checked = false;
            }
        }
    }
}

