using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class CameraControlMenuUI
    {
        public CameraMovementController movementController;
        public GameObject _cameraMovementCameraControlMenu;
        public UITextInput _cameraPosXinput;
        public UITextInput _cameraPosYinput;
        public UITextInput _cameraPosZinput;
        public UITextInput _cameraRotXinput;
        public UITextInput _cameraRotYinput;
        public UITextInput _cameraRotZinput;
        public UITextInput _cameraFOVinput;
        public UITextInput _cameraDistanceinput;
        public RectTransform _cameraMovementCameraControlMenuRect;
        public bool cameraPosRotNoUpdate = false;

        public UnityAction<string> posXChange;
        public UnityAction<string> posYChange;
        public UnityAction<string> posZChange;
        public UnityAction<string> rotXChange;
        public UnityAction<string> rotYChange;
        public UnityAction<string> rotZChange;
        public UnityAction<string> fovChange;
        public UnityAction<string> distanceChange;

        public enum CameraItem
        {
            PosX,
            PosY,
            PosZ,
            RotX,
            RotY,
            RotZ
        }
        public void CameraPosRotTextSet(Vector3 position, Vector3 rotation)
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
        public void CameraPosRotSet(CameraItem item, float value)
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

        public void AddMenu(MapEditorUI mapEditorUI)
        {
            movementController = Plugin.movement;
            var parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementCameraControlMenu = new GameObject("Camera Control");
            _cameraMovementCameraControlMenu.transform.parent = parent.transform;

            // Camera Control
            _cameraMovementCameraControlMenuRect = UIUtil.AttachTransform(_cameraMovementCameraControlMenu, 500, 55, 0.7f, 0.2f, 310, 40, 1, 1);

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
            var cameraControlMenuPosXinput = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "Pos X", "Pos X", new Vector2(0, -15), "", posXChange);
            UIUtil.MoveTransform(cameraControlMenuPosXinput.Item1, 30, 16, 0f, 1, 15, -15);
            UIUtil.MoveTransform(cameraControlMenuPosXinput.Item3.transform, 40, 20, 0.1f, 1, 5, -15);
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
            var cameraControlMenuPosYinput = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "Pos Y", "Pos Y", new Vector2(0, -15), "", posYChange);
            UIUtil.MoveTransform(cameraControlMenuPosYinput.Item1, 30, 16, 0f, 1, 85, -15);
            UIUtil.MoveTransform(cameraControlMenuPosYinput.Item3.transform, 40, 20, 0.1f, 1, 75, -15);
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
            var cameraControlMenuPosZinput = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "Pos Z", "Pos Z", new Vector2(0, -15), "", posZChange);
            UIUtil.MoveTransform(cameraControlMenuPosZinput.Item1, 30, 16, 0f, 1, 155, -15);
            UIUtil.MoveTransform(cameraControlMenuPosZinput.Item3.transform, 40, 20, 0.1f, 1, 145, -15);
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
            var cameraControlMenuRotXinput = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "Rot X", "Rot X", new Vector2(0, -15), "", rotXChange);
            UIUtil.MoveTransform(cameraControlMenuRotXinput.Item1, 30, 16, 0f, 1, 230, -15);
            UIUtil.MoveTransform(cameraControlMenuRotXinput.Item3.transform, 40, 20, 0.1f, 1, 220, -15);
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
            var cameraControlMenuRotYinput = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "Rot Y", "Rot Y", new Vector2(0, -15), "", rotYChange);
            UIUtil.MoveTransform(cameraControlMenuRotYinput.Item1, 30, 16, 0f, 1, 300, -15);
            UIUtil.MoveTransform(cameraControlMenuRotYinput.Item3.transform, 40, 20, 0.1f, 1, 290, -15);
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
            var cameraControlMenuRotZinput = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "Rot Z", "Rot Z", new Vector2(0, -15), "", rotZChange);
            UIUtil.MoveTransform(cameraControlMenuRotZinput.Item1, 30, 16, 0f, 1, 370, -15);
            UIUtil.MoveTransform(cameraControlMenuRotZinput.Item3.transform, 40, 20, 0.1f, 1, 360, -15);
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
            var cameraControlMenuFOVinput = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "FOV", "FOV", new Vector2(0, -15), Settings.Instance.CameraFOV.ToString(), fovChange);
            UIUtil.MoveTransform(cameraControlMenuFOVinput.Item1, 25, 16, 0f, 1, 440, -15);
            UIUtil.MoveTransform(cameraControlMenuFOVinput.Item3.transform, 35, 20, 0.1f, 1, 425, -15);
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
            var cameraControlMenuDistanceinput = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "Dist", "Dist", new Vector2(0, -40), "", distanceChange);
            UIUtil.MoveTransform(cameraControlMenuDistanceinput.Item1, 30, 16, 0f, 1, 15, -40);
            UIUtil.MoveTransform(cameraControlMenuDistanceinput.Item3.transform, 40, 20, 0.1f, 1, 5, -40);
            _cameraDistanceinput = cameraControlMenuDistanceinput.Item3;
            _cameraDistanceinput.InputField.textComponent.fontSize = 14;

            var cameraControlMenuMoveSpeed = UIUtil.AddTextInput(_cameraMovementCameraControlMenu.transform, "Move Speed", "Move Speed", new Vector2(0, -15), Settings.Instance.Camera_MovementSpeed.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Settings.Instance.Camera_MovementSpeed = res;
                }
            });
            UIUtil.MoveTransform(cameraControlMenuMoveSpeed.Item1, 30, 16, 0f, 1, 85, -40);
            UIUtil.MoveTransform(cameraControlMenuMoveSpeed.Item3.transform, 40, 20, 0.1f, 1, 75, -40);
            cameraControlMenuMoveSpeed.Item3.InputField.textComponent.fontSize = 14;

            var cameraControlMenuLookAtButton = UIUtil.AddButton(_cameraMovementCameraControlMenu.transform, "Look At", "Look At", new Vector2(0, -40), () =>
            {
                var position = movementController.CameraPositionGet();
                var direction = movementController.AvatarPositionGet() - position;
                var lookRotation = Quaternion.LookRotation(direction);
                var new_rotation = lookRotation.eulerAngles;
                movementController.CameraPositionAndRotationSet(position, new_rotation);
            });
            UIUtil.MoveTransform(cameraControlMenuLookAtButton.transform, 50, 20, 0, 1, 175, -40);

            var cameraControlMenuPasteButton = UIUtil.AddButton(_cameraMovementCameraControlMenu.transform, "Paste", "Paste", new Vector2(0, -40), () =>
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
            UIUtil.MoveTransform(cameraControlMenuPasteButton.transform, 50, 20, 0, 1, 410, -40);

            var cameraControlMenuCopyButton = UIUtil.AddButton(_cameraMovementCameraControlMenu.transform, "Copy", "Copy", new Vector2(0, -40), () =>
            {
                var positon = movementController.CameraPositionGet();
                var rotation = movementController.CameraTransformGet().eulerAngles;
                var text = $"{positon.x.ToString("0.###")}\t{positon.y.ToString("0.###")}\t{positon.z.ToString("0.###")}\tFALSE\t{rotation.x.ToString("0.##")}\t{rotation.y.ToString("0.##")}\t{rotation.z.ToString("0.##")}\t{Settings.Instance.CameraFOV.ToString("0.##")}";
                GUIUtility.systemCopyBuffer = text;
            });
            UIUtil.MoveTransform(cameraControlMenuCopyButton.transform, 50, 20, 0, 1, 470, -40);

            _cameraMovementCameraControlMenu.SetActive(Options.Instance.cameraControl);
        }
    }
}
