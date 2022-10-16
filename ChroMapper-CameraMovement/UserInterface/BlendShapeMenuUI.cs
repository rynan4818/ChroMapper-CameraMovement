using UnityEngine;
using ChroMapper_CameraMovement.Configuration;
using TMPro;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class BlendShapeMenuUI
    {
        public GameObject _cameraMovementBlendShapeMenu;
        public TextMeshProUGUI _timeLable;
        public UITextInput _settingText;
        public void AnchoredPosSave()
        {
            Options.Instance.blendShapeMenuUIAnchoredPosX = _cameraMovementBlendShapeMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.blendShapeMenuUIAnchoredPosY = _cameraMovementBlendShapeMenu.GetComponent<RectTransform>().anchoredPosition.y;
        }
        public void AddMenu(CanvasGroup topBarCanvas)
        {
            //BlendShape Menu
            _cameraMovementBlendShapeMenu = UI.SetMenu(new GameObject("CameraMovement Blend Shape Menu"), topBarCanvas, AnchoredPosSave, 300, 320, Options.Instance.blendShapeMenuUIAnchoredPosX, Options.Instance.blendShapeMenuUIAnchoredPosY);

            var menuTransform = _cameraMovementBlendShapeMenu.transform;

            UI.AddLabel(menuTransform, "Blend Shape Menu", "Blend Shape Menu", new Vector2(5.57814f, -14.02186f), 300);

            var label = UI.AddLabel(menuTransform, "Time", "Time 0:00.00 (0)", TextAlignmentOptions.Left, 12);
            UI.MoveTransform(label.Item1, 200, 16, 0, 1, 126.3417f, -33.02549f);
            _timeLable = label.Item2;

            _settingText = UI.AddTextInput(menuTransform, "", TextAlignmentOptions.TopLeft, 12, (value) => { });
            _settingText.InputField.lineType = TMP_InputField.LineType.MultiLineNewline;
            UI.MoveTransform(_settingText.transform, 250, 90, 0, 1, 148.269f, -88.57463f);

            var checkBox = UI.AddCheckbox(menuTransform, "BlendShape", "BlendShape", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 126.9327f, -148.3155f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 45.75079f, -153.9614f);

            var button = UI.AddButton(menuTransform, "BlendShapeSet", "Set", () =>
            {
                UI._blendShapeSetUI._cameraMovementBlendShapeSet.SetActive(true);
            });
            UI.MoveTransform(button.transform, 50, 18, 0, 1, 176.0436f, -147.5473f);

            checkBox = UI.AddCheckbox(menuTransform, "SetBlendShapeNeutral", "SetBlendShapeNeutral", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 126.9327f, -170.6456f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 45.75079f, -176.2914f);

            button = UI.AddButton(menuTransform, "SetBlendShapeNeutralSet", "Set", () =>
            {
                UI._blendShapeSetUI._cameraMovementBlendShapeSet.SetActive(true);
            });
            UI.MoveTransform(button.transform, 50, 18, 0, 1, 176.0436f, -170.2055f);

            checkBox = UI.AddCheckbox(menuTransform, "SetBlendShapeSing", "SetBlendShapeSing", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 126.9327f, -192.5728f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 45.75079f, -198.2186f);

            button = UI.AddButton(menuTransform, "SetBlendShapeSingSet", "Set", () =>
            {
                UI._blendShapeSetUI._cameraMovementBlendShapeSet.SetActive(true);
            });
            UI.MoveTransform(button.transform, 50, 18, 0, 1, 176.0436f, -192.1328f);

            checkBox = UI.AddCheckbox(menuTransform, "StartAutoBlendShape", "StartAutoBlendShape", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 127.2727f, -214.8778f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 45.41081f, -220.5237f);

            checkBox = UI.AddCheckbox(menuTransform, "StopAutoBlendShape", "StopAutoBlendShape", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 248.8546f, -214.8778f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 167.2726f, -220.5237f);

            checkBox = UI.AddCheckbox(menuTransform, "StartLipSync", "StartLipSync", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 127.2727f, -238.2669f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 45.41081f, -243.9128f);

            checkBox = UI.AddCheckbox(menuTransform, "StopLipSync", "StopLipSync", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 248.8546f, -238.2669f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 167.2726f, -243.9128f);

            checkBox = UI.AddCheckbox(menuTransform, "StartSing", "StartSing", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 127.2727f, -261.8573f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 45.41081f, -267.3019f);

            checkBox = UI.AddCheckbox(menuTransform, "StopSing", "StopSing", false, (check) => { });
            UI.MoveTransform(checkBox.Item1, 160, 16, 0, 1, 248.8546f, -261.8573f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 167.2726f, -267.3019f);

            label = UI.AddLabel(menuTransform, "Duration", "Duration", TextAlignmentOptions.Center, 12);
            UI.MoveTransform(label.Item1, 50, 16, 0, 1, 246.2108f, -245.0464f);
            var textInput = UI.AddTextInput(menuTransform, "0", (value) => { });
            UI.MoveTransform(textInput.Transform, 50, 20, 0, 1, 247.6727f, -262.5883f);

            checkBox = UI.AddCheckbox(menuTransform, "NalulunaAvatarsEvents0.0.1", "NalulunaAvatars\nEvents0.0.1", false, (check) =>
            {
                UI._blendShapeSetUI.SetEvents001(check);
            });
            checkBox.Item2.fontSize = 9;
            UI.MoveTransform(checkBox.Item1, 60, 16, 0, 1, 78.30408f, -295.2505f);
            UI.MoveTransform(checkBox.Item3.transform, 30, 25, 0, 1, 45.95319f, -300.8963f);

            button = UI.AddButton(menuTransform, "Cancel", "Cancel", () => { });
            UI.MoveTransform(button.transform, 70, 25, 0, 1, 158.1f, -295.0765f);

            button = UI.AddButton(menuTransform, "OK", "OK", () => { });
            UI.MoveTransform(button.transform, 70, 25, 0, 1, 245.7f, -295.0765f);
        }
    }
}
