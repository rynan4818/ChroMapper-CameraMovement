using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.Util;

namespace ChroMapper_CameraMovement.Controller
{
    public class ScriptMapperController
    {
        private static ScriptMapperController instance;
        public bool _scriptMapperAlive = false;
        public Coroutine _scriptMapperCoroutine;
        public Process _scriptMapperProcess;
        public event Action ScriptMapperFinished;
        public static ScriptMapperController Instance
        {
            get
            {
                if (instance is null)
                    instance = new ScriptMapperController();
                return instance;
            }
        }

        public void ScriptMapperRun()
        {
            if (Options.Instance.scriptMapperCSharp)
            {
                _ = ScriptMapperCSharpController.Instance.ScriptMapperRun();
                return;
            }
            var path = CameraMovementController.MapGet();
            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogError("No map data!");
                return;
            }
            var scriptmapper = Path.Combine(Environment.CurrentDirectory, Options.Instance.scriptMapperExe);
            if (!File.Exists(scriptmapper))
            {
                UnityEngine.Debug.LogError("ScriptMapper is not installed!");
                return;
            }
            if (_scriptMapperAlive)
                return;
            _scriptMapperCoroutine = CoroutineStarter.Instance.StartCoroutine(ScriptMapperCoroutine(path, scriptmapper));
        }
        public IEnumerator ScriptMapperCoroutine(string path, string scriptmapper)
        {
            _scriptMapperAlive = true;
            CameraMovementController.autoSave.Save();
            yield return new WaitWhile(() => CameraMovementController.autoSave.IsSaving);
            var si = new ProcessStartInfo
            {
                FileName = scriptmapper,
                Arguments = $@"""{path}""",
                RedirectStandardError = false,
                RedirectStandardOutput = false,
                UseShellExecute = true
            };
            using (_scriptMapperProcess = new Process())
            {
                _scriptMapperProcess.EnableRaisingEvents = false;
                _scriptMapperProcess.PriorityBoostEnabled = true;
                _scriptMapperProcess.StartInfo = si;
                Task.Run(() =>
                {
                    _scriptMapperProcess.Start();
                    _scriptMapperProcess.WaitForExit();
                    _scriptMapperProcess.Close();
                });
                var startProcessTimeout = new TimeoutTimer(10);
                yield return new WaitUntil(() => IsProcessRunning(_scriptMapperProcess) || startProcessTimeout.HasTimedOut);
                startProcessTimeout.Stop();
                var timeout = new TimeoutTimer(45);
                yield return new WaitUntil(() => !IsProcessRunning(_scriptMapperProcess) || timeout.HasTimedOut);
                timeout.Stop();
                ScriptMapperFinished?.Invoke();
                DisposeProcess(_scriptMapperProcess);
            }
            _scriptMapperProcess = null;
            var err = false;
            var logfile = Path.Combine(BeatSaberSongContainer.Instance.Info.Directory, Options.Instance.scriptMapperLog).Replace("/", "\\");
            if (File.Exists(logfile))
            {
                using (StreamReader results = File.OpenText(logfile))
                {
                    string text = "";
                    while ((text = results.ReadLine()) != null)
                    {
                        if (Regex.IsMatch(text, @"^!.+!$"))
                        {
                            UnityEngine.Debug.LogError($"ScriptMapperWarning:{text}");
                            err = true;
                        }
                    }
                    results.Close();
                }
            }
            else
            {
                UnityEngine.Debug.LogError("ScriptMapper No Logfile");
                yield break;
            }
            if (err)
                PersistentUI.Instance.DisplayMessage("ScriptMapper ERROR!", PersistentUI.DisplayMessageType.Center);
            Plugin.movement.Reload();
            _scriptMapperAlive = false;
        }

        public static void DisposeProcess(Process process)
        {
            if (process == null)
            {
                return;
            }
            int processId;
            try
            {
                processId = process.Id;
            }
            catch (Exception)
            {
                return;
            }
            UnityEngine.Debug.Log($"[{processId}] Cleaning up process");
            if (!process.HasExited)
                process.Kill();
            process.Dispose();
        }
        public static bool IsProcessRunning(Process process)
        {
            try
            {
                if (!process.HasExited)
                    return true;
            }
            catch (Exception e)
            {
                if (!(e is InvalidOperationException))
                {
                    UnityEngine.Debug.LogWarning(e);
                }
            }
            return false;
        }
    }
}
