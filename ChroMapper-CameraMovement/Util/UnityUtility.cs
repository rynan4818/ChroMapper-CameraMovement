using System;
using System.Collections;
using UnityEngine;

namespace ChroMapper_CameraMovement.Util
{
    internal static class UnityUtility
    {
        /// <summary>
        /// 自分と子オブジェクトの全てのレイヤーを設定
        /// </summary>
        public static void AllSetLayer(GameObject selfObject, int layer)
        {
            selfObject.layer = layer;
            foreach (Transform childObject in selfObject.transform)
                AllSetLayer(childObject.gameObject, layer);
        }
        /// <summary>
        ///  フルパスを取得する
        /// </summary>
        public static string GetFullPathName(this Transform transform)
        {
            if (transform.parent == null)
                return transform.name;
            return GetFullPathName(transform.parent) + "/" + transform.name;
        }
        /// <summary>
        /// Vector3で線形補間する
        /// </summary>
        /// <param name="from">開始値</param>
        /// <param name="to">終了値</param>
        /// <param name="percent">線形補間する値(0～1)</param>
        /// <returns>補間後の値</returns>
        public static Vector3 LerpVector3(Vector3 from, Vector3 to, float percent)
        {
            return new Vector3(Mathf.Lerp(from.x, to.x, percent), Mathf.Lerp(from.y, to.y, percent), Mathf.Lerp(from.z, to.z, percent));
        }
        /// <summary>
        /// Quaternionで線形補間する
        /// </summary>
        /// <param name="from">開始値</param>
        /// <param name="to">終了値</param>
        /// <param name="percent">線形補間する値(0～1)</param>
        /// <returns>補間後の値</returns>
        public static Quaternion LerpQuaternion(Quaternion from, Quaternion to, float percent)
        {
            var value = Quaternion.Dot(from, to);
            if (value > 0.0)
                return new Quaternion(Mathf.Lerp(from.x, to.x, percent), Mathf.Lerp(from.y, to.y, percent), Mathf.Lerp(from.z, to.z, percent), Mathf.Lerp(from.w, to.w, percent));
            else
                return new Quaternion(Mathf.Lerp(from.x, -to.x, percent), Mathf.Lerp(from.y, -to.y, percent), Mathf.Lerp(from.z, -to.z, percent), Mathf.Lerp(from.w, -to.w, percent));
        }
        public static IEnumerator AssetLoadCoroutine(string assetFile, string name, int setLayer, Action<bool, GameObject> result)
        {
            var request = AssetBundle.LoadFromFileAsync(assetFile);
            yield return request;
            if (request.isDone && request.assetBundle)
            {
                var assetBundleRequest = request.assetBundle.LoadAssetWithSubAssetsAsync<GameObject>(name);
                yield return assetBundleRequest;
                request.assetBundle.Unload(false);
                if (assetBundleRequest.isDone && assetBundleRequest.asset != null)
                {
                    try
                    {
                        var model = (GameObject)UnityEngine.Object.Instantiate(assetBundleRequest.asset);
                        AllSetLayer(model, setLayer);
                        result?.Invoke(true, model);
                    }
                    catch
                    {
                        Debug.LogError("Asset model Instantiate ERR!");
                        result?.Invoke(false, null);
                    }
                }
                else
                {
                    Debug.LogError("Asset Load2 ERR!");
                    result?.Invoke(false, null);
                }
            }
            else
            {
                Debug.LogError("Asset Load1 ERR!");
                result?.Invoke(false, null);
            }
        }
    }
}
