using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Util;

namespace ChroMapper_CameraMovement
{
    [Plugin("Camera Movement")]
    public class Plugin
    {
        public static CameraMovementController movement;
        public static MainMenuUI _mainMenuUI;
        public static SettingMenuUI _settingMenuUI;
        public static BookmarkMenuUI _bookmarkMenuUI;
        public static CameraControlMenuUI _cameraControlMenuUI;
        public static Harmony _harmony;
        public const string HARMONY_ID = "com.github.rynan4818.ChroMapper-CameraMovement";

        [Init]
        private void Init()
        {
            _harmony = new Harmony(HARMONY_ID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log("Camera Movement Plugin has loaded!");
            SceneManager.sceneLoaded += SceneLoaded;
            _mainMenuUI = new MainMenuUI();
            _settingMenuUI = new SettingMenuUI();
            _bookmarkMenuUI = new BookmarkMenuUI();
            _cameraControlMenuUI = new CameraControlMenuUI();
            AddVRMShaders.Initialize(Path.Combine(Environment.CurrentDirectory, "vrmavatar.shaders"), "UniGLTF/UniUnlit");
        }

        [Exit]
        private void Exit()
        {
            _harmony.UnpatchAll(HARMONY_ID);
            Debug.Log("Camera Movement:Application has closed!");
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex == 3) // Mapper scene
            {
                if (movement != null && movement.isActiveAndEnabled)
                    return;
                movement = new GameObject("CameraMovement").AddComponent<CameraMovementController>();
                MapEditorUI mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();
                _mainMenuUI.AddMenu(mapEditorUI);
                _settingMenuUI.AddMenu(mapEditorUI);
                _bookmarkMenuUI.AddMenu(mapEditorUI);
                _cameraControlMenuUI.AddMenu(mapEditorUI);
            }
        }
    }
}
