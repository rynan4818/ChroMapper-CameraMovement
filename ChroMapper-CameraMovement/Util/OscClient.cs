// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

// Ocs送信機能はすのーさんのCameraPlus(https://github.com/Snow1226/CameraPlus)のソースコードをコピー・修正して使用しています。
// コピー元:https://github.com/Snow1226/CameraPlus/blob/master/CameraPlus/VMCProtocol/OscClient.cs
// CameraPlusの著作権表記・ライセンスは以下の通りです。
// https://github.com/Snow1226/CameraPlus/blob/master/LICENSE

using System;
using System.Net;
using System.Net.Sockets;

namespace ChroMapper_CameraMovement.Util
{
    /*
      追加ファイル: Util\OscClient.cs, Util\OscPacketEncoder.cs, Util\OscDataTypes.cs

      使い方

       using ChroMapper_CameraMovement.Util;

       // 1. クライアント作成（宛先IP, ポート）
       var client = new OscClient("127.0.0.1", 39550);

       // 2. 送信（様々なオーバーロード）
       client.Send("/camera/position", pos.x, pos.y, pos.z);         // float×3
       client.Send("/camera/fov", fov);                              // float×1
       client.Send("/playback/time", currentBeat);                   // float×1
       client.Send("/camera/info", "Camera", new float[] {           // string + float配列
           pos.x, pos.y, pos.z,
           rot.x, rot.y, rot.z, rot.w,
           fov
       });
       client.Send("/some/event");                                    // 引数なし
       client.Send("/some/int", 42);                                  // int×1
       client.Send("/some/text", "hello");                            // string×1

       // 3. 終了時にDispose
       client.Dispose();
    */
    public sealed class OscClient : IDisposable
    {
        public OscClient(string destination, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            if (destination == "255.255.255.255")
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            var dest = new IPEndPoint(IPAddress.Parse(destination), port);
            _socket.Connect(dest);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Packet sender methods

        public void Send(string address)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",");
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, int data)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",i");
            _encoder.Append(data);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, float data)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",f");
            _encoder.Append(data);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, float element1, float element2, float element3)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",fff");
            _encoder.Append(element1);
            _encoder.Append(element2);
            _encoder.Append(element3);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, string data)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append(",s");
            _encoder.Append(data);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        public void Send(string address, string target, float[] element)
        {
            _encoder.Clear();
            _encoder.Append(address);
            _encoder.Append($",s{new string('f', element.Length)}");
            _encoder.Append(target);
            for (int i = 0; i < element.Length; i++)
                _encoder.Append(element[i]);
            _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
        }

        #endregion

        #region IDisposable implementation

        bool _disposed;

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                _encoder = null;
            }
        }

        ~OscClient()
        {
            Dispose(false);
        }

        #endregion

        OscPacketEncoder _encoder = new OscPacketEncoder();
        Socket _socket;
    }
}
