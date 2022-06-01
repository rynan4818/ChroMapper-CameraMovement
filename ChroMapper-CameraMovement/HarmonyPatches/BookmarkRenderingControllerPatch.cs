using TMPro;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    class BookmarkRenderingControllerPatch
    {
        public static void CreateGridBookmarkPostfix(ref TextMeshProUGUI __result)
        {
            if (Options.Instance.cameraMovementEnable)
                __result.text = "";
        }
    }
}
