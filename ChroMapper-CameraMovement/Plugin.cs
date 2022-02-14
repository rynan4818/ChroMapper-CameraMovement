using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Configuration;
using System.IO;
using System;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ChroMapper_CameraMovement
{
    [Plugin("Camera Movement")]
    public class Plugin
    {
        public static CameraMovement movement;
        private UI _ui;
        public bool scriptMapperAlive;
        public static string setting_file;
        private static Harmony _harmony;
        public const string HARMONY_ID = "com.github.rynan4818.ChroMapper-CameraMovement";

        [Init]
        private void Init()
        {
            _harmony = new Harmony(HARMONY_ID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            UnityEngine.Debug.Log("Camera Movement Plugin has loaded!");
            SceneManager.sceneLoaded += SceneLoaded;
            setting_file = (Application.persistentDataPath + "/cameramovement.json").Replace("/","\\");
            SettingData.SettingLoad(setting_file);
            _ui = new UI(this);
        }

        [Exit]
        private void Exit()
        {
            _harmony.UnpatchAll(HARMONY_ID);
            UnityEngine.Debug.Log("Camera Movement:Application has closed!");
        }
        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex == 3) // Mapper scene
            {
                if (movement != null && movement.isActiveAndEnabled)
                    return;

                movement = new GameObject("CameraMovement").AddComponent<CameraMovement>();
                movement.UI_set(_ui);
                MapEditorUI mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();
                _ui.AddMenu(mapEditorUI);
            }
        }
        public void Reload()
        {
            movement.Reload();
        }
        public void UiHidden()
        {
            movement.UiHidden();
        }
        public void BookmarkChange(int no,string name)
        {
            movement.BookmarkChange(no);
        }
        public void BookmarkDelete(int no)
        {
            movement.BookmarkDelete(no);
        }
        public void BookmarkNew(string name)
        {
            movement.BookmarkNew(name);
        }

        public async void ScriptMapperRun()
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
                if(err)
                    PersistentUI.Instance.DisplayMessage("ScriptMapper ERROR!", PersistentUI.DisplayMessageType.Center);
                movement.Reload();
            }
            scriptMapperAlive = false;
        }
        public void SettingSave()
        {
            var setting = new SettingData();
            setting.SettingSave(setting_file);
        }
    }
}
