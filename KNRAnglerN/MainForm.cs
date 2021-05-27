using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SharpDX.DirectInput;
using Utf8Json;

namespace KNRAnglerN
{
    public partial class MainForm : Form
    {
        private Sensors _sens = new Sensors();

        private const string Ver = "7.3";
        public readonly ConsoleForm consoleForm;
        public readonly SettingsForm settingsForm;
        public readonly CloudForm cloudForm;
        public OkonClient okonClient;
        public int requestedVideoFeedFrames;
        public int requestedDepthMapFrames;
        private int _framesNum;
        private int _ping;
        private DateTime _framesLastCheck = DateTime.Now;
        private HUD _hud;

        //Controller
        private DirectInput directInput = new DirectInput();
        private JoystickState joystickState = new JoystickState();
        private Guid joystickGuid = Guid.Empty;
        private Joystick joystick;
        //

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
            RST_SIM = 0xC4,
            PING = 0xC5,
            GET_CPS = 0xC6,
            HIT_NGZ = 0xC7,
            HIT_FZ = 0xC8,
            CHK_AP = 0xC9,
            REC_STRT = 0xD0,
            REC_ST = 0xD1,
            REC_RST = 0xD2,
            GET_REC = 0xD3,
            GET_DETE = 0xDE
        }

        [Flags]
        private enum Flag
        {
            NONE = 0,
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
            Text = "KNR Wędkarz - Okoń Sim control v" + Ver + " by Vectro 2021";
            /*cloudForm = new CloudForm(this) { Visible = true };
            cloudForm.Hide();*/
           
            //DInput
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
            {
                joystickGuid = deviceInstance.InstanceGuid;
                consoleForm.txtConsole.AppendText("Found: " + deviceInstance.InstanceName + Environment.NewLine);
            }
            if (joystickGuid == Guid.Empty) return;
            joystick = new Joystick(directInput, joystickGuid);
            joystick.Properties.BufferSize = 128;
            joystick.Acquire();
            int axes = joystick.Capabilities.AxeCount;
            int buttons = joystick.Capabilities.ButtonCount;
            int povs = joystick.Capabilities.PovCount;

            consoleForm.txtConsole.AppendText("used: " + joystick.Information.ProductName + Environment.NewLine);
            consoleForm.txtConsole.AppendText("axes:" + axes + " buttons:" + buttons + " povs:" + povs + Environment.NewLine);
        }

        public class Info : OkonClient.IInfo
        {
            private readonly MainForm _mainFormInstance;
            public Info(MainForm instance) => _mainFormInstance = instance;
            public void YeetLog(string info)
            {
                if (_mainFormInstance.settingsForm.chkYeetLog.Checked)
                    _mainFormInstance.consoleForm.txtConsole.AppendText("[LOG]: " + info + Environment.NewLine);
            }
            public void YeetException(Exception exp)
            {
                try
                {
                    if (exp.Message.ToLower().Contains("aborted")) return;
                    _mainFormInstance.consoleForm.txtConsole.AppendText("[ERR]: " + exp.Message + Environment.NewLine);
                    _mainFormInstance.consoleForm.txtConsole.AppendText("[ERR]: " + exp.StackTrace + Environment.NewLine);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //  this.BeginInvoke((Action)(() => MessageBox.Show("Warning!\nDevelopment build, some components may not work"))); 
            pictureBox1.Controls.Add(picHUD);
            picHUD.BackColor = Color.Transparent;
            _hud = new HUD(pictureBox1.Width, pictureBox1.Height, 60, 91.5f);
        }

        public void HandleReceivedPacket(object o, OkonClient.PacketEventArgs e)
        {
            int maxLength = 0;
            foreach (var s in Enum.GetNames(typeof(Packet))) maxLength = Math.Max(maxLength, s.Length);
            switch ((Packet)e.packetType)
            {
               case Packet.GET_DEPTH:
                    consoleForm.Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + Encoding.ASCII.GetString(e.packetData, 0, e.dataLength);
                    var json = JsonSerializer.Deserialize<dynamic>(Encoding.ASCII.GetString(e.packetData, 0, e.dataLength));
                    /* using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(json["depth"])))
                     {
                         picDepthMap.Invoke(new Action(() =>
                          {
                              Image img = picDepthMap.Image;
                              picDepthMap.Image = Image.FromStream(ms);
                              if (img != null) img.Dispose();
                          }));
                     }*/
                    break;
                case Packet.GET_DEPTH_BYTES:
                    requestedDepthMapFrames--;
                    //consoleForm.Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + "size: " + e.dataLength +"B";
                    using (var ms = new MemoryStream(e.packetData, 0, e.dataLength))
                    {
                        picDepthMap.Invoke(new Action(() =>
                         {
                             Image img = picDepthMap.Image;
                             picDepthMap.Image = Image.FromStream(ms);
                             if (img != null) img.Dispose();
                         }));
                    }
                    break;
                case Packet.GET_VIDEO_BYTES:
                    requestedVideoFeedFrames--;
                    //consoleForm.Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + "size: " + e.dataLength +"B";
                    using (var ms = new MemoryStream(e.packetData, 0, e.dataLength))
                    {
                        pictureBox1.Invoke(new Action(() =>
                        {
                            Image img = pictureBox1.Image;
                            pictureBox1.Image = Image.FromStream(ms);
                            if (img != null) img.Dispose();
                        }));
                        _framesNum++;
                    }
                    break;
                case Packet.GET_SENS:
                    try
                    {
                        _sens = JsonSerializer.Deserialize<Sensors>(e.packetData);
                    }//TODO fix invalid string parse double
                    catch { }
                    break;
                case Packet.PING:
                    json = JsonSerializer.Deserialize<dynamic>(Encoding.ASCII.GetString(e.packetData, 0, e.dataLength));
                    _ping = (int)json["ping"];
                    break;
                case Packet.HIT_NGZ:
                    json = JsonSerializer.Deserialize<dynamic>(Encoding.ASCII.GetString(e.packetData, 0, e.dataLength));
                    consoleForm.Log = "HIT NGZ " + (string)json["id"];
                    break;
                case Packet.HIT_FZ:
                    json = JsonSerializer.Deserialize<dynamic>(Encoding.ASCII.GetString(e.packetData, 0, e.dataLength));
                    consoleForm.Log = "HIT FZ " + (string)json["id"];
                    break;
                default:
                   consoleForm.Log = " RECV[" + Enum.GetName(typeof(Packet), e.packetType).PadRight(maxLength) + "] " + Encoding.ASCII.GetString(e.packetData, 0, e.dataLength);
                    break;
            }
        }

        private void tmrFrameRate_Tick(object sender, EventArgs e)
        {
            if (okonClient != null && okonClient.IsConnected() && settingsForm.chkVideoFeed.Checked)
            {
                try
                {
                    if (requestedVideoFeedFrames < 2)
                    {
                        okonClient.EnqueuePacket((byte)Packet.GET_VIDEO_BYTES, (byte)Flag.DO_NOT_LOG_PACKET, "");
                        requestedVideoFeedFrames++;
                    }
                    
                    if (requestedDepthMapFrames < 2)
                    {
                        okonClient.EnqueuePacket((byte)Packet.GET_DEPTH_BYTES, (byte)Flag.DO_NOT_LOG_PACKET, "");
                        requestedDepthMapFrames++;
                    }
                }
                catch
                {
                    consoleForm.Log = "Error, packet not sent in tmrVideoFeed";
                    settingsForm.chkVideoFeed.Checked = false;
                }
            }
        }

        private readonly HashSet<Keys> _keys = new HashSet<Keys>();

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _keys.Add(e.KeyCode);
            if (e.KeyCode == Keys.H) _joystickEnabled = !_joystickEnabled;
            if (okonClient == null || !okonClient.IsConnected() || !settingsForm.chkManualControl.Checked) return;
            UpdateManualControl();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            _keys.Remove(e.KeyCode);
            if (okonClient == null || !okonClient.IsConnected() || !settingsForm.chkManualControl.Checked) return;
            UpdateManualControl();
        }

        private void UpdateManualControl()
        {//W A S D  Q E
            float forward = 0, right = 0, up = 0, yaw = 0, pitch = 0, roll = 0;
            if (_keys.Contains(Keys.W)) forward += .7f;
            if (_keys.Contains(Keys.S)) forward -= .7f;
            if (_keys.Contains(Keys.A)) right -= .7f;
            if (_keys.Contains(Keys.D)) right += .7f;
            if (_keys.Contains(Keys.ShiftKey)) up += 0.5f;
            if (_keys.Contains(Keys.ControlKey)) up -= 0.5f;
            if (_keys.Contains(Keys.Q)) yaw -= 0.5f;
            if (_keys.Contains(Keys.E)) yaw += 0.5f;
            if (_keys.Contains(Keys.I)) pitch -= 0.5f;
            if (_keys.Contains(Keys.K)) pitch += 0.5f;
            if (_keys.Contains(Keys.J)) roll -= 0.5f;
            if (_keys.Contains(Keys.L)) roll += 0.5f;

            //float l = Math.Max(Math.Min((.5f * dir + 1 * vel), 1), -1);
            //float r = Math.Max(Math.Min((-.5f * dir + 1 * vel), 1), -1);
           /* if (joystick != null)
            {
               
               joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                    Console.WriteLine(state);
                consoleForm.txtConsole.AppendText("x" + joystick.ToString() + Environment.NewLine);
            }*/

            try
            {
                if (_keys.Contains(Keys.T)) {
                    okonClient.EnqueuePacket((byte)Packet.SET_MTR, (byte)Flag.DO_NOT_LOG_PACKET,
                        "{\"FLH\":" + (0.5).ToString().Replace(',', '.') +
                        ",\"FLV\":" + (0.5).ToString().Replace(',', '.') +
                        ",\"BLV\":" + (0.5).ToString().Replace(',', '.') +
                        ",\"BLH\":" + (0.5).ToString().Replace(',', '.') +
                        ",\"FRH\":" + (0.5).ToString().Replace(',', '.') +
                        ",\"FRV\":" + (0.5).ToString().Replace(',', '.') +
                        ",\"BRV\":" + (0.5).ToString().Replace(',', '.') +
                        ",\"BRH\":" + (0.5).ToString().Replace(',', '.') +
                        "}");
                }
                else
                {
                    okonClient.EnqueuePacket((byte)Packet.SET_MTR, (byte)Flag.DO_NOT_LOG_PACKET,
                    "{\"FLH\":" + (forward + right + yaw).ToString().Replace(',', '.') +
                    ",\"FLV\":" + (up + pitch + roll).ToString().Replace(',', '.') +
                    ",\"BLV\":" + (up - pitch + roll).ToString().Replace(',', '.') +
                    ",\"BLH\":" + (-forward + right - yaw).ToString().Replace(',', '.') +
                    ",\"FRH\":" + (forward - right - yaw).ToString().Replace(',', '.') +
                    ",\"FRV\":" + (up + pitch - roll).ToString().Replace(',', '.') +
                    ",\"BRV\":" + (up - pitch - roll).ToString().Replace(',', '.') +
                    ",\"BRH\":" + (-forward - right + yaw).ToString().Replace(',', '.') +
                    "}");
                }
            }
            catch
            {
                settingsForm.chkManualControl.Checked = false;
            }
        }

        private void UpdateMotors(float forward = 0, float right = 0, float up = 0, float yaw = 0, float pitch = 0, float roll = 0)
        {
            try
            {
                okonClient.EnqueuePacket((byte)Packet.SET_MTR, (byte)Flag.DO_NOT_LOG_PACKET,
                    "{\"FLH\":" + (forward + right + yaw).ToString().Replace(',', '.') +
                    ",\"FLV\":" + (up + pitch + roll).ToString().Replace(',', '.') +
                    ",\"BLV\":" + (up - pitch + roll).ToString().Replace(',', '.') +
                    ",\"BLH\":" + (-forward + right - yaw).ToString().Replace(',', '.') +
                    ",\"FRH\":" + (forward - right - yaw).ToString().Replace(',', '.') +
                    ",\"FRV\":" + (up + pitch - roll).ToString().Replace(',', '.') +
                    ",\"BRV\":" + (up - pitch - roll).ToString().Replace(',', '.') +
                    ",\"BRH\":" + (-forward - right + yaw).ToString().Replace(',', '.') +
                    "}");
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

        private void btnShowCloud_Click(object sender, EventArgs e)
        {
            if (cloudForm.Visible) cloudForm.Hide();
            else cloudForm.Show();
        }

        private void tmrFramerate_Tick_1(object sender, EventArgs e)
        {
            lblFrameRate.Text = Math.Round((1000.0 * _framesNum / (DateTime.Now.Subtract(_framesLastCheck).TotalMilliseconds))) + "FPS ping " + _ping + "ms";

            _framesLastCheck = DateTime.Now;
            _framesNum = 0;
            if (okonClient != null && okonClient.IsConnected()) okonClient.EnqueuePacket((byte)Packet.PING, (byte)Flag.DO_NOT_LOG_PACKET, "{\"timestamp\":" + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + ",\"ping\":0}");
        }//TODO check if try catch was needed

        private void tmrHUD_Tick(object sender, EventArgs e)
        {
            picHUD.Invoke(new Action(() =>
            {
                Image img = picHUD.Image;
                _hud.Update(_sens.gyro);
                _hud.metersUnderWater = _sens.baro.pressure / 4200 / 9.81f;
                picHUD.Image = _hud.Generate();
                if (img != null) img.Dispose();
            }));

            if (okonClient != null && okonClient.IsConnected()) okonClient.EnqueuePacket((byte)Packet.GET_SENS, (byte)Flag.DO_NOT_LOG_PACKET, "");
        }

        private bool _joystickEnabled = true;
        private void tmrGamepad_Tick(object sender, EventArgs e)
        {
            if (joystick == null) return;
            joystick.GetCurrentState(ref joystickState);

            float X = RemapValue(joystickState.X);
            float Y = RemapValue(joystickState.Y);
            float Z = RemapValue(joystickState.Z);
    
            float RX = RemapValue(joystickState.RotationX);
            float RY = RemapValue(joystickState.RotationY);

            float btns = 0;
            if (joystickState.Buttons[4]) btns -= 0.4f;
            if (joystickState.Buttons[5]) btns += 0.4f;

            if (!(okonClient == null || !okonClient.IsConnected() || !settingsForm.chkManualControl.Checked || !_joystickEnabled))
                UpdateMotors(-Z, btns, -Y, X, RY, RX);
            //old  UpdateMotors(-Y, X, -Z, RX*0.8f, RY, roll);

            if (joystickState.Buttons[2])
            {
                var sa = new List<int>();
                foreach (var jo in Enum.GetValues(typeof(JoystickOffset)))
                {
                    try
                    {
                        var mightGoBoom = joystick.GetObjectInfoByName(jo.ToString());
                        consoleForm.txtConsole.AppendText(jo + Environment.NewLine);
                    }
                    catch { }
                }
            }
        }

        private float RemapValue(int value)
        {
            float deadzone = 0.05f;
            float raw = (2 * value) / 65536f - 1f;
            if (Math.Abs(raw) < deadzone) return 0;
            if (raw > 0) return (raw - deadzone) / (1f - deadzone);
            return (raw + deadzone) / (1f - deadzone) ;
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

