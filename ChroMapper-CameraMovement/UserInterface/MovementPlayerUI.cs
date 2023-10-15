using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using SFB;
using System;
using System.IO;
using UnityEngine;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class MovementPlayerUI
    {
        public GameObject _movementPlayerMenu;
        public CameraMovementController movementController;
        public UITextInput _movementFileInputText;
        public UITextInput _saberFileInputText;
        public string _movementFileNameBackup;
        public string _saberFileNameBackup;

        public void settingBackup()
        {
            _movementFileNameBackup = Options.Instance.movementFileName;
            _saberFileNameBackup = Options.Instance.saberFileName;
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

            var movementFileInput = UI.AddTextInput(_movementPlayerMenu.transform, "Movement File", "Movement File", Options.Instance.movementFileName, (value) =>
            {
                Options.Instance.movementFileName = value;
            });
            _movementFileInputText = movementFileInput.Item3;
            UI.MoveTransform(movementFileInput.Item1, 60, 16, 0, 1, 63.1f, -28.2f);
            UI.MoveTransform(movementFileInput.Item3.transform, 425, 20, 0.1f, 1, 196, -46.6f);

            var saberFileInput = UI.AddTextInput(_movementPlayerMenu.transform, "Saber File", "Saber File", Options.Instance.saberFileName, (value) =>
            {
                Options.Instance.saberFileName = value;
            });
            _saberFileInputText = saberFileInput.Item3;
            UI.MoveTransform(saberFileInput.Item1, 60, 16, 0, 1, 63.1f, -73.1f);
            UI.MoveTransform(saberFileInput.Item3.transform, 425, 20, 0.1f, 1, 196, -89.7f);

            var movementSelectButton = UI.AddButton(_movementPlayerMenu.transform, "Select", "Select", () =>
            {
                var ext = new ExtensionFilter[] { new ExtensionFilter("Movement File", new string[] { "json" }) };
                string dir;
                try
                {
                    dir = Path.GetDirectoryName(Options.Instance.movementFileName);
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
                    Options.Instance.movementFileName = paths[0];
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
                    dir = Path.GetDirectoryName(Options.Instance.saberFileName);
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
                    Options.Instance.saberFileName = paths[0];
                    _saberFileInputText.InputField.text = paths[0];
                }
            });
            UI.MoveTransform(saberSelectButton.transform, 40, 20, 0, 1, 440, -69.8f);

            var cancelButton = UI.AddButton(_movementPlayerMenu.transform, "Cancel", "Cancel", () =>
            {
                Options.Instance.movementFileName = _movementFileNameBackup;
                Options.Instance.saberFileName = _saberFileNameBackup;
                _movementPlayerMenu.SetActive(false);
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(cancelButton.transform, 70, 25, 0, 1, 339.8f, -134);

            var openButton = UI.AddButton(_movementPlayerMenu.transform, "Open", "Open", () =>
            {
                _movementPlayerMenu.SetActive(false);
                UI.KeyDisableCheck();
            });
            UI.MoveTransform(openButton.transform, 70, 25, 0, 1, 424.4f, -134);

            _movementPlayerMenu.SetActive(false);
        }
    }
}
