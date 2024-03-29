﻿using UnityEngine;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;
using System.Text.RegularExpressions;
using TMPro;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class MainMenuUI
    {
        public GameObject _cameraMovementMainMenu;
        public static CameraMovementController movementController;
        public static GameObject cm_MapEditorCamera;

        public void AnchoredPosSave()
        {
            Options.Instance.mainMenuUIAnchoredPosX = _cameraMovementMainMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.mainMenuUIAnchoredPosY = _cameraMovementMainMenu.GetComponent<RectTransform>().anchoredPosition.y;
        }

        public void AddMenu(CanvasGroup topBarCanvas)
        {
            movementController = Plugin.movement;
            //Main Menu
            _cameraMovementMainMenu = UI.SetMenu(new GameObject("CameraMovement Menu"), topBarCanvas, AnchoredPosSave, 170, 315, Options.Instance.mainMenuUIAnchoredPosX, Options.Instance.mainMenuUIAnchoredPosY);
            var cameraMovementTitle = UI.AddCheckbox(_cameraMovementMainMenu.transform, "CameraMovement", "CameraMovement", new Vector2(-17, -19), Options.Instance.cameraMovementEnable, (check) =>
            {
                Options.Instance.cameraMovementEnable = check;
                if (check)
                {
                    UI._bookmarkMenuUI._cameraMovementBookmarkMenu.SetActive(Options.Instance.bookmarkEdit);
                    UI._cameraControlMenuUI._cameraControlMenu.SetActive(Options.Instance.cameraControl);
                    Plugin.movement.KeyEnable();
                }
                else
                {
                    UI._bookmarkMenuUI._cameraMovementBookmarkMenu.SetActive(false);
                    UI._cameraControlMenuUI._cameraControlMenu.SetActive(false);
                    UI._settingMenuUI._cameraMovementSettingMenu.SetActive(false);
                    UI._multiDisplayUI._cameraMovementMultiDisplay.SetActive(false);
                    Plugin.movement.KeyDisable();
                }
                movementController._bookmarkController?.BookMarkChangeUpdate();
                movementController.Reload();
            });
            cameraMovementTitle.Item2.fontSize = 16;
            cameraMovementTitle.Item2.alignment = TextAlignmentOptions.Center;
            UI.MoveTransform(cameraMovementTitle.Item1, 110, 24, 0.5f, 1, 0, -15);
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Movement Enable", "Movement Enable", new Vector2(0, -40), Options.Instance.movement, (check) =>
            {
                Options.Instance.movement = check;
                if (!Options.Instance.cameraMovementEnable) return;
                movementController.Reload();
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "UI Hidden", "UI Hidden", new Vector2(0, -55), Options.Instance.uIhidden, (check) =>
            {
                Options.Instance.uIhidden = check;
                if (!Options.Instance.cameraMovementEnable) return;
                movementController.UiHidden();
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "NJS Editor Scale", "NJS Editor Scale", new Vector2(0, -70), Settings.Instance.NoteJumpSpeedForEditorScale, (check) =>
            {
                Settings.Instance.NoteJumpSpeedForEditorScale = check;
                Settings.ManuallyNotifySettingUpdatedEvent("NoteJumpSpeedForEditorScale", Settings.Instance.NoteJumpSpeedForEditorScale);
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Turn To Head", "Turn To Head", new Vector2(0, -85), Options.Instance.turnToHead, (check) =>
            {
                Options.Instance.turnToHead = check;
                if (!Options.Instance.cameraMovementEnable) return;
                movementController.Reload();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Avatar", "Avatar", new Vector2(0, -100), Options.Instance.avatar, (check) =>
            {
                Options.Instance.avatar = check;
                if (!Options.Instance.cameraMovementEnable) return;
                movementController.Reload();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Bookmark Lines", "Bookmark Lines", new Vector2(0, -115), Options.Instance.bookmarkLines, (check) =>
            {
                Options.Instance.bookmarkLines = check;
                if (!Options.Instance.cameraMovementEnable) return;
                movementController.Reload();
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Sub Camera", "Sub Camera", new Vector2(0, -130), Options.Instance.subCamera, (check) =>
            {
                Options.Instance.subCamera = check;
                if (!Options.Instance.cameraMovementEnable) return;
                movementController.Reload();
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Bookmark Edit", "Bookmark Edit", new Vector2(0, -145), Options.Instance.bookmarkEdit, (check) =>
            {
                Options.Instance.bookmarkEdit = check;
                if (!Options.Instance.cameraMovementEnable) return;
                UI._bookmarkMenuUI._cameraMovementBookmarkMenu.SetActive(check);
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Camera Control", "Camera Control", new Vector2(0, -160), Options.Instance.cameraControl, (check) =>
            {
                Options.Instance.cameraControl = check;
                if (!Options.Instance.cameraMovementEnable) return;
                UI._cameraControlMenuUI._cameraControlMenu.SetActive(check);
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Movement Player", "Movement Player", new Vector2(0, -175), Options.Instance.movementPlayer, (check) =>
            {
                Options.Instance.movementPlayer = check;
                if (!Options.Instance.cameraMovementEnable) return;
                movementController.Reload();
            });

            var mainMenuMoreSettingsButton = UI.AddButton(_cameraMovementMainMenu.transform, "More Settings", "More Settings", () =>
            {
                if (!Options.Instance.cameraMovementEnable) return;
                UI._settingMenuUI._cameraMovementSettingMenu.SetActive(true);
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(mainMenuMoreSettingsButton.transform, 70, 25, 0.28f, 1, 0, -200);

            var mainMenuReloadButton = UI.AddButton(_cameraMovementMainMenu.transform, "Reload", "Reload", () =>
            {
                if (!Options.Instance.cameraMovementEnable) return;
                movementController.Reload();
            });
            UI.MoveTransform(mainMenuReloadButton.transform, 70, 25, 0.72f, 1, 0, -200);

            var mainMenuMultiDisplayButton = UI.AddButton(_cameraMovementMainMenu.transform, "Multi Display", "Multi Display", () =>
            {
                if (!Options.Instance.cameraMovementEnable) return;
                UI._multiDisplayUI._cameraMovementMultiDisplay.SetActive(true);
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(mainMenuMultiDisplayButton.transform, 70, 25, 0.28f, 1, 0, -230);

            var regexKey = new Regex(@"<\w+>/");
            var mainMenuScriptMapperRunButton = UI.AddButton(_cameraMovementMainMenu.transform, "Script Mapper Run", $"Script Mapper Run [{regexKey.Replace(Options.Instance.scriptMapperKeyBinding, "").ToUpper()}]", () =>
            {
                if (!Options.Instance.cameraMovementEnable) return;
                ScriptMapperController.Instance.ScriptMapperRun();
            });
            UI.MoveTransform(mainMenuScriptMapperRunButton.transform, 70, 25, 0.72f, 1, 0, -230);

            var mainMenuSettingSaveButton = UI.AddButton(_cameraMovementMainMenu.transform, "Setting Save", "Setting Save", () =>
            {
                Options.Instance.SettingSave();
            });
            UI.MoveTransform(mainMenuSettingSaveButton.transform, 70, 25, 0.28f, 1, 0, -260);

            var movementPlayerButton = UI.AddButton(_cameraMovementMainMenu.transform, "Movement Player", "Movement Player", () =>
            {
                if (!Options.Instance.cameraMovementEnable) return;
                UI._movementPlayerUI.settingBackup();
                UI._movementPlayerUI._movementPlayerMenu.SetActive(true);
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(movementPlayerButton.transform, 70, 25, 0.72f, 1, 0, -260);

            var mainMenuCloseButton = UI.AddButton(_cameraMovementMainMenu.transform, "Close", "Close", () =>
            {
                _cameraMovementMainMenu.SetActive(false);
            });
            UI.MoveTransform(mainMenuCloseButton.transform, 70, 25, 0.72f, 1, 0, -290);

            _cameraMovementMainMenu.SetActive(false);
            UI._extensionBtn.Click = () =>
            {
                _cameraMovementMainMenu.SetActive(!_cameraMovementMainMenu.activeSelf);
            };
        }
    }
}
