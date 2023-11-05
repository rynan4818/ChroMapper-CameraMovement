// BlendShape周りは、あきらさんのVirtualMotionCapture(https://github.com/sh-akira/VirtualMotionCapture)のソースコードをコピー・修正して使用しています。
// コピー元:https://github.com/sh-akira/VirtualMotionCapture/blob/master/Assets/Scripts/Avatar/FaceController.cs
// VirtualMotionCaptureの著作権表記・ライセンスは以下の通りです。
// Copyright (c) 2018 sh-akira
// MIT LICENSE https://github.com/sh-akira/VirtualMotionCapture/blob/master/LICENSE

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using VRM;
using ChroMapper_CameraMovement.Configuration;
using System.Text;

namespace ChroMapper_CameraMovement.Controller
{
    public class BlendShapeController
    {
        public VRMBlendShapeProxy m_proxy { set; get; }
        public Blinker m_blink { set; get; }
        public List<BlendShapeClip> blendShapeClips { set; get; }
        public Dictionary<string, BlendShapeKey> blendShapeKeyString { set; get; }
        public Dictionary<string, string> keyUpperCaseDictionary { set; get; }
        public List<(float, Dictionary<BlendShapeKey, float>, bool)> songTimeBlendShapeKeys { set; get; }
        public float nextSongTime { set; get; }
        public int eventID { set; get; }
        public List<BlendShapeKey> beforeKeys { get; set; }
        public List<float> beforeStrength { get; set; }
        public bool beforeBlink { get; set; } = Options.Instance.avatarBlinker;
        public int beforeEventID { set; get; } = -1;

        public void Setup()
        {
            if (m_proxy == null)
                return;
            blendShapeClips = m_proxy.BlendShapeAvatar.Clips;
            blendShapeKeyString = new Dictionary<string, BlendShapeKey>();
            keyUpperCaseDictionary = new Dictionary<string, string>();
            foreach (var clip in blendShapeClips)
            {
                Debug.Log($"{clip.Key}:{clip.BlendShapeName}");
                if (clip.Preset == BlendShapePreset.Unknown)
                {
                    blendShapeKeyString[clip.BlendShapeName] = clip.Key;
                    keyUpperCaseDictionary[clip.BlendShapeName.ToUpper()] = clip.BlendShapeName;
                }
                else
                {
                    blendShapeKeyString[clip.Preset.ToString()] = clip.Key;
                    keyUpperCaseDictionary[clip.Preset.ToString().ToUpper()] = clip.Preset.ToString();
                }
            }
            SetFace(BlendShapePreset.Neutral, 1f);
        }

        public void SongTimeReset()
        {
            this.nextSongTime = 0;
            this.eventID = 0;
        }

        public string DebugBlendShapeKyes(Dictionary<BlendShapeKey, float> a)
        {
            var builder = new StringBuilder(100);
            foreach(var key in a)
                builder.Append($"{key.Key}={key.Value},");
            return builder.ToString();
        }

        public void SongTimeUpdate(float currentSeconds)
        {
            if (songTimeBlendShapeKeys == null || songTimeBlendShapeKeys.Count == 0)
                return;
            var setBlendShapeKeys = new List<BlendShapeKey>();
            var setValues = new List<float>();
            bool blink;
            if (songTimeBlendShapeKeys.Count == 1)
            {
                foreach (var setBlendShape in songTimeBlendShapeKeys[0].Item2)
                {
                    setBlendShapeKeys.Add(setBlendShape.Key);
                    setValues.Add(setBlendShape.Value);
                }
#if DEBUG
                var log1 = SetFace(setBlendShapeKeys, setValues);
                if (log1 != "")
                    Debug.Log($"{currentSeconds}s :{log1}");
#endif
                if (this.beforeBlink != songTimeBlendShapeKeys[0].Item3 && VRMAvatarController.m_blink != null)
                {
#if DEBUG
                    Debug.Log($"{currentSeconds}s :AutoBlink {songTimeBlendShapeKeys[0].Item3}");
#endif
                    VRMAvatarController.m_blink.enabled = songTimeBlendShapeKeys[0].Item3;
                    this.beforeBlink = songTimeBlendShapeKeys[0].Item3;
                }
                return;
            }
            this.nextSongTime = songTimeBlendShapeKeys[eventID + 1].Item1;
            while (this.nextSongTime <= currentSeconds)
            {
                eventID++;
                if (eventID + 1 >= songTimeBlendShapeKeys.Count)
                {
                    eventID = songTimeBlendShapeKeys.Count - 2;
                    this.nextSongTime = songTimeBlendShapeKeys[eventID + 1].Item1;
                    break;
                }
                this.nextSongTime = songTimeBlendShapeKeys[eventID + 1].Item1;
            }
#if DEBUG
            if (this.beforeEventID != eventID)
            {
                Debug.Log($"Start:{songTimeBlendShapeKeys[eventID].Item1:0.000}:{this.DebugBlendShapeKyes(songTimeBlendShapeKeys[eventID].Item2)}:{songTimeBlendShapeKeys[eventID].Item3}");
                Debug.Log($"End  :{songTimeBlendShapeKeys[eventID + 1].Item1:0.000}:{this.DebugBlendShapeKyes(songTimeBlendShapeKeys[eventID + 1].Item2)}:{songTimeBlendShapeKeys[eventID + 1].Item3}");
                this.beforeEventID = eventID;
            }
#endif
            if (currentSeconds < this.nextSongTime)
            {
                blink = songTimeBlendShapeKeys[eventID].Item3;
                foreach (var startBlendShape in songTimeBlendShapeKeys[eventID].Item2)
                {
                    if (songTimeBlendShapeKeys[eventID + 1].Item2.ContainsKey(startBlendShape.Key))
                    {
                        var nextBlendShape = songTimeBlendShapeKeys[eventID + 1].Item2[startBlendShape.Key];
                        setBlendShapeKeys.Add(startBlendShape.Key);
                        if (startBlendShape.Value == nextBlendShape)
                            setValues.Add(startBlendShape.Value);
                        else
                        {
                            var movementStartTime = songTimeBlendShapeKeys[eventID].Item1;
                            var difference = this.nextSongTime - movementStartTime;
                            var current = currentSeconds - movementStartTime;
                            float movePerc;
                            if (difference == 0)
                                movePerc = 1;
                            else
                                movePerc = Mathf.Clamp(current / difference, 0, 1);
                            setValues.Add(Mathf.Lerp(startBlendShape.Value, nextBlendShape, movePerc));
                        }
                    }
                    else
                    {
                        setBlendShapeKeys.Add(startBlendShape.Key);
                        setValues.Add(startBlendShape.Value);
                    }
                }
            }
            else
            {
                blink = songTimeBlendShapeKeys[eventID + 1].Item3;
                foreach (var setBlendShape in songTimeBlendShapeKeys[eventID + 1].Item2)
                {
                    setBlendShapeKeys.Add(setBlendShape.Key);
                    setValues.Add(setBlendShape.Value);
                }
            }
#if DEBUG
            var log = SetFace(setBlendShapeKeys, setValues);
            if (log != "")
                Debug.Log($"{currentSeconds}s :{log}");
#endif
            if (this.beforeBlink != blink && VRMAvatarController.m_blink != null)
            {
#if DEBUG
                Debug.Log($"{currentSeconds}s :AutoBlink {blink}");
#endif
                VRMAvatarController.m_blink.enabled = blink;
                this.beforeBlink = blink;
            }
        }

        public string GetCaseSensitiveKeyName(string upperCase)
        {
            if (keyUpperCaseDictionary.Count == 0)
            {
                foreach (var presetName in System.Enum.GetNames(typeof(BlendShapePreset)))
                {
                    keyUpperCaseDictionary[presetName.ToUpper()] = presetName;
                }
            }
            return keyUpperCaseDictionary.ContainsKey(upperCase) ? keyUpperCaseDictionary[upperCase] : upperCase;
        }

        public (List<BlendShapeKey>, List<float>) GetBlendShapeKeys(List<string> keys, List<float> strength)
        {
            if (m_proxy == null)
                return (new List<BlendShapeKey>(), new List<float>());
            if (keys.Any(d => blendShapeKeyString.ContainsKey(d) == false))
            {
                var convertKeys = new List<BlendShapeKey>();
                var convertValues = new List<float>();
                for (int i = 0; i < keys.Count; i++)
                {
                    var caseSensitiveKeyName = GetCaseSensitiveKeyName(keys[i]);
                    if (blendShapeKeyString.ContainsKey(caseSensitiveKeyName))
                    {
                        convertKeys.Add(blendShapeKeyString[caseSensitiveKeyName]);
                        convertValues.Add(strength[i]);
                    }
                }
                return (convertKeys, convertValues);
            }
            else
            {
                return (keys.Select(d => blendShapeKeyString[d]).ToList(), strength);
            }
        }
        public void SetFace(BlendShapePreset preset, float strength)
        {
            SetFace(BlendShapeKey.CreateFromPreset(preset), strength);
        }

        public void SetFace(BlendShapeKey key, float strength)
        {
            SetFace(new List<BlendShapeKey> { key }, new List<float> { strength });
        }
        public void SetFace(List<string> keys, List<float> strength)
        {
            if (m_proxy == null)
                return;
            var blendShapeKeys = GetBlendShapeKeys(keys, strength);
            SetFace(blendShapeKeys.Item1, blendShapeKeys.Item2);
        }
        public string SetFace(List<BlendShapeKey> keys, List<float> strength)
        {
            if (m_proxy == null)
                return "";
            var change = false;
            var builder = new StringBuilder(100);
            if (this.beforeKeys != null && this.beforeKeys.Count == keys.Count)
            {
                for (var i = 0; i < this.beforeKeys.Count; i++)
                {
                    if (keys.Contains(this.beforeKeys[i]))
                    {
                        var j = keys.IndexOf(this.beforeKeys[i]);
                        if (!(this.beforeStrength[i] == strength[j]))
                        {
                            change = true;
                            break;
                        }
                    }
                    else
                    {
                        change = true;
                        break;
                    }
                }
            }
            else
            {
                change = true;
            }
            if (change)
            {
                this.beforeKeys = keys;
                this.beforeStrength = strength;
                var dict = new Dictionary<BlendShapeKey, float>();
                foreach (var clip in blendShapeClips)
                {
                    dict.Add(clip.Key, 0.0f);
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    dict[keys[i]] = strength[i];
                    if (strength[i] > 0f)
                        builder.Append($"{keys[i]} => {strength[i]}  ");
                }
                m_proxy.SetValues(dict);
            }
            return builder.ToString();
        }
    }
}
