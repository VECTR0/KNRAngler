using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KNRAngler
{
    public partial class Form1 : Form
    {
        private TcpClient videoClient, jsonClient;
        private NetworkStream videoStream, jsonStream;
        private Thread videoChannel, jsonChannel;
        private int framesPerSecond, lastFrameSize;

        private OkonClient okonClient;

        public class IInfo : OkonClient.IInfo
        {
            public Form1 instance;
            public IInfo(Form1 instance) => this.instance = instance;
            public void YeetLog(string info)
            {
                instance.txtConsole.AppendText("[LOG]: " + info + Environment.NewLine);
            }
            public void YeetException(Exception exp)
            {
                instance.txtConsole.AppendText("[ERR]: " + exp.Message + Environment.NewLine);
                instance.txtConsole.AppendText("[ERR]: " + exp.StackTrace + Environment.NewLine);
            }
        }

        private enum Packet : byte
        {
            SET_MTR = 0xA0,
            GET_SENS = 0xB0,
            SET_SIM = 0xC0,
            ACK = 0xC1,
            GET_ORIEN = 0xC2,
            SET_ORIEN = 0xC3,
            REC_STRT = 0xD0,
            REC_ST = 0xD1,
            REC_RST = 0xD2,
            GE_REC = 0xD3,
            PING = 0xC5,
            GET_DEPTH = 0xB1,
            GET_DETE = 0xDE
        }

        public string Log { get { return "nothing"; } set { txtConsole.AppendText(value + Environment.NewLine); } }

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
    
        private void BtnConnectJson_Click(object sender, EventArgs e)
        {
            if (jsonClient != null && jsonClient.Connected)
            {
                jsonStream?.Close();
                jsonClient?.Close();
                jsonStream?.Dispose();
                jsonClient?.Dispose();
                jsonChannel?.Abort();
                btnConnectJson.Enabled = true;
                btnConnectJson.Text = "Connect";
                Log = "JSON disconnected";
                return;
            }
            Log = "JSON connecting...";
            btnConnectJson.Text = "Connecting...";
            btnConnectJson.Enabled = false;
            try
            {
                //jsonClient = new TcpClient(txtIp.Text, int.Parse(txtJsonPort.Text));
                okonClient = new OkonClient(txtIp.Text, ushort.Parse(txtJsonPort.Text), new IInfo(this));
            }
            catch (Exception exception)
            {
                btnConnectJson.Enabled = true;
                btnConnectJson.Text = "Connect";
                Log = "JSON connection failed...";
                Log = exception.Message;
                btnConnectJson.Enabled = true;
                btnConnectJson.Text = "Connect";
                return;
            }
            txtConsole.Clear();
            Log = "JSON connected";
            /*jsonStream = jsonClient.GetStream();
            jsonChannel = new Thread(JsonLoop);
            jsonChannel.IsBackground = true;
            jsonChannel.Start();*/
            try
            {
                okonClient.Connect();
            }
            catch
            {
                btnConnectJson.Enabled = true;
                btnConnectJson.Text = "Connect";
                return;
            }
            btnConnectJson.Text = "Disconnect";
            btnConnectJson.Enabled = true;
        }

        private void JsonLoop()
        {
            Packet packet;
            string jsonFromServer = "{}";
            try
            {
                while (jsonClient.Connected)
                {
                    if (jsonStream.DataAvailable)
                    {
                        packet = (Packet)jsonStream.ReadByte();
                        byte[] dataLenBytes = new byte[4];
                        int check = jsonStream.Read(dataLenBytes, 0, 4);
                        if (check != 4) Log = "Fatal comunication error";
                        int dataLength = System.BitConverter.ToInt32(dataLenBytes, 0);
                        if (dataLength > 0)
                        {
                            byte[] jsonBytes = new byte[dataLength];
                            int readBytes = 0;
                            while (readBytes < dataLength)
                            {
                                int b = jsonStream.ReadByte();
                                if (b != -1) jsonBytes[readBytes++] = (byte)b;
                                else Log = "zyebalo";
                            }//TODO
                            jsonFromServer = Encoding.ASCII.GetString(jsonBytes, 0, dataLength);
                        }

                        if ((byte)packet == 0xff) { Log = "Conn lost, reconnect"; return; }
                        if (packet == Packet.GET_DEPTH)
                        {
                            var json = Utf8Json.JsonSerializer.Deserialize<dynamic>(jsonFromServer);
                            MemoryStream ms = new MemoryStream(Convert.FromBase64String(json["depth"]));
                            picDepthMap.Image = Image.FromStream(ms);
                        }
                        Log = Enum.GetName(typeof(Packet), packet) + " size: " + dataLength.ToString() + Environment.NewLine + (dataLength > 0 ? jsonFromServer : "none");
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception exp)
            {
                Log = "\n" + exp.Message + '\n' + exp.StackTrace;
                Log = "Connection lost, reconnect";
            }
        }

        private void BtnSendJson_Click(object sender, EventArgs e)
        {
            try
            {/*
                Packet packet = (Packet)Enum.Parse(typeof(Packet), cmbPacket.Text);
                string jsonToSend = txtJson.Text;
                if (packet == Packet.PING)
                {
                   jsonToSend = "{\"timestamp\":" + DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond +",\"ping\":0}";
                }
                Thread.Sleep(10);
                SendJsonPacket(packet, jsonToSend);*/
                okonClient.SendString((byte)Enum.Parse(typeof(Packet), cmbPacket.Text), txtJson.Text);
            }catch (Exception ex)
            {
                Log = "Error, wrong type selected " + ex.Message;
            }    
        }

        private void SendJsonPacket(Packet packet, string json)
        {
            if (jsonStream == null || !jsonStream.CanWrite) return;
            jsonStream.WriteByte((byte)packet);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(json);
            jsonStream.Write(System.BitConverter.GetBytes(jsonBytes.Length), 0, 4);
            if(json.Length>0)jsonStream.Write(jsonBytes, 0, jsonBytes.Length);
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void ChkHideHUD_CheckedChanged(object sender, EventArgs e)
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
            picDepthMap.Visible = chkHideHUD.Checked;
            //.Visible = chkHideHUD.Checked;
        }

        private void TmrFrameRate_Tick(object sender, EventArgs e)
        {
            lblFrameRate.Text = framesPerSecond.ToString() + " FPS " + (lastFrameSize*framesPerSecond/1000000) + "MBs" + 
                Environment.NewLine + "Image size " + lastFrameSize/1000 + "KB";
            framesPerSecond = 0;
        }

        private void BtnStartVideo_Click(object sender, EventArgs e)
        {
            videoStream?.WriteByte(0x69);
            btnStartVideo.Enabled = false;
        }

        private void BtnConnectVideo_Click(object sender, EventArgs e)
        {

            if (videoClient != null && videoClient.Connected)
            {
                videoStream?.Close();
                videoClient?.Close();
                videoStream?.Dispose();
                videoClient?.Dispose();
                videoChannel?.Abort();
                btnConnectVideo.Enabled = true;
                btnConnectVideo.Text = "Connect";
                Log = "video disconnected";
                btnStartVideo.Enabled = true;
                return;
            }
            Log = "video connecting...";
            btnConnectVideo.Text = "Connecting...";
            btnConnectVideo.Enabled = false;
            try
            {
                videoClient = new TcpClient(txtIp.Text, int.Parse(txtVideoPort.Text));
            }
            catch (Exception exception)
            {
                btnConnectVideo.Enabled = true;
                btnConnectVideo.Text = "Connect";
                Log = "video connection failed...";
                Log = exception.Message;
                return;
            }
            Log = "Video connected";
            videoStream = videoClient.GetStream();
            videoChannel = new Thread(VideoLoop);
            videoChannel.IsBackground = true;
            videoChannel.Start();

            btnConnectVideo.Text = "Disconnect";
            btnConnectVideo.Enabled = true;
        }

        private void VideoLoop()
        {
            byte[] imageBytes = new byte[1000000];
            try
            {
                while (videoClient != null && videoClient.Connected)
                {
                    do
                    {
                        byte packetType = (byte)videoStream.ReadByte();
                        byte[] dataLenBytes = new byte[4];
                        videoStream.Read(dataLenBytes, 0, 4);
                        int dataLength = System.BitConverter.ToInt32(dataLenBytes, 0);
                        
                        if (dataLength > 0 && packetType == 0x69)
                        {
                            lastFrameSize = dataLength;
                            framesPerSecond++;
                            int ptr = 0;
                            do
                            {
                                if (dataLength - ptr < videoClient.ReceiveBufferSize)
                                {
                                    int bytesRead = videoStream.Read(imageBytes, ptr, dataLength - ptr);
                                    ptr += bytesRead;
                                }
                                else
                                {
                                    int bytesRead = videoStream.Read(imageBytes, ptr, videoClient.ReceiveBufferSize);
                                    ptr += bytesRead;
                                }

                            } while (ptr != dataLength);

                            try
                            {
                                Image img;

                                MemoryStream ms = new MemoryStream(imageBytes);
                                img = Image.FromStream(ms);
                                pictureBox1.Image = img;
                            }
                            catch {}
                            Thread.Sleep(1);
                            videoStream.WriteByte(0x69);
                        }

                    } while (videoStream.DataAvailable);
                }
            }catch(Exception exp)
            {
                Log = "Connection closed, Reconnect video";
                Log = exp.Message;
            }
        }
    }
}

public class OkonClient : IDisposable
{
    public const int Version = 1;
    public event PacketEventHandler PacketReceived;
    public readonly string hostname;
    public readonly ushort port;
    private readonly IInfo _info;
    private volatile bool _connected;
    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _thread;

    public OkonClient(string hostname, ushort port, IInfo info)
    {
        this.hostname = hostname;
        this.port = port;
        this._info = info;
    }
    public void Connect()
    {
        if (_connected) throw new Exception("Already connected");
        _connected = true;
        try
        {
            _client = new TcpClient(hostname, port);
            _stream = _client.GetStream();
            _thread = new Thread(Transreceive)
            {
                Name = "Transreceive",
                IsBackground = true
            };

            _thread.Start();
        }
        catch (Exception exp)
        {
            _info.YeetException(exp);
            throw;
        }
    }
    private void Transreceive()
    {
        byte packetType;
        byte[] dataLenBytes = new byte[4];
        byte[] jsonBytes = new byte[0];
        try
        {
            while (_client.Connected)
            {
                if (_stream.DataAvailable)
                {
                    packetType = (byte)_stream.ReadByte();
                    ReadAllFromStream(_stream, dataLenBytes, 4);
                    int dataLength = System.BitConverter.ToInt32(dataLenBytes, 0);
                    if (dataLength > 0)
                    {
                        jsonBytes = new byte[dataLength];
                        ReadAllFromStream(_stream, jsonBytes, dataLength);
                    }
                    PacketReceived(this, new PacketEventArgs(packetType, jsonBytes));
                    _info.YeetLog("Received packet type: " + packetType.ToString("x2") + " len: " + dataLenBytes);
                }
                else Thread.Sleep(10);
            }
        }
        catch (Exception exp)
        {
            _info.YeetException(exp);
        }
        finally
        {
            Disconnect();
        }
    }
    public void SendBytes(byte packetType, byte[] data)
    {
        _stream.WriteByte(packetType);
        _stream.Write(BitConverter.GetBytes(data.Length), 0, 4);
        _stream.Write(data, 0, data.Length);
    }

    public void SendString(byte packetType, string data)
    {
        _stream.WriteByte(packetType);
        byte[] bytes = Encoding.ASCII.GetBytes(data);
        _stream.Write(System.BitConverter.GetBytes(bytes.Length), 0, 4);
        if (bytes.Length > 0) _stream.Write(bytes, 0, bytes.Length);
    }

    public void Disconnect()
    {
        if (!_connected) return;
        _thread?.Abort();
        _stream?.Dispose();
        _client?.Dispose();
    }

    public void Dispose() => Disconnect();

    public delegate void PacketEventHandler(object source, PacketEventArgs e);
    public class PacketEventArgs : EventArgs
    {
        public readonly byte[] packetData;
        public readonly byte packetType;
        public PacketEventArgs(byte type, byte[] data)
        {
            this.packetType = type;
            this.packetData = data;
        }
    }
    private void ReadAllFromStream(NetworkStream stream, byte[] buffer, int len)
    {
        int current = 0;
        while (current < buffer.Length)
            current += stream.Read(buffer, current, len - current > buffer.Length ? buffer.Length : len - current);
    }

    public interface IInfo
    {
        void YeetException(Exception exp);
        void YeetLog(string info);
    }
}
