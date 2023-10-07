using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ChroMapper_CameraMovement.Controller
{
    public class MovementJson
    {
        public List<Record> record { get; set; }
        public List<string> avatarObjectNames { get; set; }
        public List<float> avatarScale { get; set; }
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

        public async Task InitMovementDataAsync()
        {
            _init = false;
            var path = Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, "Test_Movement.json").Replace("/", "\\");
            this._records = await this.ReadRecordFileAsync(path);
            Debug.Log("Load Movment Data");
            _init = true;
        }

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

        public void SetMovementData(GameObject models)
        {
            if (!this._init || this._records == null)
                return;
            this._movementData = new List<MovementData>();
            this._movementModels = new List<(Transform, int)>();
            var modelTree = models.GetComponentsInChildren<Transform>(true);
            var topNameLength = modelTree[0].name.Length + 1;
            for (var i = 1; i < modelTree.Length; i++)
            {
                var name = modelTree[i].GetFullPathName().Substring(topNameLength);
                for (var j = 0; j < this._records.avatarObjectNames.Count; j++)
                {
                    var objectName = this._records.avatarObjectNames[j];
                    if (objectName.Length - name.Length >= 0 && objectName.Substring(objectName.Length - name.Length) == name)
                        this._movementModels.Add((modelTree[i], j));
                }
            }
            foreach (var record in this._records.record)
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
            models.transform.localScale = new Vector3(this._records.avatarScale[0], this._records.avatarScale[1], this._records.avatarScale[2]) * Options.Instance.avatarCameraScale;
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
