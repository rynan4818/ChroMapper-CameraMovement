using System;
using System.Reflection;
using System.IO;
using ChroMapper_CameraMovement.Util.SimpleJSON;
using UnityEngine;

namespace ChroMapper_CameraMovement.Configuration
{
    public class Options
    {
        private static Options instance;
        public static readonly string settingJsonFile = Application.persistentDataPath + "/cameramovement.json";

        public string avatarFileName = Path.Combine(Environment.CurrentDirectory, "Sour Miku Black v2.avatar");
        public bool avatarBlinker = true;
        public bool avatarLookAt = true;
        public bool avatarAnimation = true;
        public bool movement = false;
        public bool turnToHead = false;
        public bool avatar = true;
        public bool uIhidden = false;
        public float avatarHeadHight = 1.5f;
        public float avatarHeadSize = 0.26f;
        public float avatarArmSize = 1.17f;
        public string scriptFileName = "SongScript.json";
        public string scriptMapperExe = "scriptmapper.exe";
        public string scriptMapperLog = "log_latest.txt";
        public float bookmarkWidth = 10f;
        public bool bookmarkLines = true;
        public bool bookmarkLinesShowOnTop = false;
        public float bookmarkInsertOffset = 12f;
        public bool subCamera = true;
        public float subCameraRectX = 0f;
        public float subCameraRectY = 0.6f;
        public float subCameraRectW = 0.25f;
        public float subCameraRectH = 0.25f;
        public bool bookmarkEdit = true;
        public string quickCommand1 = "center";
        public string quickCommand2 = "top";
        public string quickCommand3 = "side";
        public string quickCommand4 = "diagf";
        public string quickCommand5 = "diagb";
        public string quickCommand6 = "random";
        public bool simpleAvatar = true;
        public bool customAvatar = false;
        public float avatarScale = 1f;
        public float avatarYoffset = 0f;
        public float avatarCameraScale = 1.5f;   //ChroMapperとBeatSaberの1単位のスケール違いを補正するためのアバターサイズとカメラ位置の倍数
        public float originMatchOffsetY = -0.5f; //ChroMapperとBeatSaberの原点をあわせるためのオフセット値Y
        public float originMatchOffsetZ = -1.5f; //ChroMapperとBeatSaberの原点をあわせるためのオフセット値Z
        public float originXoffset = 0;          //原点の補正値(BeatSaberスケール)
        public float originYoffset = 0;
        public float originZoffset = 0;
        public bool cameraControl = false;
        public bool vrmAvatarSetting = false;
        public bool qFormat = true;
        public bool mappingDisable = true;
        public bool cameraControlSub = false;
        public static Options Instance {
            get {
                if (instance is null)
                    instance = SettingLoad();
                return instance;
            }
        }

        public static Options SettingLoad()
        {
            var options = new Options();
            if (!File.Exists(settingJsonFile))
                return options;
            var members = options.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
            using (var jsonReader = new StreamReader(settingJsonFile))
            {
                var optionsNode = JSON.Parse(jsonReader.ReadToEnd());
                foreach (var member in members)
                {
                    try
                    {
                        if (!(member is FieldInfo field))
                            continue;
                        var optionValue = optionsNode[field.Name];
                        if (optionValue != null)
                            field.SetValue(options, Convert.ChangeType(optionValue.Value, field.FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Optiong {member.Name} member to load ERROR!.\n{e}");
                        options = new Options();
                    }
                }
            }
            return options;
        }
        public void SettingSave()
        {
            var optionsNode = new JSONObject();
            var members = this.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
            foreach (var member in members)
            {
                if (!(member is FieldInfo field))
                    continue;
                optionsNode[field.Name] = field.GetValue(this).ToString();
            }
            using (var jsonWriter = new StreamWriter(settingJsonFile, false))
                jsonWriter.Write(optionsNode.ToString(2));
        }

    }
}
