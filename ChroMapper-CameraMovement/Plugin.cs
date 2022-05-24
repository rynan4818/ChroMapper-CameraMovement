using HarmonyLib;
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
        public static OrbitCameraController orbitCamera;
        public static PlusCameraController plusCamera;
        public static DefaultCameraController defaultCamera;
        public static Harmony _harmony;
        public static UI _ui;
        public static int activeWindow = 1;
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
            _harmony.UnpatchAll(HARMONY_ID);
            Debug.Log("Camera Movement:Application has closed!");
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex != 3) // Mapper scene
                return;
            if (movement != null && movement.isActiveAndEnabled)
                return;
            movement = new GameObject("CameraMovement").AddComponent<CameraMovementController>();
            orbitCamera = movement.gameObject.gameObject.AddComponent<OrbitCameraController>();
            plusCamera = movement.gameObject.AddComponent<PlusCameraController>();
            defaultCamera = movement.gameObject.AddComponent<DefaultCameraController>();
            MapEditorUI mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();
            _ui.AddMenu(mapEditorUI);
        }
    }
}
