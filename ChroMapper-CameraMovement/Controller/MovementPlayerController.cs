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
        public int recordFrameRate { get; set; }
        public List<string> motionCaptures { get; set; }
        public List<string> topObjectStrings { get; set; }
        public List<string> rescaleStrings { get; set; }
        public List<string> objectNames { get; set; }
        public List<Scale> objectScales { get; set; }
        public List<Record> records { get; set; }
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
        public List<(Transform, int)> _movementModels;
        public float nextSongTime;
        public int eventID;

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

        public async Task SetMovementData(GameObject avatarModel, GameObject saberModel)
        {
            _init = false;
            this._records = await this.ReadRecordFileAsync(Options.Instance.movementFileName);
            if (this._records == null)
            {
                Debug.Log("Load Movment Data Error");
                return;
            }
            Debug.Log("Load Movment Data");
            this._movementData = new List<MovementData>();
            this._movementModels = new List<(Transform, int)>();
            var deletePattan = new Regex(Options.Instance.deletePattan, RegexOptions.Compiled | RegexOptions.CultureInvariant);
            var topObjectPattans = new List<Regex>();
            foreach (var topObjectString in this._records.topObjectStrings)
                topObjectPattans.Add(new Regex(topObjectString, RegexOptions.Compiled | RegexOptions.CultureInvariant));
            var rescalePattans = new List<Regex>();
            foreach (var rescaleString in this._records.rescaleStrings)
                rescalePattans.Add(new Regex(rescaleString, RegexOptions.Compiled | RegexOptions.CultureInvariant));
            if (avatarModel != null)
            {
                var avatarModelTree = avatarModel.GetComponentsInChildren<Transform>(true);
                var topNameLength = avatarModelTree[0].name.Length + 1;
                for (var i = 1; i < avatarModelTree.Length; i++)
                {
                    var name = avatarModelTree[i].GetFullPathName().Substring(topNameLength);
                    for (var j = 0; j < this._records.objectNames.Count; j++)
                    {
                        var rescale = false;
                        var objectName = this._records.objectNames[j];
                        foreach (var rescalePattan in rescalePattans)
                        {
                            if (rescalePattan.IsMatch(objectName))
                            {
                                rescale = true;
                                break;
                            }
                        }
                        foreach (var topObjectPattan in topObjectPattans)
                            objectName = topObjectPattan.Replace(objectName, string.Empty);
                        objectName = deletePattan.Replace(objectName, string.Empty);
                        if (objectName == name)
                        {
                            this._movementModels.Add((avatarModelTree[i], j));
                            if (rescale)
                                avatarModelTree[i].localScale = new Vector3(this._records.objectScales[j].x, this._records.objectScales[j].y, this._records.objectScales[j].z);
                            Debug.Log(objectName);
                            break;
                        }
                    }
                }
            }
            if (saberModel != null)
            {
                var saberModelTree = saberModel.GetComponentsInChildren<Transform>(true);
                var topNameLength = saberModelTree[0].name.Length + 1;
                for (var i = 1; i < saberModelTree.Length; i++)
                {
                    var name = saberModelTree[i].GetFullPathName().Substring(topNameLength);
                    for (var j = 0; j < this._records.objectNames.Count; j++)
                    {
                        var rescale = false;
                        var objectName = this._records.objectNames[j];
                        foreach (var rescalePattan in rescalePattans)
                        {
                            if (rescalePattan.IsMatch(objectName))
                            {
                                rescale = true;
                                break;
                            }
                        }
                        foreach (var topObjectPattan in topObjectPattans)
                            objectName = topObjectPattan.Replace(objectName, string.Empty);
                        objectName = deletePattan.Replace(objectName, string.Empty);
                        if (objectName == name)
                        {
                            this._movementModels.Add((saberModelTree[i], j));
                            if (rescale)
                                saberModelTree[i].localScale = new Vector3(this._records.objectScales[j].x, this._records.objectScales[j].y, this._records.objectScales[j].z);
                            Debug.Log(objectName);
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
            this.nextSongTime = 0;
            this.eventID = 0;
            _init = true;
        }

        public async Task<MovementJson> ReadRecordFileAsync(string path)
        {
            MovementJson result;
            var json = await this.ReadAllTextAsync(path);
            try
            {
                if (json == null)
                    throw new JsonReaderException($"Json file error {path}");
                result = JsonConvert.DeserializeObject<MovementJson>(json);
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
