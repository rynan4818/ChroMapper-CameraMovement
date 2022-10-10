using UnityEngine;
using UnityEngine.UI;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class BlendShapeMenuUI
    {
        public CameraMovementController movementController;
        public GameObject _cameraMovementBlendShapeMenu;
        public void AnchoredPosSave()
        {
            Options.Instance.blendShapeMenuUIAnchoredPosX = _cameraMovementBlendShapeMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.blendShapeMenuUIAnchoredPosY = _cameraMovementBlendShapeMenu.GetComponent<RectTransform>().anchoredPosition.y;
        }
        public void AddMenu(MapEditorUI mapEditorUI)
        {
            movementController = Plugin.movement;
            var parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementBlendShapeMenu = new GameObject("CameraMovement BlendShape Menu");
            _cameraMovementBlendShapeMenu.transform.parent = parent.transform;
            _cameraMovementBlendShapeMenu.AddComponent<DragWindowController>();
            _cameraMovementBlendShapeMenu.GetComponent<DragWindowController>().canvas = parent.GetComponent<Canvas>();
            _cameraMovementBlendShapeMenu.GetComponent<DragWindowController>().OnDragWindow += AnchoredPosSave;

            //More Settings Menu
            UI.AttachTransform(_cameraMovementBlendShapeMenu, 500, 210, 1, 1, Options.Instance.blendShapeMenuUIAnchoredPosX, Options.Instance.blendShapeMenuUIAnchoredPosY, 1, 1);


        }
    }
}
