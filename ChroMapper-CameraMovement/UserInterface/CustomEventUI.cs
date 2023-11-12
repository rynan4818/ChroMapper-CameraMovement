using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using UnityEngine;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class CustomEventUI
    {
        public GameObject _customEventMenu;
        public CameraMovementController movementController;
        public void AnchoredPosSave()
        {
            Options.Instance.customEventUIAnchoredPosX = _customEventMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.customEventUIAnchoredPosY = _customEventMenu.GetComponent<RectTransform>().anchoredPosition.y;
        }
        public void AddMenu(CanvasGroup topBarCanvas)
        {
            movementController = Plugin.movement;
            //Movement Player Menu
            _customEventMenu = UI.SetMenu(new GameObject("Custom Event Menu"), topBarCanvas, AnchoredPosSave, 500, 160, Options.Instance.customEventUIAnchoredPosX, Options.Instance.customEventUIAnchoredPosY);
            UI.AddLabel(_customEventMenu.transform, "Custom Event", "Custom Event", new Vector2(0, -19.5f));

            var cancelButton = UI.AddButton(_customEventMenu.transform, "Output", "Output", () =>
            {
                var a = new CustomEventController();
                a.Output();
            });
            UI.MoveTransform(cancelButton.transform, 70, 25, 0, 1, 339.8f, -134);


            var closeButton = UI.AddButton(_customEventMenu.transform, "Close", "Close", () =>
            {
                _customEventMenu.SetActive(false);
            });
            UI.MoveTransform(closeButton.transform, 70, 25, 0, 1, 424.4f, -134);

            _customEventMenu.SetActive(false);
        }
    }
}
