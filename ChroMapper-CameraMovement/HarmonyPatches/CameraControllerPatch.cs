using HarmonyLib;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    [HarmonyPatch(typeof(CameraController), "SetLockState")]
    public class CameraControllerPatch
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
