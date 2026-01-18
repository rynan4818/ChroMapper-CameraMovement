using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Util;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;

namespace ChroMapper_CameraMovement
{
    [Plugin("Camera Movement")]
    public class Plugin
    {
        public static CameraMovementController movement;
        public static Harmony _harmony;
        public static UI _ui;
        public const string HARMONY_ID = "com.github.rynan4818.ChroMapper-CameraMovement";

        [Init]
        private void Init()
        {
            _harmony = new Harmony(HARMONY_ID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log("Camera Movement Plugin has loaded!");
            SceneManager.sceneLoaded += SceneLoaded;
            AddVRMShaders.Initialize("ChroMapper_CameraMovement.Resources.vrmavatar.shaders", "UniGLTF/UniUnlit");
            _ui = new UI();
        }

        [Exit]
        private void Exit()
        {
            _harmony.UnpatchSelf();
            Debug.Log("Camera Movement:Application has closed!");
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex != 3) // Mapper scene
                return;
            if (movement != null && movement.isActiveAndEnabled)
                return;
            Options.Instance.cameraMovementEnable = false;
            movement = new GameObject("CameraMovement").AddComponent<CameraMovementController>();
            MapEditorUI mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();
            _ui.AddMenu(mapEditorUI);

            // IronPythonエンジンの先行初期化
            if (Options.Instance.scriptMapperIronPython)
            {
                IronPythonController.Instance.Initialize();
            }
        }
    }
}
