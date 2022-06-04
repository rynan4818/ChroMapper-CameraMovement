using UnityEngine;
using System;
using System.Collections;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Component
{
    public class MultiDisplayController : MonoBehaviour
    {
        public const string subWindowName = "CameraMovement Sub Camera";
        public const string layoutWindowName = "CameraMovement Layout Camera";
        public const string defaultWindowName = "Unity Secondary Display";
        public static (bool, IntPtr, int, int, string)[] windowInfos = new (bool, IntPtr, int, int, string)[3];
        public static int activeWindow = 1;
        public static bool createDisplayActive = false;
        public static int activeWindowNumber = -1;
        public static bool subActive = false;

        public void CreateDisplay()
        {
            if (Display.displays.Length <= 1)
                return;
            if (activeWindow == 3)
                return;
            if (createDisplayActive)
                return;
            if (activeWindow == 1 && Display.displays[1].active)
                return;
            if (Options.Instance.subWindow || Options.Instance.layoutWindow)
                StartCoroutine("DelayWindow");
        }
        private IEnumerator DelayWindow()
        {
            createDisplayActive = true;
            var mainPosSize = WindowController.getWindowPosSize(Application.productName);
            var beforeActiveWindow = activeWindow;
            var windowName = subWindowName;
            if (!Options.Instance.subWindow && Options.Instance.layoutWindow)
                windowName = layoutWindowName;
            if (activeWindow == 2 && windowInfos[1].Item5 != windowName)
            {
                WindowController.windowReplace(windowInfos[1].Item5, windowName, 0, 0, 0, 0, null, null);
                windowInfos[1].Item5 = windowName;
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay); //Window操作ディレイ用 
            }
            if (activeWindow == 1)
            {
                Display.displays[1].Activate();
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                windowInfos[1] = WindowController.windowReplace(defaultWindowName, windowName, mainPosSize.Item1 + 100, mainPosSize.Item2 + 100, 0.5, false);
                if (Options.Instance.subDisplay1Width != 0)
                {
                    yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                    WindowController.windowReplace(windowInfos[1].Item2, null, Options.Instance.subDisplay1PosX, Options.Instance.subDisplay1PosY, Options.Instance.subDisplay1Width, Options.Instance.subDisplay1Height, false);
                }
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                activeWindow = 2;
            }
            if (Display.displays.Length > 2 && activeWindow == 2 && Options.Instance.subWindow && Options.Instance.layoutWindow)
            {
                Display.displays[2].Activate();
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                windowInfos[2] = WindowController.windowReplace(defaultWindowName, layoutWindowName, mainPosSize.Item1 + 100, mainPosSize.Item2 + 200, 0.5, false);
                if (Options.Instance.subDisplay2Width != 0)
                {
                    yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                    WindowController.windowReplace(windowInfos[2].Item2, null, Options.Instance.subDisplay2PosX, Options.Instance.subDisplay2PosY, Options.Instance.subDisplay2Width, Options.Instance.subDisplay2Height, false);
                }
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                activeWindow = 3;
            }
            if (beforeActiveWindow != activeWindow)
            {
                if (Options.Instance.mainDisplayWidth == 0)
                    windowInfos[0] = WindowController.windowReplace(Application.productName, null, mainPosSize.Item1, mainPosSize.Item2, mainPosSize.Item3, mainPosSize.Item4, false, 1);
                else
                    windowInfos[0] = WindowController.windowReplace(Application.productName, null, Options.Instance.mainDisplayPosX, Options.Instance.mainDisplayPosY, Options.Instance.mainDisplayWidth, Options.Instance.mainDisplayHeight, false, 1);
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
            }
            SetTargetDisplay();
            createDisplayActive = false;
        }
        public void SetTargetDisplay()
        {
            if (activeWindow < 2)
                return;
            if (Options.Instance.subWindow && Options.Instance.layoutWindow)
            {
                CameraMovementController.orbitCamera.targetCamera[1] = CameraMovementController.subCamera;
                CameraMovementController.orbitCamera.targetCamera[2] = CameraMovementController.layoutCamera;
                CameraMovementController.plusCamera.targetCamera[1] = CameraMovementController.subCamera;
                CameraMovementController.plusCamera.targetCamera[2] = CameraMovementController.layoutCamera;
                CameraMovementController.defaultCamera.targetCamera[1] = CameraMovementController.subCamera;
                CameraMovementController.defaultCamera.targetCamera[2] = CameraMovementController.layoutCamera;
                CameraMovementController.subCamera.gameObject.SetActive(true);
                CameraMovementController.subCamera.targetDisplay = 1;
                CameraMovementController.subCamera.rect = new Rect(0, 0, 1, 1);
                CameraMovementController.layoutCamera.gameObject.SetActive(true);
                CameraMovementController.layoutCamera.targetDisplay = 2;
                subActive = true;
                Plugin.movement.Reload();
                return;
            }
            if (Options.Instance.subWindow)
            {
                CameraMovementController.orbitCamera.targetCamera[1] = CameraMovementController.subCamera;
                CameraMovementController.orbitCamera.targetCamera[2] = null;
                CameraMovementController.plusCamera.targetCamera[1] = CameraMovementController.subCamera;
                CameraMovementController.plusCamera.targetCamera[2] = null;
                CameraMovementController.defaultCamera.targetCamera[1] = CameraMovementController.subCamera;
                CameraMovementController.defaultCamera.targetCamera[2] = null;
                CameraMovementController.subCamera.gameObject.SetActive(true);
                CameraMovementController.subCamera.targetDisplay = 1;
                CameraMovementController.subCamera.rect = new Rect(0, 0, 1, 1);
                CameraMovementController.layoutCamera.gameObject.SetActive(false);
                subActive = true;
            }
            if (Options.Instance.layoutWindow)
            {
                CameraMovementController.orbitCamera.targetCamera[1] = CameraMovementController.layoutCamera;
                CameraMovementController.orbitCamera.targetCamera[2] = null;
                CameraMovementController.plusCamera.targetCamera[1] = CameraMovementController.layoutCamera;
                CameraMovementController.plusCamera.targetCamera[2] = null;
                CameraMovementController.defaultCamera.targetCamera[1] = CameraMovementController.layoutCamera;
                CameraMovementController.defaultCamera.targetCamera[2] = null;
                CameraMovementController.subCamera.targetDisplay = 0;
                CameraMovementController.layoutCamera.gameObject.SetActive(true);
                CameraMovementController.layoutCamera.targetDisplay = 1;
                subActive = false;
            }
            Plugin.movement.Reload();
        }
        public bool SaveWindowLayout()
        {
            if (activeWindow < 2)
                return false;
            var mainDisplay = WindowController.getWindowPosSize(Application.productName);
            Options.Instance.mainDisplayPosX = mainDisplay.Item1;
            Options.Instance.mainDisplayPosY = mainDisplay.Item2;
            Options.Instance.mainDisplayWidth = mainDisplay.Item3;
            Options.Instance.mainDisplayHeight = mainDisplay.Item4;
            var subDisplay1 = WindowController.getWindowPosSize(windowInfos[1].Item2);
            Options.Instance.subDisplay1PosX = subDisplay1.Item1;
            Options.Instance.subDisplay1PosY = subDisplay1.Item2;
            Options.Instance.subDisplay1Width = subDisplay1.Item3;
            Options.Instance.subDisplay1Height = subDisplay1.Item4;
            if (activeWindow == 3)
            {
                var subDisplay2 = WindowController.getWindowPosSize(windowInfos[2].Item2);
                Options.Instance.subDisplay2PosX = subDisplay2.Item1;
                Options.Instance.subDisplay2PosY = subDisplay2.Item2;
                Options.Instance.subDisplay2Width = subDisplay2.Item3;
                Options.Instance.subDisplay2Height = subDisplay2.Item4;
            }
            Options.Instance.SettingSave();
            return true;
        }
        public bool ResetWindowLayout()
        {
            if (activeWindow < 2)
                return false;
            Options.Instance.mainDisplayPosX = 0;
            Options.Instance.mainDisplayPosY = 0;
            Options.Instance.mainDisplayWidth = 0;
            Options.Instance.mainDisplayHeight= 0;
            Options.Instance.subDisplay1PosX = 0;
            Options.Instance.subDisplay1PosY = 0;
            Options.Instance.subDisplay1Width = 0;
            Options.Instance.subDisplay1Height = 0;
            Options.Instance.subDisplay2PosX = 0;
            Options.Instance.subDisplay2PosY = 0;
            Options.Instance.subDisplay2Width = 0;
            Options.Instance.subDisplay2Height = 0;
            var mainPosSize = WindowController.getWindowPosSize(Application.productName);
            WindowController.windowReplace(windowInfos[0].Item2, null, mainPosSize.Item1, mainPosSize.Item2, mainPosSize.Item3, mainPosSize.Item4, false);
            WindowController.windowReplace(windowInfos[1].Item2, null, mainPosSize.Item1 + 100, mainPosSize.Item2 + 100, (int)(windowInfos[1].Item3 * 0.5), (int)(windowInfos[1].Item4 * 0.5), false);
            if (activeWindow == 3)
                WindowController.windowReplace(windowInfos[2].Item2, null, mainPosSize.Item1 + 100, mainPosSize.Item2 + 300, (int)(windowInfos[2].Item3 * 0.5), (int)(windowInfos[2].Item4 * 0.5), false);
            return true;
        }
        public void Start()
        {
            activeWindowNumber = -1;
            subActive = false;
        }

        public void Update()
        {
            if (createDisplayActive)
                return;
            if (activeWindow == 1)
            {
                activeWindowNumber = 0;
                return;
            }
            var window = WindowController.getForegroundWindowHandle();
            activeWindowNumber = -1;
            for (int i = 0; i < activeWindow; i++)
            {
                if (windowInfos[i].Item2 == window)
                {
                    activeWindowNumber = i;
                    if (i > 0)
                        WindowController.windowAspectResize(windowInfos[i]);
                    break;
                }
            }
        }
    }
}
