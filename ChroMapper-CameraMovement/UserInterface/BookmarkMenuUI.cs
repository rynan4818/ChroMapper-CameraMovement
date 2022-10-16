using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;
using System.Text.RegularExpressions;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class BookmarkMenuUI
    {
        public GameObject _cameraMovementBookmarkMenu;
        public TextMeshProUGUI currentBookmarkLabelText;
        public TextMeshProUGUI bookmarkMenuInputLabel;
        public UITextInput bookmarkMenuInputText;
        public Toggle bookmarkSetCheckboxToggle;
        public UIButton quickCommand1Button;
        public UIButton quickCommand2Button;
        public UIButton quickCommand3Button;
        public UIButton quickCommand4Button;
        public UIButton quickCommand5Button;
        public UIButton quickCommand6Button;
        public int currentBookmarkNo = 0;
        public bool commandSet = false;

        public void CurrentBookmarkUpdate(string bookmarkName, int bookmarkNo, float time)
        {
            currentBookmarkNo = bookmarkNo;
            currentBookmarkLabelText.text = bookmarkName;
            if (bookmarkNo == 0)
            {
                bookmarkMenuInputLabel.text = "No.-";
            }
            else
            {
                bookmarkMenuInputLabel.text = $"No.{bookmarkNo} [{time.ToString("0.####")}]";
            }
        }

        public void AnchoredPosSave()
        {
            Options.Instance.bookmarkUIAnchoredPosX = _cameraMovementBookmarkMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.bookmarkUIAnchoredPosY = _cameraMovementBookmarkMenu.GetComponent<RectTransform>().anchoredPosition.y;
        }

        public void AddMenu(CanvasGroup topBarCanvas)
        {
            //Bookmark
            _cameraMovementBookmarkMenu = UI.SetMenu(new GameObject("CameraMovement Bookmark"), topBarCanvas, AnchoredPosSave, 750, 55, Options.Instance.bookmarkUIAnchoredPosX, Options.Instance.bookmarkUIAnchoredPosY, 0.7f, 0.09f);

            var currentBookmarkLabel = UI.AddLabel(_cameraMovementBookmarkMenu.transform, "", "", TextAlignmentOptions.Left, 12);
            UI.MoveTransform(currentBookmarkLabel.Item1, 400, 20, 0.1f, 1, 210, -15);
            currentBookmarkLabelText = currentBookmarkLabel.Item2;

            var bookmarkSetCheckbox = UI.AddCheckbox(_cameraMovementBookmarkMenu.transform, "Set", "Set", false, (check) =>
            {
                commandSet = check;
            });
            UI.MoveTransform(bookmarkSetCheckbox.Item1, 50, 16, 0.1f, 1, -40, -37);
            UI.MoveTransform(bookmarkSetCheckbox.Item3.transform, 30, 16, 0.1f, 1, -35, -37);
            bookmarkSetCheckboxToggle = bookmarkSetCheckbox.Item3;

            var bookmarkMenuInput = UI.AddTextInput(_cameraMovementBookmarkMenu.transform, "No.-", "No.-", "", (value) =>
            {
            });
            UI.MoveTransform(bookmarkMenuInput.Item1, 150, 16, 0.1f, 1, 10, -15);
            bookmarkMenuInputLabel = bookmarkMenuInput.Item2;
            bookmarkMenuInputLabel.alignment = TextAlignmentOptions.Left;
            UI.MoveTransform(bookmarkMenuInput.Item3.transform, 450, 20, 0.1f, 1, 270, -35);
            bookmarkMenuInputText = bookmarkMenuInput.Item3;
            bookmarkMenuInputText.InputField.textComponent.fontSize = 14;

            var bookmarkMenuCopyButton = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Copy to Edit", "Copy to Edit", () =>
            {
                bookmarkMenuInputText.InputField.text = currentBookmarkLabelText.text;
            });
            bookmarkMenuCopyButton.Text.fontSize = 9;
            UI.MoveTransform(bookmarkMenuCopyButton.transform, 35, 20, 0.1f, 1, -10, -35);

            var bookmarkMenuClearButton = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Clear", "Clear", () =>
            {
                bookmarkMenuInputText.InputField.text = "";
            });
            bookmarkMenuClearButton.Text.fontSize = 10;
            UI.MoveTransform(bookmarkMenuClearButton.transform, 30, 20, 0.1f, 1, 25, -35);

            var bookmarkMenuNewButton = UI.AddButton(_cameraMovementBookmarkMenu.transform, "New", "New", () =>
            {
                if (bookmarkMenuInputText.InputField.text.Trim() != "")
                    Plugin.movement._bookmarkController.BookmarkNew(bookmarkMenuInputText.InputField.text);
            });
            UI.MoveTransform(bookmarkMenuNewButton.transform, 30, 20, 0.1f, 1, 515, -35);

            var bookmarkMenuChangeButton = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Change", "Change", () =>
            {
                if (currentBookmarkNo > 0)
                    Plugin.movement._bookmarkController.BookmarkChange(currentBookmarkNo);
            });
            UI.MoveTransform(bookmarkMenuChangeButton.transform, 40, 20, 0.1f, 1, 550, -35);

            var bookmarkMenuDeleteButton = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Delete", "Delete", () =>
            {
                if (currentBookmarkNo > 0)
                    Plugin.movement._bookmarkController.BookmarkDelete(currentBookmarkNo);
            });
            UI.MoveTransform(bookmarkMenuDeleteButton.transform, 40, 20, 0.1f, 1, 590, -35);

            var regexKey = new Regex(@"<\w+>/");
            var scriptMapperRunButton = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Script Mapper Run", $"Script Mapper Run [{regexKey.Replace(Options.Instance.scriptMapperKeyBinding, "").ToUpper()}]", () =>
            {
                ScriptMapperController.ScriptMapperRun();
            });
            scriptMapperRunButton.Text.fontSize = 9;
            UI.MoveTransform(scriptMapperRunButton.transform, 55, 20, 0.1f, 1, 640, -35);

            quickCommand1Button = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Command1", Options.Instance.quickCommand1, () =>
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
            UI.MoveTransform(quickCommand1Button.transform, 50, 20, 0.1f, 1, 390, -15);

            quickCommand2Button = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Command2", Options.Instance.quickCommand2, () =>
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
            UI.MoveTransform(quickCommand2Button.transform, 50, 20, 0.1f, 1, 440, -15);

            quickCommand3Button = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Command3", Options.Instance.quickCommand3, () =>
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
            UI.MoveTransform(quickCommand3Button.transform, 50, 20, 0.1f, 1, 490, -15);

            quickCommand4Button = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Command4", Options.Instance.quickCommand4, () =>
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
            UI.MoveTransform(quickCommand4Button.transform, 50, 20, 0.1f, 1, 540, -15);

            quickCommand5Button = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Command5", Options.Instance.quickCommand5, () =>
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
            UI.MoveTransform(quickCommand5Button.transform, 50, 20, 0.1f, 1, 590, -15);

            quickCommand6Button = UI.AddButton(_cameraMovementBookmarkMenu.transform, "Command6", Options.Instance.quickCommand6, () =>
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
            UI.MoveTransform(quickCommand6Button.transform, 50, 20, 0.1f, 1, 640, -15);

            _cameraMovementBookmarkMenu.SetActive(Options.Instance.cameraMovementEnable && Options.Instance.bookmarkEdit);
        }
    }
}
