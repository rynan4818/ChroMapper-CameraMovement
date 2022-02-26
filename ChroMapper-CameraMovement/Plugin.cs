using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.UserInterface;
using System.Reflection;

namespace ChroMapper_CameraMovement
{
    [Plugin("Camera Movement")]
    public class Plugin
    {
        public static GameObject cameraMovement;
        public static CameraMovementController movement;
        private UI _ui;
        public static string setting_file;
        private static Harmony _harmony;
        public const string HARMONY_ID = "com.github.rynan4818.ChroMapper-CameraMovement";

        [Init]
        private void Init()
        {
            _harmony = new Harmony(HARMONY_ID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log("Camera Movement Plugin has loaded!");
            SceneManager.sceneLoaded += SceneLoaded;
            _ui = new UI();
        }

        [Exit]
        private void Exit()
        {
            _harmony.UnpatchAll(HARMONY_ID);
            Debug.Log("Camera Movement:Application has closed!");
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (cameraMovement == null)
                cameraMovement = new GameObject("CameraMovement");
            if (arg0.buildIndex == 3) // Mapper scene
            {
                if (movement != null && movement.isActiveAndEnabled)
                    return;

                movement = cameraMovement.AddComponent<CameraMovementController>();
                movement.UI_set(_ui);
                _ui.CameraMovementControllerSet();
                MapEditorUI mapEditorUI = Object.FindObjectOfType<MapEditorUI>();
                _ui.AddMenu(mapEditorUI);
            }
        }
    }
}
