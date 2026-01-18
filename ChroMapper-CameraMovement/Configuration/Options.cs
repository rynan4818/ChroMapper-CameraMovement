using System;
using System.Reflection;
using System.IO;
using UnityEngine;
using SimpleJSON;

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
        public bool scriptMapperIronPython = false;
        public string scriptMapperLog = "log_latest.txt";
        public int bookmarkWidth = 10;
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
        public bool qFormat = true;
        public bool mappingDisable = true;
        public bool cameraControlSub = false;
        public bool cameraControlLay = false;
        public string previewKeyBinding = "<Keyboard>/f4";
        public string scriptMapperKeyBinding = "<Keyboard>/f3";
        public string dragWindowKeyBinding = "<Keyboard>/shift";
        public string inputFocusMoveKeyBinding = "<Keyboard>/tab";
        public string input1upKeyBinding = "<Keyboard>/f1";
        public string input1downKeyBinding = "<Keyboard>/f2";
        public string input10upKeyBinding = "<Keyboard>/f3";
        public string input10downKeyBinding = "<Keyboard>/f4";
        public string input100modifierKeyBinding = "<Keyboard>/shift";
        public float bookmarkUIAnchoredPosX = 150;
        public float bookmarkUIAnchoredPosY = 40;
        public float cameraControlUIAnchoredPosX = 310;
        public float cameraControlUIAnchoredPosY = 40;
        public float mainMenuUIAnchoredPosX = -50;
        public float mainMenuUIAnchoredPosY = -30;
        public float settingMenuUIAnchoredPosX = 0;
        public float settingMenuUIAnchoredPosY = 0;
        public float multiDisplayMenuUIAnchoredPosX = 0;
        public float multiDisplayMenuUIAnchoredPosY = 0;
        public float movementPlayerUIAnchoredPosX = 0;
        public float movementPlayerUIAnchoredPosY = 0;
        public float orbitDefaultFOV = 60f;
        public float orbitRotSensitivity = 0.5f;
        public float orbitZoomSensitivity = 0.001f;
        public float orbitOffsetSensitivity = 0.01f;
        public float orbitFovSensitivity = 0.005f;
        public float orbitZrotSensitivity = 0.01f;
        public float orbitMinDistance = 0.2f;
        public float orbitMaxDistance = 100f;
        public string orbitActiveKeyBinding = "<Keyboard>/alt";
        public string orbitSubActiveKeyBinding = "<Keyboard>/ctrl";
        public string orbitZrotActiveKeyBinding = "<Keyboard>/shift";
        public string orbitMoveActiveKeyBinding = "<Mouse>/leftButton";
        public string orbitRotActiveKeyBinding = "<Mouse>/rightButton";
        public bool subCameraNoUI = true;
        public bool layoutCameraNoUI = true;
        public bool subWindow = false;
        public bool layoutWindow = false;
        public float multiDislayCreateDelay = 0.1f;
        public int mainDisplayPosX = 0;
        public int mainDisplayPosY = 0;
        public int mainDisplayWidth = 0;
        public int mainDisplayHeight = 0;
        public int subDisplay1PosX = 0;
        public int subDisplay1PosY = 0;
        public int subDisplay1Width = 0;
        public int subDisplay1Height = 0;
        public int subDisplay2PosX = 0;
        public int subDisplay2PosY = 0;
        public int subDisplay2Width = 0;
        public int subDisplay2Height = 0;
        public string plusActiveKeyBinding = "<Keyboard>/z";
        public string plusZrotActiveKeyBinding = "<Mouse>/leftButton";
        public string plusPosActiveKeyBinding = "<Mouse>/middleButton";
        public string plusRotActiveKeyBinding = "<Mouse>/rightButton";
        public float plusZrotSensitivity = 0.004165f;
        public float plusPosSensitivity = 0.01f;
        public float plusRotSensitivity = 0.03f;
        public float plusZoomSensitivity = 0.003125f;
        public float plusFovSensitivity = 0.005f;
        public float plusDefaultFOV = 60f;
        public string defaultCameraActiveKeyBinding = "<Mouse>/rightButton";
        public string defaultCameraElevatePositiveKeyBinding = "<Keyboard>/space";
        public string defaultCameraElevateNegativeKeyBinding = "<Keyboard>/ctrl";
        public string defaultCameraMoveUpKeyBinding = "<Keyboard>/w";
        public string defaultCameraMoveLeftKeyBinding = "<Keyboard>/a";
        public string defaultCameraMoveDownKeyBinding = "<Keyboard>/s";
        public string defaultCameraMoveRightKeyBinding = "<Keyboard>/d";
        public string defaultCameraZrotResetKeyBinding = "<Keyboard>/z";
        public bool defaultCameraZrotReset = false;
        public bool maxDurationErrorOffset = true;
        public float durationErrorWarning = 0.05f;
        public bool cameraMovementEnable = true;
        public bool cameraKeyMouseControlSub = true;
        public int bookmarkLinesFontSize = 100;
        public bool subCameraModel = false;
        public float subCameraModelTrailTime = 4f;
        public bool cameraDirectionScrollReversal = false;
        public bool defaultInvertScrollTime = false;
        public string deletePattan = @"\(Clone\)";
        public string saberFileName = "";
        public bool movementPlayer = false;
        public bool vrmSpringBone = false;
        public bool orbitTargetObject = true;
        public string nalulunaAvatarsEventFileName = "NalulunaAvatarsEvents.json";

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
