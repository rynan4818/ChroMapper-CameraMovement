using System;
using HarmonyLib;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    [HarmonyPatch(typeof(SpectrogramSideSwapper), "SwapSides")]
    public class SpectrogramSideSwapperPatch
    {
        public static void Postfix()
        {
            OnSwapSides?.Invoke();
        }
        public static event Action OnSwapSides;
    }
}
