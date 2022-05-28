using UnityEngine;
using System;
using System.Collections;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Component
{
    public class MultiDisplayController : MonoBehaviour
    {
        public static (bool, IntPtr, int, int, string)[] windowInfos = new (bool, IntPtr, int, int, string)[3];
        public static bool subActive = false;
        public static bool createDisplayActive = false;
        public const string subWindowName = "CameraMovement Sub Camera";
        public const string layoutWindowName = "CameraMovement Layout Camera";
        public const string defaultWindowName = "Unity Secondary Display";
        public static (int, int, int, int) mainPosSize;
        public static int activeWindowNumber = -1;

        public void CreateDisplay()
        {
            if (Display.displays.Length <= 1)
                return;
            if (Plugin.activeWindow == 3)
                return;
            if (createDisplayActive)
                return;
            if (Options.Instance.subWindow || Options.Instance.layoutWindow)
                StartCoroutine("DelayWindow");
        }
        private IEnumerator DelayWindow()
        {
            createDisplayActive = true;
            mainPosSize = WindowController.getWindowPosSize(Application.productName);
            var beforeActiveWindow = Plugin.activeWindow;
            var windowName = subWindowName;
            if (!Options.Instance.subWindow && Options.Instance.layoutWindow)
                windowName = layoutWindowName;
            if (Plugin.activeWindow == 2 && windowInfos[1].Item5 != windowName)
            {
                WindowController.windowReplace(windowInfos[1].Item5, windowName, 0, 0, 0, 0, null, null);
                windowInfos[1].Item5 = windowName;
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay); //Window操作ディレイ用 
            }
            if (Plugin.activeWindow == 1)
            {
                Display.displays[1].Activate();
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                if (Options.Instance.subDisplay1Width == 0)
                    windowInfos[1] = WindowController.windowReplace(defaultWindowName, windowName, mainPosSize.Item1 + 100, mainPosSize.Item2 + 100, 0.5, false);
                else
                    windowInfos[1] = WindowController.windowReplace(defaultWindowName, windowName, (int)Options.Instance.subDisplay1PosX, (int)Options.Instance.subDisplay1PosY, (int)Options.Instance.subDisplay1Width, (int)Options.Instance.subDisplay1Height, false);
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                Plugin.activeWindow = 2;
            }
            if (Display.displays.Length > 2 && Plugin.activeWindow == 2 && Options.Instance.subWindow && Options.Instance.layoutWindow)
            {
                Display.displays[2].Activate();
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                if (Options.Instance.subDisplay2Width == 0)
                    windowInfos[2] = WindowController.windowReplace(defaultWindowName, layoutWindowName, mainPosSize.Item1 + 100, mainPosSize.Item2 + 200, 0.5, false);
                else
                    windowInfos[2] = WindowController.windowReplace(defaultWindowName, layoutWindowName, (int)Options.Instance.subDisplay2PosX, (int)Options.Instance.subDisplay2PosY, (int)Options.Instance.subDisplay2Width, (int)Options.Instance.subDisplay2Height, false);
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
                Plugin.activeWindow = 3;
            }
            if (beforeActiveWindow != Plugin.activeWindow)
            {
                if (Options.Instance.mainDisplayWidth == 0)
                    windowInfos[0] = WindowController.windowReplace(Application.productName, null, mainPosSize.Item1, mainPosSize.Item2, mainPosSize.Item3, mainPosSize.Item4, false, 1);
                else
                    windowInfos[0] = WindowController.windowReplace(Application.productName, null, (int)Options.Instance.mainDisplayPosX, (int)Options.Instance.mainDisplayPosY, (int)Options.Instance.mainDisplayWidth, (int)Options.Instance.mainDisplayHeight, false, 1);
                yield return new WaitForSeconds(Options.Instance.multiDislayCreateDelay);
            }
            SetTargetDisplay();
            createDisplayActive = false;
        }
        public void SetTargetDisplay()
        {
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
        public void SaveWindowLayout()
        {
            if (Plugin.activeWindow < 2)
                return;
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
            if (Plugin.activeWindow == 3)
            {
                var subDisplay2 = WindowController.getWindowPosSize(windowInfos[2].Item2);
                Options.Instance.subDisplay2PosX = subDisplay2.Item1;
                Options.Instance.subDisplay2PosY = subDisplay2.Item2;
                Options.Instance.subDisplay2Width = subDisplay2.Item3;
                Options.Instance.subDisplay2Height = subDisplay2.Item4;
            }
            Options.Instance.SettingSave();
        }
        public void ResetWindowLayout()
        {
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
            if (Plugin.activeWindow < 2)
                return;
            WindowController.windowReplace(windowInfos[0].Item2, null, mainPosSize.Item1, mainPosSize.Item2, mainPosSize.Item3, mainPosSize.Item4, false);
            WindowController.windowReplace(windowInfos[1].Item2, null, mainPosSize.Item1 + 100, mainPosSize.Item2 + 100, (int)(windowInfos[1].Item3 * 0.5), (int)(windowInfos[1].Item4 * 0.5), false);
            if (Plugin.activeWindow == 3)
                WindowController.windowReplace(windowInfos[2].Item2, null, mainPosSize.Item1 + 100, mainPosSize.Item2 + 300, (int)(windowInfos[2].Item3 * 0.5), (int)(windowInfos[2].Item4 * 0.5), false);
        }

        public void Update()
        {
            if (Plugin.activeWindow == 1)
            {
                activeWindowNumber = 0;
                return;
            }
            var window = WindowController.getForegroundWindowHandle();
            activeWindowNumber = -1;
            for (int i = 0; i < Plugin.activeWindow; i++)
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
