using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Util;
using ChroMapper_CameraMovement.HarmonyPatches;

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
            var orginal = AccessTools.Method("BookmarkRenderingController:CreateGridBookmark");
            var postfix = AccessTools.Method(typeof(BookmarkRenderingControllerPatch), "CreateGridBookmarkPostfix");
            if (orginal != null)
            {
                Debug.Log("CreateGridBookmark Patch Load");
                _harmony.Patch(orginal, null, new HarmonyMethod(postfix));
            }
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
            MapEditorUI mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();
            _ui.AddMenu(mapEditorUI);
        }
    }
}
