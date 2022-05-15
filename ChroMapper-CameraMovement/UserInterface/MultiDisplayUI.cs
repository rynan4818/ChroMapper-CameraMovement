using UnityEngine;
using UnityEngine.UI;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using TMPro;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class MultiDisplayUI
    {
        public GameObject _cameraMovementMultiDisplay;
        public Toggle _subWindow;
        public Toggle _layoutWindow;
        public TextMeshProUGUI _message;
        public CameraMovementController movementController;
        public const string deleteDisplayMessage = "Created displays cannot be deleted!";
        public const string noteEnoughDisplayMessage = "Not enough displays!";
        public void AnchoredPosSave()
        {
            Options.Instance.multiDisplayMenuUIAnchoredPosX = _cameraMovementMultiDisplay.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.multiDisplayMenuUIAnchoredPosY = _cameraMovementMultiDisplay.GetComponent<RectTransform>().anchoredPosition.y;
        }
        public void AddMenu(MapEditorUI mapEditorUI)
        {
            movementController = Plugin.movement;
            var parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementMultiDisplay = new GameObject("CameraMovement Multi Display Menu");
            _cameraMovementMultiDisplay.transform.parent = parent.transform;
            _cameraMovementMultiDisplay.AddComponent<DragWindowController>();
            _cameraMovementMultiDisplay.GetComponent<DragWindowController>().canvas = parent.GetComponent<Canvas>();
            _cameraMovementMultiDisplay.GetComponent<DragWindowController>().OnDragWindow += AnchoredPosSave;

            //Multi Display Menu
            UI.AttachTransform(_cameraMovementMultiDisplay, 300, 200, 1, 1, Options.Instance.multiDisplayMenuUIAnchoredPosX, Options.Instance.multiDisplayMenuUIAnchoredPosY, 1, 1);

            Image imageSetting = _cameraMovementMultiDisplay.AddComponent<Image>();
            imageSetting.sprite = PersistentUI.Instance.Sprites.Background;
            imageSetting.type = Image.Type.Sliced;
            imageSetting.color = new Color(0.24f, 0.24f, 0.24f);

            UI.AddLabel(_cameraMovementMultiDisplay.transform, "Multi Display Window", "Multi Display Window", new Vector2(0, -15), 200);

            UI.AddLabel(_cameraMovementMultiDisplay.transform, "Display Counts", $"Display Counts = {Display.displays.Length}", new Vector2(0, -35), 200);

            _message = UI.AddLabel(_cameraMovementMultiDisplay.transform, "Message", "", new Vector2(0, -100), 300).Item2;
            _message.fontSize = 20;

            if (Display.displays.Length > 1)
            {
                if (Display.displays.Length == 2 && Options.Instance.subWindow)
                    Options.Instance.layoutWindow = false;
                var subWindowCheck = UI.AddCheckbox(_cameraMovementMultiDisplay.transform, "Sub Window", "Sub Window", new Vector2(0, -40), Options.Instance.subWindow, (check) =>
                {
                    _message.text = "";
                    if (Plugin.activeWindow == 3)
                    {
                        _subWindow.isOn = true;
                        _message.text = deleteDisplayMessage;
                        return;
                    }
                    if (Plugin.activeWindow == 2 && !check && !Options.Instance.layoutWindow)
                    {
                        Options.Instance.layoutWindow = true;
                        _layoutWindow.isOn = true;
                        _message.text = deleteDisplayMessage;
                    }
                    if (Display.displays.Length == 2 && check && Options.Instance.layoutWindow)
                    {
                        Options.Instance.layoutWindow = false;
                        _layoutWindow.isOn = false;
                        _message.text = noteEnoughDisplayMessage;
                    }
                    Options.Instance.subWindow = check;
                });
                UI.MoveTransform(subWindowCheck.Item3.transform, 30, 25, 0, 1, 50, -75);
                UI.MoveTransform(subWindowCheck.Item1, 160, 16, 0, 1, 140, -70);
                _subWindow = subWindowCheck.Item3;

                var layoutWindowCheck = UI.AddCheckbox(_cameraMovementMultiDisplay.transform, "Layout Window", "Layout Window", new Vector2(0, -40), Options.Instance.layoutWindow, (check) =>
                {
                    _message.text = "";
                    if (Plugin.activeWindow == 3)
                    {
                        _layoutWindow.isOn = true;
                        _message.text = deleteDisplayMessage;
                        return;
                    }
                    if (Plugin.activeWindow == 2 && !check && !Options.Instance.subWindow)
                    {
                        Options.Instance.subWindow = true;
                        _subWindow.isOn = true;
                        _message.text = deleteDisplayMessage;
                    }
                    if (Display.displays.Length == 2 && check && Options.Instance.subWindow)
                    {
                        Options.Instance.subWindow = false;
                        _subWindow.isOn = false;
                        _message.text = noteEnoughDisplayMessage;
                    }
                    Options.Instance.layoutWindow = check;
                });
                UI.MoveTransform(layoutWindowCheck.Item3.transform, 30, 25, 0, 1, 170, -75);
                UI.MoveTransform(layoutWindowCheck.Item1, 160, 16, 0, 1, 260, -70);
                _layoutWindow = layoutWindowCheck.Item3;

                var createButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Create Window", "Create Window", new Vector2(0, -250), () =>
                {
                    CameraMovementController.multiDisplayController.CreateDisplay();
                    _message.text = "Window Create!";
                });
                UI.MoveTransform(createButton.transform, 70, 25, 0, 1, 50, -140);

                var saveLayoutButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Save Layout", "Save Window Layout", new Vector2(0, -250), () =>
                {
                    CameraMovementController.multiDisplayController.SaveWindowLayout();
                    _message.text = "Save Window Layout!";
                });
                UI.MoveTransform(saveLayoutButton.transform, 70, 25, 0, 1, 140, -140);

                var resetLayoutButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Reset Layout", "Reset Window Layout", new Vector2(0, -250), () =>
                {
                    CameraMovementController.multiDisplayController.ResetWindowLayout();
                    _message.text = "Reset Layout!";
                });
                UI.MoveTransform(resetLayoutButton.transform, 70, 25, 0, 1, 230, -140);

                var saveButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Setting Save", "Setting Save", new Vector2(0, -250), () =>
                {
                    Options.Instance.SettingSave();
                    _message.text = "Save Setting!";
                });
                UI.MoveTransform(saveButton.transform, 70, 25, 0, 1, 140, -170);
            }
            else
            {
                _message.text = "Not multi-display!";
            }

            var closeButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Close", "Close", new Vector2(0, -415), () =>
            {
                _message.text = "";
                _cameraMovementMultiDisplay.SetActive(false);
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(closeButton.transform, 70, 25, 0, 1, 230, -170);

            _cameraMovementMultiDisplay.SetActive(false);
        }
    }
}
