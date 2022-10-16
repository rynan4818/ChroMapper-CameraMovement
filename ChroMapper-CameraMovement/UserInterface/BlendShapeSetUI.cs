using ChroMapper_CameraMovement.Configuration;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class BlendShapeSetUI
    {
        public GameObject _cameraMovementBlendShapeSet;
        public UIDropdown _DropDownClip2;
        public UIDropdown _DropDownClip3;
        public UITextInput _TextInputValue2;
        public UITextInput _TextInputValue3;
        public UIButton _ButtonCancel;
        public UIButton _ButtonOk;
        public void AnchoredPosSave()
        {
            Options.Instance.blendShapeSetUIAnchoredPosX = _cameraMovementBlendShapeSet.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.blendShapeSetUIAnchoredPosY = _cameraMovementBlendShapeSet.GetComponent<RectTransform>().anchoredPosition.y;
        }
        public void AddMenu(CanvasGroup topBarCanvas)
        {
            //BlendShape Set
            _cameraMovementBlendShapeSet = UI.SetMenu(new GameObject("CameraMovement Blend Shape Set Menu"), topBarCanvas, AnchoredPosSave, 300, 180, Options.Instance.blendShapeSetUIAnchoredPosX, Options.Instance.blendShapeSetUIAnchoredPosY);

            var menuTransform = _cameraMovementBlendShapeSet.transform;
            var blendShapeList = new List<string> { "Natural", "A", "I", "U", "E", "O", "Fun" };

            UI.AddLabel(menuTransform, "Blend Shape Setting", "Blend Shape Setting", new Vector2(5.656387f, -14.02186f), 300);

            var label = UI.AddLabel(menuTransform, "Duration", "Duration", TextAlignmentOptions.Center, 12);
            UI.MoveTransform(label.Item1, 50, 16, 0, 1, 50.27077f, -44.62716f);
            var textInput = UI.AddTextInput(menuTransform, "0", (value) => { });
            UI.MoveTransform(textInput.transform, 50, 20, 0, 1, 50.40541f, -63.72368f);

            label = UI.AddLabel(menuTransform, "Clip", "Clip", TextAlignmentOptions.Center, 12);
            UI.MoveTransform(label.Item1, 120, 16, 0, 1, 154.1946f, -44.62716f);
            var dropDown = UI.AddDropdown(menuTransform, blendShapeList, 0, (value) => { });
            UI.MoveTransform(dropDown.transform, 120, 20, 0 ,1 , 154.1945f, -63.72368f);

            label = UI.AddLabel(menuTransform, "Value", "Value", TextAlignmentOptions.Center, 12);
            UI.MoveTransform(label.Item1, 50, 16, 0, 1, 252.2109f, -44.62716f);
            textInput = UI.AddTextInput(menuTransform, "1", (value) => { });
            UI.MoveTransform(textInput.transform, 50, 20, 0, 1, 252.1218f, -63.72368f);

            _DropDownClip2 = UI.AddDropdown(menuTransform, blendShapeList, 0, (value) => { });
            UI.MoveTransform(_DropDownClip2.transform, 120, 20, 0, 1, 154.1945f, -90.86008f);

            _TextInputValue2 = UI.AddTextInput(menuTransform, "1", (value) => { });
            UI.MoveTransform(_TextInputValue2.transform, 50, 20, 0, 1, 252.1218f, -90.86008f);

            _DropDownClip3 = UI.AddDropdown(menuTransform, blendShapeList, 0, (value) => { });
            UI.MoveTransform(_DropDownClip3.transform, 120, 20, 0, 1, 154.1945f, -118.6346f);

            _TextInputValue3 = UI.AddTextInput(menuTransform, "1", (value) => { });
            UI.MoveTransform(_TextInputValue3.transform, 50, 20, 0, 1, 252.1218f, -118.6346f);

            _ButtonCancel = UI.AddButton(menuTransform, "Cancel", "Cancel", () =>
            {
                _cameraMovementBlendShapeSet.SetActive(false);
            });

            _ButtonOk = UI.AddButton(menuTransform, "OK", "OK", () =>
            {
                _cameraMovementBlendShapeSet.SetActive(false);
            });

            SetEvents001(true);
            _cameraMovementBlendShapeSet.SetActive(false);
        }

        public void SetEvents001(bool value)
        {
            var offset = 55.5f;
            if (!value)
                offset = 0;
            UI.MoveTransform(_cameraMovementBlendShapeSet.transform, 380, 180 - offset, 1, 1, _cameraMovementBlendShapeSet.GetComponent<RectTransform>().anchoredPosition.x, _cameraMovementBlendShapeSet.GetComponent<RectTransform>().anchoredPosition.y, 1, 1);
            UI.MoveTransform(_ButtonCancel.transform, 70, 25, 0, 1, 158.1f, -152.2565f + offset);
            UI.MoveTransform(_ButtonOk.transform, 70, 25, 0, 1, 245.7f, -152.2565f + offset);
            _DropDownClip2.gameObject.SetActive(!value);
            _DropDownClip3.gameObject.SetActive(!value);
            _TextInputValue2.gameObject.SetActive(!value);
            _TextInputValue3.gameObject.SetActive(!value);
        }
    }
}
