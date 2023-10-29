using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRM;

namespace ChroMapper_CameraMovement.Component
{
    // NalulunaAvatarsEvents仕様
    // 
    // [_key]                                          [_value]
    // BlendShape           BlendShape設定             "プリセット, 値"
    // SetBlendShapeNeutral BlendShapeデフォルト変更   "プリセット, 値"
    // StartAutoBlink       AutoBlink開始 [VMCAvatarMaterialChange用]
    // StopAutoBlink        AutoBlink停止 [VMCAvatarMaterialChange用]
    // SetBlendShapeSing    口パク用のBlendShape設定                [口パクは再現しないので無効]
    // StartSing            口パク開始                              [口パクは再現しないので無効]
    // StopSing             口パク停止                              [口パクは再現しないので無効]
    // StopAutoBlendShape   自動表情機能を無効                      [ChroMapperでは不要なので無効]
    // StartAutoBlendShape  自動表情機能を有効                      [ChroMapperでは不要なので無効]
    // StopLipSync          リップシンクを一時的に無効              [ChroMapperでは不要なので無効]
    // StartLipSync         リップシンクを有効                      [ChroMapperでは不要なので無効]
    //
    // NalulunaAvatarsではAutoBlinkはBlink、BlinkL, BlinkRのプリセット時に自動停止, blendShapesNoBlinkUserで追加設定可能
    // defaultFacialExpressionTransitionSpeed　表情遷移の速度。デフォルトは10。数値が大きいほど遷移が速くなり、100でほぼ瞬間的に遷移  → プレビューなので実装しない
    public class NalulunaAvatarsEvents
    {
        public string _version { get; set; }
        public List<Events> _events { get; set; } = new List<Events>();
    }
    public class Events
    {
        public float _time { get; set; }
        public float _duration { get; set; }
        public string _key { get; set; }
        public string _value { get; set; }
    }
    public class NalulunaAvatarsEventController : MonoBehaviour
    {
        public AudioTimeSyncController _atsc { private get; set; }
        public NalulunaAvatarsEvents _nalulunaAvatarsEvents;
        public Events _timeEvent;
        public string _scriptFile;
        public bool _nalulunaAvatarsEventEnable;
        public bool _startEventSend;
        public float _beforeSeconds;
        public int eventID;
        public DateTime _scriptWriteTime = DateTime.MinValue;
        public bool _ScriptOK = false;

        private void Start()
        {
            this._scriptFile = Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, Options.Instance.blendShapeFileName).Replace("/", "\\");
            this._nalulunaAvatarsEventEnable = false;
            this._beforeSeconds = 0;
            this._nalulunaAvatarsEventEnable = this.LoadSongTimeData();
            if (!this._nalulunaAvatarsEventEnable)
                return;
            this.ResetEventID();
        }
        private void Update()
        {
            if (this._beforeSeconds == this._atsc.CurrentSeconds && this._nalulunaAvatarsEvents == null)
                return;
            this._nalulunaAvatarsEventEnable = this.LoadSongTimeData();
            if (this._beforeSeconds > this._atsc.CurrentSeconds)
            {
                this.ResetEventID();
            }
            this._beforeSeconds = this._atsc.CurrentSeconds;
            if (!this._startEventSend)
            {
                this.BlendShapeControle();
                this._startEventSend = true;
                Debug.Log($"NalulunaAvatars Event: {this._nalulunaAvatarsEventEnable}");
            }
            if (!this._nalulunaAvatarsEventEnable)
                return;
            while((this._timeEvent = this.UpdateEvent(this._atsc.CurrentSeconds)) != null)
            {
                if (this._timeEvent._key != null)
                {
                    this.BlendShapeControle();
                    Debug.Log($"NalulunaAvatars Event: Seconds{this._atsc.CurrentSeconds} : BpmTime{this._atsc.CurrentSongBpmTime}Sec: {this._timeEvent._key}");
                }
            }
        }
        private void OnDestroy()
        {
        }

        public void BlendShapeControle()
        {
            if (this._startEventSend)
            {
                VRMAvatarController.SetFace(BlendShapePreset.Neutral, 1f, true);
                return;
            }
            switch (this._timeEvent._key)
            {
                case "BlendShape":

                    break;
                case "SetBlendShapeNeutral":

                    break;
                case "StartAutoBlink":

                    break;
                case "StopAutoBlink":

                    break;
            }
            /*
            string value = data["_value"];
            bool blink = true;
            if (data["_blink"] != null)
                blink = data["_blink"];
            float strength = 1f;
            if (data["_strength"] != null)
                strength = data["_strength"];
            if (_previousValue == value && _previousStringth == strength && _previousBlink == blink)
                return;
            _previousValue = value;
            _previousStringth = strength;
            _previousBlink = blink;
            VRMAvatarController.SetFace(new List<string> { value }, new List<float> { strength }, blink);
            */
        }

        public bool LoadFromJson(string jsonString)
        {
            try
            {
                this._nalulunaAvatarsEvents = JsonConvert.DeserializeObject<NalulunaAvatarsEvents>(jsonString);
            }
            catch (Exception ex)
            {
                Debug.LogError($"NalulunaAvatarsEvents JSON file Error:{ex.Message}");
            }
            if (this._nalulunaAvatarsEvents == null)
                return false;
            return true;
        }
        public bool LoadSongTimeData()
        {
            if (!File.Exists(this._scriptFile))
                return false;
            var scriptTime = File.GetLastWriteTime(this._scriptFile);
            if (this._scriptWriteTime == scriptTime)
                return this._ScriptOK;
            this._scriptWriteTime = scriptTime;
            this._ScriptOK = false;
            var jsonText = File.ReadAllText(this._scriptFile);
            if (!this.LoadFromJson(jsonText))
                return false;
            if (this._nalulunaAvatarsEvents._events == null || this._nalulunaAvatarsEvents._events.Count == 0)
            {
                Debug.Log("No NalulunaAvatars Event data!");
                return false;
            }
            Debug.Log($"Found {this._nalulunaAvatarsEvents._events.Count} entries in: {this._scriptFile}");
            this._ScriptOK = true;
            this.ResetEventID();
            return true;
        }
        public void ResetEventID()
        {
            this._startEventSend = false;
            this.eventID = 0;
        }
        public Events UpdateEvent(float songtime)
        {
            var events = this._nalulunaAvatarsEvents._events;
            if (this.eventID >= events.Count)
                return null;
            if (events[eventID]._time <= songtime)
            {
#if DEBUG
                Debug.Log($"EventID:{eventID}");
#endif
                return events[eventID++];
            }
            else
                return null;
        }
    }
}
