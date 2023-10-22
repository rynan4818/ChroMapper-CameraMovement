using System;
using System.Reflection;
using System.IO;
using ChroMapper_CameraMovement.Util.SimpleJSON;
using UnityEngine;

namespace ChroMapper_CameraMovement.Configuration
{
    public class MovementPlayerOptions
    {
        public static readonly string settingJsonFile = "Movement_Player_Setting.json";

        public string movementFileName = "";
        public string saberFileName = "";
        public bool saberEnabled = true;
        public static string GetSettingFile()
        {
            return Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, settingJsonFile);
        }
        public static MovementPlayerOptions SettingLoad()
        {
            var options = new MovementPlayerOptions();
            if (!File.Exists(GetSettingFile()))
                return options;
            var members = options.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
            using (var jsonReader = new StreamReader(GetSettingFile()))
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
                        options = new MovementPlayerOptions();
                    }
                }
            }
            return options;
        }
        public void SettingSave()
        {
            var dir = Path.GetDirectoryName(GetSettingFile());
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var optionsNode = new JSONObject();
            var members = this.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
            foreach (var member in members)
            {
                if (!(member is FieldInfo field))
                    continue;
                optionsNode[field.Name] = field.GetValue(this).ToString();
            }
            using (var jsonWriter = new StreamWriter(GetSettingFile(), false))
                jsonWriter.Write(optionsNode.ToString(2));
        }

    }
}
