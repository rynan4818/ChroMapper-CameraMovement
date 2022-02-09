using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChroMapper_CameraMovement.Configuration
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
    }
}
