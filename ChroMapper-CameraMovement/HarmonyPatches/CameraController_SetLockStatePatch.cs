using HarmonyLib;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.SetLockState))]
    public class CameraController_SetLockStatePatch
    {
        public static bool OriginalSetLockStateDisable = false;
        public static bool Prefix()
        {
            if (OriginalSetLockStateDisable)
                return false;
            else
                return true;
        }
    }
}
