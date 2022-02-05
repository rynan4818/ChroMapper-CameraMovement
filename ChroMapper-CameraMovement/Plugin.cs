using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Configuration;
using System.IO;
using System;
using System.Diagnostics;

namespace ChroMapper_CameraMovement
{
    [Plugin("Camera Movement")]
    public class Plugin
    {
        public static CameraMovement movement;
        public static AudioTimeSyncController atsc;
        private UI _ui;
        public delegate void ExitEventHandler();
        public event ExitEventHandler evt;

        [Init]
        private void Init()
        {
            UnityEngine.Debug.Log("Camera Movement Plugin has loaded!");
            SceneManager.sceneLoaded += SceneLoaded;
            _ui = new UI(this);
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
                MapEditorUI mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();
                _ui.AddMenu(mapEditorUI);
            }
            else
            {
                movement?.Shutdown();
            }
        }
        public void Reload()
        {
            movement.Reload();
        }
        public void ScriptMapperRun()
        {
            var path = movement.MapGet();
            var scriptmapper = Path.Combine(Environment.CurrentDirectory, Options.Modifier.ScriptMapperExe);
            if (File.Exists(path) && File.Exists(scriptmapper))
            {
                Process proc = new Process();
                proc.StartInfo.FileName = scriptmapper;
                proc.StartInfo.Arguments = $@"""{path}""";
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(proc_Exited);
                proc.Start();
            }
        }
        void proc_Exited(object sender, EventArgs e)
        {
            movement.Reload();
        }
    }
}
