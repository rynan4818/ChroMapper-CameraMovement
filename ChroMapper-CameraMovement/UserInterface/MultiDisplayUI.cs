﻿using UnityEngine;
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
        public void AddMenu(CanvasGroup topBarCanvas)
        {
            movementController = Plugin.movement;
            //Multi Display Menu
            _cameraMovementMultiDisplay = UI.SetMenu(new GameObject("CameraMovement Multi Display Menu"), topBarCanvas, AnchoredPosSave, 300, 200, Options.Instance.multiDisplayMenuUIAnchoredPosX, Options.Instance.multiDisplayMenuUIAnchoredPosY);

            UI.AddLabel(_cameraMovementMultiDisplay.transform, "Multi Display Window", "Multi Display Window", new Vector2(0, -15), 200);

            UI.AddLabel(_cameraMovementMultiDisplay.transform, "Display Counts", $"Display Counts = {Display.displays.Length}", new Vector2(0, -35), 200);

            _message = UI.AddLabel(_cameraMovementMultiDisplay.transform, "Message", "", new Vector2(0, -100), 300).Item2;
            _message.fontSize = 20;

            if (Display.displays.Length > 1)
            {
                if (Display.displays.Length == 2 && Options.Instance.subWindow)
                    Options.Instance.layoutWindow = false;
                var subWindowCheck = UI.AddCheckbox(_cameraMovementMultiDisplay.transform, "Sub Window", "Sub Window", Options.Instance.subWindow, (check) =>
                {
                    _message.text = "";
                    if (MultiDisplayController.activeWindow == 3)
                    {
                        _subWindow.isOn = true;
                        _message.text = deleteDisplayMessage;
                        return;
                    }
                    if (MultiDisplayController.activeWindow == 2 && !check && !Options.Instance.layoutWindow)
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

                var layoutWindowCheck = UI.AddCheckbox(_cameraMovementMultiDisplay.transform, "Layout Window", "Layout Window", Options.Instance.layoutWindow, (check) =>
                {
                    _message.text = "";
                    if (MultiDisplayController.activeWindow == 3)
                    {
                        _layoutWindow.isOn = true;
                        _message.text = deleteDisplayMessage;
                        return;
                    }
                    if (MultiDisplayController.activeWindow == 2 && !check && !Options.Instance.subWindow)
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

                var createButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Create Window", "Create Window", () =>
                {
                    CameraMovementController.multiDisplayController.CreateDisplay();
                    if (MultiDisplayController.createDisplayActive)
                        _message.text = "Window Create!";
                    else
                        _message.text = "";
                });
                UI.MoveTransform(createButton.transform, 70, 25, 0, 1, 50, -140);

                var saveLayoutButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Save Layout", "Save Window Layout", () =>
                {
                   if(CameraMovementController.multiDisplayController.SaveWindowLayout())
                        _message.text = "Save Window Layout!";
                    else
                        _message.text = "";
                });
                UI.MoveTransform(saveLayoutButton.transform, 70, 25, 0, 1, 140, -140);

                var resetLayoutButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Reset Layout", "Reset Window Layout", () =>
                {
                    if(CameraMovementController.multiDisplayController.ResetWindowLayout())
                        _message.text = "Reset Layout!";
                    else
                        _message.text = "";
                });
                UI.MoveTransform(resetLayoutButton.transform, 70, 25, 0, 1, 230, -140);

                var saveButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Setting Save", "Setting Save", () =>
                {
                    Options.Instance.SettingSave();
                    _message.text = "Save Setting!";
                });
                UI.MoveTransform(saveButton.transform, 70, 25, 0, 1, 140, -170);
            }
            else
                _message.text = "Not multi-display!";

            var closeButton = UI.AddButton(_cameraMovementMultiDisplay.transform, "Close", "Close", () =>
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
