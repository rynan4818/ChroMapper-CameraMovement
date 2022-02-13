using UnityEngine;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Component
{
    public class BookmarkLinesRenderingOrderController : MonoBehaviour
    {
        public Canvas effectingCanvas;

        // Start is called before the first frame update
        private void Start()
        {
            UpdateCanvasOrder(Options.BookmarkLinesShowOnTop);
        }

        // Update is called once per frame
        private void UpdateCanvasOrder(object obj) =>
            effectingCanvas.sortingLayerName = (bool)obj ? "Background" : "Default";
    }
}
