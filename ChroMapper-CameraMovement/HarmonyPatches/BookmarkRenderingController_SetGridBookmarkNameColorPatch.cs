using ChroMapper_CameraMovement.Configuration;
using HarmonyLib;
using TMPro;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    [HarmonyPatch(typeof(BookmarkRenderingController), "SetGridBookmarkNameColor")]
    public class BookmarkRenderingController_SetGridBookmarkNameColorPatch
    {
        public static void Postfix(TextMeshProUGUI text)
        {
            if (Options.Instance.bookmarkLines && Options.Instance.cameraMovementEnable)
                text.text = "";
        }
    }
}
