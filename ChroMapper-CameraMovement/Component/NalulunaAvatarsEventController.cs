using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    // defaultFacialExpressionTransitionSpeed　表情遷移の速度。デフォルトは10。数値が大きいほど遷移が速くなり、100でほぼ瞬間的に遷移
    // 100で瞬間なら1フレーム=1/60=0.016s  10=0.16s  => 1.66 / defaultFacialExpressionTransitionSpeed
    public class NalulunaAvatarsEvents
    {
        public string _version { get; set; }
        public List<Events> _events { get; set; } = new List<Events>();
        public EventSetting _settings { get; set; } = new EventSetting();
    }
    public class Events
    {
        public float _time { get; set; }
        public float _duration { get; set; }
        public string _key { get; set; }
        public string _value { get; set; }
    }
    public class EventSetting
    {
        public List<string> blendShapesNoBlinkUser { get; set; } = new List<string>();
        public float defaultFacialExpressionTransitionSpeed { get; set; } = NalulunaAvatarsEventController.DefaultTransitionSpeed;
        public List<string> noDefaultBlendShapeChangeKeys { get; set; } = new List<string>();
    }
    public class NalulunaAvatarsEventController : MonoBehaviour
    {
        public NalulunaAvatarsEvents _nalulunaAvatarsEvents;
        public string _scriptFile;
        public bool _nalulunaAvatarsEventEnable;
        public DateTime _scriptWriteTime;
        public bool _scriptOK;
        public float _checkTime;
        public static readonly List<BlendShapeKey> DefaultNoDefaultBlendShapeChangeKeys = new List<BlendShapeKey>()
        {
            BlendShapeKey.CreateFromPreset(BlendShapePreset.A),
            BlendShapeKey.CreateFromPreset(BlendShapePreset.I),
            BlendShapeKey.CreateFromPreset(BlendShapePreset.U),
            BlendShapeKey.CreateFromPreset(BlendShapePreset.E),
            BlendShapeKey.CreateFromPreset(BlendShapePreset.O)
        };
        public static readonly List<BlendShapeKey> DefaultNoBlinkKeys = new List<BlendShapeKey>()
        {
            BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink),
            BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_L),
            BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_R),
        };
        public static readonly float DefaultTransitionSpeed = 10f;
        public List<BlendShapeKey> noDefaultBlendShapeChangeKeys = DefaultNoDefaultBlendShapeChangeKeys;
        public List<BlendShapeKey> noBlinkKeys = DefaultNoBlinkKeys;
        public float transitionSpeed = DefaultTransitionSpeed;

        private void Start()
        {
            this._checkTime = 0;
            this._scriptFile = Path.Combine(BeatSaberSongContainer.Instance.Info.Directory, Options.Instance.nalulunaAvatarsEventFileName).Replace("/", "\\");
            this.ResetEventData();
        }
        private void Update()
        {
            if (VRMAvatarController.avatar == null || !VRMAvatarController.avatar.Root.activeSelf)
                return;
            this._checkTime += Time.deltaTime;
            if (this._checkTime < 0.5f)
                return;
            this._checkTime = 0;
            this._nalulunaAvatarsEventEnable = this.LoadEventData();
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
        public bool LoadEventData()
        {
            if (!File.Exists(this._scriptFile))
                return false;
            var scriptTime = File.GetLastWriteTime(this._scriptFile);
            if (this._scriptWriteTime == scriptTime)
                return this._scriptOK;
            this._scriptWriteTime = scriptTime;
            this._scriptOK = false;
            var jsonText = File.ReadAllText(this._scriptFile);
            this._nalulunaAvatarsEvents = null;
            if (!this.LoadFromJson(jsonText))
                return false;
            if (this._nalulunaAvatarsEvents._events == null || this._nalulunaAvatarsEvents._events.Count == 0)
            {
                Debug.Log("No NalulunaAvatars Event data!");
                return false;
            }
            noBlinkKeys = DefaultNoBlinkKeys;
            noDefaultBlendShapeChangeKeys = DefaultNoDefaultBlendShapeChangeKeys;
            this.transitionSpeed = DefaultTransitionSpeed;
            if (this._nalulunaAvatarsEvents._settings != null)
            {
                var blendShapesNoBlinkUser = this._nalulunaAvatarsEvents._settings.blendShapesNoBlinkUser;
                if (blendShapesNoBlinkUser != null && blendShapesNoBlinkUser.Count > 0)
                {
                    foreach (var a in blendShapesNoBlinkUser)
                    {
                        var b = VRMAvatarController.blendShapeController.GetBlendShapeKeys(new List<string>() { a.Trim().ToUpper() }, new List<float>() { 0 }).Item1;
                        if (b.Count > 0)
                            noBlinkKeys.Add(b[0]);
                    }
                }
                var noChangeKeys = this._nalulunaAvatarsEvents._settings.noDefaultBlendShapeChangeKeys;
                if (noChangeKeys != null && noChangeKeys.Count > 0)
                {
                    foreach (var a in noChangeKeys)
                    {
                        var b = VRMAvatarController.blendShapeController.GetBlendShapeKeys(new List<string>() { a.Trim().ToUpper() }, new List<float>() { 0 }).Item1;
                        if (b.Count > 0)
                            noDefaultBlendShapeChangeKeys.Add(b[0]);
                    }
                }
                if (this._nalulunaAvatarsEvents._settings.defaultFacialExpressionTransitionSpeed > 0f)
                {
                    this.transitionSpeed = this._nalulunaAvatarsEvents._settings.defaultFacialExpressionTransitionSpeed;
                }
            }
            this._nalulunaAvatarsEvents._events.Sort((a, b) => a._time.CompareTo(b._time));
            var tempEventData = new List<(float, string, BlendShapeKey, float)>();
            (List<BlendShapeKey>, List<float>) blendValue;
            foreach (var eventData in this._nalulunaAvatarsEvents._events)
            {
                switch (eventData._key)
                {
                    case "BlendShape":
                        blendValue = GetValue(eventData._value);
                        if (blendValue.Item1.Count > 0 && eventData._duration > 0)
                        {
                            tempEventData.Add((eventData._time, eventData._key, blendValue.Item1[0], blendValue.Item2[0]));
                            tempEventData.Add((eventData._time + eventData._duration, eventData._key, blendValue.Item1[0], 0f));
                        }
                        break;
                    case "SetBlendShapeNeutral":
                        blendValue = GetValue(eventData._value);
                        if (blendValue.Item1.Count > 0)
                            tempEventData.Add((eventData._time, eventData._key, blendValue.Item1[0], blendValue.Item2[0]));
                        break;
                    case "StartAutoBlink":
                    case "StopAutoBlink":
                        tempEventData.Add((eventData._time, eventData._key, new BlendShapeKey(), 0));
                        break;
                }
            }
            tempEventData.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            var defaultBlendShapeKey = BlendShapeKey.CreateFromPreset(BlendShapePreset.Neutral);
            var defalutBlendShapeValue = 1f;
            var blendShapeController = VRMAvatarController.blendShapeController;
            var currentClips = new Dictionary<BlendShapeKey, float>();
            var beforeClips = new Dictionary<BlendShapeKey, float>();
            var beforeBlink = Options.Instance.avatarBlinker;
            var autoBlink = Options.Instance.avatarBlinker;
            var noBlink = false;
            (BlendShapeKey, float, bool) defalutClips;
            (BlendShapeKey, float, bool) beforeDefalutClips = (defaultBlendShapeKey, defalutBlendShapeValue, true);
            foreach (var a in blendShapeController.blendShapeClips)
                currentClips[a.Key] = 0f;
            var currentTime = 0f;
            var songTimeBlendShapeKeys = new List<(float, Dictionary<BlendShapeKey, float>, bool)>();
            tempEventData.Add((float.MaxValue, "", new BlendShapeKey(), 0));
            foreach (var eventData in tempEventData)
            {
                if (currentTime < eventData.Item1)
                {
                    var defaultChange = true;
                    foreach (var a in currentClips)
                    {
                        if (a.Value > 0f && !noDefaultBlendShapeChangeKeys.Contains(a.Key))
                        {
                            defaultChange = false;
                            break;
                        }
                    }
                    defalutClips = (defaultBlendShapeKey, defalutBlendShapeValue, defaultChange);
                    var outClips = currentClips.ToDictionary(x => x.Key, x => x.Value);
                    if (defalutClips.Item3)
                        outClips[defalutClips.Item1] = defalutClips.Item2;
                    noBlink = false;
                    foreach (var checkClips in outClips)
                    {
                        if (noBlinkKeys.Contains(checkClips.Key) && checkClips.Value > 0f)
                        {
                            noBlink = true;
                            break;
                        }
                    }
                    if (currentTime > 0f)
                    {
                        var clipChange = false;
                        foreach (var a in currentClips)
                        {
                            if (!(beforeClips.ContainsKey(a.Key) && beforeClips[a.Key] == a.Value))
                            {
                                clipChange = true;
                                var beforeOutClips = beforeClips.ToDictionary(x => x.Key, x => x.Value);
                                if (beforeDefalutClips.Item3)
                                    beforeOutClips[beforeDefalutClips.Item1] = beforeDefalutClips.Item2;
                                songTimeBlendShapeKeys.Add((currentTime, beforeOutClips.ToDictionary(x => x.Key, x => x.Value), beforeBlink && !noBlink && autoBlink));
                                currentTime += 1.66f / this.transitionSpeed;
                                break;
                            }
                        }
                        if (!clipChange)
                        {
                            if (beforeDefalutClips.Item1.Name != defalutClips.Item1.Name || beforeDefalutClips.Item2 != defalutClips.Item2)
                            {
                                var beforeOutClips = beforeClips.ToDictionary(x => x.Key, x => x.Value);
                                if (beforeDefalutClips.Item3)
                                    beforeOutClips[beforeDefalutClips.Item1] = beforeDefalutClips.Item2;
                                songTimeBlendShapeKeys.Add((currentTime, beforeOutClips.ToDictionary(x => x.Key, x => x.Value), beforeBlink && !noBlink && autoBlink));
                                currentTime += 1.66f / this.transitionSpeed;
                            }
                        }
                    }
                    songTimeBlendShapeKeys.Add((currentTime, outClips, !noBlink && autoBlink));
                    if (currentTime < eventData.Item1)
                        currentTime = eventData.Item1;
                    beforeBlink = !noBlink && autoBlink;
                    beforeClips = currentClips.ToDictionary(x => x.Key, x => x.Value);
                    beforeDefalutClips = defalutClips;
                }
                switch (eventData.Item2)
                {
                    case "BlendShape":
                        currentClips[eventData.Item3] = eventData.Item4;
                        break;
                    case "SetBlendShapeNeutral":
                        defaultBlendShapeKey = eventData.Item3;
                        defalutBlendShapeValue = eventData.Item4;
                        break;
                    case "StartAutoBlink":
                        autoBlink = true;
                        break;
                    case "StopAutoBlink":
                        autoBlink = false;
                        break;
                }
            }
            songTimeBlendShapeKeys.Sort((a, b) => a.Item1.CompareTo(b.Item1));
#if DEBUG
            foreach (var key in songTimeBlendShapeKeys)
                Debug.Log($"{key.Item1:0.000}s:{VRMAvatarController.blendShapeController.DebugBlendShapeKyes(key.Item2)}:{key.Item3}");
#endif
            VRMAvatarController.blendShapeController.songTimeBlendShapeKeys = songTimeBlendShapeKeys;
            VRMAvatarController.blendShapeController.SongTimeReset();
            Debug.Log($"Found {this._nalulunaAvatarsEvents._events.Count} entries in: {this._scriptFile} :{songTimeBlendShapeKeys.Count}");
            this._scriptOK = true;
            return true;
        }
        public (List<BlendShapeKey>, List<float>) GetValue(string value)
        {
            var setData = value.Split(',');
            var strength = 1f;
            if (setData.Length >= 2 && !float.TryParse(setData[1].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out strength))
                strength = 1f;
            return VRMAvatarController.blendShapeController.GetBlendShapeKeys(new List<string>() { setData[0].Trim().ToUpper() }, new List<float>() { strength });
        }

        public void ResetEventData()
        {
            this._scriptWriteTime = DateTime.MinValue;
            this._scriptOK = false;
            this._nalulunaAvatarsEventEnable = false;
        }
    }
}
