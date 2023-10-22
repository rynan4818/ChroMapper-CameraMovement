using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using SFB;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class MovementPlayerUI
    {
        public GameObject _movementPlayerMenu;
        public CameraMovementController movementController;
        public UITextInput _movementFileInputText;
        public UITextInput _saberFileInputText;
        public Toggle _setDefaultSaberCheck;
        public Toggle _saberEnabledCheck;
        public UITextInput _playersPlaceOffsetCheck;
        public string _movementFileNameBackup;
        public string _saberFileNameBackup;
        public bool _setDefaultSaber;
        public float _playersPlaceBackup;

        public void settingBackup()
        {
            _movementFileNameBackup = CameraMovementController.movementPlayerOptions.movementFileName;
            if (CameraMovementController.movementPlayerOptions.saberFileName == "")
                _saberFileNameBackup = Options.Instance.saberFileName;
            else
               _saberFileNameBackup = CameraMovementController.movementPlayerOptions.saberFileName;
            _movementFileInputText.InputField.text = _movementFileNameBackup;
            _saberFileInputText.InputField.text = _saberFileNameBackup;
            _saberEnabledCheck.isOn = CameraMovementController.movementPlayerOptions.saberEnabled;
            _playersPlaceBackup = CameraMovementController.movementPlayerOptions.playersPlaceOffset;
            _playersPlaceOffsetCheck.InputField.text = _playersPlaceBackup.ToString();
        }
        public void AnchoredPosSave()
        {
            Options.Instance.movementPlayerUIAnchoredPosX = _movementPlayerMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.movementPlayerUIAnchoredPosY = _movementPlayerMenu.GetComponent<RectTransform>().anchoredPosition.y;
        }
        public void AddMenu(CanvasGroup topBarCanvas)
        {
            movementController = Plugin.movement;
            //Movement Player Menu
            _movementPlayerMenu = UI.SetMenu(new GameObject("Movemnet Player Menu"), topBarCanvas, AnchoredPosSave, 500, 160, Options.Instance.movementPlayerUIAnchoredPosX, Options.Instance.movementPlayerUIAnchoredPosY);
            UI.AddLabel(_movementPlayerMenu.transform, "Movemnet Player", "Movemnet Player", new Vector2(0, -19.5f));

            var movementFileInput = UI.AddTextInput(_movementPlayerMenu.transform, "Movement File", "Movement File", _movementFileNameBackup, (value) =>
            {
                CameraMovementController.movementPlayerOptions.movementFileName = value;
            });
            _movementFileInputText = movementFileInput.Item3;
            UI.MoveTransform(movementFileInput.Item1, 60, 16, 0, 1, 63.1f, -28.2f);
            UI.MoveTransform(movementFileInput.Item3.transform, 425, 20, 0.1f, 1, 196, -46.6f);

            var saberFileInput = UI.AddTextInput(_movementPlayerMenu.transform, "Saber File", "Saber File", _saberFileNameBackup, (value) =>
            {
                CameraMovementController.movementPlayerOptions.saberFileName = value;
            });
            _saberFileInputText = saberFileInput.Item3;
            UI.MoveTransform(saberFileInput.Item1, 40, 16, 0, 1, 53.1f, -73.1f);
            UI.MoveTransform(saberFileInput.Item3.transform, 425, 20, 0.1f, 1, 196, -89.7f);

            var saberEnabled = UI.AddCheckbox(_movementPlayerMenu.transform, "Saber Enabled", "Saber Enabled", false, (check) =>
            {
                CameraMovementController.movementPlayerOptions.saberEnabled = check;
            });
            UI.MoveTransform(saberEnabled.Item3.transform, 30, 16, 0, 1, 100, -73.1f);
            UI.MoveTransform(saberEnabled.Item1, 70, 16, 0, 1, 140, -73.1f);
            _saberEnabledCheck = saberEnabled.Item3;

            var movementSelectButton = UI.AddButton(_movementPlayerMenu.transform, "Select", "Select", () =>
            {
                var ext = new ExtensionFilter[] { new ExtensionFilter("Movement File", new string[] { "json" }) };
                string dir;
                try
                {
                    dir = Path.GetDirectoryName(CameraMovementController.movementPlayerOptions.movementFileName);
                }
                catch
                {
                    dir = BeatSaberSongContainer.Instance.Song.Directory;
                }
                if (!Directory.Exists(dir))
                    dir = BeatSaberSongContainer.Instance.Song.Directory;
                var paths = StandaloneFileBrowser.OpenFilePanel("Movement File", dir, ext, false);
                if (paths.Length > 0 && File.Exists(paths[0]))
                {
                    CameraMovementController.movementPlayerOptions.movementFileName = paths[0];
                    _movementFileInputText.InputField.text = paths[0];
                }
            });
            UI.MoveTransform(movementSelectButton.transform, 40, 20, 0, 1, 440, -24.8f);

            var saberSelectButton = UI.AddButton(_movementPlayerMenu.transform, "Select", "Select", () =>
            {
                var ext = new ExtensionFilter[] { new ExtensionFilter("Saber File", new string[] { "saber" }) };
                string dir;
                try
                {
                    dir = Path.GetDirectoryName(CameraMovementController.movementPlayerOptions.saberFileName);
                }
                catch
                {
                    dir = Environment.CurrentDirectory;
                }
                if (!Directory.Exists(dir))
                    dir = Environment.CurrentDirectory;
                var paths = StandaloneFileBrowser.OpenFilePanel("Saber File", dir, ext, false);
                if (paths.Length > 0 && File.Exists(paths[0]))
                {
                    CameraMovementController.movementPlayerOptions.saberFileName = paths[0];
                    _saberFileInputText.InputField.text = paths[0];
                }
            });
            UI.MoveTransform(saberSelectButton.transform, 40, 20, 0, 1, 440, -69.8f);

            var setDefaultSaber = UI.AddCheckbox(_movementPlayerMenu.transform, "Set Default Saber", "Set Default Saber", false, (check) =>
            {
                _setDefaultSaber = check;
            });
            UI.MoveTransform(setDefaultSaber.Item3.transform, 30, 16, 0, 1, 50, -115);
            UI.MoveTransform(setDefaultSaber.Item1, 70, 16, 0, 1, 90, -115);
            _setDefaultSaberCheck = setDefaultSaber.Item3;

            var headSizeInput = UI.AddTextInput(_movementPlayerMenu.transform, "Players Place Offset", "Players Place Offset", "", (value) =>
            {
                float res;
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out res))
                {
                    CameraMovementController.movementPlayerOptions.playersPlaceOffset = res;
                    if (CameraMovementController.playersPlace != null)
                    {
                        var a = CameraMovementController.playersPlace.transform.position;
                        CameraMovementController.playersPlace.transform.position = new Vector3(a.x, movementController.playersPlaceDefault + CameraMovementController.movementPlayerOptions.playersPlaceOffset, a.z);
                    }
                }
            });
            UI.MoveTransform(headSizeInput.Item1, 100, 16, 0, 1, 70, -135);
            UI.MoveTransform(headSizeInput.Item3.transform, 40, 20, 0.1f, 1, 100, -135);
            _playersPlaceOffsetCheck = headSizeInput.Item3;

            var saveButton = UI.AddButton(_movementPlayerMenu.transform, "Save", "Save", () =>
            {
                CameraMovementController.movementPlayerOptions.SettingSave();
                Options.Instance.SettingSave();
            });
            UI.MoveTransform(saveButton.transform, 70, 25, 0, 1, 250, -134);

            var cancelButton = UI.AddButton(_movementPlayerMenu.transform, "Cancel", "Cancel", () =>
            {
                CameraMovementController.movementPlayerOptions.saberFileName = _movementFileNameBackup;
                _movementFileInputText.InputField.text = _movementFileNameBackup;
                CameraMovementController.movementPlayerOptions.saberFileName = _saberFileNameBackup;
                _saberFileInputText.InputField.text = _saberFileNameBackup;
                CameraMovementController.movementPlayerOptions.playersPlaceOffset = _playersPlaceBackup;
                _playersPlaceOffsetCheck.InputField.text = _playersPlaceBackup.ToString();
                _setDefaultSaberCheck.isOn = false;
                _setDefaultSaber = false;
                _movementPlayerMenu.SetActive(false);
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(cancelButton.transform, 70, 25, 0, 1, 339.8f, -134);

            var openButton = UI.AddButton(_movementPlayerMenu.transform, "Open", "Open", () =>
            {
                if (_setDefaultSaber)
                {
                    Options.Instance.saberFileName = CameraMovementController.movementPlayerOptions.saberFileName;
                }
                _setDefaultSaber = false;
                _setDefaultSaberCheck.isOn = false;
                _movementPlayerMenu.SetActive(false);
                UI.KeyDisableCheck();
                movementController.Reload();
            });
            UI.MoveTransform(openButton.transform, 70, 25, 0, 1, 424.4f, -134);

            _movementPlayerMenu.SetActive(false);
        }
    }
}
