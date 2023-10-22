using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ChroMapper_CameraMovement.Controller
{
    public class MovementJson
    {
        public List<Setting> Settings { get; set; }
        public int recordFrameRate { get; set; }
        public List<string> objectNames { get; set; }
        public List<Scale> objectScales { get; set; }
        public List<Record> records { get; set; }
    }
    public class Setting
    {
        public virtual string name { get; set; }
        public virtual string type { get; set; }
        public virtual List<string> topObjectStrings { get; set; }
        public virtual string rescaleString { get; set; }
        public virtual List<string> searchStirngs { get; set; }
        public virtual List<string> exclusionStrings { get; set; }
    }
    public class Record
    {
        public float songTIme { get; set; }
        public List<float> posX { get; set; }
        public List<float> posY { get; set; }
        public List<float> posZ { get; set; }
        public List<float> rotX { get; set; }
        public List<float> rotY { get; set; }
        public List<float> rotZ { get; set; }
        public List<float> rotW { get; set; }
    }
    public class Scale
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
    public class MovementData
    {
        public float songTime { get; set; }
        public List<Vector3> position { get; set; }
        public List<Quaternion> rotation { get; set; }
    }
    public class MovementPlayerController
    {
        public static SemaphoreSlim RecordsSemaphore = new SemaphoreSlim(1, 1);
        public MovementJson _records;
        public List<MovementData> _movementData;
        public bool _init;
        public bool _initActive;
        public List<(Transform, int)> _movementModels;
        public float nextSongTime;
        public int eventID;
        public Vector3 _avatarScale;
        public Vector3 _saberScale;
        public string _loadMovementFileName;
        public GameObject _loadAvatarModel;
        public GameObject _loadSaberModel;

        public void MovementUpdate(float currentSeconds)
        {
            if (!this._init || this._records == null || this._movementData == null)
                return;
            while (this.nextSongTime <= currentSeconds)
            {
                this.eventID++;
                if (this.eventID + 1 >= this._movementData.Count)
                {
                    this.eventID = this._movementData.Count - 2;
                    break;
                }
                this.nextSongTime = _movementData[this.eventID + 1].songTime;
            }
            var movementStartTime = this._movementData[this.eventID].songTime;
            var difference = this.nextSongTime - movementStartTime;
            var current = currentSeconds - movementStartTime;
            float movePerc;
            if (difference == 0)
                movePerc = 1;
            else
                movePerc = Mathf.Clamp(current / difference, 0, 1);
            for (int i = 0; i < this._movementModels.Count; i++)
            {
                var modle = this._movementModels[i].Item1;
                var StartPos = this._movementData[this.eventID].position[i];
                var EndPos = this._movementData[this.eventID + 1].position[i];
                var pos = UnityUtility.LerpVector3(StartPos, EndPos, movePerc);
                var StartRot = this._movementData[this.eventID].rotation[i];
                var EndRot = this._movementData[this.eventID + 1].rotation[i];
                var rot = UnityUtility.LerpQuaternion(StartRot, EndRot, movePerc);
                modle.position = pos;
                modle.rotation = rot;
            }
        }
        public void MovementPositionReset()
        {
            nextSongTime = 0;
            this.eventID = 0;
        }

        public async Task SetMovementDataAsync(GameObject avatarModel, GameObject saberModel)
        {
            if (this._initActive || avatarModel == null && saberModel == null)
                return;
            if (this._loadMovementFileName == Options.Instance.movementFileName && avatarModel == this._loadAvatarModel && saberModel == this._loadSaberModel)
            {
                this.nextSongTime = 0;
                this.eventID = 0;
                avatarModel.transform.localScale = this._avatarScale;
                saberModel.transform.localScale = this._saberScale;
                return;
            }
            this._loadAvatarModel = avatarModel;
            this._loadSaberModel = saberModel;
            this._loadMovementFileName = null;
            this._init = false;
            this._initActive = true;
            this._avatarScale = new Vector3(Options.Instance.avatarScale, Options.Instance.avatarScale, Options.Instance.avatarScale) * Options.Instance.avatarCameraScale;
            this._saberScale = Vector3.one * Options.Instance.avatarCameraScale;
            this._records = await this.ReadRecordFileAsync(Options.Instance.movementFileName);
            if (this._records == null)
            {
                Debug.Log("Load Movment Data Error");
                return;
            }
            this._movementData = new List<MovementData>();
            this._movementModels = new List<(Transform, int)>();
            var deletePattan = new Regex(Options.Instance.deletePattan, RegexOptions.Compiled | RegexOptions.CultureInvariant);
            if (avatarModel != null)
            {
                Debug.Log("Load Movment Data:Avatar");
                var topObjectPattans = new List<Regex>();
                var rescalePattans = new List<Regex>();
                foreach (var setting in this._records.Settings)
                {
                    if (setting.type == "Avatar")
                    {
                        foreach (var topObjectString in setting.topObjectStrings)
                            topObjectPattans.Add(new Regex(topObjectString, RegexOptions.Compiled | RegexOptions.CultureInvariant));
                        if (setting.rescaleString != null)
                            rescalePattans.Add(new Regex(setting.rescaleString, RegexOptions.Compiled | RegexOptions.CultureInvariant));
                    }
                }
                var avatarModelTree = avatarModel.GetComponentsInChildren<Transform>(true);
                var topNameLength = avatarModelTree[0].name.Length + 1;
                for (var i = 1; i < avatarModelTree.Length; i++)
                {
                    var name = avatarModelTree[i].GetFullPathName().Substring(topNameLength);
                    for (var j = 0; j < this._records.objectNames.Count; j++)
                    {
                        var objectName = this._records.objectNames[j];
                        foreach (var rescalePattan in rescalePattans)
                        {
                            if (rescalePattan.IsMatch(objectName))
                            {
                                this._avatarScale = new Vector3(this._records.objectScales[j].x, this._records.objectScales[j].y, this._records.objectScales[j].z) * Options.Instance.avatarCameraScale;
                                break;
                            }
                        }
                        foreach (var topObjectPattan in topObjectPattans)
                            objectName = topObjectPattan.Replace(objectName, string.Empty);
                        objectName = deletePattan.Replace(objectName, string.Empty);
                        if (objectName == name)
                        {
                            this._movementModels.Add((avatarModelTree[i], j));
                            Debug.Log(name);
                            break;
                        }
                    }
                }
            }
            if (saberModel != null)
            {
                Debug.Log("Load Movment Data:Saber");
                var topObjectPattans = new List<Regex>();
                var rescalePattans = new List<Regex>();
                foreach (var setting in this._records.Settings)
                {
                    if (setting.type == "Saber")
                    {
                        foreach (var topObjectString in setting.topObjectStrings)
                            topObjectPattans.Add(new Regex(topObjectString, RegexOptions.Compiled | RegexOptions.CultureInvariant));
                        if (setting.rescaleString != null)
                            rescalePattans.Add(new Regex(setting.rescaleString, RegexOptions.Compiled | RegexOptions.CultureInvariant));
                    }
                }
                var saberModelTree = saberModel.GetComponentsInChildren<Transform>(true);
                var topNameLength = saberModelTree[0].name.Length + 1;
                for (var i = 1; i < saberModelTree.Length; i++)
                {
                    var name = saberModelTree[i].GetFullPathName().Substring(topNameLength);
                    for (var j = 0; j < this._records.objectNames.Count; j++)
                    {
                        var objectName = this._records.objectNames[j];
                        foreach (var rescalePattan in rescalePattans)
                        {
                            if (rescalePattan.IsMatch(objectName))
                            {
                                this._saberScale = new Vector3(this._records.objectScales[j].x, this._records.objectScales[j].y, this._records.objectScales[j].z) * Options.Instance.avatarCameraScale;
                                break;
                            }
                        }
                        foreach (var topObjectPattan in topObjectPattans)
                            objectName = topObjectPattan.Replace(objectName, string.Empty);
                        objectName = deletePattan.Replace(objectName, string.Empty);
                        if (objectName == name)
                        {
                            this._movementModels.Add((saberModelTree[i], j));
                            Debug.Log(name);
                            break;
                        }
                    }
                }
            }
            foreach (var record in this._records.records)
            {
                var movementData = new MovementData();
                movementData.position = new List<Vector3>();
                movementData.rotation = new List<Quaternion>();
                movementData.songTime = record.songTIme;
                foreach(var movementModel in this._movementModels)
                {
                    var posX = record.posX[movementModel.Item2] + Options.Instance.originXoffset;
                    var posY = record.posY[movementModel.Item2] + Options.Instance.originYoffset;
                    var posZ = record.posZ[movementModel.Item2] + Options.Instance.originZoffset;
                    var pos = new Vector3(posX, posY, posZ) * Options.Instance.avatarCameraScale;
                    pos.y += Options.Instance.originMatchOffsetY;
                    pos.z += Options.Instance.originMatchOffsetZ;
                    movementData.position.Add(pos);
                    var rotX = record.rotX[movementModel.Item2];
                    var rotY = record.rotY[movementModel.Item2];
                    var rotZ = record.rotZ[movementModel.Item2];
                    var rotW = record.rotW[movementModel.Item2];
                    movementData.rotation.Add(new Quaternion(rotX, rotY, rotZ, rotW));
                }
                this._movementData.Add(movementData);
            }
            Debug.Log($"{_records.records.Count}");
            Debug.Log($"{_movementData.Count}");
            this.nextSongTime = 0;
            this.eventID = 0;
            this._initActive = false;
            this._init = true;
            this._loadMovementFileName = Options.Instance.movementFileName;
            avatarModel.transform.localScale = this._avatarScale;
            saberModel.transform.localScale = this._saberScale;
        }

        public async Task<MovementJson> ReadRecordFileAsync(string path)
        {
            MovementJson result;
            var json = await this.ReadAllTextAsync(path);
            try
            {
                if (json == null)
                    throw new JsonReaderException($"Json file error {path}");
                result = await Task.Run(() => JsonConvert.DeserializeObject<MovementJson>(json));
                if (result == null)
                    throw new JsonReaderException($"Empty json {path}");
            }
            catch (JsonException ex)
            {
                Debug.LogError(ex.ToString());
                result = null;
            }
            return result;
        }
        public async Task<string> ReadAllTextAsync(string path)
        {
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists || fileInfo.Length == 0)
                return null;
            string result;
            await RecordsSemaphore.WaitAsync();
            try
            {
                using (var sr = new StreamReader(path))
                {
                    result = await sr.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                result = null;
            }
            finally
            {
                RecordsSemaphore.Release();
            }
            return result;
        }
    }
}
