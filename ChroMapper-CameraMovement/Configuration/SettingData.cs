using Newtonsoft.Json;
using System.IO;

namespace ChroMapper_CameraMovement.Configuration
{
    public class SettingData
    {
        public static SettingData Instance;
        public static string setting_file;
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
        public string customAvatarFileName = "Sour Miku Black v2.avatar";
        public float avatarScale = 1f;
        public float avatarYoffset = 0f;
        public float avatarCameraScale = 1.5f;
        public float origenMatchOffsetY = -0.5f;
        public float origenMatchOffsetZ = -1.5f;
        public float originXoffset = 0;
        public float originYoffset = 0;
        public float originZoffset = 0;
        public bool cameraControl = false;

        public static void SettingFileSet(string filename)
        {
            setting_file = filename;
        }
        public static void SettingLoad()
        {
            SettingData data;
            try
            {
                if (File.Exists(setting_file))
                    data = JsonConvert.DeserializeObject<SettingData>(File.ReadAllText(setting_file));
                else
                    data = new SettingData();
            }
            catch
            {
                data = new SettingData();
            }
            Instance = data;
            Options.Movement = data.movement;
            Options.TurnToHead = data.turnToHead;
            Options.Avatar = data.avatar;
            Options.UIhidden = data.uIhidden;
            Options.AvatarHeadHight = data.avatarHeadHight;
            Options.AvatarHeadSize = data.avatarHeadSize;
            Options.AvatarArmSize = data.avatarArmSize;
            Options.ScriptFileName = data.scriptFileName;
            Options.ScriptMapperExe = data.scriptMapperExe;
            Options.ScriptMapperLog = data.scriptMapperLog;
            Options.BookMarkWidth = data.bookmarkWidth;
            Options.BookmarkLines = data.bookmarkLines;
            Options.BookmarkLinesShowOnTop = data.bookmarkLinesShowOnTop;
            Options.BookmarkInsertOffset = data.bookmarkInsertOffset;
            Options.SubCamera = data.subCamera;
            Options.SubCameraRectX = data.subCameraRectX;
            Options.SubCameraRectY = data.subCameraRectY;
            Options.SubCameraRectW = data.subCameraRectW;
            Options.SubCameraRectH = data.subCameraRectH;
            Options.BookmarkEdit = data.bookmarkEdit;
            Options.QuickCommand1 = data.quickCommand1;
            Options.QuickCommand2 = data.quickCommand2;
            Options.QuickCommand3 = data.quickCommand3;
            Options.QuickCommand4 = data.quickCommand4;
            Options.QuickCommand5 = data.quickCommand5;
            Options.QuickCommand6 = data.quickCommand6;
            Options.SimpleAvatar = data.simpleAvatar;
            Options.CustomAvatar = data.customAvatar;
            Options.CustomAvatarFileName = data.customAvatarFileName;
            Options.AvatarScale = data.avatarScale;
            Options.AvatarYoffset = data.avatarYoffset;
            Options.AvatarCameraScale = data.avatarCameraScale;
            Options.OrigenMatchOffsetY = data.origenMatchOffsetY;
            Options.OrigenMatchOffsetZ = data.origenMatchOffsetZ;
            Options.OriginXoffset = data.originXoffset;
            Options.OriginYoffset = data.originYoffset;
            Options.OriginZoffset = data.originZoffset;
            Options.CameraControl = data.cameraControl;
        }
        public static void SettingSave()
        {
            SettingData data = new SettingData
            {
                movement = Options.Movement,
                turnToHead = Options.TurnToHead,
                avatar = Options.Avatar,
                uIhidden = Options.UIhidden,
                avatarHeadHight = Options.AvatarHeadHight,
                avatarHeadSize = Options.AvatarHeadSize,
                avatarArmSize = Options.AvatarArmSize,
                scriptFileName = Options.ScriptFileName,
                scriptMapperExe = Options.ScriptMapperExe,
                scriptMapperLog = Options.ScriptMapperLog,
                bookmarkWidth = Options.BookMarkWidth,
                bookmarkLines = Options.BookmarkLines,
                bookmarkLinesShowOnTop = Options.BookmarkLinesShowOnTop,
                bookmarkInsertOffset = Options.BookmarkInsertOffset,
                subCamera = Options.SubCamera,
                subCameraRectX = Options.SubCameraRectX,
                subCameraRectY = Options.SubCameraRectY,
                subCameraRectW = Options.SubCameraRectW,
                subCameraRectH = Options.SubCameraRectH,
                bookmarkEdit = Options.BookmarkEdit,
                quickCommand1 = Options.QuickCommand1,
                quickCommand2 = Options.QuickCommand2,
                quickCommand3 = Options.QuickCommand3,
                quickCommand4 = Options.QuickCommand4,
                quickCommand5 = Options.QuickCommand5,
                quickCommand6 = Options.QuickCommand6,
                simpleAvatar = Options.SimpleAvatar,
                customAvatar = Options.CustomAvatar,
                customAvatarFileName = Options.CustomAvatarFileName,
                avatarScale = Options.AvatarScale,
                avatarYoffset = Options.AvatarYoffset,
                avatarCameraScale = Options.AvatarCameraScale,
                origenMatchOffsetY = Options.OrigenMatchOffsetY,
                origenMatchOffsetZ = Options.OrigenMatchOffsetZ,
                originXoffset = Options.OriginXoffset,
                originYoffset = Options.OriginYoffset,
                originZoffset = Options.OriginZoffset,
                cameraControl = Options.CameraControl
            };
        File.WriteAllText(setting_file, JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
