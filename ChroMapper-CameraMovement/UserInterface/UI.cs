using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using ChroMapper_CameraMovement.Configuration;
using UnityEngine.InputSystem;
using ChroMapper_CameraMovement.HarmonyPatches;
using ChroMapper_CameraMovement.Component;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class UI
    {
        public static readonly ExtensionButton _extensionBtn = new ExtensionButton();
        public static MainMenuUI _mainMenuUI = new MainMenuUI();
        public static SettingMenuUI _settingMenuUI = new SettingMenuUI();
        public static BookmarkMenuUI _bookmarkMenuUI = new BookmarkMenuUI();
        public static CameraControlMenuUI _cameraControlMenuUI = new CameraControlMenuUI();
        public static MultiDisplayUI _multiDisplayUI = new MultiDisplayUI();
        public static BlendShapeMenuUI _blendShapeMenuUI = new BlendShapeMenuUI();
        public static BlendShapeSetUI _blendShapeSetUI = new BlendShapeSetUI();
        public static List<Type> queuedToDisable = new List<Type>();
        public static List<Type> queuedToEnable = new List<Type>();
        public static bool keyDisable { get; private set; } = false;
        public static Vector2 savedMousePos = Vector2.zero;

        private static readonly Type[] editActionMapsDisabled =
        {
            typeof(CMInput.IBookmarksActions), typeof(CMInput.IEditorScaleActions),
            typeof(CMInput.ISongSpeedActions), typeof(CMInput.IPlaybackActions),
            typeof(CMInput.IUIModeActions), typeof(CMInput.IPauseMenuActions),
            typeof(CMInput.IAudioActions), typeof(CMInput.ILightshowActions),
            typeof(CMInput.IDebugActions), typeof(CMInput.IMenusExtendedActions),
            typeof(CMInput.IPlatformDisableableObjectsActions), typeof(CMInput.IRefreshMapActions),
            typeof(CMInput.IEventUIActions), typeof(CMInput.IWorkflowsActions)
        };

        private static readonly Type[] actionMapsEnabledWhenNodeEditing =
        {
            typeof(CMInput.IBeatmapObjectsActions),
            typeof(CMInput.ISavingActions), typeof(CMInput.ITimelineActions)
        };

        private static Type[] actionMapsDisabled => typeof(CMInput).GetNestedTypes()
            .Where(x => x.IsInterface && !actionMapsEnabledWhenNodeEditing.Contains(x)).ToArray();

        public UI()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChroMapper_CameraMovement.Resources.Icon.png");
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
            var topBarCanvas = mapEditorUI.MainUIGroup[5];
            _mainMenuUI.AddMenu(topBarCanvas);
            _settingMenuUI.AddMenu(topBarCanvas);
            _bookmarkMenuUI.AddMenu(topBarCanvas);
            _cameraControlMenuUI.AddMenu(topBarCanvas);
            _multiDisplayUI.AddMenu(topBarCanvas);
            _blendShapeMenuUI.AddMenu(topBarCanvas);
            _blendShapeSetUI.AddMenu(topBarCanvas);
            KeyDisableCheck();
        }

        public static void DisableAction(Type[] actionMaps)
        {
            foreach (Type actionMap in actionMaps)
            {
                queuedToEnable.Remove(actionMap);
                if (!queuedToDisable.Contains(actionMap))
                    queuedToDisable.Add(actionMap);
            }
        }

        public static void EnableAction(Type[] actionMaps)
        {
            foreach (Type actionMap in actionMaps)
            {
                queuedToDisable.Remove(actionMap);
                if (!queuedToEnable.Contains(actionMap))
                    queuedToEnable.Add(actionMap);
            }
        }

        public static void KeyDisableCheck()
        {
            if (Options.Instance.cameraMovementEnable && Options.Instance.mappingDisable && (_settingMenuUI._cameraMovementSettingMenu.activeSelf || _bookmarkMenuUI._cameraMovementBookmarkMenu.activeSelf ||
                _cameraControlMenuUI._cameraControlMenu.activeSelf || _multiDisplayUI._cameraMovementMultiDisplay.activeSelf || Options.Instance.subCamera ||
                Options.Instance.movement || Options.Instance.uIhidden || Options.Instance.bookmarkLines))
            {
                DisableAction(actionMapsDisabled);
                EnableAction(editActionMapsDisabled);
                keyDisable = true;
            }
            else
            {
                EnableAction(actionMapsDisabled);
                keyDisable = false;
            }
        }

        public static void QueuedActionMaps()
        {
            if (queuedToDisable.Any())
                CMInputCallbackInstaller.DisableActionMaps(typeof(UI), queuedToDisable.ToArray());
            queuedToDisable.Clear();
            if (queuedToEnable.Any())
                CMInputCallbackInstaller.ClearDisabledActionMaps(typeof(UI), queuedToEnable.ToArray());
            queuedToEnable.Clear();
        }

        public static void SetLockState(bool lockMouse)
        {
            var mouseLocked = Cursor.lockState == CursorLockMode.Locked;
            if (lockMouse && !mouseLocked)
            {
                savedMousePos = Mouse.current.position.ReadValue();
                Cursor.lockState = CursorLockMode.Locked;
                CameraControllerPatch.OriginalSetLockStateDisable = true;
            }
            else if (!lockMouse && mouseLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Mouse.current.WarpCursorPosition(new Vector2(savedMousePos.x, Screen.height - savedMousePos.y));
                CameraControllerPatch.OriginalSetLockStateDisable = false;
            }
        }

        public static UIButton AddButton(Transform parent, string title, string text, Vector2 pos, UnityAction onClick)
        {
            var button = AddButton(parent, title, text, onClick);
            MoveTransform(button.transform, 100, 25, 0.5f, 1, pos.x, pos.y);
            return button;
        }
        public static UIButton AddButton(Transform parent, string title, string text, UnityAction onClick)
        {
            return AddButton(parent, title, text, 12, onClick);
        }
        public static UIButton AddButton(Transform parent, string title, string text, float fontSize, UnityAction onClick)
        {
            var button = UnityEngine.Object.Instantiate(PersistentUI.Instance.ButtonPrefab, parent);
            button.name = title;
            button.Button.onClick.AddListener(onClick);
            button.SetText(text);
            button.Text.enableAutoSizing = false;
            button.Text.fontSize = fontSize;
            return button;
        }

        public static (RectTransform, TextMeshProUGUI) AddLabel(Transform parent, string title, string text, Vector2 pos, float width = 110, float height = 24)
        {
            var label = AddLabel(parent, title, text);
            MoveTransform(label.Item1, width, height, 0.5f, 1, pos.x, pos.y);
            return label;
        }
        public static (RectTransform, TextMeshProUGUI) AddLabel(Transform parent, string title, string text, TextAlignmentOptions alignment = TextAlignmentOptions.Center, float fontSize = 16)
        {
            var entryLabel = new GameObject(title + " Label", typeof(TextMeshProUGUI));
            var rectTransform = (RectTransform)entryLabel.transform;
            rectTransform.SetParent(parent);
            var textComponent = entryLabel.GetComponent<TextMeshProUGUI>();
            textComponent.name = title;
            textComponent.font = PersistentUI.Instance.ButtonPrefab.Text.font;
            textComponent.alignment = alignment;
            textComponent.fontSize = fontSize;
            textComponent.text = text;
            return (rectTransform, textComponent);
        }

        public static (RectTransform, TextMeshProUGUI, UITextInput) AddTextInput(Transform parent, string title, string text, Vector2 pos, string value, UnityAction<string> onChange)
        {
            var textInput = AddTextInput(parent, title, text, value, onChange);
            MoveTransform(textInput.Item1, 50, 16, 0.5f, 1, pos.x - 47.5f, pos.y);
            MoveTransform(textInput.Item3.transform, 75, 20, 0.5f, 1, pos.x + 27.5f, pos.y);
            return textInput;
        }
        public static (RectTransform, TextMeshProUGUI, UITextInput) AddTextInput(Transform parent, string title, string text, string value, UnityAction<string> onChange)
        {
            var label = AddLabel(parent, title, text, TextAlignmentOptions.Right, 12);
            return (label.Item1, label.Item2, AddTextInput(parent, value, onChange));
        }
        public static UITextInput AddTextInput(Transform parent, string value, UnityAction<string> onChange)
        {
            return AddTextInput(parent, value, TextAlignmentOptions.Left, 10, onChange);
        }
        public static UITextInput AddTextInput(Transform parent, string value, TextAlignmentOptions alignment,float fontSize , UnityAction<string> onChange)
        {
            var textInput = UnityEngine.Object.Instantiate(PersistentUI.Instance.TextInputPrefab, parent);
            textInput.GetComponent<Image>().pixelsPerUnitMultiplier = 3;
            textInput.InputField.text = value;
            textInput.InputField.onFocusSelectAll = false;
            textInput.InputField.textComponent.alignment = alignment;
            textInput.InputField.textComponent.fontSize = fontSize;
            textInput.InputField.onValueChanged.AddListener(onChange);
            textInput.InputField.onEndEdit.AddListener(delegate {
                KeyDisableCheck();
                Plugin.movement.KeyEnable();
            });
            textInput.InputField.onSelect.AddListener(delegate {
                DisableAction(actionMapsDisabled);
                Plugin.movement.KeyDisable();
            });
            return textInput;
        }

        public static (RectTransform, TextMeshProUGUI, Toggle) AddCheckbox(Transform parent, string title, string text, Vector2 pos, bool value, UnityAction<bool> onClick)
        {
            var checkBox = AddCheckbox(parent, title, text, value, onClick);
            MoveTransform(checkBox.Item1, 80, 16, 0.5f, 1, pos.x + 10, pos.y + 5);
            MoveTransform(checkBox.Item3.transform, 100, 25, 0.5f, 1, pos.x, pos.y);
            return checkBox;
        }
        public static (RectTransform, TextMeshProUGUI, Toggle) AddCheckbox(Transform parent, string title, string text, bool value, UnityAction<bool> onClick)
        {
            var label = AddLabel(parent, title, text, TextAlignmentOptions.Left, 12);
            return (label.Item1, label.Item2, AddCheckbox(parent, value, onClick));
        }
        public static Toggle AddCheckbox(Transform parent, bool value, UnityAction<bool> onClick)
        {
            var original = GameObject.Find("Strobe Generator").GetComponentInChildren<Toggle>(true);
            var toggleObject = UnityEngine.Object.Instantiate(original, parent.transform);
            var toggleComponent = toggleObject.GetComponent<Toggle>();
            var colorBlock = toggleComponent.colors;
            colorBlock.normalColor = Color.white;
            toggleComponent.colors = colorBlock;
            toggleComponent.isOn = value;
            toggleComponent.onValueChanged.AddListener(onClick);
            return toggleComponent;
        }

        public static UIDropdown AddDropdown(Transform parent, List<string> options, int value, UnityAction<int> onChange)
        {
            var dropdown = UnityEngine.Object.Instantiate(PersistentUI.Instance.DropdownPrefab, parent);
            dropdown.SetOptions(options);
            dropdown.Dropdown.onValueChanged.AddListener(onChange);
            dropdown.Dropdown.SetValueWithoutNotify(value);
            var image = dropdown.GetComponent<Image>();
            image.color = new Color(0.35f, 0.35f, 0.35f, 1f);
            image.pixelsPerUnitMultiplier = 1.5f;
            return dropdown;
        }
        public static GameObject SetMenu(GameObject obj, CanvasGroup canvas, Action posSave, float sizeX, float sizeY, float anchorPosX, float anchorPosY, float anchorX = 1, float anchorY = 1, float pivotX = 1, float pivotY = 1)
        {
            SetMenu(obj, canvas, posSave);
            AttachTransform(obj, sizeX, sizeY, anchorX, anchorY, anchorPosX, anchorPosY, pivotX, pivotY);
            AttachImage(obj, new Color(0.24f, 0.24f, 0.24f));
            return obj;
        }

        public static GameObject SetMenu(GameObject obj, CanvasGroup canvas, Action posSave)
        {
            obj.transform.parent = canvas.transform;
            var dragWindow = obj.AddComponent<DragWindowController>();
            dragWindow.canvas = canvas.GetComponent<Canvas>();
            dragWindow.OnDragWindow += posSave;
            return obj;
        }
        public static void AttachImage(GameObject obj, Color color)
        {
            var imageSetting = obj.AddComponent<Image>();
            imageSetting.sprite = PersistentUI.Instance.Sprites.Background;
            imageSetting.type = Image.Type.Sliced;
            imageSetting.color = color;
        }
        public static RectTransform AttachTransform(GameObject obj, float sizeX, float sizeY, float anchorX, float anchorY, float anchorPosX, float anchorPosY, float pivotX = 0.5f, float pivotY = 0.5f)
        {
            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            MoveTransform(rectTransform, sizeX, sizeY, anchorX, anchorY, anchorPosX, anchorPosY, pivotX, pivotY);
            return rectTransform;
        }

        public static void MoveTransform(Transform transform, float sizeX, float sizeY, float anchorX, float anchorY, float anchorPosX, float anchorPosY, float pivotX = 0.5f, float pivotY = 0.5f)
        {
            if (!(transform is RectTransform rectTransform)) return;
            MoveTransform(rectTransform, sizeX, sizeY, anchorX, anchorY, anchorPosX, anchorPosY, pivotX, pivotY);
        }
        public static void MoveTransform(RectTransform rectTransform, float sizeX, float sizeY, float anchorX, float anchorY, float anchorPosX, float anchorPosY, float pivotX = 0.5f, float pivotY = 0.5f)
        {
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(anchorX, anchorY);
            rectTransform.anchoredPosition = new Vector3(anchorPosX, anchorPosY, 0);
        }
    }
}
