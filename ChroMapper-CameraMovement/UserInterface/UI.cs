using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Options = ChroMapper_CameraMovement.Configuration.Options;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class UI
    {
        private GameObject _cameraMovementMainMenu;
        private GameObject _cameraMovementSettingMenu;
        private readonly Plugin _plugin;
        private readonly ExtensionButton _extensionBtn = new ExtensionButton();
        private UITextInput _cameraPosRot;
        public static GameObject cm_MapEditorCamera;

        public void CameraPosRotUpdate(Transform transform)
        {
            _cameraPosRot.InputField.text = $"px:{transform.position.x} py:{transform.position.y - Options.CameraYoffset} pz:{transform.position.z - Options.CameraZoffset} rx:{transform.eulerAngles.x} ry:{transform.eulerAngles.y} rz:{transform.eulerAngles.z}";
        }

        public UI(Plugin plugin)
        {
            this._plugin = plugin;

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChroMapper_CameraMovement.Icon.png");
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);

            Texture2D texture2D = new Texture2D(256, 256);
            texture2D.LoadImage(data);

            _extensionBtn.Icon = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0), 100.0f);
            _extensionBtn.Tooltip = "CameraMovement";
            ExtensionButtons.AddButton(_extensionBtn);
        }
        public void AddMenu(MapEditorUI mapEditorUI)
        {
            CanvasGroup parent = mapEditorUI.MainUIGroup[5];
            _cameraMovementMainMenu = new GameObject("CameraMovement Menu");
            _cameraMovementSettingMenu = new GameObject("CameraMovement Setting Menu");
            _cameraMovementMainMenu.transform.parent = parent.transform;
            _cameraMovementSettingMenu.transform.parent = parent.transform;

            //Main Menu
            AttachTransform(_cameraMovementMainMenu, 170, 245, 1, 1, -50, -30, 1, 1);

            Image imageMain = _cameraMovementMainMenu.AddComponent<Image>();
            imageMain.sprite = PersistentUI.Instance.Sprites.Background;
            imageMain.type = Image.Type.Sliced;
            imageMain.color = new Color(0.24f, 0.24f, 0.24f);

            AddLabel(_cameraMovementMainMenu.transform, "CameraMovement", "CameraMovement", new Vector2(0, -15));
            AddCheckbox(_cameraMovementMainMenu.transform, "Movement Enable", "Movement Enable", new Vector2(0, -40), Options.Movement, (check) =>
            {
                Options.Movement = check;
                _plugin.Reload();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "UI Hidden", "UI Hidden", new Vector2(0, -55), Options.UIhidden, (check) =>
            {
                Options.UIhidden = check;
                _plugin.Reload();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "Turn To Head", "Turn To Head", new Vector2(0, -70), Options.TurnToHead, (check) =>
            {
                Options.TurnToHead = check;
                _plugin.Reload();
            });
            AddCheckbox(_cameraMovementMainMenu.transform, "Avatar", "Avatar", new Vector2(0, -85), Options.Avatar, (check) =>
            {
                Options.Avatar = check;
                _plugin.Reload();
            });
            _cameraPosRot = AddTextInput(_cameraMovementMainMenu.transform, "Cam Pos Rot", "Cam Pos Rot", new Vector2(0, -105), "", (value) =>
            {
            });
            cm_MapEditorCamera = GameObject.Find("MapEditor Camera");
            CameraPosRotUpdate(cm_MapEditorCamera.transform);
            AddButton(_cameraMovementMainMenu.transform, "More Settings", "More Settings", new Vector2(0, -135), () =>
            {
                _cameraMovementSettingMenu.SetActive(true);
            });
            AddButton(_cameraMovementMainMenu.transform, "Reload", "Reload", new Vector2(0, -165), () =>
            {
                _plugin.Reload();
            });
            AddButton(_cameraMovementMainMenu.transform, "Setting Save", "Setting Save", new Vector2(0, -195), () =>
            {
                _plugin.SettingSave();
            });
            AddButton(_cameraMovementMainMenu.transform, "Script Mapper Run", "Script Mapper Run", new Vector2(0, -225), () =>
            {
                _plugin.ScriptMapperRun();
            });

            _cameraMovementMainMenu.SetActive(false);
            _extensionBtn.Click = () =>
            {
                _cameraMovementMainMenu.SetActive(!_cameraMovementMainMenu.activeSelf);
            };

            //Setting Menu
            AttachTransform(_cameraMovementSettingMenu, 200, 240, 1, 1, -50, -120, 1, 1);

            Image imageSetting = _cameraMovementSettingMenu.AddComponent<Image>();
            imageSetting.sprite = PersistentUI.Instance.Sprites.Background;
            imageSetting.type = Image.Type.Sliced;
            imageSetting.color = new Color(0.24f, 0.24f, 0.24f);

            AddLabel(_cameraMovementSettingMenu.transform, "More Settings", "More Settings", new Vector2(0, -15));
            AddTextInput(_cameraMovementSettingMenu.transform, "Head Hight", "Head Hight", new Vector2(0, -40), Options.AvatarHeadHight.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.AvatarHeadHight = res;
                    _plugin.Reload();
                }
            });
            AddTextInput(_cameraMovementSettingMenu.transform, "Head Size", "Head Size", new Vector2(0, -60), Options.AvatarHeadSize.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.AvatarHeadSize = res;
                    _plugin.Reload();
                }
            });
            AddTextInput(_cameraMovementSettingMenu.transform, "Arm Size", "Arm Size", new Vector2(0, -80), Options.AvatarArmSize.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.AvatarArmSize = res;
                    _plugin.Reload();
                }
            });
            AddTextInput(_cameraMovementSettingMenu.transform, "Bookmark Width", "Bookmark Width", new Vector2(0, -100), Options.BookMarkWidth.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.BookMarkWidth = res;
                    _plugin.Reload();
                }
            });
            AddTextInput(_cameraMovementSettingMenu.transform, "Offset Y", "Offset Y", new Vector2(0, -120), Options.CameraYoffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.CameraYoffset = res;
                    _plugin.Reload();
                }
            });
            AddTextInput(_cameraMovementSettingMenu.transform, "Offset Z", "Offset Z", new Vector2(0, -140), Options.CameraZoffset.ToString(), (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    Options.CameraZoffset = res;
                    _plugin.Reload();
                }
            });
            AddTextInput(_cameraMovementSettingMenu.transform, "Script File", "Script File", new Vector2(0, -160), Options.ScriptFileName, (value) =>
            {
                Options.ScriptFileName = value.Trim();
            });
            AddButton(_cameraMovementSettingMenu.transform, "Setting Save", "Setting Save", new Vector2(0, -190), () =>
            {
                _plugin.SettingSave();
            });
            AddButton(_cameraMovementSettingMenu.transform, "Close", "Close", new Vector2(0, -220), () =>
            {
                _cameraMovementSettingMenu.SetActive(false);
            });

            _cameraMovementSettingMenu.SetActive(false);

        }

        // i ended up copying Top_Cat's CM-JS UI helper, too useful to make my own tho
        // after askin TC if it's one of the only way, he let me use this
        private void AddButton(Transform parent, string title, string text, Vector2 pos, UnityAction onClick)
        {
            var button = Object.Instantiate(PersistentUI.Instance.ButtonPrefab, parent);
            MoveTransform(button.transform, 100, 25, 0.5f, 1, pos.x, pos.y);

            button.name = title;
            button.Button.onClick.AddListener(onClick);

            button.SetText(text);
            button.Text.enableAutoSizing = false;
            button.Text.fontSize = 12;
        }

        private void AddLabel(Transform parent, string title, string text, Vector2 pos, Vector2? size = null)
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
        }

        private UITextInput AddTextInput(Transform parent, string title, string text, Vector2 pos, string value, UnityAction<string> onChange)
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
            return textInput;
        }

        private void AddCheckbox(Transform parent, string title, string text, Vector2 pos, bool value, UnityAction<bool> onClick)
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
