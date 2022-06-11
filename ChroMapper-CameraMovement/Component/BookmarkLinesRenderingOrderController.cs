using UnityEngine;
using UnityEngine.Rendering.Universal;
using ChroMapper_CameraMovement.Util;

namespace ChroMapper_CameraMovement.Component
{
    public class BookmarkLinesRenderingOrderController : MonoBehaviour
    {
        public Canvas effectingCanvas;
        public GameObject bookmarkLines;
        public GameObject cm_MapEditorCamera;
        public GameObject bookmarkLinesCameraObject = null;
        public Camera bookmarkLinesCamera;
        public float beforeFOV;
        public int bookmarkLinesLayer = 26;

        // Start is called before the first frame update
        private void Update()
        {
            if (bookmarkLinesCameraObject != null && beforeFOV != Settings.Instance.CameraFOV)
                bookmarkLinesCamera.fieldOfView = Settings.Instance.CameraFOV;
        }

        public void BookmakLinesCameraINIT(bool showOnTop)
        {
            UpdateCanvasOrder(showOnTop);
            if (showOnTop)
            {
                if (bookmarkLinesCameraObject == null)
                {
                    bookmarkLinesCameraObject = new GameObject("Bookmark Lines Camera");
                    bookmarkLinesCameraObject.transform.position = cm_MapEditorCamera.transform.position;
                    bookmarkLinesCameraObject.transform.rotation = cm_MapEditorCamera.transform.rotation;
                    bookmarkLinesCameraObject.transform.parent = cm_MapEditorCamera.gameObject.transform;
                    bookmarkLinesCamera = bookmarkLinesCameraObject.AddComponent<Camera>();
                    bookmarkLinesCamera.fieldOfView = Settings.Instance.CameraFOV;
                    beforeFOV = Settings.Instance.CameraFOV;
                    bookmarkLinesCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
                    bookmarkLinesCamera.cullingMask = 1 << bookmarkLinesLayer;
                    cm_MapEditorCamera.GetComponent<Camera>().GetUniversalAdditionalCameraData().cameraStack.Add(bookmarkLinesCamera);
                    UnityUtility.AllSetLayer(bookmarkLines, bookmarkLinesLayer);
                }
                bookmarkLinesCameraObject.SetActive(true);
                cm_MapEditorCamera.GetComponent<Camera>().cullingMask &= ~(1 << bookmarkLinesLayer);
            }
            else
            {
                if (bookmarkLinesCameraObject != null)
                    bookmarkLinesCameraObject.SetActive(false);
                cm_MapEditorCamera.GetComponent<Camera>().cullingMask |= 1 << bookmarkLinesLayer;
            }
        }

        // Update is called once per frame
        private void UpdateCanvasOrder(object obj) =>
            effectingCanvas.sortingLayerName = (bool)obj ? "Background" : "Default";
    }
}
