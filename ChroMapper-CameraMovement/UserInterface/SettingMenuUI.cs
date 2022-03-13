using System.IO;
using UnityEngine;
using UnityEngine.UI;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using SFB;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class SettingMenuUI
    {
        public GameObject _cameraMovementSettingMenu;
        public CameraMovementController movementController;
        public UITextInput _avatarFileInputText;
        public UITextInput subRectX;
        public UITextInput subRectY;
        public UITextInput subRectW;
        public UITextInput subRectH;

        public void AnchoredPosSave()
        {
            Options.Instance.settingMenuUIAnchoredPosX = _cameraMovementSettingMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.settingMenuUIAnchoredPosY = _cameraMovementSettingMenu.GetComponent<RectTransform>().anchoredPosition.y;
        }

        public void SubCameraRectSet()
        {
            subRectX.InputField.text = Options.Instance.subCameraRectX.ToString("0.##");
            subRectY.InputField.text = Options.Instance.subCameraRectY.ToString("0.##");
            subRectW.InputField.text = Options.Instance.subCameraRectW.ToString("0.##");
            subRectH.InputField.text = Options.Instance.subCameraRectH.ToString("0.##");
        }

        public void AddMenu(MapEditorUI mapEditorUI)
        {
            movementController = Plugin.movement;
            var parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementSettingMenu = new GameObject("CameraMovement Setting Menu");
            _cameraMovementSettingMenu.transform.parent = parent.transform;
            _cameraMovementSettingMenu.AddComponent<DragWindowController>();
            _cameraMovementSettingMenu.GetComponent<DragWindowController>().canvas = parent.GetComponent<Canvas>();
            _cameraMovementSettingMenu.GetComponent<DragWindowController>().OnDragWindow += AnchoredPosSave;

            //More Settings Menu
            UI.AttachTransform(_cameraMovementSettingMenu, 500, 210, 1, 1, Options.Instance.settingMenuUIAnchoredPosX, Options.Instance.settingMenuUIAnchoredPosY, 1, 1);

            Image imageSetting = _cameraMovementSettingMenu.AddComponent<Image>();
            imageSetting.sprite = PersistentUI.Instance.Sprites.Background;
            imageSetting.type = Image.Type.Sliced;
            imageSetting.color = new Color(0.24f, 0.24f, 0.24f);

            UI.AddLabel(_cameraMovementSettingMenu.transform, "More Settings", "More Settings", new Vector2(0, -15));

            var avatarCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Custom VRM Avatar", "Custom or VRM Avatar", new Vector2(0, -40), Options.Instance.customAvatar, (check) =>
            {
                Options.Instance.customAvatar = check;
                movementController.Reload();
            });
            UI.MoveTransform(avatarCheck.Item3.transform, 30, 25, 0, 1, 30, -45);
            UI.MoveTransform(avatarCheck.Item1, 160, 16, 0, 1, 120, -40);

            var reloadButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Reload", "Reload", new Vector2(0, -190), () =>
            {
                movementController.Reload();
            });
            UI.MoveTransform(reloadButton.transform, 40, 20, 1, 1, -80, -40);

            var selectButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Select", "Select", new Vector2(0, -190), () =>
            {
                var ext = new ExtensionFilter[] { new ExtensionFilter("Avatar File", new string[] { "avatar", "vrm" }) };
                var paths = StandaloneFileBrowser.OpenFilePanel("Avatar File", Path.GetDirectoryName(Options.Instance.avatarFileName), ext, false);
                if (paths.Length > 0 && File.Exists(paths[0]))
                {
                    Options.Instance.avatarFileName = paths[0];
                    _avatarFileInputText.InputField.text = paths[0];
                }
            });
            UI.MoveTransform(selectButton.transform, 40, 20, 1, 1, -35, -40);

            var vrmFileInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar File", "Avatar File", new Vector2(0, -75), Options.Instance.avatarFileName, (value) =>
            {
                Options.Instance.avatarFileName = value;
            });
            _avatarFileInputText = vrmFileInput.Item3;
            UI.MoveTransform(vrmFileInput.Item1, 60, 16, 0, 1, 25, -60);
            UI.MoveTransform(vrmFileInput.Item3.transform, 425, 20, 0.1f, 1, 220, -60);

            var scaleInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar Scale", "Avatar Scale", new Vector2(0, -175), Options.Instance.avatarScale.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarScale = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(scaleInput.Item1, 60, 16, 0, 1, 30, -85);
            UI.MoveTransform(scaleInput.Item3.transform, 40, 20, 0.1f, 1, 35, -85);

            var yOffsetInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar Y offset", "Avatar Y offset", new Vector2(0, -195), Options.Instance.avatarYoffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarYoffset = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(yOffsetInput.Item1, 60, 16, 0, 1, 140, -85);
            UI.MoveTransform(yOffsetInput.Item3.transform, 40, 20, 0.1f, 1, 145, -85);

            var blinkerCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Blinker", "VRM Blinker", new Vector2(0, -190), Options.Instance.avatarBlinker, (check) =>
            {
                Options.Instance.avatarBlinker = check;
                movementController.Reload();
            });
            UI.MoveTransform(blinkerCheck.Item3.transform, 30, 16, 0, 1, 250, -85);
            UI.MoveTransform(blinkerCheck.Item1, 50, 16, 0, 1, 280, -85);

            var lookAtCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM LookAt", "VRM LookAt", new Vector2(0, -190), Options.Instance.avatarLookAt, (check) =>
            {
                Options.Instance.avatarLookAt = check;
                movementController.Reload();
            });
            UI.MoveTransform(lookAtCheck.Item3.transform, 30, 16, 0, 1, 330, -85);
            UI.MoveTransform(lookAtCheck.Item1, 70, 16, 0, 1, 370, -85);

            var animationCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Animation", "VRM Animation", new Vector2(0, -190), Options.Instance.avatarAnimation, (check) =>
            {
                Options.Instance.avatarAnimation = check;
                movementController.Reload();
            });
            UI.MoveTransform(animationCheck.Item3.transform, 30, 16, 0, 1, 410, -85);
            UI.MoveTransform(animationCheck.Item1, 70, 16, 0, 1, 450, -85);

            var simpleAvatarCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Simple Avatar", "Simple Avatar", new Vector2(0, -140), Options.Instance.simpleAvatar, (check) =>
            {
                Options.Instance.simpleAvatar = check;
                movementController.Reload();
            });
            UI.MoveTransform(simpleAvatarCheck.Item3.transform, 30, 16, 0, 1, 30, -110);
            UI.MoveTransform(simpleAvatarCheck.Item1, 70, 16, 0, 1, 70, -110);

            var headHightInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Head Hight", "Head Hight lookat point", new Vector2(0, -140), Options.Instance.avatarHeadHight.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarHeadHight = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(headHightInput.Item1, 60, 16, 0, 1, 130, -110);
            UI.MoveTransform(headHightInput.Item3.transform, 40, 20, 0.1f, 1, 135, -110);

            var headSizeInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Head Size", "Head Size", new Vector2(0, -160), Options.Instance.avatarHeadSize.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarHeadSize = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(headSizeInput.Item1, 60, 16, 0, 1, 230, -110);
            UI.MoveTransform(headSizeInput.Item3.transform, 40, 20, 0.1f, 1, 235, -110);

            var armSizeInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Arm Size", "Arm Size", new Vector2(0, -180), Options.Instance.avatarArmSize.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarArmSize = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(armSizeInput.Item1, 60, 16, 0, 1, 320, -110);
            UI.MoveTransform(armSizeInput.Item3.transform, 40, 20, 0.1f, 1, 325, -110);

            var bookmarkWidthInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Width", "Bookmark Width", new Vector2(0, -205), Options.Instance.bookmarkWidth.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.bookmarkWidth = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(bookmarkWidthInput.Item1, 60, 16, 0, 1, 30, -135);
            UI.MoveTransform(bookmarkWidthInput.Item3.transform, 40, 20, 0.1f, 1, 35, -135);

            var bookmarkAreaInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Area", "Bookmark Area", new Vector2(0, -225), Options.Instance.bookmarkInsertOffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.bookmarkInsertOffset = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(bookmarkAreaInput.Item1, 60, 16, 0, 1, 140, -135);
            UI.MoveTransform(bookmarkAreaInput.Item3.transform, 40, 20, 0.1f, 1, 145, -135);

            var settingMenuBookmarkExportButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Bookmark Export", "Bookmark Export", new Vector2(0, -385), () =>
            {
                movementController.BookmarkExport();
            });
            UI.MoveTransform(settingMenuBookmarkExportButton.transform, 70, 25, 0, 1, 280, -135);

            /*
            var settingMenuBookmarkImportButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Bookmark Import", "Bookmark Import", new Vector2(0, -385), () =>
            {
                movementController.BookmarkImport();
            });
            UI.MoveTransform(settingMenuBookmarkImportButton.transform, 70, 25, 0, 1, 370, -135);
            */

            var rectXInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect X", "Sub X", new Vector2(0, -245), Options.Instance.subCameraRectX.ToString(), (value) =>
            {
                if (movementController.SubCameraRectEnableGet())
                    return;
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectX = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(rectXInput.Item1, 60, 16, 0, 1, 30, -160);
            UI.MoveTransform(rectXInput.Item3.transform, 40, 20, 0.1f, 1, 35, -160);
            subRectX = rectXInput.Item3;

            var rectYInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect Y", "Sub Y", new Vector2(0, -265), Options.Instance.subCameraRectY.ToString(), (value) =>
            {
                if (movementController.SubCameraRectEnableGet())
                    return;
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectY = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(rectYInput.Item1, 60, 16, 0, 1, 110, -160);
            UI.MoveTransform(rectYInput.Item3.transform, 40, 20, 0.1f, 1, 115, -160);
            subRectY = rectYInput.Item3;

            var subMoveButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Move", "Cursor Key Move", new Vector2(0, -190), () =>
            {
                movementController.SubCameraRectEnable(true);
            });
            UI.MoveTransform(subMoveButton.transform, 50, 25, 0, 1, 220, -160);

            var rectWInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect W", "Sub W", new Vector2(0, -285), Options.Instance.subCameraRectW.ToString(), (value) =>
            {
                if (movementController.SubCameraRectEnableGet())
                    return;
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectW = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(rectWInput.Item1, 60, 16, 0, 1, 250, -160);
            UI.MoveTransform(rectWInput.Item3.transform, 40, 20, 0.1f, 1, 255, -160);
            subRectW = rectWInput.Item3;

            var rectHInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect H", "Sub H", new Vector2(0, -305), Options.Instance.subCameraRectH.ToString(), (value) =>
            {
                if (movementController.SubCameraRectEnableGet())
                    return;
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectH = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(rectHInput.Item1, 60, 16, 0, 1, 330, -160);
            UI.MoveTransform(rectHInput.Item3.transform, 40, 20, 0.1f, 1, 335, -160);
            subRectH = rectHInput.Item3;

            var subSizeButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Move", "Cursor Key Size", new Vector2(0, -190), () =>
            {
                movementController.SubCameraRectEnable(false);
            });
            UI.MoveTransform(subSizeButton.transform, 50, 25, 0, 1, 440, -160);

            var scriptFileInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Script File", "Script File", new Vector2(0, -325), Options.Instance.scriptFileName, (value) =>
            {
                Options.Instance.scriptFileName = value.Trim();
            });
            UI.MoveTransform(scriptFileInput.Item1, 60, 16, 0, 1, 30, -185);
            UI.MoveTransform(scriptFileInput.Item3.transform, 80, 20, 0.1f, 1, 55, -185);

            var mappingDisableCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Mapping Disable", "Mapping Disable", new Vector2(0, -140), Options.Instance.mappingDisable, (check) =>
            {
                Options.Instance.mappingDisable = check;
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(mappingDisableCheck.Item3.transform, 30, 16, 0, 1, 170, -185);
            UI.MoveTransform(mappingDisableCheck.Item1, 70, 16, 0, 1, 210, -185);

            var saveButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Setting Save", "Setting Save", new Vector2(0, -250), () =>
            {
                Options.Instance.SettingSave();
            });
            UI.MoveTransform(saveButton.transform, 70, 25, 0, 1, 350, -190);

            var closeButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Close", "Close", new Vector2(0, -415), () =>
            {
                _cameraMovementSettingMenu.SetActive(false);
                UI.KeyDisableCheck();
                movementController.SubCameraRectDisable();
            });
            UI.MoveTransform(closeButton.transform, 70, 25, 0, 1, 440, -190);

            _cameraMovementSettingMenu.SetActive(false);
        }
    }
}
