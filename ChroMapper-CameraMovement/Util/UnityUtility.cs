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
        public static Vector3 LerpVector3(Vector3 from, Vector3 to, float percent)
        {
            return new Vector3(Mathf.LerpAngle(from.x, to.x, percent), Mathf.LerpAngle(from.y, to.y, percent), Mathf.LerpAngle(from.z, to.z, percent));
        }
        public static Quaternion LerpQuaternion(Quaternion from, Quaternion to, float percent)
        {
            var value = Quaternion.Dot(from, to);
            if (value > 0.0)
                return new Quaternion(Mathf.LerpAngle(from.x, to.x, percent), Mathf.LerpAngle(from.y, to.y, percent), Mathf.LerpAngle(from.z, to.z, percent), Mathf.LerpAngle(from.w, to.w, percent));
            else
                return new Quaternion(Mathf.LerpAngle(from.x, -to.x, percent), Mathf.LerpAngle(from.y, -to.y, percent), Mathf.LerpAngle(from.z, -to.z, percent), Mathf.LerpAngle(from.w, -to.w, percent));
        }
    }
}
