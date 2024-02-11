using System;
using HarmonyLib;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    [HarmonyPatch(typeof(BookmarkContainer), "HandleNewBookmarkName")]
    public class BookmarkContainer_HandleNewBookmarkNamePatch
    {
        public static void Postfix()
        {
            OnNewBookmarkName?.Invoke();
        }
        public static event Action OnNewBookmarkName;
    }
}
