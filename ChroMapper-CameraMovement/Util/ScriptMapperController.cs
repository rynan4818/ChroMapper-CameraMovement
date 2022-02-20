﻿using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Util
{
    public static class ScriptMapperController
    {
        public static bool scriptMapperAlive;
        public static async void ScriptMapperRun(CameraMovementController movement)
        {
            if (scriptMapperAlive)
                return;
            scriptMapperAlive = true;
            movement.MapSave();
            while (movement.SavingThread())
                await Task.Delay(100);
            var path = movement.MapGet();
            var scriptmapper = Path.Combine(Environment.CurrentDirectory, Options.ScriptMapperExe);
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
                var logfile = Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, Options.ScriptMapperLog).Replace("/", "\\");
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
