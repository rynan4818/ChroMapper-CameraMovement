using Newtonsoft.Json;
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
            Options.Modifier.Movement = data.movement;
            Options.Modifier.TurnToHead = data.turnToHead;
            Options.Modifier.Avatar = data.avatar;
            Options.Modifier.UIhidden = data.uIhidden;
            Options.Modifier.AvatarHeadHight = data.avatarHeadHight;
            Options.Modifier.AvatarHeadSize = data.avatarHeadSize;
            Options.Modifier.AvatarArmSize = data.avatarArmSize;
            Options.Modifier.ScriptFileName = data.scriptFileName;
            Options.Modifier.ScriptMapperExe = data.scriptMapperExe;
            Options.Modifier.ScriptMapperLog = data.scriptMapperLog;
        }
        public void SettingSave(string setting_file)
        {
            this.movement = Options.Modifier.Movement;
            this.turnToHead = Options.Modifier.TurnToHead;
            this.avatar = Options.Modifier.Avatar;
            this.uIhidden = Options.Modifier.UIhidden;
            this.avatarHeadHight = Options.Modifier.AvatarHeadHight;
            this.avatarHeadSize = Options.Modifier.AvatarHeadSize;
            this.avatarArmSize = Options.Modifier.AvatarArmSize;
            this.scriptFileName = Options.Modifier.ScriptFileName;
            this.scriptMapperExe = Options.Modifier.ScriptMapperExe;
            this.scriptMapperLog = Options.Modifier.ScriptMapperLog;
            File.WriteAllText(setting_file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
