using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ChroMapper_CameraMovement.Controller
{
    public class MovementJson
    {
        public int objectCount { get; set; }
        public int recordCount { get; set; }
        public string levelID { get; set; }
        public string songName { get; set; }
        public string serializedName { get; set; }
        public string difficulty { get; set; }
        public List<Setting> Settings { get; set; }
        public int recordFrameRate { get; set; }
        public List<string> objectNames { get; set; }
        public List<Scale> objectScales { get; set; }
        public List<NUllObject> recordNullObjects { get; set; }
    }
    public class Setting
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<string> topObjectStrings { get; set; }
        public string rescaleString { get; set; }
        public List<string> searchStirngs { get; set; }
        public List<string> exclusionStrings { get; set; }
    }
    public class Scale
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
    public class NUllObject
    {
        public float songTime { get; set; }
        public int objIndex { get; set; }
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
        public MovementJson _metaData;
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
            if (!this._init || this._metaData == null || this._movementData == null)
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
                pos.y += Options.Instance.avatarYoffset;
                pos *= Options.Instance.avatarCameraScale;
                pos.y += Options.Instance.originMatchOffsetY;
                pos.z += Options.Instance.originMatchOffsetZ;
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
            if (this._initActive || avatarModel == null && saberModel == null || !File.Exists(CameraMovementController.movementPlayerOptions.movementFileName))
                return;
            if (this._loadMovementFileName == CameraMovementController.movementPlayerOptions.movementFileName && avatarModel == this._loadAvatarModel && saberModel == this._loadSaberModel)
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
            this._metaData = await this.ReadMetaDataAsync(CameraMovementController.movementPlayerOptions.movementFileName);
            if (this._metaData == null)
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
                foreach (var setting in this._metaData.Settings)
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
                    for (var j = 0; j < this._metaData.objectNames.Count; j++)
                    {
                        var objectName = this._metaData.objectNames[j];
                        foreach (var rescalePattan in rescalePattans)
                        {
                            if (rescalePattan.IsMatch(objectName))
                            {
                                this._avatarScale = new Vector3(this._metaData.objectScales[j].x, this._metaData.objectScales[j].y, this._metaData.objectScales[j].z) * Options.Instance.avatarCameraScale;
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
                foreach (var setting in this._metaData.Settings)
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
                    for (var j = 0; j < this._metaData.objectNames.Count; j++)
                    {
                        var objectName = this._metaData.objectNames[j];
                        foreach (var rescalePattan in rescalePattans)
                        {
                            if (rescalePattan.IsMatch(objectName))
                            {
                                this._saberScale = new Vector3(this._metaData.objectScales[j].x, this._metaData.objectScales[j].y, this._metaData.objectScales[j].z) * Options.Instance.avatarCameraScale;
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
            await this.ReadRecordDataAsync(CameraMovementController.movementPlayerOptions.movementFileName);
            Debug.Log($"{this._metaData.recordCount}");
            Debug.Log($"{this._movementData.Count}");
            this.nextSongTime = 0;
            this.eventID = 0;
            this._initActive = false;
            this._init = true;
            this._loadMovementFileName = CameraMovementController.movementPlayerOptions.movementFileName;
            avatarModel.transform.localScale = this._avatarScale;
            saberModel.transform.localScale = this._saberScale;
        }

        public async Task<MovementJson> ReadMetaDataAsync(string path)
        {
            MovementJson result = null;
            try
            {
                await Task.Run(() =>
                {
                    using (var reader = new BinaryReader(File.OpenRead(path), Encoding.UTF8))
                    {
                        string json;
                        json = reader.ReadString();
                        if (json == null)
                            throw new JsonReaderException($"Meta Data Json error {path}");
                        result = JsonConvert.DeserializeObject<MovementJson>(json);
                        if (result == null)
                            throw new JsonReaderException($"Meta Data Empty json {path}");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                result = null;
            }
            return result;
        }
        public async Task ReadRecordDataAsync(string path)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var reader = new BinaryReader(File.OpenRead(path), Encoding.UTF8))
                    {
                        reader.ReadString();
                        for (var i = 0; i < this._metaData.recordCount; i++)
                        {
                            var movementData = new MovementData();
                            movementData.position = new List<Vector3>();
                            movementData.rotation = new List<Quaternion>();
                            movementData.songTime = reader.ReadSingle();
                            var pos = new Vector3[this._metaData.objectNames.Count];
                            var rot = new Quaternion[this._metaData.objectNames.Count];
                            for (int j = 0; j < this._metaData.objectNames.Count; j++)
                            {
                                pos[j] = new Vector3(
                                    reader.ReadSingle() + Options.Instance.originXoffset,
                                    reader.ReadSingle() + Options.Instance.originYoffset,
                                    reader.ReadSingle() + Options.Instance.originZoffset
                                );
                                rot[j] = new Quaternion(
                                    reader.ReadSingle(),
                                    reader.ReadSingle(),
                                    reader.ReadSingle(),
                                    reader.ReadSingle()
                                );
                            }
                            foreach (var movementModel in this._movementModels)
                            {
                                movementData.position.Add(pos[movementModel.Item2]);
                                movementData.rotation.Add(rot[movementModel.Item2]);
                            }
                            this._movementData.Add(movementData);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
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
            FileInfo fileInfo;
            try
            {
                fileInfo = new FileInfo(path);
            }
            catch
            {
                return null;
            }
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
