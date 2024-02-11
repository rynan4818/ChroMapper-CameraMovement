using System;
using HarmonyLib;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    [HarmonyPatch(typeof(SpectrogramSideSwapper), nameof(SpectrogramSideSwapper.SwapSides))]
    public class SpectrogramSideSwapper_SwapSidesPatch
    {
        public static void Postfix()
        {
            OnSwapSides?.Invoke();
        }
        public static event Action OnSwapSides;
    }
}
