using System.Collections.Generic;
using System.IO;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Controller;
using SFB;
using TMPro;
using UnityEngine;

namespace ChroMapper_CameraMovement.UserInterface
{
    public class PackageExportMenuUI
    {
        private const float LeftLabelWidth = 165f;
        private const float LeftLabelCenterX = 102.5f;
        private const float DefaultInputWidth = 455f;
        private const float DefaultInputCenterX = 427.5f;
        private const float HashInputWidth = 315f;
        private const float HashInputCenterX = 357.5f;
        private const float HashButtonCenterX = 585f;
        private const float RenamePreviewWidth = 330f;
        private const float RenamePreviewCenterX = 530f;
        private const float LabelFontSize = 13f;

        private static readonly List<string> NamingModeOptions = new List<string>
        {
            "Default",
            "Custom"
        };

        private static readonly List<string> PackagingModeOptions = new List<string>
        {
            "1. SongScript.json + configured folder",
            "2. Rename JSON + no folder",
            "3. JSON name + configured folder"
        };

        public GameObject _cameraMovementPackageExportMenu;
        public CameraMovementController movementController;

        private readonly PackageExportController exportController = new PackageExportController();
        private UITextInput cameraScriptAuthorNameInput;
        private UITextInput mapIdInput;
        private UITextInput hashInput;
        private UITextInput songNameInput;
        private UITextInput songSubNameInput;
        private UITextInput songAuthorNameInput;
        private UITextInput levelAuthorNameInput;
        private UITextInput bpmInput;
        private UITextInput durationInput;
        private UITextInput avatarHeightInput;
        private UITextInput descriptionInput;
        private UITextInput customFormatInput;
        private UIDropdown namingModeDropdown;
        private UIDropdown packagingModeDropdown;
        private TextMeshProUGUI renamePreviewText;
        private TextMeshProUGUI previewText;
        private TextMeshProUGUI statusText;
        private TextMeshProUGUI sourceText;
        private bool suppressPreviewUpdate;

        public void AnchoredPosSave()
        {
            Options.Instance.packageExportMenuUIAnchoredPosX = _cameraMovementPackageExportMenu.GetComponent<RectTransform>().anchoredPosition.x;
            Options.Instance.packageExportMenuUIAnchoredPosY = _cameraMovementPackageExportMenu.GetComponent<RectTransform>().anchoredPosition.y;
        }

        public void AddMenu(CanvasGroup topBarCanvas)
        {
            movementController = Plugin.movement;
            _cameraMovementPackageExportMenu = UI.SetMenu(
                new GameObject("CameraMovement Export Package Menu"),
                topBarCanvas,
                AnchoredPosSave,
                720,
                520,
                Options.Instance.packageExportMenuUIAnchoredPosX,
                Options.Instance.packageExportMenuUIAnchoredPosY);

            UI.AddLabel(_cameraMovementPackageExportMenu.transform, "Export Package", "Export Package", new Vector2(0, -15), 240);
            sourceText = UI.AddLabel(_cameraMovementPackageExportMenu.transform, "Package Source", "", new Vector2(0, -35), 660, 18).Item2;
            sourceText.fontSize = 10;
            sourceText.alignment = TextAlignmentOptions.Left;

            cameraScriptAuthorNameInput = CreateInput("Package CameraScriptAuthorName", "Camera Script Author", -60).Item3;
            mapIdInput = CreateInput("Package MapId", "Map ID (bsr)", -85).Item3;
            hashInput = CreateInput("Package Hash", "Hash", -110, HashInputWidth, HashInputCenterX).Item3;
            songNameInput = CreateInput("Package SongName", "Song Name", -135).Item3;
            songSubNameInput = CreateInput("Package SongSubName", "Song Sub Name", -160).Item3;
            songAuthorNameInput = CreateInput("Package SongAuthorName", "Song Author Name", -185).Item3;
            levelAuthorNameInput = CreateInput("Package LevelAuthorName", "Level Author Name", -210).Item3;
            bpmInput = CreateInput("Package Bpm", "BPM", -235).Item3;
            durationInput = CreateInput("Package Duration", "Duration (sec)", -260).Item3;
            avatarHeightInput = CreateInput("Package AvatarHeight", "Avatar Height (cm)", -285).Item3;
            descriptionInput = CreateInput("Package Description", "Description", -310).Item3;

            var fetchHashButton = UI.AddButton(_cameraMovementPackageExportMenu.transform, "Fetch BeatSaver Hash", "BeatSaver hash", () =>
            {
                CoroutineStarter.Instance.StartCoroutine(exportController.FetchHashCoroutine(
                    mapIdInput.InputField.text,
                    value => hashInput.InputField.text = value,
                    SetStatus,
                    ShowErrorDialog));
            });
            fetchHashButton.Text.fontSize = 10;
            UI.MoveTransform(fetchHashButton.transform, 120, 20, 0, 1, HashButtonCenterX, -110);

            var namingModeLabel = UI.AddLabel(_cameraMovementPackageExportMenu.transform, "Package Naming Mode", "Rename", TextAlignmentOptions.Right, 12);
            namingModeDropdown = UI.AddDropdown(_cameraMovementPackageExportMenu.transform, NamingModeOptions, 0, OnNamingModeChanged);
            LayoutDropdownRow(namingModeLabel, namingModeDropdown, -340, 160, 280);

            var renamePreview = UI.AddLabel(_cameraMovementPackageExportMenu.transform, "Package Rename Preview", "", TextAlignmentOptions.Left, 11);
            renamePreviewText = renamePreview.Item2;
            renamePreviewText.enableWordWrapping = false;
            renamePreviewText.overflowMode = TextOverflowModes.Ellipsis;
            UI.MoveTransform(renamePreview.Item1, RenamePreviewWidth, 18, 0, 1, RenamePreviewCenterX, -340);

            var customFormat = UI.AddTextInput(_cameraMovementPackageExportMenu.transform, "Package Custom Format", "Custom Format", "", value =>
            {
                Options.Instance.packageExportCustomFormat = value;
                UpdatePreview();
            });
            LayoutInputRow(customFormat, -365);
            customFormatInput = customFormat.Item3;

            var packagingModeLabel = UI.AddLabel(_cameraMovementPackageExportMenu.transform, "Package Packaging Mode", "Packaging", TextAlignmentOptions.Right, 12);
            packagingModeDropdown = UI.AddDropdown(_cameraMovementPackageExportMenu.transform, PackagingModeOptions, 0, OnPackagingModeChanged);
            LayoutDropdownRow(packagingModeLabel, packagingModeDropdown, -390, 450, 425);

            previewText = UI.AddLabel(_cameraMovementPackageExportMenu.transform, "Package Preview", "", new Vector2(0, -420), 660, 18).Item2;
            previewText.fontSize = 11;
            previewText.alignment = TextAlignmentOptions.Left;

            var tagHint = UI.AddLabel(
                _cameraMovementPackageExportMenu.transform,
                "Package Tags",
                "Tags: {MapId} {SongName} {SongSubName} {SongAuthorName} {LevelAuthorName} {CameraScriptAuthorName} {FileName} {Bpm}",
                new Vector2(0, -442),
                660,
                18).Item2;
            tagHint.fontSize = 10;
            tagHint.alignment = TextAlignmentOptions.Left;

            statusText = UI.AddLabel(_cameraMovementPackageExportMenu.transform, "Package Status", "", new Vector2(0, -465), 660, 18).Item2;
            statusText.fontSize = 11;
            statusText.alignment = TextAlignmentOptions.Left;

            var reloadButton = UI.AddButton(_cameraMovementPackageExportMenu.transform, "Reload Defaults", "Reload Defaults", () =>
            {
                ReloadDefaults();
            });
            UI.MoveTransform(reloadButton.transform, 100, 25, 0, 1, 70, -492);

            var saveButton = UI.AddButton(_cameraMovementPackageExportMenu.transform, "Setting Save", "Setting Save", () =>
            {
                Options.Instance.packageExportCameraScriptAuthorName = cameraScriptAuthorNameInput.InputField.text;
                Options.Instance.packageExportNamingMode = GetNamingModeByIndex(namingModeDropdown.Dropdown.value);
                Options.Instance.packageExportCustomFormat = customFormatInput.InputField.text;
                Options.Instance.packageExportPackagingMode = GetPackagingModeByIndex(packagingModeDropdown.Dropdown.value);
                Options.Instance.SettingSave();
                SetStatus("Export package settings saved.");
            });
            UI.MoveTransform(saveButton.transform, 90, 25, 0, 1, 185, -492);

            var folderExportButton = UI.AddButton(_cameraMovementPackageExportMenu.transform, "Export as Folder", "Export as Folder", ExportToFolder);
            UI.MoveTransform(folderExportButton.transform, 115, 25, 0, 1, 315, -492);

            var zipExportButton = UI.AddButton(_cameraMovementPackageExportMenu.transform, "Export as ZIP", "Export as ZIP", ExportToZip);
            UI.MoveTransform(zipExportButton.transform, 115, 25, 0, 1, 445, -492);

            var closeButton = UI.AddButton(_cameraMovementPackageExportMenu.transform, "Close", "Close", CloseMenu);
            UI.MoveTransform(closeButton.transform, 80, 25, 0, 1, 555, -492);

            _cameraMovementPackageExportMenu.SetActive(false);
        }

        public void OpenMenu()
        {
            if (!ReloadDefaults())
                return;

            _cameraMovementPackageExportMenu.SetActive(true);
            UI.KeyDisableCheck();
        }

        private void CloseMenu()
        {
            SetStatus("");
            UI.HideMenu(_cameraMovementPackageExportMenu);
        }

        private bool ReloadDefaults()
        {
            PackageExportController.PackageExportFormData formData;
            string errorMessage;
            if (!exportController.TryBuildDefaultFormData(out formData, out errorMessage))
            {
                ShowErrorDialog(errorMessage);
                return false;
            }

            suppressPreviewUpdate = true;
            sourceText.text = $"Source: {formData.ScriptPath}";
            cameraScriptAuthorNameInput.InputField.text = formData.CameraScriptAuthorName;
            mapIdInput.InputField.text = formData.MapId;
            hashInput.InputField.text = formData.Hash;
            songNameInput.InputField.text = formData.SongName;
            songSubNameInput.InputField.text = formData.SongSubName;
            songAuthorNameInput.InputField.text = formData.SongAuthorName;
            levelAuthorNameInput.InputField.text = formData.LevelAuthorName;
            bpmInput.InputField.text = formData.Bpm;
            durationInput.InputField.text = formData.Duration;
            avatarHeightInput.InputField.text = formData.AvatarHeight;
            descriptionInput.InputField.text = formData.Description;
            customFormatInput.InputField.text = formData.CustomFormat;
            namingModeDropdown.Dropdown.SetValueWithoutNotify(GetNamingModeIndex(formData.NamingMode));
            packagingModeDropdown.Dropdown.SetValueWithoutNotify(GetPackagingModeIndex(formData.PackagingMode));
            suppressPreviewUpdate = false;

            Options.Instance.packageExportNamingMode = formData.NamingMode;
            Options.Instance.packageExportCustomFormat = formData.CustomFormat;
            Options.Instance.packageExportPackagingMode = formData.PackagingMode;

            SetStatus("Current song metadata loaded.");
            UpdatePreview();
            return true;
        }

        private void ExportToFolder()
        {
            PackageExportController.PackageExportRequest request;
            string errorMessage;
            if (!exportController.TryCreateRequest(BuildFormData(), out request, out errorMessage))
            {
                ShowErrorDialog(errorMessage);
                return;
            }

            string initialDirectory = request.SourceDirectory;
            if (string.IsNullOrWhiteSpace(initialDirectory) || !Directory.Exists(initialDirectory))
                initialDirectory = Path.GetDirectoryName(request.ScriptPath);

            var paths = StandaloneFileBrowser.OpenFolderPanel("Export Package to Folder", initialDirectory, false);
            if (paths == null || paths.Length == 0 || string.IsNullOrWhiteSpace(paths[0]))
                return;

            try
            {
                string outputDirectory = exportController.ExportToDirectory(request, paths[0]);
                SetStatus($"Folder export completed: {outputDirectory}");
                PersistentUI.Instance.DisplayMessage("Folder export completed.", PersistentUI.DisplayMessageType.Bottom);
                OpenFolderInFileBrowser(outputDirectory, "Could not open the export folder.");
            }
            catch (System.Exception ex)
            {
                ShowErrorDialog($"Folder export failed: {ex.Message}");
            }
        }

        private void ExportToZip()
        {
            PackageExportController.PackageExportFormData formData = BuildFormData();
            PackageExportController.PackageExportRequest request;
            string errorMessage;
            if (!exportController.TryCreateRequest(formData, out request, out errorMessage))
            {
                ShowErrorDialog(errorMessage);
                return;
            }

            string initialDirectory = request.SourceDirectory;
            if (string.IsNullOrWhiteSpace(initialDirectory) || !Directory.Exists(initialDirectory))
                initialDirectory = Path.GetDirectoryName(request.ScriptPath);

            string savePath = StandaloneFileBrowser.SaveFilePanel(
                "Export Package as ZIP",
                initialDirectory,
                Path.GetFileNameWithoutExtension(exportController.GetDefaultZipFileName(formData)),
                "zip");

            if (string.IsNullOrWhiteSpace(savePath))
                return;

            try
            {
                exportController.ExportToZip(request, savePath);
                SetStatus($"ZIP export completed: {savePath}");
                PersistentUI.Instance.DisplayMessage("ZIP export completed.", PersistentUI.DisplayMessageType.Bottom);
                OpenFolderInFileBrowser(Path.GetDirectoryName(savePath), "Could not open the ZIP folder.");
            }
            catch (System.Exception ex)
            {
                ShowErrorDialog($"ZIP export failed: {ex.Message}");
            }
        }

        private void OpenFolderInFileBrowser(string path, string warningMessage)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                ShowErrorDialog(warningMessage);
                return;
            }

            OSTools.OpenFileBrowser(path);
        }

        private PackageExportController.PackageExportFormData BuildFormData()
        {
            return new PackageExportController.PackageExportFormData
            {
                ScriptPath = ExtractSourcePath(),
                SourceDirectory = Path.GetDirectoryName(ExtractSourcePath()) ?? "",
                OriginalFileName = Path.GetFileName(ExtractSourcePath()),
                MapId = mapIdInput.InputField.text,
                Hash = hashInput.InputField.text,
                CameraScriptAuthorName = cameraScriptAuthorNameInput.InputField.text,
                SongName = songNameInput.InputField.text,
                SongSubName = songSubNameInput.InputField.text,
                SongAuthorName = songAuthorNameInput.InputField.text,
                LevelAuthorName = levelAuthorNameInput.InputField.text,
                Bpm = bpmInput.InputField.text,
                Duration = durationInput.InputField.text,
                AvatarHeight = avatarHeightInput.InputField.text,
                Description = descriptionInput.InputField.text,
                NamingMode = GetNamingModeByIndex(namingModeDropdown.Dropdown.value),
                CustomFormat = customFormatInput.InputField.text,
                PackagingMode = GetPackagingModeByIndex(packagingModeDropdown.Dropdown.value)
            };
        }

        private string ExtractSourcePath()
        {
            const string prefix = "Source: ";
            if (sourceText == null || string.IsNullOrWhiteSpace(sourceText.text))
                return CameraMovementController.ScriptGet();

            return sourceText.text.StartsWith(prefix) ? sourceText.text.Substring(prefix.Length) : sourceText.text;
        }

        private void UpdatePreview()
        {
            if (suppressPreviewUpdate)
                return;

            PackageExportController.PackageExportFormData formData = BuildFormData();
            if (renamePreviewText != null)
                renamePreviewText.text = $"->  {exportController.GetRenamePreview(formData)}";
            if (previewText != null)
                previewText.text = $"Preview: {exportController.GetPreviewPath(formData)}";
        }

        private void OnNamingModeChanged(int index)
        {
            Options.Instance.packageExportNamingMode = GetNamingModeByIndex(index);
            UpdatePreview();
        }

        private void OnPackagingModeChanged(int index)
        {
            Options.Instance.packageExportPackagingMode = GetPackagingModeByIndex(index);
            UpdatePreview();
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message ?? "";
        }

        private void ShowErrorDialog(string message)
        {
            SetStatus(message);
            PersistentUI.Instance.ShowDialogBox(message, null, PersistentUI.DialogBoxPresetType.Ok);
        }

        private (RectTransform, TextMeshProUGUI, UITextInput) CreateInput(string title, string label, float y, float inputWidth = DefaultInputWidth, float inputCenterX = DefaultInputCenterX)
        {
            var input = UI.AddTextInput(_cameraMovementPackageExportMenu.transform, title, label, "", value =>
            {
                UpdatePreview();
            });
            LayoutInputRow(input, y, inputWidth, inputCenterX);
            return input;
        }

        private static void LayoutInputRow((RectTransform, TextMeshProUGUI, UITextInput) input, float y, float inputWidth = DefaultInputWidth, float inputCenterX = DefaultInputCenterX)
        {
            input.Item2.fontSize = LabelFontSize;
            input.Item2.enableAutoSizing = false;
            UI.MoveTransform(input.Item1, LeftLabelWidth, 16, 0, 1, LeftLabelCenterX, y);
            UI.MoveTransform(input.Item3.transform, inputWidth, 20, 0, 1, inputCenterX, y);
            input.Item1.SetAsLastSibling();
        }

        private static void LayoutDropdownRow((RectTransform, TextMeshProUGUI) label, UIDropdown dropdown, float y, float dropdownWidth, float dropdownCenterX)
        {
            label.Item2.fontSize = LabelFontSize;
            label.Item2.enableAutoSizing = false;
            UI.MoveTransform(label.Item1, LeftLabelWidth, 16, 0, 1, LeftLabelCenterX, y);
            UI.MoveTransform(dropdown.transform, dropdownWidth, 20, 0, 1, dropdownCenterX, y);
            label.Item1.SetAsLastSibling();
        }

        private static int GetNamingModeIndex(string namingMode)
        {
            return namingMode == PackageExportController.NamingModeCustom ? 1 : 0;
        }

        private static string GetNamingModeByIndex(int index)
        {
            return index == 1 ? PackageExportController.NamingModeCustom : PackageExportController.NamingModeDefault;
        }

        private static int GetPackagingModeIndex(string packagingMode)
        {
            switch (packagingMode)
            {
                case PackageExportController.PackagingFolderSongScriptJson:
                    return 0;
                case PackageExportController.PackagingFlatRenameJson:
                    return 1;
                default:
                    return 2;
            }
        }

        private static string GetPackagingModeByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return PackageExportController.PackagingFolderSongScriptJson;
                case 1:
                    return PackageExportController.PackagingFlatRenameJson;
                default:
                    return PackageExportController.PackagingFolderKeepOriginalJson;
            }
        }
    }
}
