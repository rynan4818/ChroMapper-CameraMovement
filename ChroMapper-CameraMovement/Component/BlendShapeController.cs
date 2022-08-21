using ChroMapper_CameraMovement.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChroMapper_CameraMovement.Component
{
    public class BlendShapeController : MonoBehaviour
    {
        private CustomEventsContainer _customEventsContainer;
        private float _previousAtscBeat = 0;
        private string _previousValue = "";
        private float _previousStringth = 1f;
        private bool _previousBlink = true;
        private bool _refresh = false;
        private int eventID = 0;
        public AudioTimeSyncController _atsc { private get; set; }
        public List<BeatmapCustomEvent> _loadedObjects = new List<BeatmapCustomEvent>();
        const string blendShapeTrakName = "BlendShape";

        private void Start()
        {
            _customEventsContainer = FindObjectOfType<CustomEventsContainer>();
            Debug.Log($"Custom Event Count = {_customEventsContainer.LoadedObjects.Count}");
            _customEventsContainer.ObjectDeletedEvent += CustomEventDelete;
            _customEventsContainer.ObjectSpawnedEvent += CustomEventSpawned;
            //var a = _customEventsContainer.LoadedObjects.ToArray()[0];
            //var b = _customEventsContainer.LoadedContainers[a];
            LoadEventRefresh();
        }
        private void Update()
        {
            if (_atsc.CurrentBeat == _previousAtscBeat)
                return;
            if (_previousAtscBeat > _atsc.CurrentBeat)
                eventID = 0;
            _previousAtscBeat = _atsc.CurrentBeat;
            if (_refresh)
                LoadEventRefresh();
            if (_loadedObjects.Count == 0)
                return;
            while (_loadedObjects.Count > eventID && _loadedObjects[eventID].Time <= _atsc.CurrentBeat)
                eventID += 1;
            if (_loadedObjects.Count <= eventID)
                eventID = _loadedObjects.Count - 1;
            if (eventID > 0 && _loadedObjects[eventID].Time > _atsc.CurrentBeat)
                eventID -= 1;
            var eventElapTime = _atsc.GetSecondsFromBeat(_atsc.CurrentBeat - _loadedObjects[eventID].Time);
            var data = _loadedObjects[eventID].CustomData;
            Debug.Log($"{eventElapTime}:{data["_duration"]}:{data["_key"]}:{data["_value"]}:{data["_strength"]}:{data["_blink"]}");
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
        }
        private void OnDestroy()
        {
            _customEventsContainer.ObjectDeletedEvent -= CustomEventDelete;
            _customEventsContainer.ObjectSpawnedEvent -= CustomEventSpawned;
        }
        private void LoadEventRefresh()
        {
            _refresh = false;
            _loadedObjects.Clear();
            foreach (BeatmapCustomEvent customEvent in _customEventsContainer.LoadedObjects)
            {
                if (customEvent.Type != blendShapeTrakName)
                    continue;
                if (customEvent.CustomData != null)
                    _loadedObjects.Add(customEvent);
            }
        }
        private void CustomEventDelete(BeatmapObject obj)
        {
            var customEvent = obj as BeatmapCustomEvent;
            if (customEvent.Type != blendShapeTrakName)
                return;
            _refresh = true; //変更の時に一度削除されて置き直されるのでフラグを立ててupdateで更新する
            //Debug.Log($"DELETE = {(obj as BeatmapCustomEvent).Type}");
            //Debug.Log($"DELETE = {obj}");
        }
        private void CustomEventSpawned(BeatmapObject obj)
        {
            var customEvent = obj as BeatmapCustomEvent;
            if (customEvent.Type != blendShapeTrakName)
                return;
            _refresh = true;
            //Debug.Log($"SPAWNED = {(obj as BeatmapCustomEvent).Type}");
            //Debug.Log($"SPAWNED = {obj}");
        }
    }
}
