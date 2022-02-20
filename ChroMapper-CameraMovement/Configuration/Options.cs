﻿namespace ChroMapper_CameraMovement.Configuration
{
    static class Options
    {
        public static bool Movement { set; get; }
        public static bool TurnToHead { set; get; }
        public static bool Avatar { set; get; }
        public static bool UIhidden { set; get; }
        public static float AvatarHeadHight { set; get; }
        public static float AvatarHeadSize { set; get; }
        public static float AvatarArmSize { set; get; }
        public static string ScriptFileName { set; get; }
        public static string ScriptMapperExe { set; get; }
        public static string ScriptMapperLog { set; get; }
        public static float BookMarkWidth { set; get; }
        public static bool BookmarkLines { set; get; }
        public static bool BookmarkLinesShowOnTop { set; get; }
        public static float BookmarkInsertOffset { set; get; }
        public static bool SubCamera { set; get; }
        public static float SubCameraRectX { set; get; }
        public static float SubCameraRectY { set; get; }
        public static float SubCameraRectW { set; get; }
        public static float SubCameraRectH { set; get; }
        public static bool BookmarkEdit { set; get; }
        public static string QuickCommand1 { set; get; }
        public static string QuickCommand2 { set; get; }
        public static string QuickCommand3 { set; get; }
        public static string QuickCommand4 { set; get; }
        public static string QuickCommand5 { set; get; }
        public static string QuickCommand6 { set; get; }
        public static bool SimpleAvatar { set; get; }
        public static bool CustomAvatar { set; get; }
        public static string CustomAvatarFileName { set; get; }
        public static float AvatarScale { set; get; }
        public static float AvatarYoffset { set; get; }
        public static float AvatarCameraScale { set; get; }  //ChroMapperとBeatSaberの1単位のスケール違いを補正するためのアバターサイズとカメラ位置の倍数
        public static float OrigenMatchOffsetY { set; get; }  //ChroMapperとBeatSaberの原点をあわせるためのオフセット値Y
        public static float OrigenMatchOffsetZ { set; get; }  //ChroMapperとBeatSaberの原点をあわせるためのオフセット値Z
        public static float OriginXoffset { set; get; }  //原点の補正値(BeatSaberスケール)
        public static float OriginYoffset { set; get; }
        public static float OriginZoffset { set; get; }
        public static bool CameraControl { set; get; }
    }
}
