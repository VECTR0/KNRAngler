using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNRAnglerN
{
    public class OkonClient : IDisposable
    {
        public const int Version = 3;
        public event PacketEventHandler PacketReceived;
        private readonly string _hostname;
        private readonly ushort _port;
        private readonly IInfo _info;
        private volatile bool _connected;
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _thread;
        private readonly ConcurrentQueue<Packet> _toSend;

        public OkonClient(string hostname, ushort port, IInfo info)
        {
            _hostname = hostname;
            _port = port;
            _info = info;
            _info.YeetLog("OkonClient instance created");
            _toSend = new ConcurrentQueue<Packet>();
        }
        public void Connect()
        {
            if (_connected) throw new Exception("Already connected");
            _connected = true;
            try
            {
                _client = new TcpClient(_hostname, _port);
                _stream = _client.GetStream();
                _thread = new Thread(Transreceive)
                {
                    Name = "Transreceive",
                    IsBackground = true
                };

                _thread.Start();
                _info.YeetLog("Connection successful");
            }
            catch (Exception exp)
            {
                _info.YeetException(exp);
                throw;
            }
        }
        private void Transreceive()
        {
            byte[] dataLenBytes = new byte[4];
            try
            {
                while (_client.Connected)
                {
                    if (_stream.DataAvailable)
                    {
                        byte packetType = ReadByteFromStream(_stream);
                        byte packetFlag = ReadByteFromStream(_stream);
                        ReadAllFromStream(_stream, dataLenBytes, 4);
                        int dataLength = BitConverter.ToInt32(dataLenBytes, 0);
                        byte[] dataBytes = ArrayPool<byte>.Shared.Rent(dataLength);
                        ReadAllFromStream(_stream, dataBytes, dataLength);
                        PacketReceived?.Invoke(this, new PacketEventArgs(packetType, packetFlag, dataBytes, dataLength));
                        ArrayPool<byte>.Shared.Return(dataBytes);
                       // _info.YeetLog("Received packet type: " + packetType.ToString("x2") + " flag:" + Convert.ToString(packetFlag, 2).PadLeft(8, '0') + " len: " + dataLength);
                    }
                    else
                    {
                        if (!_toSend.IsEmpty) {
                            while (!_toSend.IsEmpty)
                                if (_toSend.TryDequeue(out Packet packet))
                                {
                                    Send(packet);
                                    if(packet.rented) ArrayPool<byte>.Shared.Return(packet.bytes);
                                }
                        }
                        else Thread.Sleep(1);
                    }
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

        public void EnqueuePacket(byte packetType, byte packetFlag, string json)
        {
            byte[] bytes = ArrayPool<byte>.Shared.Rent(Encoding.ASCII.GetMaxByteCount(json.Length));
            int len = Encoding.ASCII.GetBytes(json, 0, json.Length, bytes, 0);
            _toSend.Enqueue(new Packet(packetType, packetFlag, bytes, len, true));
        }
        public void EnqueuePacket(byte packetType, string json) => EnqueuePacket(packetType, 0, json);
        public void EnqueuePacket(byte packetType, byte packetFlag, byte[] bytes, int len, bool rented) => _toSend.Enqueue(new Packet(packetType, packetFlag, bytes, len, rented));
        public void EnqueuePacket(byte packetType, byte[] bytes, int len, bool rented) => _toSend.Enqueue(new Packet(packetType, 0, bytes, len, rented));

        private void Send(Packet packet)
        {
            _stream.WriteByte(packet.type);
            _stream.WriteByte(packet.flag);
            _stream.Write(BitConverter.GetBytes(packet.length), 0, 4);
            _stream.Write(packet.bytes, 0, packet.length);
        }

        private void Disconnect()
        {
            if (!_connected) return;
            _thread?.Abort();
            _stream?.Dispose();
            _client?.Dispose();
            _info.YeetLog("Disconnected");
        }

        public void Dispose() => Disconnect();

        public delegate void PacketEventHandler(object source, PacketEventArgs e);
        public class PacketEventArgs : EventArgs
        {
            public readonly byte[] packetData;
            public readonly byte packetType;
            public readonly byte packetFlag;
            public readonly int dataLength;
            public PacketEventArgs(byte type, byte flag, byte[] data, int len)
            {
                packetType = type;
                packetFlag = flag;
                packetData = data;
                dataLength = len;
            }
        }
        private static void ReadAllFromStream(NetworkStream stream, byte[] buffer, int len)
        {
            int current = 0;
            while (current < len)
                current += stream.Read(buffer, current, len - current > len ? len : len - current);
        }

        private static byte ReadByteFromStream(Stream stream)
        {
            int ret;
            do ret = stream.ReadByte();
            while (ret == -1);
            return (byte)ret;
        }

        public interface IInfo
        {
            void YeetException(Exception exp);
            void YeetLog(string info);
        }

        public bool IsConnected() => _connected;

        private readonly struct Packet
        {   
            public readonly byte type;
            public readonly byte flag;
            public readonly byte[] bytes;
            public readonly bool rented;
            public readonly int length;

            public Packet(byte type, byte flag, byte[] bytes, int length, bool rented)
            {
                this.type = type;
                this.flag = flag;
                this.bytes = bytes;
                this.length = length;
                this.rented = rented;
            }
        }
    }
}
