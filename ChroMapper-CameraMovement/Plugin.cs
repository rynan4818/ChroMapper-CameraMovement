using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Configuration;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChroMapper_CameraMovement
{
    [Plugin("Camera Movement")]
    public class Plugin
    {
        public static CameraMovement movement;
        private UI _ui;
        public bool scriptMapperAlive;
        public static string setting_file;

        [Init]
        private void Init()
        {
            UnityEngine.Debug.Log("Camera Movement Plugin has loaded!");
            SceneManager.sceneLoaded += SceneLoaded;
            _ui = new UI(this);
            setting_file = Path.Combine(Environment.CurrentDirectory, "cameramovement.json");
            SattingData.SettingLoad(setting_file);
        }

        [Exit]
        private void Exit()
        {
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
        public async void ScriptMapperRun()
        {
            if (scriptMapperAlive)
                return;
            scriptMapperAlive = true;
            movement.MapSave();
            while (movement.SavingThread())
                await Task.Delay(100);
            var path = movement.MapGet();
            var scriptmapper = Path.Combine(Environment.CurrentDirectory, Options.Modifier.ScriptMapperExe);
            if (File.Exists(path) && File.Exists(scriptmapper))
            {
                try
                {
                    Process.Start(scriptmapper, $@"""{path}""");
                }
                catch
                {
                    UnityEngine.Debug.Log("ScriptMapperError");
                    return;
                }
                await Task.Delay(1000);
                movement.Reload();
            }
            scriptMapperAlive = false;
        }
        public void SettingSave()
        {
            var setting = new SattingData();
            setting.SettingSave(setting_file);
        }
    }
}
