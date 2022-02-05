using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChroMapper_CameraMovement.Configuration
{
    static class Options
    {
        public static class Modifier
        {
            public static bool Movement = false;
            public static bool TurnToHead = false;
            public static float AvatarHeadHight = 1.5f;
            public static float AvatarHeadSize = 0.3f;
            public static string ScriptFileName = "SongScript.json";
            public static string ScriptMapperExe = "scriptmapper.exe";
        }
    }
}
