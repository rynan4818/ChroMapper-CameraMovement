using Beatmap.Enums;
using ChroMapper_CameraMovement.Util;
using ScriptMapper.Core;
using ScriptMapper.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ChroMapper_CameraMovement.Controller
{
    public class ScriptMapperCSharpController
    {
        private static ScriptMapperCSharpController instance;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private CancellationTokenSource _currentCts;
        public CancellationToken CancellationToken => _currentCts?.Token ?? CancellationToken.None;
        public static ScriptMapperCSharpController Instance
        {
            get
            {
                if (instance is null)
                    instance = new ScriptMapperCSharpController();
                return instance;
            }
        }
        public async Task ScriptMapperRun()
        {
            _currentCts?.Cancel();
            var dir = BeatSaberSongContainer.Instance.Info.Directory;
            var myBookmarks = new List<Bookmark>();
            var bookmarks = BeatSaberSongContainer.Instance.Map.Bookmarks;
            bookmarks.ForEach(bookmark =>
            {
                myBookmarks.Add(new Bookmark(bookmark.JsonTime, bookmark.Name));
            });

            var myBpmChanges = new List<BpmChange>();
            var bpmCollection = BeatmapObjectContainerCollection.GetCollectionForType<BPMChangeGridContainer>(ObjectType.BpmChange);
            if (bpmCollection != null)
            {
                var bpmEvents = bpmCollection.MapObjects;
                bpmEvents.ForEach(bpmEvent =>
                {
                    myBpmChanges.Add(new BpmChange { Grid = bpmEvent.JsonTime, Bpm = bpmEvent.Bpm });
                });
            }
            var defalutBpm = BeatSaberSongContainer.Instance.Info.BeatsPerMinute;
            var input = new InputData { DefaultBpm = defalutBpm, Bookmarks = myBookmarks, BpmChanges = myBpmChanges, OutputDirectory = dir, WriteOutputFile = false };
            await _semaphore.WaitAsync();
            var logger = new ScriptMapperLogger();
            var mapper = new ScriptMapperService(logger);
            try
            {
                _currentCts?.Dispose();
                _currentCts = new CancellationTokenSource();
                var token = _currentCts.Token;
                var sw = new Stopwatch();
                sw.Start();
                var result = await mapper.MapAsync(input, token);
                token.ThrowIfCancellationRequested();
                var movement = Plugin.movement;
                movement._cameraMovement.LoadCameraData(result);
                movement._reload = true;
                movement.beforeSeconds = float.MaxValue;
                await mapper.CreateFileAsync(dir, token);
                token.ThrowIfCancellationRequested();
                await logger.SaveToFileAsync(Path.Combine(dir, "log_latest.txt"));
                token.ThrowIfCancellationRequested();
                var logDir = Path.Combine(dir, "logs");
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);
                var now = DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");
                var logPath = Path.Combine(logDir, $"log_{now}.txt");
                await logger.SaveToFileAsync(logPath);
                token.ThrowIfCancellationRequested();
                sw.Stop();
            }
            catch (OperationCanceledException)
            {
                UnityEngine.Debug.Log("Script Mapper Run Cancel");
            }
            finally
            {
                logger.Dispose();
                _semaphore.Release();
            }
        }
    }
}
