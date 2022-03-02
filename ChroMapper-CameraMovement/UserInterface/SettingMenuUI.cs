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

        public void AddMenu(MapEditorUI mapEditorUI)
        {
            movementController = Plugin.movement;
            var parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementSettingMenu = new GameObject("CameraMovement Setting Menu");
            _cameraMovementSettingMenu.transform.parent = parent.transform;

            //More Settings Menu
            UIUtil.AttachTransform(_cameraMovementSettingMenu, 500, 210, 1, 1, 0, 0, 1, 1);

            Image imageSetting = _cameraMovementSettingMenu.AddComponent<Image>();
            imageSetting.sprite = PersistentUI.Instance.Sprites.Background;
            imageSetting.type = Image.Type.Sliced;
            imageSetting.color = new Color(0.24f, 0.24f, 0.24f);

            UIUtil.AddLabel(_cameraMovementSettingMenu.transform, "More Settings", "More Settings", new Vector2(0, -15));

            var avatarCheck = UIUtil.AddCheckbox(_cameraMovementSettingMenu.transform, "Custom VRM Avatar", "Custom or VRM Avatar", new Vector2(0, -40), Options.Instance.customAvatar, (check) =>
            {
                Options.Instance.customAvatar = check;
                movementController.Reload();
            });
            UIUtil.MoveTransform(avatarCheck.Item3.transform, 30, 25, 0, 1, 30, -45);
            UIUtil.MoveTransform(avatarCheck.Item1, 160, 16, 0, 1, 120, -40);

            var reloadButton = UIUtil.AddButton(_cameraMovementSettingMenu.transform, "Reload", "Reload", new Vector2(0, -190), () =>
            {
                movementController.Reload();
            });
            UIUtil.MoveTransform(reloadButton.transform, 40, 20, 1, 1, -80, -40);

            var selectButton = UIUtil.AddButton(_cameraMovementSettingMenu.transform, "Select", "Select", new Vector2(0, -190), () =>
            {
                var ext = new ExtensionFilter[] { new ExtensionFilter("Avatar File", new string[] { "avatar", "vrm" }) };
                var paths = StandaloneFileBrowser.OpenFilePanel("Avatar File", Path.GetDirectoryName(Options.Instance.avatarFileName), ext, false);
                if (paths.Length > 0 && File.Exists(paths[0]))
                {
                    Options.Instance.avatarFileName = paths[0];
                    _avatarFileInputText.InputField.text = paths[0];
                }
            });
            UIUtil.MoveTransform(selectButton.transform, 40, 20, 1, 1, -35, -40);

            var vrmFileInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar File", "Avatar File", new Vector2(0, -75), Options.Instance.avatarFileName, (value) =>
            {
                Options.Instance.avatarFileName = value;
            });
            _avatarFileInputText = vrmFileInput.Item3;
            UIUtil.MoveTransform(vrmFileInput.Item1, 60, 16, 0, 1, 25, -60);
            UIUtil.MoveTransform(vrmFileInput.Item3.transform, 425, 20, 0.1f, 1, 220, -60);

            var scaleInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar Scale", "Avatar Scale", new Vector2(0, -175), Options.Instance.avatarScale.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarScale = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(scaleInput.Item1, 60, 16, 0, 1, 30, -85);
            UIUtil.MoveTransform(scaleInput.Item3.transform, 40, 20, 0.1f, 1, 35, -85);

            var yOffsetInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar Y offset", "Avatar Y offset", new Vector2(0, -195), Options.Instance.avatarYoffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarYoffset = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(yOffsetInput.Item1, 60, 16, 0, 1, 140, -85);
            UIUtil.MoveTransform(yOffsetInput.Item3.transform, 40, 20, 0.1f, 1, 145, -85);

            var blinkerCheck = UIUtil.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Blinker", "VRM Blinker", new Vector2(0, -190), Options.Instance.avatarBlinker, (check) =>
            {
                Options.Instance.avatarBlinker = check;
                movementController.Reload();
            });
            UIUtil.MoveTransform(blinkerCheck.Item3.transform, 30, 16, 0, 1, 250, -85);
            UIUtil.MoveTransform(blinkerCheck.Item1, 50, 16, 0, 1, 280, -85);

            var lookAtCheck = UIUtil.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM LookAt", "VRM LookAt", new Vector2(0, -190), Options.Instance.avatarLookAt, (check) =>
            {
                Options.Instance.avatarLookAt = check;
                movementController.Reload();
            });
            UIUtil.MoveTransform(lookAtCheck.Item3.transform, 30, 16, 0, 1, 330, -85);
            UIUtil.MoveTransform(lookAtCheck.Item1, 70, 16, 0, 1, 370, -85);

            var animationCheck = UIUtil.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Animation", "VRM Animation", new Vector2(0, -190), Options.Instance.avatarAnimation, (check) =>
            {
                Options.Instance.avatarAnimation = check;
                movementController.Reload();
            });
            UIUtil.MoveTransform(animationCheck.Item3.transform, 30, 16, 0, 1, 410, -85);
            UIUtil.MoveTransform(animationCheck.Item1, 70, 16, 0, 1, 450, -85);

            var simpleAvatarCheck = UIUtil.AddCheckbox(_cameraMovementSettingMenu.transform, "Simple Avatar", "Simple Avatar", new Vector2(0, -140), Options.Instance.simpleAvatar, (check) =>
            {
                Options.Instance.simpleAvatar = check;
                movementController.Reload();
            });
            UIUtil.MoveTransform(simpleAvatarCheck.Item3.transform, 30, 16, 0, 1, 30, -110);
            UIUtil.MoveTransform(simpleAvatarCheck.Item1, 70, 16, 0, 1, 70, -110);

            var headHightInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Head Hight", "Head Hight lookat point", new Vector2(0, -140), Options.Instance.avatarHeadHight.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarHeadHight = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(headHightInput.Item1, 60, 16, 0, 1, 130, -110);
            UIUtil.MoveTransform(headHightInput.Item3.transform, 40, 20, 0.1f, 1, 135, -110);

            var headSizeInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Head Size", "Head Size", new Vector2(0, -160), Options.Instance.avatarHeadSize.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarHeadSize = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(headSizeInput.Item1, 60, 16, 0, 1, 230, -110);
            UIUtil.MoveTransform(headSizeInput.Item3.transform, 40, 20, 0.1f, 1, 235, -110);

            var armSizeInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Arm Size", "Arm Size", new Vector2(0, -180), Options.Instance.avatarArmSize.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarArmSize = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(armSizeInput.Item1, 60, 16, 0, 1, 320, -110);
            UIUtil.MoveTransform(armSizeInput.Item3.transform, 40, 20, 0.1f, 1, 325, -110);

            var bookmarkWidthInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Width", "Bookmark Width", new Vector2(0, -205), Options.Instance.bookmarkWidth.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.bookmarkWidth = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(bookmarkWidthInput.Item1, 60, 16, 0, 1, 30, -135);
            UIUtil.MoveTransform(bookmarkWidthInput.Item3.transform, 40, 20, 0.1f, 1, 35, -135);

            var bookmarkAreaInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Area", "Bookmark Area", new Vector2(0, -225), Options.Instance.bookmarkInsertOffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.bookmarkInsertOffset = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(bookmarkAreaInput.Item1, 60, 16, 0, 1, 140, -135);
            UIUtil.MoveTransform(bookmarkAreaInput.Item3.transform, 40, 20, 0.1f, 1, 145, -135);

            var settingMenuBookmarkExportButton = UIUtil.AddButton(_cameraMovementSettingMenu.transform, "Bookmark Export", "Bookmark Export", new Vector2(0, -385), () =>
            {
                movementController.BookmarkExport();
            });
            UIUtil.MoveTransform(settingMenuBookmarkExportButton.transform, 70, 25, 0, 1, 280, -135);

            /*
            var settingMenuBookmarkImportButton = UIUtil.AddButton(_cameraMovementSettingMenu.transform, "Bookmark Import", "Bookmark Import", new Vector2(0, -385), () =>
            {
                movementController.BookmarkImport();
            });
            UIUtil.MoveTransform(settingMenuBookmarkImportButton.transform, 70, 25, 0, 1, 370, -135);
            */

            var rectXInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect X", "Sub Rect X", new Vector2(0, -245), Options.Instance.subCameraRectX.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectX = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(rectXInput.Item1, 60, 16, 0, 1, 30, -160);
            UIUtil.MoveTransform(rectXInput.Item3.transform, 40, 20, 0.1f, 1, 35, -160);

            var rectYInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect Y", "Sub Rect Y", new Vector2(0, -265), Options.Instance.subCameraRectY.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectY = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(rectYInput.Item1, 60, 16, 0, 1, 140, -160);
            UIUtil.MoveTransform(rectYInput.Item3.transform, 40, 20, 0.1f, 1, 145, -160);

            var rectWInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect W", "Sub Rect W", new Vector2(0, -285), Options.Instance.subCameraRectW.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectW = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(rectWInput.Item1, 60, 16, 0, 1, 250, -160);
            UIUtil.MoveTransform(rectWInput.Item3.transform, 40, 20, 0.1f, 1, 255, -160);

            var rectHInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect H", "Sub Rect H", new Vector2(0, -305), Options.Instance.subCameraRectH.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectH = res;
                    movementController.Reload();
                }
            });
            UIUtil.MoveTransform(rectHInput.Item1, 60, 16, 0, 1, 360, -160);
            UIUtil.MoveTransform(rectHInput.Item3.transform, 40, 20, 0.1f, 1, 365, -160);

            var scriptFileInput = UIUtil.AddTextInput(_cameraMovementSettingMenu.transform, "Script File", "Script File", new Vector2(0, -325), Options.Instance.scriptFileName, (value) =>
            {
                Options.Instance.scriptFileName = value.Trim();
            });
            UIUtil.MoveTransform(scriptFileInput.Item1, 60, 16, 0, 1, 30, -185);
            UIUtil.MoveTransform(scriptFileInput.Item3.transform, 80, 20, 0.1f, 1, 55, -185);

            var saveButton = UIUtil.AddButton(_cameraMovementSettingMenu.transform, "Setting Save", "Setting Save", new Vector2(0, -250), () =>
            {
                Options.Instance.SettingSave();
            });
            UIUtil.MoveTransform(saveButton.transform, 70, 25, 0, 1, 350, -190);

            var closeButton = UIUtil.AddButton(_cameraMovementSettingMenu.transform, "Close", "Close", new Vector2(0, -415), () =>
            {
                _cameraMovementSettingMenu.SetActive(false);
            });
            UIUtil.MoveTransform(closeButton.transform, 70, 25, 0, 1, 440, -190);

            _cameraMovementSettingMenu.SetActive(false);
        }
    }
}
