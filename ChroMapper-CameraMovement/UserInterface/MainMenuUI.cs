using UnityEngine;
using UnityEngine.UI;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;
using System.Text.RegularExpressions;

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

        public void AddMenu(MapEditorUI mapEditorUI)
        {
            movementController = Plugin.movement;
            var parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementMainMenu = new GameObject("CameraMovement Menu");
            _cameraMovementMainMenu.transform.parent = parent.transform;
            _cameraMovementMainMenu.AddComponent<DragWindowController>();
            _cameraMovementMainMenu.GetComponent<DragWindowController>().canvas = parent.GetComponent<Canvas>();
            _cameraMovementMainMenu.GetComponent<DragWindowController>().OnDragWindow += AnchoredPosSave;

            //Main Menu
            UI.AttachTransform(_cameraMovementMainMenu, 170, 240, 1, 1, Options.Instance.mainMenuUIAnchoredPosX, Options.Instance.mainMenuUIAnchoredPosY, 1, 1);

            Image imageMain = _cameraMovementMainMenu.AddComponent<Image>();
            imageMain.sprite = PersistentUI.Instance.Sprites.Background;
            imageMain.type = Image.Type.Sliced;
            imageMain.color = new Color(0.24f, 0.24f, 0.24f);

            UI.AddLabel(_cameraMovementMainMenu.transform, "CameraMovement", "CameraMovement", new Vector2(0, -15));
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Movement Enable", "Movement Enable", new Vector2(0, -40), Options.Instance.movement, (check) =>
            {
                Options.Instance.movement = check;
                movementController.Reload();
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "UI Hidden", "UI Hidden", new Vector2(0, -55), Options.Instance.uIhidden, (check) =>
            {
                Options.Instance.uIhidden = check;
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
                movementController.Reload();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Avatar", "Avatar", new Vector2(0, -100), Options.Instance.avatar, (check) =>
            {
                Options.Instance.avatar = check;
                movementController.Reload();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Bookmark Lines", "Bookmark Lines", new Vector2(0, -115), Options.Instance.bookmarkLines, (check) =>
            {
                Options.Instance.bookmarkLines = check;
                movementController.Reload();
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Sub Camera", "Sub Camera", new Vector2(0, -130), Options.Instance.subCamera, (check) =>
            {
                Options.Instance.subCamera = check;
                movementController.Reload();
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Bookmark Edit", "Bookmark Edit", new Vector2(0, -145), Options.Instance.bookmarkEdit, (check) =>
            {
                Options.Instance.bookmarkEdit = check;
                UI._bookmarkMenuUI._cameraMovementBookmarkMenu.SetActive(check);
                UI.KeyDisableCheck();
            });
            UI.AddCheckbox(_cameraMovementMainMenu.transform, "Camera Control", "Camera Control", new Vector2(0, -160), Options.Instance.cameraControl, (check) =>
            {
                Options.Instance.cameraControl = check;
                UI._cameraControlMenuUI._cameraControlMenu.SetActive(check);
                UI.KeyDisableCheck();
            });
            var mainMenuMoreSettingsButton = UI.AddButton(_cameraMovementMainMenu.transform, "More Settings", "More Settings", new Vector2(0, -185), () =>
            {
                UI._settingMenuUI._cameraMovementSettingMenu.SetActive(true);
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(mainMenuMoreSettingsButton.transform, 70, 25, 0.28f, 1, 0, -185);

            var mainMenuReloadButton = UI.AddButton(_cameraMovementMainMenu.transform, "Reload", "Reload", new Vector2(0, -185), () =>
            {
                movementController.Reload();
            });
            UI.MoveTransform(mainMenuReloadButton.transform, 70, 25, 0.72f, 1, 0, -185);

            var mainMenuSettingSaveButton = UI.AddButton(_cameraMovementMainMenu.transform, "Setting Save", "Setting Save", new Vector2(0, -215), () =>
            {
                Options.Instance.SettingSave();
            });
            UI.MoveTransform(mainMenuSettingSaveButton.transform, 70, 25, 0.28f, 1, 0, -215);

            var regexKey = new Regex(@"<\w+>/");
            var mainMenuScriptMapperRunButton = UI.AddButton(_cameraMovementMainMenu.transform, "Script Mapper Run", $"Script Mapper Run [{regexKey.Replace(Options.Instance.scriptMapperKeyBinding, "").ToUpper()}]", new Vector2(0, -215), () =>
            {
                ScriptMapperController.ScriptMapperRun();
            });
            UI.MoveTransform(mainMenuScriptMapperRunButton.transform, 70, 25, 0.72f, 1, 0, -215);

            _cameraMovementMainMenu.SetActive(false);
            UI._extensionBtn.Click = () =>
            {
                _cameraMovementMainMenu.SetActive(!_cameraMovementMainMenu.activeSelf);
            };
        }
    }
}
