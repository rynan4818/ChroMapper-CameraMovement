using HarmonyLib;
using UnityEngine;
using ChroMapper_CameraMovement.Util;

namespace ChroMapper_CameraMovement.HarmonyPatches
{
    [HarmonyPatch(typeof(Shader), nameof(Shader.Find))]
    static class Shader_FindPatch
    {
        static bool Prefix(ref Shader __result, string name)
        {
            if (AddVRMShaders.Shaders.TryGetValue(name, out var shader))
            {
                __result = shader;
                return false;
            }

            return true;
        }
    }
}
