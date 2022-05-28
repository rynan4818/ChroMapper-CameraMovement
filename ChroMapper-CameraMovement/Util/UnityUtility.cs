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
    }
}
