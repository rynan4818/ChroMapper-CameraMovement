using System;
using UnityEngine;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Util;

namespace ChroMapper_CameraMovement.Controller
{
    public class OscController
    {
        private OscClient _client;
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 初期化処理: OSC接続を行う。
        /// 機能有効時および、別のコンポーネントの初期化(Start時)に呼び出します。
        /// </summary>
        public void Connect()
        {
            if (IsConnected)
            {
                Disconnect();
            }
            try
            {
                _client = new OscClient(Options.Instance.ocsAddress, Options.Instance.ocsPort);
                IsConnected = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[OscController] Connection failed: {e.Message}");
                IsConnected = false;
            }
        }

        /// <summary>
        /// 送信処理: カメラ座標・角度・FOV・再生位置を引数に受け取って接続中なら送信します。
        /// 未接続状態ならfalseを返します。
        /// </summary>
        public bool SendState(Vector3 pos, Quaternion rot, float fov, float currentBeat)
        {
            if (!IsConnected || _client == null)
            {
                return false;
            }
            try
            {
                _client.Send("/camera/info", "Camera", new float[] {
                    pos.x, pos.y, pos.z,
                    rot.x, rot.y, rot.z, rot.w,
                    fov, currentBeat
                });
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[OscController] Send failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 終了処理: 機能不要時および、別のコンポーネントの終了時(Dispose時)に呼び出します。
        /// </summary>
        public void Disconnect()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
            IsConnected = false;
        }
    }
}
