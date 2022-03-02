﻿using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;
using SFB;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class UI
    {
        private GameObject _cameraMovementMainMenu;
        private GameObject _cameraMovementSettingMenu;
        private GameObject _cameraMovementBookmarkMenu;
        private GameObject _cameraMovementCameraControlMenu;
        public static CameraMovementController movementController;
        private readonly ExtensionButton _extensionBtn = new ExtensionButton();
        private UITextInput _avatarFileInputText;
        private UITextInput _cameraPosXinput;
        private UITextInput _cameraPosYinput;
        private UITextInput _cameraPosZinput;
        private UITextInput _cameraRotXinput;
        private UITextInput _cameraRotYinput;
        private UITextInput _cameraRotZinput;
        private UITextInput _cameraFOVinput;
        private UITextInput _cameraDistanceinput;
        public static GameObject cm_MapEditorCamera;
        public TextMeshProUGUI currentBookmarkLabelText;
        public UITextInput bookmarkMenuInputText;
        public TextMeshProUGUI bookmarkMenuInputLabel;
        public int currentBookmarkNo = 0;
        public bool commandSet = false;
        public Toggle bookmarkSetCheckboxToggle;
        public UIButton quickCommand1Button;
        public UIButton quickCommand2Button;
        public UIButton quickCommand3Button;
        public UIButton quickCommand4Button;
        public UIButton quickCommand5Button;
        public UIButton quickCommand6Button;
        public RectTransform _cameraMovementCameraControlMenuRect;
        public bool cameraPosRotNoUpdate = false;

        UnityAction<string> posXChange;
        UnityAction<string> posYChange;
        UnityAction<string> posZChange;
        UnityAction<string> rotXChange;
        UnityAction<string> rotYChange;
        UnityAction<string> rotZChange;
        UnityAction<string> fovChange;
        UnityAction<string> distanceChange;

        public enum CameraItem
        {
            PosX,
            PosY,
            PosZ,
            RotX,
            RotY,
            RotZ
        }
        public void CameraPosRotTextSet(Vector3 position,Vector3 rotation)
        {
            _cameraPosXinput.InputField.text = position.x.ToString("0.##");
            _cameraPosYinput.InputField.text = position.y.ToString("0.##");
            _cameraPosZinput.InputField.text = position.z.ToString("0.##");
            _cameraRotXinput.InputField.text = rotation.x.ToString("0.#");
            _cameraRotYinput.InputField.text = rotation.y.ToString("0.#");
            _cameraRotZinput.InputField.text = rotation.z.ToString("0.#");
        }
        public void CameraPosRotUpdate()
        {
            if (!cameraPosRotNoUpdate)
            {
            _cameraPosXinput.InputField.onValueChanged.RemoveAllListeners();
            _cameraPosYinput.InputField.onValueChanged.RemoveAllListeners();
            _cameraPosZinput.InputField.onValueChanged.RemoveAllListeners();
            _cameraRotXinput.InputField.onValueChanged.RemoveAllListeners();
            _cameraRotYinput.InputField.onValueChanged.RemoveAllListeners();
            _cameraRotZinput.InputField.onValueChanged.RemoveAllListeners();
            _cameraFOVinput.InputField.onValueChanged.RemoveAllListeners();
            _cameraDistanceinput.InputField.onValueChanged.RemoveAllListeners();

            var cameraPosition = movementController.CameraPositionGet();
            CameraPosRotTextSet(cameraPosition, movementController.CameraTransformGet().eulerAngles);
            _cameraFOVinput.InputField.text = Settings.Instance.CameraFOV.ToString("0.#");
            var distance = Vector3.Distance(movementController.AvatarPositionGet(), cameraPosition).ToString("0.#");
            _cameraDistanceinput.InputField.text = distance;

            _cameraPosXinput.InputField.onValueChanged.AddListener(posXChange);
            _cameraPosYinput.InputField.onValueChanged.AddListener(posYChange);
            _cameraPosZinput.InputField.onValueChanged.AddListener(posZChange);
            _cameraRotXinput.InputField.onValueChanged.AddListener(rotXChange);
            _cameraRotYinput.InputField.onValueChanged.AddListener(rotYChange);
            _cameraRotZinput.InputField.onValueChanged.AddListener(rotZChange);
            _cameraFOVinput.InputField.onValueChanged.AddListener(fovChange);
            _cameraDistanceinput.InputField.onValueChanged.AddListener(distanceChange);
            }
            else
            {
                cameraPosRotNoUpdate = false;
            }
        }
        public void CameraPosRotSet(CameraItem item,float value)
        {
            var position = movementController.CameraPositionGet();
            var rotation = movementController.CameraTransformGet().eulerAngles;
            switch (item)
            {
                case CameraItem.PosX:
                    position.x = value;
                    break;
                case CameraItem.PosY:
                    position.y = value;
                    break;
                case CameraItem.PosZ:
                    position.z = value;
                    break;
                case CameraItem.RotX:
                    rotation.x = value;
                    break;
                case CameraItem.RotY:
                    rotation.y = value;
                    break;
                case CameraItem.RotZ:
                    rotation.z = value;
                    break;
            }
            movementController.CameraPositionAndRotationSet(position, rotation);
            cameraPosRotNoUpdate = true;
        }

        public void CurrentBookmarkUpdate(string bookmarkName,int bookmarkNo, float time)
        {
            currentBookmarkNo = bookmarkNo;
            currentBookmarkLabelText.text = bookmarkName;
            if (bookmarkNo == 0)
            {
                bookmarkMenuInputLabel.text = "Current No.-";
            }
            else
            {
                bookmarkMenuInputLabel.text = $"Current No.{bookmarkNo} [{time.ToString("0.####")}]";
            }
        }

        public UI()
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

        public void CameraMovementControllerSet()
        {
            movementController = GameObject.Find("/CameraMovement").gameObject.GetComponent<CameraMovementController>();
        }
        public void CameraControlPanelPosition()
        {
            if (Options.Instance.bookmarkEdit)
            {
                _cameraMovementCameraControlMenuRect.anchorMin = _cameraMovementCameraControlMenuRect.anchorMax = new Vector2(0.7f, 0.2f);
            }
            else
            {
                _cameraMovementCameraControlMenuRect.anchorMin = _cameraMovementCameraControlMenuRect.anchorMax = new Vector2(0.55f, 0.09f);
            }
        }

        public void AddMenu(MapEditorUI mapEditorUI)
        {
            CanvasGroup parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementMainMenu = new GameObject("CameraMovement Menu");
            _cameraMovementSettingMenu = new GameObject("CameraMovement Setting Menu");
            _cameraMovementBookmarkMenu = new GameObject("CameraMovement Bookmark");
            _cameraMovementCameraControlMenu = new GameObject("Camera Control");
            _cameraMovementMainMenu.transform.parent = parent.transform;
            _cameraMovementBookmarkMenu.transform.parent = parent.transform;
            _cameraMovementCameraControlMenu.transform.parent = parent.transform;
            _cameraMovementSettingMenu.transform.parent = parent.transform;

            //Main Menu
            AttachTransform(_cameraMovementMainMenu, 170, 225, 1, 1, -50, -30, 1, 1);

            Image imageMain = _cameraMovementMainMenu.AddComponent<Image>();
            imageMain.sprite = PersistentUI.Instance.Sprites.Background;
            imageMain.type = Image.Type.Sliced;
            imageMain.color = new Color(0.24f, 0.24f, 0.24f);

            AddLabel(_cameraMovementMainMenu.transform, "CameraMovement", "CameraMovement", new Vector2(0, -15));
            AddCheckbox(_cameraMovementMainMenu.transform, "Movement Enable", "Movement Enable", new Vector2(0, -40), Options.Instance.movement, (check) =>
            {
                Options.Instance.movement = check;
                movementController.Reload();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "UI Hidden", "UI Hidden", new Vector2(0, -55), Options.Instance.uIhidden, (check) =>
            {
                Options.Instance.uIhidden = check;
                movementController.UiHidden();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "Turn To Head", "Turn To Head", new Vector2(0, -70), Options.Instance.turnToHead, (check) =>
            {
                Options.Instance.turnToHead = check;
                movementController.Reload();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "Avatar", "Avatar", new Vector2(0, -85), Options.Instance.avatar, (check) =>
            {
                Options.Instance.avatar = check;
                movementController.Reload();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "Bookmark Lines", "Bookmark Lines", new Vector2(0, -100), Options.Instance.bookmarkLines, (check) =>
            {
                Options.Instance.bookmarkLines = check;
                movementController.Reload();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "Sub Camera", "Sub Camera", new Vector2(0, -115), Options.Instance.subCamera, (check) =>
            {
                Options.Instance.subCamera = check;
                movementController.Reload();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "Bookmark Edit", "Bookmark Edit", new Vector2(0, -130), Options.Instance.bookmarkEdit, (check) =>
            {
                Options.Instance.bookmarkEdit = check;
                _cameraMovementBookmarkMenu.SetActive(check);
                CameraControlPanelPosition();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "Camera Control", "Camera Control", new Vector2(0, -145), Options.Instance.cameraControl, (check) =>
            {
                Options.Instance.cameraControl = check;
                _cameraMovementCameraControlMenu.SetActive(check);
                CameraControlPanelPosition();
            });
            var mainMenuMoreSettingsButton = AddButton(_cameraMovementMainMenu.transform, "More Settings", "More Settings", new Vector2(0, -170), () =>
            {
                _cameraMovementSettingMenu.SetActive(true);
            });
            MoveTransform(mainMenuMoreSettingsButton.transform, 70, 25, 0.28f, 1, 0, -170);

            var mainMenuReloadButton = AddButton(_cameraMovementMainMenu.transform, "Reload", "Reload", new Vector2(0, -170), () =>
            {
                movementController.Reload();
            });
            MoveTransform(mainMenuReloadButton.transform, 70, 25, 0.72f, 1, 0, -170);

            var mainMenuSettingSaveButton = AddButton(_cameraMovementMainMenu.transform, "Setting Save", "Setting Save", new Vector2(0, -200), () =>
            {
                Options.Instance.SettingSave();
            });
            MoveTransform(mainMenuSettingSaveButton.transform, 70, 25, 0.28f, 1, 0, -200);

            var mainMenuScriptMapperRunButton = AddButton(_cameraMovementMainMenu.transform, "Script Mapper Run", "Script Mapper Run", new Vector2(0, -200), () =>
            {
                ScriptMapperController.ScriptMapperRun(movementController);
            });
            MoveTransform(mainMenuScriptMapperRunButton.transform, 70, 25, 0.72f, 1, 0, -200);

            _cameraMovementMainMenu.SetActive(false);
            _extensionBtn.Click = () =>
            {
                _cameraMovementMainMenu.SetActive(!_cameraMovementMainMenu.activeSelf);
            };

            //More Settings Menu
            AttachTransform(_cameraMovementSettingMenu, 500, 210, 1, 1, 0, 0, 1, 1);

            Image imageSetting = _cameraMovementSettingMenu.AddComponent<Image>();
            imageSetting.sprite = PersistentUI.Instance.Sprites.Background;
            imageSetting.type = Image.Type.Sliced;
            imageSetting.color = new Color(0.24f, 0.24f, 0.24f);

            AddLabel(_cameraMovementSettingMenu.transform, "More Settings", "More Settings", new Vector2(0, -15));

            var avatarCheck = AddCheckbox(_cameraMovementSettingMenu.transform, "Custom VRM Avatar", "Custom or VRM Avatar", new Vector2(0, -40), Options.Instance.customAvatar, (check) =>
            {
                Options.Instance.customAvatar = check;
                movementController.Reload();
            });
            MoveTransform(avatarCheck.Item3.transform, 30, 25, 0, 1, 30, -45);
            MoveTransform(avatarCheck.Item1, 160, 16, 0, 1, 120, -40);

            var reloadButton = AddButton(_cameraMovementSettingMenu.transform, "Reload", "Reload", new Vector2(0, -190), () =>
            {
                movementController.Reload();
            });
            MoveTransform(reloadButton.transform, 40, 20, 1, 1, -80, -40);

            var selectButton = AddButton(_cameraMovementSettingMenu.transform, "Select", "Select", new Vector2(0, -190), () =>
            {
                var ext = new ExtensionFilter[] { new ExtensionFilter("Avatar File", new string[] { "avatar", "vrm" }) };
                var paths = StandaloneFileBrowser.OpenFilePanel("Avatar File", Path.GetDirectoryName(Options.Instance.avatarFileName), ext, false);
                if (paths.Length > 0 && File.Exists(paths[0]))
                {
                    Options.Instance.avatarFileName = paths[0];
                    _avatarFileInputText.InputField.text = paths[0];
                }
            });
            MoveTransform(selectButton.transform, 40, 20, 1, 1, -35, -40);

            var vrmFileInput = AddTextInput(_cameraMovementSettingMenu.transform, "Avatar File", "Avatar File", new Vector2(0, -75), Options.Instance.avatarFileName, (value) =>
            {
                Options.Instance.avatarFileName = value;
            });
            _avatarFileInputText = vrmFileInput.Item3;
            MoveTransform(vrmFileInput.Item1, 60, 16, 0, 1, 25, -60);
            MoveTransform(vrmFileInput.Item3.transform, 425, 20, 0.1f, 1, 220, -60);

            var scaleInput = AddTextInput(_cameraMovementSettingMenu.transform, "Avatar Scale", "Avatar Scale", new Vector2(0, -175), Options.Instance.avatarScale.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarScale = res;
                    movementController.Reload();
                }
            });
            MoveTransform(scaleInput.Item1, 60, 16, 0, 1, 30, -85);
            MoveTransform(scaleInput.Item3.transform, 40, 20, 0.1f, 1, 35, -85);

            var yOffsetInput = AddTextInput(_cameraMovementSettingMenu.transform, "Avatar Y offset", "Avatar Y offset", new Vector2(0, -195), Options.Instance.avatarYoffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarYoffset = res;
                    movementController.Reload();
                }
            });
            MoveTransform(yOffsetInput.Item1, 60, 16, 0, 1, 140, -85);
            MoveTransform(yOffsetInput.Item3.transform, 40, 20, 0.1f, 1, 145, -85);

            var blinkerCheck = AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Blinker", "VRM Blinker", new Vector2(0, -190), Options.Instance.avatarBlinker, (check) =>
            {
                Options.Instance.avatarBlinker = check;
                movementController.Reload();
            });
            MoveTransform(blinkerCheck.Item3.transform, 30, 16, 0, 1, 250, -85);
            MoveTransform(blinkerCheck.Item1, 50, 16, 0, 1, 280, -85);

            var lookAtCheck = AddCheckbox(_cameraMovementSettingMenu.transform, "VRM LookAt", "VRM LookAt", new Vector2(0, -190), Options.Instance.avatarLookAt, (check) =>
            {
                Options.Instance.avatarLookAt = check;
                movementController.Reload();
            });
            MoveTransform(lookAtCheck.Item3.transform, 30, 16, 0, 1, 330, -85);
            MoveTransform(lookAtCheck.Item1, 70, 16, 0, 1, 370, -85);

            var animationCheck = AddCheckbox(_cameraMovementSettingMenu.transform, "VRM Animation", "VRM Animation", new Vector2(0, -190), Options.Instance.avatarAnimation, (check) =>
            {
                Options.Instance.avatarAnimation = check;
                movementController.Reload();
            });
            MoveTransform(animationCheck.Item3.transform, 30, 16, 0, 1, 410, -85);
            MoveTransform(animationCheck.Item1, 70, 16, 0, 1, 450, -85);

            var simpleAvatarCheck = AddCheckbox(_cameraMovementSettingMenu.transform, "Simple Avatar", "Simple Avatar", new Vector2(0, -140), Options.Instance.simpleAvatar, (check) =>
            {
                Options.Instance.simpleAvatar = check;
                movementController.Reload();
            });
            MoveTransform(simpleAvatarCheck.Item3.transform, 30, 16, 0, 1, 30, -110);
            MoveTransform(simpleAvatarCheck.Item1, 70, 16, 0, 1, 70, -110);

            var headHightInput = AddTextInput(_cameraMovementSettingMenu.transform, "Head Hight", "Head Hight lookat point", new Vector2(0, -140), Options.Instance.avatarHeadHight.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarHeadHight = res;
                    movementController.Reload();
                }
            });
            MoveTransform(headHightInput.Item1, 60, 16, 0, 1, 130, -110);
            MoveTransform(headHightInput.Item3.transform, 40, 20, 0.1f, 1, 135, -110);

            var headSizeInput = AddTextInput(_cameraMovementSettingMenu.transform, "Head Size", "Head Size", new Vector2(0, -160), Options.Instance.avatarHeadSize.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarHeadSize = res;
                    movementController.Reload();
                }
            });
            MoveTransform(headSizeInput.Item1, 60, 16, 0, 1, 230, -110);
            MoveTransform(headSizeInput.Item3.transform, 40, 20, 0.1f, 1, 235, -110);

            var armSizeInput = AddTextInput(_cameraMovementSettingMenu.transform, "Arm Size", "Arm Size", new Vector2(0, -180), Options.Instance.avatarArmSize.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.avatarArmSize = res;
                    movementController.Reload();
                }
            });
            MoveTransform(armSizeInput.Item1, 60, 16, 0, 1, 320, -110);
            MoveTransform(armSizeInput.Item3.transform, 40, 20, 0.1f, 1, 325, -110);

            var bookmarkWidthInput = AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Width", "Bookmark Width", new Vector2(0, -205), Options.Instance.bookmarkWidth.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.bookmarkWidth = res;
                    movementController.Reload();
                }
            });
            MoveTransform(bookmarkWidthInput.Item1, 60, 16, 0, 1, 30, -135);
            MoveTransform(bookmarkWidthInput.Item3.transform, 40, 20, 0.1f, 1, 35, -135);

            var bookmarkAreaInput = AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Area", "Bookmark Area", new Vector2(0, -225), Options.Instance.bookmarkInsertOffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.bookmarkInsertOffset = res;
                    movementController.Reload();
                }
            });
            MoveTransform(bookmarkAreaInput.Item1, 60, 16, 0, 1, 140, -135);
            MoveTransform(bookmarkAreaInput.Item3.transform, 40, 20, 0.1f, 1, 145, -135);

            var settingMenuBookmarkExportButton = AddButton(_cameraMovementSettingMenu.transform, "Bookmark Export", "Bookmark Export", new Vector2(0, -385), () =>
            {
                movementController.BookmarkExport();
            });
            MoveTransform(settingMenuBookmarkExportButton.transform, 70, 25, 0, 1, 280, -135);

            /*
            var settingMenuBookmarkImportButton = AddButton(_cameraMovementSettingMenu.transform, "Bookmark Import", "Bookmark Import", new Vector2(0, -385), () =>
            {
                movementController.BookmarkImport();
            });
            MoveTransform(settingMenuBookmarkImportButton.transform, 70, 25, 0, 1, 370, -135);
            */

            var rectXInput = AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect X", "Sub Rect X", new Vector2(0, -245), Options.Instance.subCameraRectX.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectX = res;
                    movementController.Reload();
                }
            });
            MoveTransform(rectXInput.Item1, 60, 16, 0, 1, 30, -160);
            MoveTransform(rectXInput.Item3.transform, 40, 20, 0.1f, 1, 35, -160);

            var rectYInput = AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect Y", "Sub Rect Y", new Vector2(0, -265), Options.Instance.subCameraRectY.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectY = res;
                    movementController.Reload();
                }
            });
            MoveTransform(rectYInput.Item1, 60, 16, 0, 1, 140, -160);
            MoveTransform(rectYInput.Item3.transform, 40, 20, 0.1f, 1, 145, -160);

            var rectWInput = AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect W", "Sub Rect W", new Vector2(0, -285), Options.Instance.subCameraRectW.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectW = res;
                    movementController.Reload();
                }
            });
            MoveTransform(rectWInput.Item1, 60, 16, 0, 1, 250, -160);
            MoveTransform(rectWInput.Item3.transform, 40, 20, 0.1f, 1, 255, -160);

            var rectHInput = AddTextInput(_cameraMovementSettingMenu.transform, "Sub Rect H", "Sub Rect H", new Vector2(0, -305), Options.Instance.subCameraRectH.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.Instance.subCameraRectH = res;
                    movementController.Reload();
                }
            });
            MoveTransform(rectHInput.Item1, 60, 16, 0, 1, 360, -160);
            MoveTransform(rectHInput.Item3.transform, 40, 20, 0.1f, 1, 365, -160);

            var scriptFileInput = AddTextInput(_cameraMovementSettingMenu.transform, "Script File", "Script File", new Vector2(0, -325), Options.Instance.scriptFileName, (value) =>
            {
                Options.Instance.scriptFileName = value.Trim();
            });
            MoveTransform(scriptFileInput.Item1, 60, 16, 0, 1, 30, -185);
            MoveTransform(scriptFileInput.Item3.transform, 80, 20, 0.1f, 1, 55, -185);

            var saveButton = AddButton(_cameraMovementSettingMenu.transform, "Setting Save", "Setting Save", new Vector2(0, -250), () =>
            {
                Options.Instance.SettingSave();
            });
            MoveTransform(saveButton.transform, 70, 25, 0, 1, 350, -190);

            var closeButton = AddButton(_cameraMovementSettingMenu.transform, "Close", "Close", new Vector2(0, -415), () =>
            {
                _cameraMovementSettingMenu.SetActive(false);
            });
            MoveTransform(closeButton.transform, 70, 25, 0, 1, 440, -190);

            _cameraMovementSettingMenu.SetActive(false);

            //Bookmark
            AttachTransform(_cameraMovementBookmarkMenu, 700, 55, 0.7f, 0.09f, 150, 40, 1, 1);

            Image imageBookmark = _cameraMovementBookmarkMenu.AddComponent<Image>();
            imageBookmark.sprite = PersistentUI.Instance.Sprites.Background;
            imageBookmark.type = Image.Type.Sliced;
            imageBookmark.color = new Color(0.24f, 0.24f, 0.24f);

            var currentBookmarkLabel = AddLabel(_cameraMovementBookmarkMenu.transform, "", "", new Vector2(0, -15));
            MoveTransform(currentBookmarkLabel.Item1, 350, 20, 0.1f, 1, 250, -15);
            currentBookmarkLabel.Item2.fontSize = 12;
            currentBookmarkLabel.Item2.alignment = TextAlignmentOptions.Left;
            currentBookmarkLabelText = currentBookmarkLabel.Item2;

            var bookmarkSetCheckbox = AddCheckbox(_cameraMovementBookmarkMenu.transform, "Set", "Set", new Vector2(0, -40), false, (check) =>
            {
                commandSet = check;
            });
            MoveTransform(bookmarkSetCheckbox.Item1, 50, 16, 0.1f, 1, -20, -37);
            MoveTransform(bookmarkSetCheckbox.Item3.transform, 30, 16, 0.1f, 1, -10, -37);
            bookmarkSetCheckboxToggle = bookmarkSetCheckbox.Item3;

            var bookmarkMenuInput = AddTextInput(_cameraMovementBookmarkMenu.transform, "Current No.-", "Current No.-", new Vector2(0, -22), "", (value) =>
            {
            });
            MoveTransform(bookmarkMenuInput.Item1, 150, 16, 0.1f, 1, -20, -15);
            bookmarkMenuInputLabel = bookmarkMenuInput.Item2;
            MoveTransform(bookmarkMenuInput.Item3.transform, 350, 20, 0.1f, 1, 250f, -35);
            bookmarkMenuInputText = bookmarkMenuInput.Item3;
            bookmarkMenuInputText.InputField.textComponent.fontSize = 14;

            var bookmarkMenuCopyButton = AddButton(_cameraMovementBookmarkMenu.transform, "Copy to Edit", "Copy to Edit", new Vector2(460, -22), () =>
            {
                bookmarkMenuInputText.InputField.text = currentBookmarkLabelText.text;
            });
            MoveTransform(bookmarkMenuCopyButton.transform, 60, 20, 0.1f, 1, 40, -35);

            var bookmarkMenuNewButton = AddButton(_cameraMovementBookmarkMenu.transform, "New", "New", new Vector2(460, -22), () =>
            {
                if (bookmarkMenuInputText.InputField.text.Trim() != "")
                    movementController.BookmarkNew(bookmarkMenuInputText.InputField.text);
            });
            MoveTransform(bookmarkMenuNewButton.transform, 50, 20, 0.1f, 1, 460, -35);

            var bookmarkMenuChangeButton = AddButton(_cameraMovementBookmarkMenu.transform, "Change", "Change", new Vector2(460, -22), () =>
            {
                if (currentBookmarkNo > 0)
                    movementController.BookmarkChange(currentBookmarkNo);
            });
            MoveTransform(bookmarkMenuChangeButton.transform, 50, 20, 0.1f, 1, 520, -35);

            var bookmarkMenuDeleteButton = AddButton(_cameraMovementBookmarkMenu.transform, "Delete", "Delete", new Vector2(460, -22), () =>
            {
                if (currentBookmarkNo > 0)
                    movementController.BookmarkDelete(currentBookmarkNo);
            });
            MoveTransform(bookmarkMenuDeleteButton.transform, 50, 20, 0.1f, 1, 580, -35);

            quickCommand1Button = AddButton(_cameraMovementBookmarkMenu.transform, "Command1", Options.Instance.quickCommand1, new Vector2(460, -22), () =>
            {
                if (commandSet)
                {
                    commandSet = false;
                    bookmarkSetCheckboxToggle.isOn = false;
                    if (bookmarkMenuInputText.InputField.text.Trim() != "")
                    {
                        Options.Instance.quickCommand1 = bookmarkMenuInputText.InputField.text.Trim();
                        quickCommand1Button.SetText(Options.Instance.quickCommand1);
                    }
                }
                else
                {
                    bookmarkMenuInputText.InputField.text += Options.Instance.quickCommand1;
                }
            });
            MoveTransform(quickCommand1Button.transform, 50, 20, 0.1f, 1, 280, -15);

            quickCommand2Button = AddButton(_cameraMovementBookmarkMenu.transform, "Command2", Options.Instance.quickCommand2, new Vector2(460, -22), () =>
            {
                if (commandSet)
                {
                    commandSet = false;
                    bookmarkSetCheckboxToggle.isOn = false;
                    if (bookmarkMenuInputText.InputField.text.Trim() != "")
                    {
                        Options.Instance.quickCommand2 = bookmarkMenuInputText.InputField.text.Trim();
                        quickCommand2Button.SetText(Options.Instance.quickCommand2);
                    }
                }
                else
                {
                    bookmarkMenuInputText.InputField.text += Options.Instance.quickCommand2;
                }
            });
            MoveTransform(quickCommand2Button.transform, 50, 20, 0.1f, 1, 340, -15);

            quickCommand3Button = AddButton(_cameraMovementBookmarkMenu.transform, "Command3", Options.Instance.quickCommand3, new Vector2(460, -22), () =>
            {
                if (commandSet)
                {
                    commandSet = false;
                    bookmarkSetCheckboxToggle.isOn = false;
                    if (bookmarkMenuInputText.InputField.text.Trim() != "")
                    {
                        Options.Instance.quickCommand3 = bookmarkMenuInputText.InputField.text.Trim();
                        quickCommand3Button.SetText(Options.Instance.quickCommand3);
                    }
                }
                else
                {
                    bookmarkMenuInputText.InputField.text += Options.Instance.quickCommand3;
                }
            });
            MoveTransform(quickCommand3Button.transform, 50, 20, 0.1f, 1, 400, -15);

            quickCommand4Button = AddButton(_cameraMovementBookmarkMenu.transform, "Command4", Options.Instance.quickCommand4, new Vector2(460, -22), () =>
            {
                if (commandSet)
                {
                    commandSet = false;
                    bookmarkSetCheckboxToggle.isOn = false;
                    if (bookmarkMenuInputText.InputField.text.Trim() != "")
                    {
                        Options.Instance.quickCommand4 = bookmarkMenuInputText.InputField.text.Trim();
                        quickCommand4Button.SetText(Options.Instance.quickCommand4);
                    }
                }
                else
                {
                    bookmarkMenuInputText.InputField.text += Options.Instance.quickCommand4;
                }
            });
            MoveTransform(quickCommand4Button.transform, 50, 20, 0.1f, 1, 460, -15);

            quickCommand5Button = AddButton(_cameraMovementBookmarkMenu.transform, "Command5", Options.Instance.quickCommand5, new Vector2(460, -22), () =>
            {
                if (commandSet)
                {
                    commandSet = false;
                    bookmarkSetCheckboxToggle.isOn = false;
                    if (bookmarkMenuInputText.InputField.text.Trim() != "")
                    {
                        Options.Instance.quickCommand5 = bookmarkMenuInputText.InputField.text.Trim();
                        quickCommand5Button.SetText(Options.Instance.quickCommand5);
                    }
                }
                else
                {
                    bookmarkMenuInputText.InputField.text += Options.Instance.quickCommand5;
                }
            });
            MoveTransform(quickCommand5Button.transform, 50, 20, 0.1f, 1, 520, -15);

            quickCommand6Button = AddButton(_cameraMovementBookmarkMenu.transform, "Command6", Options.Instance.quickCommand6, new Vector2(460, -22), () =>
            {
                if (commandSet)
                {
                    commandSet = false;
                    bookmarkSetCheckboxToggle.isOn = false;
                    if (bookmarkMenuInputText.InputField.text.Trim() != "")
                    {
                        Options.Instance.quickCommand6 = bookmarkMenuInputText.InputField.text.Trim();
                        quickCommand6Button.SetText(Options.Instance.quickCommand6);
                    }
                }
                else
                {
                    bookmarkMenuInputText.InputField.text += Options.Instance.quickCommand6;
                }
            });
            MoveTransform(quickCommand6Button.transform, 50, 20, 0.1f, 1, 580, -15);

            _cameraMovementBookmarkMenu.SetActive(Options.Instance.bookmarkEdit);

            // Camera Control
            _cameraMovementCameraControlMenuRect = AttachTransform(_cameraMovementCameraControlMenu, 500, 55, 0.7f, 0.2f, 310, 40, 1, 1);

            Image imageCameraControl = _cameraMovementCameraControlMenu.AddComponent<Image>();
            imageCameraControl.sprite = PersistentUI.Instance.Sprites.Background;
            imageCameraControl.type = Image.Type.Sliced;
            imageCameraControl.color = new Color(0.24f, 0.24f, 0.24f);

            posXChange = (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    CameraPosRotSet(CameraItem.PosX, res);
                }
            };
            var cameraControlMenuPosXinput = AddTextInput(_cameraMovementCameraControlMenu.transform, "Pos X", "Pos X", new Vector2(0, -15), "", posXChange);
            MoveTransform(cameraControlMenuPosXinput.Item1, 30, 16, 0f, 1, 15, -15);
            MoveTransform(cameraControlMenuPosXinput.Item3.transform, 40, 20, 0.1f, 1, 5, -15);
            _cameraPosXinput = cameraControlMenuPosXinput.Item3;
            _cameraPosXinput.InputField.textComponent.fontSize = 14;

            posYChange = (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    CameraPosRotSet(CameraItem.PosY, res);
                }
            };
            var cameraControlMenuPosYinput = AddTextInput(_cameraMovementCameraControlMenu.transform, "Pos Y", "Pos Y", new Vector2(0, -15), "", posYChange);
            MoveTransform(cameraControlMenuPosYinput.Item1, 30, 16, 0f, 1, 85, -15);
            MoveTransform(cameraControlMenuPosYinput.Item3.transform, 40, 20, 0.1f, 1, 75, -15);
            _cameraPosYinput = cameraControlMenuPosYinput.Item3;
            _cameraPosYinput.InputField.textComponent.fontSize = 14;

            posZChange = (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    CameraPosRotSet(CameraItem.PosZ, res);
                }
            };
            var cameraControlMenuPosZinput = AddTextInput(_cameraMovementCameraControlMenu.transform, "Pos Z", "Pos Z", new Vector2(0, -15), "", posZChange);
            MoveTransform(cameraControlMenuPosZinput.Item1, 30, 16, 0f, 1, 155, -15);
            MoveTransform(cameraControlMenuPosZinput.Item3.transform, 40, 20, 0.1f, 1, 145, -15);
            _cameraPosZinput = cameraControlMenuPosZinput.Item3;
            _cameraPosZinput.InputField.textComponent.fontSize = 14;

            rotXChange = (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    CameraPosRotSet(CameraItem.RotX, res);
                }
            };
            var cameraControlMenuRotXinput = AddTextInput(_cameraMovementCameraControlMenu.transform, "Rot X", "Rot X", new Vector2(0, -15), "", rotXChange);
            MoveTransform(cameraControlMenuRotXinput.Item1, 30, 16, 0f, 1, 230, -15);
            MoveTransform(cameraControlMenuRotXinput.Item3.transform, 40, 20, 0.1f, 1, 220, -15);
            _cameraRotXinput = cameraControlMenuRotXinput.Item3;
            _cameraRotXinput.InputField.textComponent.fontSize = 14;

            rotYChange = (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    CameraPosRotSet(CameraItem.RotY, res);
                }
            };
            var cameraControlMenuRotYinput = AddTextInput(_cameraMovementCameraControlMenu.transform, "Rot Y", "Rot Y", new Vector2(0, -15), "", rotYChange);
            MoveTransform(cameraControlMenuRotYinput.Item1, 30, 16, 0f, 1, 300, -15);
            MoveTransform(cameraControlMenuRotYinput.Item3.transform, 40, 20, 0.1f, 1, 290, -15);
            _cameraRotYinput = cameraControlMenuRotYinput.Item3;
            _cameraRotYinput.InputField.textComponent.fontSize = 14;

            rotZChange = (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    CameraPosRotSet(CameraItem.RotZ, res);
                }
            };
            var cameraControlMenuRotZinput = AddTextInput(_cameraMovementCameraControlMenu.transform, "Rot Z", "Rot Z", new Vector2(0, -15), "", rotZChange);
            MoveTransform(cameraControlMenuRotZinput.Item1, 30, 16, 0f, 1, 370, -15);
            MoveTransform(cameraControlMenuRotZinput.Item3.transform, 40, 20, 0.1f, 1, 360, -15);
            _cameraRotZinput = cameraControlMenuRotZinput.Item3;
            _cameraRotZinput.InputField.textComponent.fontSize = 14;

            fovChange = (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Settings.Instance.CameraFOV = res;
                    cameraPosRotNoUpdate = true;
                }
            };
            var cameraControlMenuFOVinput = AddTextInput(_cameraMovementCameraControlMenu.transform, "FOV", "FOV", new Vector2(0, -15), Settings.Instance.CameraFOV.ToString(), fovChange);
            MoveTransform(cameraControlMenuFOVinput.Item1, 25, 16, 0f, 1, 440, -15);
            MoveTransform(cameraControlMenuFOVinput.Item3.transform, 35, 20, 0.1f, 1, 425, -15);
            _cameraFOVinput = cameraControlMenuFOVinput.Item3;
            _cameraFOVinput.InputField.textComponent.fontSize = 14;

            distanceChange = (string value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    var new_position = movementController.AvatarPositionGet() - movementController.CameraTransformGet().forward * res;
                    var rotation = movementController.CameraTransformGet().eulerAngles;
                    movementController.CameraPositionAndRotationSet(new_position, rotation);
                }
            };
            var cameraControlMenuDistanceinput = AddTextInput(_cameraMovementCameraControlMenu.transform, "Dist", "Dist", new Vector2(0, -40), "", distanceChange);
            MoveTransform(cameraControlMenuDistanceinput.Item1, 30, 16, 0f, 1, 15, -40);
            MoveTransform(cameraControlMenuDistanceinput.Item3.transform, 40, 20, 0.1f, 1, 5, -40);
            _cameraDistanceinput = cameraControlMenuDistanceinput.Item3;
            _cameraDistanceinput.InputField.textComponent.fontSize = 14;

            var cameraControlMenuMoveSpeed = AddTextInput(_cameraMovementCameraControlMenu.transform, "Move Speed", "Move Speed", new Vector2(0, -15), Settings.Instance.Camera_MovementSpeed.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Settings.Instance.Camera_MovementSpeed = res;
                }
            });
            MoveTransform(cameraControlMenuMoveSpeed.Item1, 30, 16, 0f, 1, 85, -40);
            MoveTransform(cameraControlMenuMoveSpeed.Item3.transform, 40, 20, 0.1f, 1, 75, -40);
            cameraControlMenuMoveSpeed.Item3.InputField.textComponent.fontSize = 14;

            var cameraControlMenuLookAtButton = AddButton(_cameraMovementCameraControlMenu.transform, "Look At", "Look At", new Vector2(0, -40), () =>
            {
                var position = movementController.CameraPositionGet();
                var direction = movementController.AvatarPositionGet() - position;
                var lookRotation = Quaternion.LookRotation(direction);
                var new_rotation = lookRotation.eulerAngles;
                movementController.CameraPositionAndRotationSet(position, new_rotation);
            });
            MoveTransform(cameraControlMenuLookAtButton.transform, 50, 20, 0, 1, 175, -40);

            var cameraControlMenuPasteButton = AddButton(_cameraMovementCameraControlMenu.transform, "Paste", "Paste", new Vector2(0, -40), () =>
            {
                var position = movementController.CameraPositionGet();
                var rotation = movementController.CameraTransformGet().eulerAngles;
                var text = Regex.Split(GUIUtility.systemCopyBuffer, "\t");
                float res, px, py, pz, rx, ry, rz, fov;
                int i = 0;
                if (text.Length > 8 || !(text.Length > 4 && float.TryParse(text[4], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res)))
                    i++;
                if (text.Length > i && float.TryParse(text[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    px = res;
                }
                else
                {
                    px = position.x;
                }
                i++;
                if (text.Length > i && float.TryParse(text[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    py = res;
                }
                else
                {
                    py = position.y;
                }
                i++;
                if (text.Length > i && float.TryParse(text[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    pz = res;
                }
                else
                {
                    pz = position.z;
                }
                i++;
                var new_position = new Vector3(px, py, pz) + new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset, Options.Instance.originZoffset);
                Vector3 new_rotation;
                if (text.Length > i && Regex.IsMatch(text[i], "true", RegexOptions.IgnoreCase))
                {
                    var direction = movementController.AvatarPositionGet() - new_position;
                    var lookRotation = Quaternion.LookRotation(direction);
                    new_rotation = lookRotation.eulerAngles;
                    i += 3;
                }
                else
                {
                    i++;
                    if (text.Length > i && float.TryParse(text[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                    {
                        rx = res;
                    }
                    else
                    {
                        rx = rotation.x;
                    }
                    i++;
                    if (text.Length > i && float.TryParse(text[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                    {
                        ry = res;
                    }
                    else
                    {
                        ry = rotation.y;
                    }
                    i++;
                    if (text.Length > i && float.TryParse(text[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                    {
                        rz = res;
                    }
                    else
                    {
                        rz = rotation.z;
                    }
                    new_rotation = new Vector3(rx, ry, rz);
                }
                i++;
                if (text.Length > i && float.TryParse(text[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    fov = res;
                }
                else
                {
                    fov = Settings.Instance.CameraFOV;
                }
                movementController.CameraPositionAndRotationSet(new_position, new_rotation);
                Settings.Instance.CameraFOV = fov;
            });
            MoveTransform(cameraControlMenuPasteButton.transform, 50, 20, 0, 1, 410, -40);

            var cameraControlMenuCopyButton = AddButton(_cameraMovementCameraControlMenu.transform, "Copy", "Copy", new Vector2(0, -40), () =>
            {
                var positon = movementController.CameraPositionGet();
                var rotation = movementController.CameraTransformGet().eulerAngles;
                var text = $"{positon.x.ToString("0.###")}\t{positon.y.ToString("0.###")}\t{positon.z.ToString("0.###")}\tFALSE\t{rotation.x.ToString("0.##")}\t{rotation.y.ToString("0.##")}\t{rotation.z.ToString("0.##")}\t{Settings.Instance.CameraFOV.ToString("0.##")}";
                GUIUtility.systemCopyBuffer = text;
            });
            MoveTransform(cameraControlMenuCopyButton.transform, 50, 20, 0, 1, 470, -40);

            _cameraMovementCameraControlMenu.SetActive(Options.Instance.cameraControl);

        }

        // i ended up copying Top_Cat's CM-JS UI helper, too useful to make my own tho
        // after askin TC if it's one of the only way, he let me use this
        private UIButton AddButton(Transform parent, string title, string text, Vector2 pos, UnityAction onClick)
        {
            var button = Object.Instantiate(PersistentUI.Instance.ButtonPrefab, parent);
            MoveTransform(button.transform, 100, 25, 0.5f, 1, pos.x, pos.y);

            button.name = title;
            button.Button.onClick.AddListener(onClick);

            button.SetText(text);
            button.Text.enableAutoSizing = false;
            button.Text.fontSize = 12;
            return button;
        }

        private (RectTransform, TextMeshProUGUI) AddLabel(Transform parent, string title, string text, Vector2 pos, Vector2? size = null)
        {
            var entryLabel = new GameObject(title + " Label", typeof(TextMeshProUGUI));
            var rectTransform = ((RectTransform)entryLabel.transform);
            rectTransform.SetParent(parent);

            MoveTransform(rectTransform, 110, 24, 0.5f, 1, pos.x, pos.y);
            var textComponent = entryLabel.GetComponent<TextMeshProUGUI>();

            textComponent.name = title;
            textComponent.font = PersistentUI.Instance.ButtonPrefab.Text.font;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 16;
            textComponent.text = text;
            return (rectTransform, textComponent);
        }

        private (RectTransform, TextMeshProUGUI, UITextInput) AddTextInput(Transform parent, string title, string text, Vector2 pos, string value, UnityAction<string> onChange)
        {
            var entryLabel = new GameObject(title + " Label", typeof(TextMeshProUGUI));
            var rectTransform = ((RectTransform)entryLabel.transform);
            rectTransform.SetParent(parent);

            MoveTransform(rectTransform, 50, 16, 0.5f, 1, pos.x - 47.5f, pos.y);
            var textComponent = entryLabel.GetComponent<TextMeshProUGUI>();

            textComponent.name = title;
            textComponent.font = PersistentUI.Instance.ButtonPrefab.Text.font;
            textComponent.alignment = TextAlignmentOptions.Right;
            textComponent.fontSize = 12;
            textComponent.text = text;

            var textInput = Object.Instantiate(PersistentUI.Instance.TextInputPrefab, parent);
            MoveTransform(textInput.transform, 75, 20, 0.5f, 1, pos.x + 27.5f, pos.y);
            textInput.GetComponent<Image>().pixelsPerUnitMultiplier = 3;
            textInput.InputField.text = value;
            textInput.InputField.onFocusSelectAll = false;
            textInput.InputField.textComponent.alignment = TextAlignmentOptions.Left;
            textInput.InputField.textComponent.fontSize = 10;

            textInput.InputField.onValueChanged.AddListener(onChange);
            return (rectTransform, textComponent, textInput);
        }

        private (RectTransform, TextMeshProUGUI, Toggle) AddCheckbox(Transform parent, string title, string text, Vector2 pos, bool value, UnityAction<bool> onClick)
        {
            var entryLabel = new GameObject(title + " Label", typeof(TextMeshProUGUI));
            var rectTransform = ((RectTransform)entryLabel.transform);
            rectTransform.SetParent(parent);
            MoveTransform(rectTransform, 80, 16, 0.5f, 1, pos.x + 10, pos.y + 5);
            var textComponent = entryLabel.GetComponent<TextMeshProUGUI>();

            textComponent.name = title;
            textComponent.font = PersistentUI.Instance.ButtonPrefab.Text.font;
            textComponent.alignment = TextAlignmentOptions.Left;
            textComponent.fontSize = 12;
            textComponent.text = text;

            var original = GameObject.Find("Strobe Generator").GetComponentInChildren<Toggle>(true);
            var toggleObject = Object.Instantiate(original, parent.transform);
            MoveTransform(toggleObject.transform, 100, 25, 0.5f, 1, pos.x, pos.y);

            var toggleComponent = toggleObject.GetComponent<Toggle>();
            var colorBlock = toggleComponent.colors;
            colorBlock.normalColor = Color.white;
            toggleComponent.colors = colorBlock;
            toggleComponent.isOn = value;

            toggleComponent.onValueChanged.AddListener(onClick);
            return (rectTransform, textComponent, toggleComponent);
        }

        private RectTransform AttachTransform(GameObject obj, float sizeX, float sizeY, float anchorX, float anchorY, float anchorPosX, float anchorPosY, float pivotX = 0.5f, float pivotY = 0.5f)
        {
            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(anchorX, anchorY);
            rectTransform.anchoredPosition = new Vector3(anchorPosX, anchorPosY, 0);

            return rectTransform;
        }

        private void MoveTransform(Transform transform, float sizeX, float sizeY, float anchorX, float anchorY, float anchorPosX, float anchorPosY, float pivotX = 0.5f, float pivotY = 0.5f)
        {
            if (!(transform is RectTransform rectTransform)) return;

            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(anchorX, anchorY);
            rectTransform.anchoredPosition = new Vector3(anchorPosX, anchorPosY, 0);
        }

    }
}
