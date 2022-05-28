using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Controller
{
    public static class ScriptMapperController
    {
        public static bool scriptMapperAlive;
        public static async void ScriptMapperRun()
        {
            var movement = Plugin.movement;
            if (scriptMapperAlive)
                return;
            scriptMapperAlive = true;
            CameraMovementController.autoSave.Save();
            while (CameraMovementController.SavingThread())
                await Task.Delay(100);
            var path = CameraMovementController.MapGet();
            var scriptmapper = Path.Combine(Environment.CurrentDirectory, Options.Instance.scriptMapperExe);
            if (File.Exists(path) && File.Exists(scriptmapper))
            {
                try
                {
                    using (Process proc = Process.Start(scriptmapper, $@"""{path}"""))
                    {
                        proc.WaitForExit();
                        proc.Close();
                    }
                }
                catch
                {
                    UnityEngine.Debug.LogError("ScriptMapperError");
                    return;
                }
                bool err = false;
                var logfile = Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, Options.Instance.scriptMapperLog).Replace("/", "\\");
                if (File.Exists(logfile))
                {
                    using (StreamReader results = File.OpenText(logfile))
                    {
                        string text = "";
                        while ((text = results.ReadLine()) != null)
                        {
                            if (Regex.IsMatch(text, @"^!.+!$"))
                            {
                                UnityEngine.Debug.LogError($"ScriptMapperWarning:{text}");
                                err = true;
                            }
                        }
                        results.Close();
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("ScriptMapper No Logfile");
                    return;
                }
                if (err)
                    PersistentUI.Instance.DisplayMessage("ScriptMapper ERROR!", PersistentUI.DisplayMessageType.Center);
                movement.Reload();
            }
            scriptMapperAlive = false;
        }
    }
}
