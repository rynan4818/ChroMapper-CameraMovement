using Newtonsoft.Json;
using System.IO;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement
{
    public class SettingData
    {
        public static SettingData Instance;
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
        public float cameraYoffset = -0.5f;
        public float cameraZoffset = -1.5f;
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

        public static void SettingLoad(string setting_file)
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
            Options.CameraYoffset = data.cameraYoffset;
            Options.CameraZoffset = data.cameraZoffset;
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
        }
        public void SettingSave(string setting_file)
        {
            this.movement = Options.Movement;
            this.turnToHead = Options.TurnToHead;
            this.avatar = Options.Avatar;
            this.uIhidden = Options.UIhidden;
            this.avatarHeadHight = Options.AvatarHeadHight;
            this.avatarHeadSize = Options.AvatarHeadSize;
            this.avatarArmSize = Options.AvatarArmSize;
            this.scriptFileName = Options.ScriptFileName;
            this.scriptMapperExe = Options.ScriptMapperExe;
            this.scriptMapperLog = Options.ScriptMapperLog;
            this.bookmarkWidth = Options.BookMarkWidth;
            this.cameraYoffset = Options.CameraYoffset;
            this.cameraZoffset = Options.CameraZoffset;
            this.bookmarkLines = Options.BookmarkLines;
            this.bookmarkLinesShowOnTop = Options.BookmarkLinesShowOnTop;
            this.bookmarkInsertOffset = Options.BookmarkInsertOffset;
            this.subCamera = Options.SubCamera;
            this.subCameraRectX = Options.SubCameraRectX;
            this.subCameraRectY = Options.SubCameraRectY;
            this.subCameraRectW = Options.SubCameraRectW;
            this.subCameraRectH = Options.SubCameraRectH;
            this.bookmarkEdit = Options.BookmarkEdit;
            this.quickCommand1 = Options.QuickCommand1;
            this.quickCommand2 = Options.QuickCommand2;
            this.quickCommand3 = Options.QuickCommand3;
            this.quickCommand4 = Options.QuickCommand4;
            this.quickCommand5 = Options.QuickCommand5;
            this.quickCommand6 = Options.QuickCommand6;
            this.simpleAvatar = Options.SimpleAvatar;
            this.customAvatar = Options.CustomAvatar;
            this.customAvatarFileName = Options.CustomAvatarFileName;
            this.avatarScale = Options.AvatarScale;
            this.avatarYoffset = Options.AvatarYoffset;
            this.avatarCameraScale = Options.AvatarCameraScale;
            File.WriteAllText(setting_file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
