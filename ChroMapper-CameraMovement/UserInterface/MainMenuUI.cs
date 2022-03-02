﻿using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class MainMenuUI
    {
        public GameObject _cameraMovementMainMenu;
        public static CameraMovementController movementController;
        public readonly ExtensionButton _extensionBtn = new ExtensionButton();
        public static GameObject cm_MapEditorCamera;

        public MainMenuUI()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChroMapper_CameraMovement.Icon.png");
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);

            Texture2D texture2D = new Texture2D(256, 256);
            texture2D.LoadImage(data);

            _extensionBtn.Icon = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0), 100.0f);
            _extensionBtn.Tooltip = "CameraMovement";
            ExtensionButtons.AddButton(_extensionBtn);
        }

        public void CameraControlPanelPosition()
        {
            var cameraMenuRect = Plugin._cameraControlMenuUI._cameraMovementCameraControlMenuRect;
            if (Options.Instance.bookmarkEdit)
            {
                cameraMenuRect.anchorMin = cameraMenuRect.anchorMax = new Vector2(0.7f, 0.2f);
            }
            else
            {
                cameraMenuRect.anchorMin = cameraMenuRect.anchorMax = new Vector2(0.55f, 0.09f);
            }
        }

        public void AddMenu(MapEditorUI mapEditorUI)
        {
            movementController = Plugin.movement;
            var parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementMainMenu = new GameObject("CameraMovement Menu");
            _cameraMovementMainMenu.transform.parent = parent.transform;

            //Main Menu
            UIUtil.AttachTransform(_cameraMovementMainMenu, 170, 225, 1, 1, -50, -30, 1, 1);

            Image imageMain = _cameraMovementMainMenu.AddComponent<Image>();
            imageMain.sprite = PersistentUI.Instance.Sprites.Background;
            imageMain.type = Image.Type.Sliced;
            imageMain.color = new Color(0.24f, 0.24f, 0.24f);

            UIUtil.AddLabel(_cameraMovementMainMenu.transform, "CameraMovement", "CameraMovement", new Vector2(0, -15));
            UIUtil.AddCheckbox(_cameraMovementMainMenu.transform, "Movement Enable", "Movement Enable", new Vector2(0, -40), Options.Instance.movement, (check) =>
            {
                Options.Instance.movement = check;
                movementController.Reload();
            });
            UIUtil.AddCheckbox(_cameraMovementMainMenu.transform, "UI Hidden", "UI Hidden", new Vector2(0, -55), Options.Instance.uIhidden, (check) =>
            {
                Options.Instance.uIhidden = check;
                movementController.UiHidden();
            });
            UIUtil.AddCheckbox(_cameraMovementMainMenu.transform, "Turn To Head", "Turn To Head", new Vector2(0, -70), Options.Instance.turnToHead, (check) =>
            {
                Options.Instance.turnToHead = check;
                movementController.Reload();
            });
            UIUtil.AddCheckbox(_cameraMovementMainMenu.transform, "Avatar", "Avatar", new Vector2(0, -85), Options.Instance.avatar, (check) =>
            {
                Options.Instance.avatar = check;
                movementController.Reload();
            });
            UIUtil.AddCheckbox(_cameraMovementMainMenu.transform, "Bookmark Lines", "Bookmark Lines", new Vector2(0, -100), Options.Instance.bookmarkLines, (check) =>
            {
                Options.Instance.bookmarkLines = check;
                movementController.Reload();
            });
            UIUtil.AddCheckbox(_cameraMovementMainMenu.transform, "Sub Camera", "Sub Camera", new Vector2(0, -115), Options.Instance.subCamera, (check) =>
            {
                Options.Instance.subCamera = check;
                movementController.Reload();
            });
            UIUtil.AddCheckbox(_cameraMovementMainMenu.transform, "Bookmark Edit", "Bookmark Edit", new Vector2(0, -130), Options.Instance.bookmarkEdit, (check) =>
            {
                Options.Instance.bookmarkEdit = check;
                Plugin._bookmarkMenuUI._cameraMovementBookmarkMenu.SetActive(check);
                CameraControlPanelPosition();
            });
            UIUtil.AddCheckbox(_cameraMovementMainMenu.transform, "Camera Control", "Camera Control", new Vector2(0, -145), Options.Instance.cameraControl, (check) =>
            {
                Options.Instance.cameraControl = check;
                Plugin._cameraControlMenuUI._cameraControlMenu.SetActive(check);
                CameraControlPanelPosition();
            });
            var mainMenuMoreSettingsButton = UIUtil.AddButton(_cameraMovementMainMenu.transform, "More Settings", "More Settings", new Vector2(0, -170), () =>
            {
                Plugin._settingMenuUI._cameraMovementSettingMenu.SetActive(true);
            });
            UIUtil.MoveTransform(mainMenuMoreSettingsButton.transform, 70, 25, 0.28f, 1, 0, -170);

            var mainMenuReloadButton = UIUtil.AddButton(_cameraMovementMainMenu.transform, "Reload", "Reload", new Vector2(0, -170), () =>
            {
                movementController.Reload();
            });
            UIUtil.MoveTransform(mainMenuReloadButton.transform, 70, 25, 0.72f, 1, 0, -170);

            var mainMenuSettingSaveButton = UIUtil.AddButton(_cameraMovementMainMenu.transform, "Setting Save", "Setting Save", new Vector2(0, -200), () =>
            {
                Options.Instance.SettingSave();
            });
            UIUtil.MoveTransform(mainMenuSettingSaveButton.transform, 70, 25, 0.28f, 1, 0, -200);

            var mainMenuScriptMapperRunButton = UIUtil.AddButton(_cameraMovementMainMenu.transform, "Script Mapper Run", "Script Mapper Run", new Vector2(0, -200), () =>
            {
                ScriptMapperController.ScriptMapperRun(movementController);
            });
            UIUtil.MoveTransform(mainMenuScriptMapperRunButton.transform, 70, 25, 0.72f, 1, 0, -200);

            _cameraMovementMainMenu.SetActive(false);
            _extensionBtn.Click = () =>
            {
                _cameraMovementMainMenu.SetActive(!_cameraMovementMainMenu.activeSelf);
            };
        }
    }
}
