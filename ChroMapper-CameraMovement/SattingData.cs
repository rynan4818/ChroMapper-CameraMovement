﻿using Newtonsoft.Json;
using System.IO;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement
{
    public class SattingData
    {
        public static SattingData Instance;
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

        public static void SettingLoad(string setting_file)
        {
            SattingData data;
            try
            {
                if (File.Exists(setting_file))
                    data = JsonConvert.DeserializeObject<SattingData>(File.ReadAllText(setting_file));
                else
                    data = new SattingData();

            }
            catch
            {
                data = new SattingData();
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
            File.WriteAllText(setting_file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
