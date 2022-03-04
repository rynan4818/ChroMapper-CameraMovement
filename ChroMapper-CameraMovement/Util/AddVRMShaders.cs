using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace ChroMapper_CameraMovement.Util
{
    public static class AddVRMShaders
    {
        public static Dictionary<string, Shader> Shaders { get; } = new Dictionary<string, Shader>();

        public static void Initialize(string resourcePath, string installCheckShader)
        {
            var installShaders = Resources.FindObjectsOfTypeAll(typeof(Shader));
            foreach (var installShader in installShaders)
            {
                if (installShader.name == installCheckShader)
                    return;
            }
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            var assetBundle = AssetBundle.LoadFromStream(stream);
            var assets = assetBundle.LoadAllAssets<Shader>();
            foreach (var asset in assets)
            {
                Debug.Log("Add Shader: " + asset.name);
                Shaders.Add(asset.name, asset);
            }
        }
    }
}
