using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Component;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ChroMapper_CameraMovement.Controller
{
    public class PackageExportController
    {
        public const string NamingModeDefault = "Default";
        public const string NamingModeCustom = "Custom";
        public const string PackagingFolderKeepOriginalJson = "FolderKeepOriginalJson";
        public const string PackagingFlatRenameJson = "FlatRenameJson";
        public const string PackagingFolderSongScriptJson = "FolderSongScriptJson";
        public const string DefaultCustomFormat = "{MapId}_{SongName}_{LevelAuthorName}";

        private static readonly Regex HexIdPattern = new Regex(@"^([0-9a-fA-F]{1,6})[\s\(]", RegexOptions.Compiled);
        private static readonly Regex HexIdFallbackPattern = new Regex(@"(?:[\s_\-\]])([0-9a-fA-F]{1,6})[\s\(]", RegexOptions.Compiled);
        private static readonly Regex RawHexPattern = new Regex(@"[0-9a-fA-F]+", RegexOptions.Compiled);

        public sealed class PackageExportFormData
        {
            public string ScriptPath = "";
            public string SourceDirectory = "";
            public string OriginalFileName = "SongScript.json";
            public string MapId = "";
            public string Hash = "";
            public string CameraScriptAuthorName = "";
            public string SongName = "";
            public string SongSubName = "";
            public string SongAuthorName = "";
            public string LevelAuthorName = "";
            public string Bpm = "";
            public string Duration = "";
            public string AvatarHeight = "";
            public string Description = "";
            public string NamingMode = NamingModeDefault;
            public string CustomFormat = DefaultCustomFormat;
            public string PackagingMode = PackagingFolderKeepOriginalJson;
        }

        public sealed class PackageExportRequest
        {
            public string ScriptPath = "";
            public string SourceDirectory = "";
            public string OriginalFileName = "SongScript.json";
            public string MapId = "";
            public string Hash = "";
            public string CameraScriptAuthorName = "";
            public string SongName = "";
            public string SongSubName = "";
            public string SongAuthorName = "";
            public string LevelAuthorName = "";
            public double Bpm;
            public double Duration;
            public double? AvatarHeight;
            public string Description = "";
            public string NamingMode = NamingModeDefault;
            public string CustomFormat = DefaultCustomFormat;
            public string PackagingMode = PackagingFolderKeepOriginalJson;
        }

        private sealed class ExportItem
        {
            public string FolderName = "";
            public string FileName = "SongScript.json";
            public string JsonContent = "";
        }

        public bool TryBuildDefaultFormData(out PackageExportFormData formData, out string errorMessage)
        {
            formData = new PackageExportFormData();
            errorMessage = "";

            if (BeatSaberSongContainer.Instance == null || BeatSaberSongContainer.Instance.Info == null)
            {
                errorMessage = "No song information is loaded.";
                return false;
            }

            string scriptPath = CameraMovementController.ScriptGet();
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                errorMessage = "Could not find the current SongScript file.";
                return false;
            }

            string jsonContent;
            try
            {
                jsonContent = File.ReadAllText(scriptPath);
            }
            catch (Exception ex)
            {
                errorMessage = $"Could not read the SongScript file: {ex.Message}";
                return false;
            }

            JObject root;
            try
            {
                root = JObject.Parse(jsonContent);
            }
            catch (Exception ex)
            {
                errorMessage = $"Could not read the SongScript JSON: {ex.Message}";
                return false;
            }

            if (!(root["Movements"] is JArray))
            {
                errorMessage = "The current file is not a CameraMovement MovementScript.";
                return false;
            }

            JObject metadata = root["metadata"] as JObject;
            var info = BeatSaberSongContainer.Instance.Info;
            string folderName = Path.GetFileName(info.Directory ?? "");

            formData.ScriptPath = scriptPath;
            formData.SourceDirectory = info.Directory ?? Path.GetDirectoryName(scriptPath) ?? "";
            formData.OriginalFileName = Path.GetFileName(scriptPath);
            formData.MapId = NormalizeMapId(FirstNonEmpty(
                ReadString(metadata, "mapId"),
                ReadString(root, "mapId"),
                ExtractHexId(folderName)));
            formData.Hash = FirstNonEmpty(
                ReadString(metadata, "hash"),
                ReadString(root, "hash"));
            formData.CameraScriptAuthorName = FirstNonEmpty(
                ReadString(metadata, "cameraScriptAuthorName"),
                ReadString(root, "cameraScriptAuthorName"),
                ReadString(root, "cameraScriptAuthor"),
                ReadString(root, "songScriptAuthor"),
                Options.Instance.packageExportCameraScriptAuthorName);
            formData.SongName = FirstNonEmpty(
                ReadString(metadata, "songName"),
                ReadString(root, "songName"),
                info.SongName);
            formData.SongSubName = FirstNonEmpty(
                ReadString(metadata, "songSubName"),
                ReadString(root, "songSubName"),
                info.SongSubName);
            formData.SongAuthorName = FirstNonEmpty(
                ReadString(metadata, "songAuthorName"),
                ReadString(root, "songAuthorName"),
                info.SongAuthorName);
            formData.LevelAuthorName = FirstNonEmpty(
                ReadString(metadata, "levelAuthorName"),
                ReadString(root, "levelAuthorName"),
                info.LevelAuthorName);
            formData.Bpm = FormatDouble(FirstPositive(
                ReadDouble(metadata, "bpm"),
                ReadDouble(root, "bpm"),
                info.BeatsPerMinute));
            formData.Duration = FormatDouble(FirstPositive(
                ReadDouble(metadata, "duration"),
                ReadDouble(root, "duration"),
                BeatSaberSongContainer.Instance.LoadedSongLength,
                info.SongDurationMetadata));

            double? metadataAvatarHeight = ReadNullableDouble(metadata, "avatarHeight");
            if (!metadataAvatarHeight.HasValue)
                metadataAvatarHeight = ReadNullableDouble(root, "avatarHeight");
            formData.AvatarHeight = metadataAvatarHeight.HasValue
                ? FormatDouble(metadataAvatarHeight.Value)
                : FormatDouble(GetDefaultAvatarHeight());

            formData.Description = FirstNonEmpty(
                ReadString(metadata, "description"),
                ReadString(root, "description"));
            formData.NamingMode = NormalizeNamingMode(Options.Instance.packageExportNamingMode);
            formData.CustomFormat = string.IsNullOrWhiteSpace(Options.Instance.packageExportCustomFormat)
                ? DefaultCustomFormat
                : Options.Instance.packageExportCustomFormat;
            formData.PackagingMode = NormalizePackagingMode(Options.Instance.packageExportPackagingMode);
            return true;
        }

        public string GetRenamePreview(PackageExportFormData formData)
        {
            if (formData == null)
                return "";

            return GetConfiguredName(
                formData.NamingMode,
                formData.CustomFormat,
                formData.MapId,
                formData.SongName,
                formData.SongSubName,
                formData.SongAuthorName,
                formData.LevelAuthorName,
                formData.CameraScriptAuthorName,
                formData.OriginalFileName,
                formData.Bpm);
        }

        public bool TryCreateRequest(PackageExportFormData formData, out PackageExportRequest request, out string errorMessage)
        {
            request = null;
            errorMessage = "";

            if (formData == null)
            {
                errorMessage = "No export data is available.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(formData.ScriptPath) || !File.Exists(formData.ScriptPath))
            {
                errorMessage = "Could not find the current SongScript file.";
                return false;
            }

            double bpm;
            if (!double.TryParse(formData.Bpm, NumberStyles.Float, CultureInfo.InvariantCulture, out bpm))
            {
                errorMessage = "The BPM value is not valid.";
                return false;
            }

            double duration;
            if (!double.TryParse(formData.Duration, NumberStyles.Float, CultureInfo.InvariantCulture, out duration))
            {
                errorMessage = "The Duration value is not valid.";
                return false;
            }

            double? avatarHeight = null;
            if (!string.IsNullOrWhiteSpace(formData.AvatarHeight))
            {
                double parsedAvatarHeight;
                if (!double.TryParse(formData.AvatarHeight, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedAvatarHeight))
                {
                    errorMessage = "The AvatarHeight value is not valid.";
                    return false;
                }
                avatarHeight = parsedAvatarHeight;
            }

            request = new PackageExportRequest
            {
                ScriptPath = formData.ScriptPath,
                SourceDirectory = formData.SourceDirectory,
                OriginalFileName = formData.OriginalFileName,
                MapId = NormalizeMapId(formData.MapId),
                Hash = (formData.Hash ?? "").Trim(),
                CameraScriptAuthorName = (formData.CameraScriptAuthorName ?? "").Trim(),
                SongName = (formData.SongName ?? "").Trim(),
                SongSubName = (formData.SongSubName ?? "").Trim(),
                SongAuthorName = (formData.SongAuthorName ?? "").Trim(),
                LevelAuthorName = (formData.LevelAuthorName ?? "").Trim(),
                Bpm = bpm,
                Duration = duration,
                AvatarHeight = avatarHeight,
                Description = formData.Description ?? "",
                NamingMode = NormalizeNamingMode(formData.NamingMode),
                CustomFormat = string.IsNullOrWhiteSpace(formData.CustomFormat) ? DefaultCustomFormat : formData.CustomFormat,
                PackagingMode = NormalizePackagingMode(formData.PackagingMode)
            };
            return true;
        }

        public string GetPreviewPath(PackageExportFormData formData)
        {
            string configuredName = GetConfiguredName(
                formData.NamingMode,
                formData.CustomFormat,
                formData.MapId,
                formData.SongName,
                formData.SongSubName,
                formData.SongAuthorName,
                formData.LevelAuthorName,
                formData.CameraScriptAuthorName,
                formData.OriginalFileName,
                formData.Bpm);
            string originalFileName = Path.GetFileName(formData.OriginalFileName);
            if (string.IsNullOrWhiteSpace(originalFileName))
                originalFileName = "SongScript.json";

            switch (NormalizePackagingMode(formData.PackagingMode))
            {
                case PackagingFlatRenameJson:
                    return EnsureJsonFileName(configuredName);
                case PackagingFolderSongScriptJson:
                    return $"{configuredName}/SongScript.json";
                default:
                    return $"{configuredName}/{SanitizeFileName(originalFileName)}";
            }
        }

        public string GetDefaultZipFileName(PackageExportFormData formData)
        {
            string configuredName = GetConfiguredName(
                formData.NamingMode,
                formData.CustomFormat,
                formData.MapId,
                formData.SongName,
                formData.SongSubName,
                formData.SongAuthorName,
                formData.LevelAuthorName,
                formData.CameraScriptAuthorName,
                formData.OriginalFileName,
                formData.Bpm);
            return EnsureZipFileName(configuredName);
        }

        public string ExportToDirectory(PackageExportRequest request, string rootDirectoryPath)
        {
            ExportItem item = BuildExportItem(request);
            string outputDirectoryPath = CreateTimestampedOutputDirectory(rootDirectoryPath);
            string targetDirectory = string.IsNullOrWhiteSpace(item.FolderName)
                ? outputDirectoryPath
                : Path.Combine(outputDirectoryPath, item.FolderName);

            Directory.CreateDirectory(targetDirectory);
            string targetPath = Path.Combine(targetDirectory, item.FileName);
            File.WriteAllText(targetPath, item.JsonContent, Encoding.UTF8);
            return outputDirectoryPath;
        }

        public void ExportToZip(PackageExportRequest request, string zipFilePath)
        {
            ExportItem item = BuildExportItem(request);
            using (var zipStream = File.Create(zipFilePath))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                string entryPath = string.IsNullOrWhiteSpace(item.FolderName)
                    ? item.FileName
                    : $"{item.FolderName}/{item.FileName}";
                var entry = archive.CreateEntry(entryPath);
                using (var entryStream = entry.Open())
                using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                {
                    writer.Write(item.JsonContent);
                }
            }
        }

        public IEnumerator FetchHashCoroutine(string mapId, Action<string> setHash, Action<string> setStatus, Action<string> showError)
        {
            string normalizedMapId = NormalizeMapId(mapId);
            if (string.IsNullOrWhiteSpace(normalizedMapId))
            {
                showError?.Invoke("Map ID is empty, so the hash cannot be fetched from BeatSaver.");
                yield break;
            }

            setStatus?.Invoke($"Getting hash from BeatSaver... {normalizedMapId}");
            using (var request = UnityWebRequest.Get($"https://api.beatsaver.com/maps/id/{normalizedMapId}"))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    setStatus?.Invoke("Could not get the hash from BeatSaver.");
                    showError?.Invoke($"BeatSaver request failed: {request.error}");
                    yield break;
                }

                string hash = ExtractHashFromBeatSaverJson(request.downloadHandler.text);
                if (string.IsNullOrWhiteSpace(hash))
                {
                    setStatus?.Invoke("Could not find a hash in the BeatSaver response.");
                    showError?.Invoke("No hash was found in the BeatSaver response.");
                    yield break;
                }

                setHash?.Invoke(hash.Trim());
                setStatus?.Invoke($"Got BeatSaver hash: {hash.Trim()}");
            }
        }

        private ExportItem BuildExportItem(PackageExportRequest request)
        {
            string originalJson = File.ReadAllText(request.ScriptPath);
            string outputJson = PrepareJsonWithMetadata(originalJson, request);
            string configuredName = GetConfiguredName(
                request.NamingMode,
                request.CustomFormat,
                request.MapId,
                request.SongName,
                request.SongSubName,
                request.SongAuthorName,
                request.LevelAuthorName,
                request.CameraScriptAuthorName,
                request.OriginalFileName,
                request.Bpm.ToString(CultureInfo.InvariantCulture));
            string originalFileName = Path.GetFileName(request.OriginalFileName);
            if (string.IsNullOrWhiteSpace(originalFileName))
                originalFileName = "SongScript.json";

            switch (NormalizePackagingMode(request.PackagingMode))
            {
                case PackagingFlatRenameJson:
                    return new ExportItem
                    {
                        FolderName = "",
                        FileName = EnsureJsonFileName(configuredName),
                        JsonContent = outputJson
                    };
                case PackagingFolderSongScriptJson:
                    return new ExportItem
                    {
                        FolderName = configuredName,
                        FileName = "SongScript.json",
                        JsonContent = outputJson
                    };
                default:
                    return new ExportItem
                    {
                        FolderName = configuredName,
                        FileName = SanitizeFileName(originalFileName),
                        JsonContent = outputJson
                    };
            }
        }

        private static string PrepareJsonWithMetadata(string originalJson, PackageExportRequest request)
        {
            var root = JObject.Parse(originalJson);
            var result = new JObject();
            result["metadata"] = BuildMetadataObject(request);

            foreach (var property in root.Properties())
            {
                if (property.Name == "metadata" ||
                    property.Name == "mapId" ||
                    property.Name == "hash" ||
                    property.Name == "songScriptAuthor" ||
                    property.Name == "cameraScriptAuthor" ||
                    property.Name == "cameraScriptAuthorName")
                {
                    continue;
                }

                result[property.Name] = property.Value.DeepClone();
            }

            return result.ToString(Formatting.Indented);
        }

        private static JObject BuildMetadataObject(PackageExportRequest request)
        {
            var metadata = new JObject
            {
                ["mapId"] = request.MapId,
                ["hash"] = request.Hash,
                ["cameraScriptAuthorName"] = request.CameraScriptAuthorName,
                ["bpm"] = request.Bpm,
                ["duration"] = request.Duration,
                ["songName"] = request.SongName,
                ["songSubName"] = request.SongSubName,
                ["songAuthorName"] = request.SongAuthorName,
                ["levelAuthorName"] = request.LevelAuthorName,
                ["description"] = request.Description
            };

            if (request.AvatarHeight.HasValue)
                metadata["avatarHeight"] = request.AvatarHeight.Value;

            return metadata;
        }

        private static string ExtractHashFromBeatSaverJson(string jsonText)
        {
            try
            {
                var root = JObject.Parse(jsonText);
                var versions = root["versions"] as JArray;
                if (versions == null)
                    return "";

                foreach (var version in versions.OfType<JObject>())
                {
                    string hash = ReadString(version, "hash");
                    if (!string.IsNullOrWhiteSpace(hash))
                        return hash;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"PackageExport: BeatSaver hash parse error. {ex.Message}");
            }

            return "";
        }

        private static string CreateTimestampedOutputDirectory(string rootDirectoryPath)
        {
            string baseName = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + "_OUTPUT";
            string candidatePath = Path.Combine(rootDirectoryPath, baseName);
            int suffix = 1;

            while (Directory.Exists(candidatePath))
            {
                candidatePath = Path.Combine(rootDirectoryPath, $"{baseName}_{suffix}");
                suffix++;
            }

            Directory.CreateDirectory(candidatePath);
            return candidatePath;
        }

        private static double GetDefaultAvatarHeight()
        {
            return (Options.Instance.avatarHeadHight + (Options.Instance.avatarHeadSize / 2.0f)) * 100.0f;
        }

        private static string NormalizeNamingMode(string namingMode)
        {
            return string.Equals(namingMode, NamingModeCustom, StringComparison.OrdinalIgnoreCase)
                ? NamingModeCustom
                : NamingModeDefault;
        }

        private static string NormalizePackagingMode(string packagingMode)
        {
            if (string.Equals(packagingMode, PackagingFlatRenameJson, StringComparison.OrdinalIgnoreCase))
                return PackagingFlatRenameJson;
            if (string.Equals(packagingMode, PackagingFolderSongScriptJson, StringComparison.OrdinalIgnoreCase))
                return PackagingFolderSongScriptJson;
            return PackagingFolderKeepOriginalJson;
        }

        private static string GetConfiguredName(
            string namingMode,
            string customFormat,
            string mapId,
            string songName,
            string songSubName,
            string songAuthorName,
            string levelAuthorName,
            string cameraScriptAuthorName,
            string originalFileName,
            string bpmText)
        {
            if (NormalizeNamingMode(namingMode) == NamingModeCustom)
            {
                string format = string.IsNullOrWhiteSpace(customFormat) ? DefaultCustomFormat : customFormat;
                var tags = new Dictionary<string, string>
                {
                    { "MapId", mapId ?? "" },
                    { "SongName", songName ?? "" },
                    { "SongSubName", songSubName ?? "" },
                    { "SongAuthorName", songAuthorName ?? "" },
                    { "LevelAuthorName", levelAuthorName ?? "" },
                    { "CameraScriptAuthorName", cameraScriptAuthorName ?? "" },
                    { "FileName", Path.GetFileName(originalFileName ?? "") ?? "" },
                    { "Bpm", bpmText ?? "" }
                };

                return ReplaceTags(format, tags);
            }

            var defaultParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(mapId))
                defaultParts.Add(mapId.Trim());
            if (!string.IsNullOrWhiteSpace(songName))
                defaultParts.Add(songName.Trim());
            if (!string.IsNullOrWhiteSpace(levelAuthorName))
                defaultParts.Add(levelAuthorName.Trim());

            if (defaultParts.Count == 0)
                defaultParts.Add(Path.GetFileNameWithoutExtension(originalFileName ?? "SongScript") ?? "SongScript");

            return SanitizeFileName(string.Join("_", defaultParts));
        }

        private static string ReplaceTags(string format, Dictionary<string, string> tags)
        {
            string result = format ?? "";
            foreach (var tag in tags)
            {
                result = result.Replace($"{{{tag.Key}}}", tag.Value ?? "");
            }
            return SanitizeFileName(result);
        }

        private static string EnsureJsonFileName(string name)
        {
            string sanitized = SanitizeFileName(name);
            return sanitized.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? sanitized
                : sanitized + ".json";
        }

        private static string EnsureZipFileName(string name)
        {
            string sanitized = SanitizeFileName(name);
            return sanitized.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                ? sanitized
                : sanitized + ".zip";
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "unnamed";

            string sanitized = name;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sanitized = sanitized.Replace(c, '_');
            }

            sanitized = sanitized.Trim();
            return string.IsNullOrWhiteSpace(sanitized) ? "unnamed" : sanitized;
        }

        private static string NormalizeMapId(string mapId)
        {
            return string.IsNullOrWhiteSpace(mapId) ? "" : mapId.Trim().ToLowerInvariant();
        }

        private static string FormatDouble(double value)
        {
            return value > 0
                ? value.ToString("0.###", CultureInfo.InvariantCulture)
                : "0";
        }

        private static double FirstPositive(params double?[] values)
        {
            foreach (double? value in values)
            {
                if (value.HasValue && value.Value > 0)
                    return value.Value;
            }
            return 0;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }
            return "";
        }

        private static string ReadString(JObject obj, string propertyName)
        {
            if (obj == null)
                return "";

            JToken token = obj[propertyName];
            if (token == null || token.Type == JTokenType.Null)
                return "";

            return token.Type == JTokenType.String
                ? token.Value<string>() ?? ""
                : token.ToString();
        }

        private static double? ReadDouble(JObject obj, string propertyName)
        {
            if (obj == null)
                return null;

            JToken token = obj[propertyName];
            if (token == null || token.Type == JTokenType.Null)
                return null;

            if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
                return token.Value<double>();

            double parsedValue;
            if (double.TryParse(token.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out parsedValue))
                return parsedValue;

            return null;
        }

        private static double? ReadNullableDouble(JObject obj, string propertyName)
        {
            return ReadDouble(obj, propertyName);
        }

        private static string ExtractHexId(string nameOrPath)
        {
            if (string.IsNullOrWhiteSpace(nameOrPath))
                return "";

            Match match = HexIdPattern.Match(nameOrPath);
            if (match.Success)
                return match.Groups[1].Value.ToLowerInvariant();

            Match fallback = HexIdFallbackPattern.Match(nameOrPath);
            if (fallback.Success)
                return fallback.Groups[1].Value.ToLowerInvariant();

            Match bestMatch = FindBestHexMatch(nameOrPath);
            return bestMatch == null ? "" : bestMatch.Value.ToLowerInvariant();
        }

        private static Match FindBestHexMatch(string input)
        {
            MatchCollection matches = RawHexPattern.Matches(input);
            Match bestMatch = null;
            int bestScore = int.MaxValue;

            foreach (Match match in matches)
            {
                int length = match.Value.Length;
                if (length < 2)
                    continue;

                int score;
                if (length >= 4 && length <= 6)
                    score = 0;
                else if (length < 4)
                    score = 4 - length;
                else
                    score = length - 6;

                bool hasHexChar = match.Value.Any(c => (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
                if (!hasHexChar)
                    score += 100;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestMatch = match;
                }
            }

            return bestMatch;
        }
    }
}
