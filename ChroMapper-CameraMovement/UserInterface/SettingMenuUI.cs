﻿using System.IO;
using UnityEngine;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using SFB;
using ChroMapper_CameraMovement.Controller;

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
        public UITextInput avatarScale;

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

        public void AddMenu(CanvasGroup topBarCanvas)
        {
            movementController = Plugin.movement;
            //More Settings Menu
            _cameraMovementSettingMenu = UI.SetMenu(new GameObject("CameraMovement Setting Menu"), topBarCanvas, AnchoredPosSave, 500, 240, Options.Instance.settingMenuUIAnchoredPosX, Options.Instance.settingMenuUIAnchoredPosY);

            UI.AddLabel(_cameraMovementSettingMenu.transform, "More Settings", "More Settings", new Vector2(0, -15));

            var avatarCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Custom VRM Avatar", "Custom or VRM Avatar", Options.Instance.customAvatar, (check) =>
            {
                Options.Instance.customAvatar = check;
                movementController.Reload();
            });
            UI.MoveTransform(avatarCheck.Item3.transform, 30, 25, 0, 1, 30, -45);
            UI.MoveTransform(avatarCheck.Item1, 160, 16, 0, 1, 120, -40);

            var reloadButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Reload", "Reload", () =>
            {
                movementController.Reload();
            });
            UI.MoveTransform(reloadButton.transform, 40, 20, 1, 1, -80, -40);

            var selectButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Select", "Select", () =>
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

            var vrmFileInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar File", "Avatar File", Options.Instance.avatarFileName, (value) =>
            {
                Options.Instance.avatarFileName = value;
            });
            _avatarFileInputText = vrmFileInput.Item3;
            UI.MoveTransform(vrmFileInput.Item1, 60, 16, 0, 1, 25, -60);
            UI.MoveTransform(vrmFileInput.Item3.transform, 425, 20, 0.1f, 1, 220, -60);

            var scaleInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar Scale", "Avatar Scale", Options.Instance.avatarScale.ToString(), (value) =>
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
            avatarScale = scaleInput.Item3;

            var yOffsetInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Avatar Y offset", "Avatar Y offset", Options.Instance.avatarYoffset.ToString(), (value) =>
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

            var blinkerCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Blinker", "VRM Blinker", Options.Instance.avatarBlinker, (check) =>
            {
                Options.Instance.avatarBlinker = check;
                movementController.Reload();
            });
            UI.MoveTransform(blinkerCheck.Item3.transform, 30, 16, 0, 1, 250, -85);
            UI.MoveTransform(blinkerCheck.Item1, 50, 16, 0, 1, 280, -85);

            var lookAtCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM LookAt", "VRM LookAt", Options.Instance.avatarLookAt, (check) =>
            {
                Options.Instance.avatarLookAt = check;
                movementController.Reload();
            });
            UI.MoveTransform(lookAtCheck.Item3.transform, 30, 16, 0, 1, 330, -85);
            UI.MoveTransform(lookAtCheck.Item1, 70, 16, 0, 1, 370, -85);

            var animationCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Animation", "VRM Animation", Options.Instance.avatarAnimation, (check) =>
            {
                Options.Instance.avatarAnimation = check;
                movementController.Reload();
            });
            UI.MoveTransform(animationCheck.Item3.transform, 30, 16, 0, 1, 410, -85);
            UI.MoveTransform(animationCheck.Item1, 70, 16, 0, 1, 450, -85);

            var simpleAvatarCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Simple Avatar", "Simple Avatar", Options.Instance.simpleAvatar, (check) =>
            {
                Options.Instance.simpleAvatar = check;
                movementController.Reload();
            });
            UI.MoveTransform(simpleAvatarCheck.Item3.transform, 30, 16, 0, 1, 30, -110);
            UI.MoveTransform(simpleAvatarCheck.Item1, 70, 16, 0, 1, 70, -110);

            var headHightInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Head Hight", "Head Hight lookat point", Options.Instance.avatarHeadHight.ToString(), (value) =>
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

            var headSizeInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Head Size", "Head Size", Options.Instance.avatarHeadSize.ToString(), (value) =>
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

            var armSizeInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Arm Size", "Arm Size", Options.Instance.avatarArmSize.ToString(), (value) =>
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

            var bookmarkWidthInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Width", "Bookmark Width", Options.Instance.bookmarkWidth.ToString(), (value) =>
            {
                int res;
                if (int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.bookmarkWidth = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(bookmarkWidthInput.Item1, 60, 16, 0, 1, 30, -135);
            UI.MoveTransform(bookmarkWidthInput.Item3.transform, 40, 20, 0.1f, 1, 35, -135);

            var bookmarkAreaInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Area", "Bookmark Area", Options.Instance.bookmarkInsertOffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.bookmarkInsertOffset = res;
                    movementController.Reload();
                }
            });
            UI.MoveTransform(bookmarkAreaInput.Item1, 50, 16, 0, 1, 130, -135);
            UI.MoveTransform(bookmarkAreaInput.Item3.transform, 40, 20, 0.1f, 1, 135, -135);

            var bookmarkSizeInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Size", "Bookmark Size(%)", Options.Instance.bookmarkLinesFontSize.ToString(), (value) =>
            {
                int res;
                if (int.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                    Options.Instance.bookmarkLinesFontSize = res;
            });
            UI.MoveTransform(bookmarkSizeInput.Item1, 50, 16, 0, 1, 230, -135);
            UI.MoveTransform(bookmarkSizeInput.Item3.transform, 40, 20, 0.1f, 1, 235, -135);

            var settingMenuBookmarkExportButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Bookmark Export", "Bookmark Export", () =>
            {
                movementController._bookmarkController.BookmarkExport();
            });
            UI.MoveTransform(settingMenuBookmarkExportButton.transform, 70, 25, 0, 1, 360, -135);

            /*
            var settingMenuBookmarkImportButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Bookmark Import", "Bookmark Import", () =>
            {
                movementController.BookmarkImport();
            });
            UI.MoveTransform(settingMenuBookmarkImportButton.transform, 70, 25, 0, 1, 450, -135);
            */

            var rectXInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect X", "Sub X", Options.Instance.subCameraRectX.ToString(), (value) =>
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

            var rectYInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect Y", "Sub Y", Options.Instance.subCameraRectY.ToString(), (value) =>
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

            var subMoveButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Move", "Cursor Key Move", () =>
            {
                movementController.SubCameraRectEnable(true);
            });
            UI.MoveTransform(subMoveButton.transform, 50, 25, 0, 1, 220, -160);

            var rectWInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect W", "Sub W", Options.Instance.subCameraRectW.ToString(), (value) =>
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

            var rectHInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect H", "Sub H", Options.Instance.subCameraRectH.ToString(), (value) =>
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

            var subSizeButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Move", "Cursor Key Size", () =>
            {
                movementController.SubCameraRectEnable(false);
            });
            UI.MoveTransform(subSizeButton.transform, 50, 25, 0, 1, 440, -160);

            var scriptFileInput = UI.AddTextInput(_cameraMovementSettingMenu.transform, "Script File", "Script File", Options.Instance.scriptFileName, (value) =>
            {
                Options.Instance.scriptFileName = value.Trim();
            });
            UI.MoveTransform(scriptFileInput.Item1, 60, 16, 0, 1, 30, -185);
            UI.MoveTransform(scriptFileInput.Item3.transform, 80, 20, 0.1f, 1, 55, -185);

            var mappingDisableCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Mapping Disable", "Mapping Disable", Options.Instance.mappingDisable, (check) =>
            {
                Options.Instance.mappingDisable = check;
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(mappingDisableCheck.Item3.transform, 30, 16, 0, 1, 170, -185);
            UI.MoveTransform(mappingDisableCheck.Item1, 70, 16, 0, 1, 210, -185);

            var bookmarkLinesTopCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Bookmark Lines Top", "Bookmark Lines Top", Options.Instance.bookmarkLinesShowOnTop, (check) =>
            {
                Options.Instance.bookmarkLinesShowOnTop = check;
                CameraMovementController.bookmarkLinesCanvas.GetComponent<BookmarkLinesRenderingOrderController>().BookmakLinesCameraINIT(Options.Instance.bookmarkLinesShowOnTop);
            });
            UI.MoveTransform(bookmarkLinesTopCheck.Item3.transform, 30, 16, 0, 1, 260, -185);
            UI.MoveTransform(bookmarkLinesTopCheck.Item1, 50, 16, 0, 1, 290, -185);

            var cameraDirectionScrollReversalCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Camera Direction Scroll Reversal", "Camera Direction Scroll Reversal", Options.Instance.cameraDirectionScrollReversal, (check) =>
            {
                Options.Instance.cameraDirectionScrollReversal = check;
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(cameraDirectionScrollReversalCheck.Item3.transform, 30, 16, 0, 1, 330, -185);
            UI.MoveTransform(cameraDirectionScrollReversalCheck.Item1, 70, 16, 0, 1, 370, -185);

            var defaultInvertScrollTimeCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Default Invert Scroll Time", "Default Invert Scroll Time", Options.Instance.defaultInvertScrollTime, (check) =>
            {
                Options.Instance.defaultInvertScrollTime = check;
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(defaultInvertScrollTimeCheck.Item3.transform, 30, 16, 0, 1, 420, -185);
            UI.MoveTransform(defaultInvertScrollTimeCheck.Item1, 70, 16, 0, 1, 460, -185);

            var setAvatarScaleButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Set Avatar Scale", "Set current display to Avatar Scale", () =>
            {
                var scale = new Vector3(Options.Instance.avatarScale, Options.Instance.avatarScale, Options.Instance.avatarScale) * Options.Instance.avatarCameraScale;
                if (CameraMovementController.avatarModel != null)
                    scale = CameraMovementController.avatarModel.transform.localScale;
                else if (VRMAvatarController.avatar != null)
                    scale = VRMAvatarController.avatar.Root.transform.localScale;
                scale /= Options.Instance.avatarCameraScale;
                if (scale.x == scale.y && scale.y == scale.z)
                {
                    Options.Instance.avatarScale = scale.x;
                    avatarScale.InputField.text = Options.Instance.avatarScale.ToString();
                }
                else
                {
                    var dialogBox = PersistentUI.Instance
                        .CreateNewDialogBox()
                        .WithTitle($"The scales x, y, z are not identical. x:{scale.x}  y:{scale.y}  z:{scale.z}");
                    dialogBox.AddFooterButton(() => dialogBox.Close(), "PersistentUI", "ok");
                    dialogBox.Open();
                }
            });
            UI.MoveTransform(setAvatarScaleButton.transform, 100, 25, 0, 1, 80, -220);

            var vrmSpringBoneCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Spring Bone", "VRM Spring Bone", Options.Instance.vrmSpringBone, (check) =>
            {
                Options.Instance.vrmSpringBone = check;
                VRMAvatarController.SpringBoneEnable();
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(vrmSpringBoneCheck.Item3.transform, 30, 16, 0, 1, 150, -220);
            UI.MoveTransform(vrmSpringBoneCheck.Item1, 70, 16, 0, 1, 190, -220);

            var orbitTargetCheck = UI.AddCheckbox(_cameraMovementSettingMenu.transform, "Orbit Camera Target", "Orbit Camera Target", Options.Instance.orbitTargetObject, (check) =>
            {
                Options.Instance.orbitTargetObject = check;
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(orbitTargetCheck.Item3.transform, 30, 16, 0, 1, 250, -220);
            UI.MoveTransform(orbitTargetCheck.Item1, 70, 16, 0, 1, 290, -220);

            var saveButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Setting Save", "Setting Save", () =>
            {
                Options.Instance.SettingSave();
            });
            UI.MoveTransform(saveButton.transform, 60, 25, 0, 1, 380, -220);

            var closeButton = UI.AddButton(_cameraMovementSettingMenu.transform, "Close", "Close", () =>
            {
                _cameraMovementSettingMenu.SetActive(false);
                UI.KeyDisableCheck();
                movementController.SubCameraRectDisable();
            });
            UI.MoveTransform(closeButton.transform, 50, 25, 0, 1, 440, -220);

            _cameraMovementSettingMenu.SetActive(false);
        }
    }
}
