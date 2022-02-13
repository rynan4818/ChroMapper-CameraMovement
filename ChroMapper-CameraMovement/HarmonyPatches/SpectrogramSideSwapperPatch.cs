using System;
using HarmonyLib;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    [HarmonyPatch(typeof(SpectrogramSideSwapper), "SwapSides")]
    class SpectrogramSideSwapperPatch
    {
        public static void Postfix()
        {
            OnSwapSides?.Invoke();
        }
        public static event Action OnSwapSides;
    }
}
