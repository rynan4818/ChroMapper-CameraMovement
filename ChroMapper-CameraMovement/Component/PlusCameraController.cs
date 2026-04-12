using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Component
{
    public class PlusCameraController : MonoBehaviour
    {
        public InputAction plusActiveAction;
        public InputAction zRotActiveAction;
        public InputAction posActiveAction;
        public InputAction rotActiveAction;
        public InputAction moveActiveAction;
        public InputAction scrollActiveAction;
        public CustomStandaloneInputModule customStandaloneInputModule;
        public bool canPlusCamera = false;
        public bool canZrotCamera = false;
        public bool canPosCamera = false;
        public bool canRotCamera = false;
        public float mouseX;
        public float mouseY;
        public Camera[] targetCamera { get; set; } = { null, null, null };
        public bool fovTargetMainCamera { get; set; } = true;
        private static readonly Type[] actionMapsDisableTimeLine = { typeof(CMInput.ITimelineActions), typeof(CMInput.IPlaybackActions) };
        private bool inputActionsDisposed;

        private void Start()
        {
            plusActiveAction = new InputAction("PlusCamera Active");
            plusActiveAction.AddBinding(Options.Instance.plusActiveKeyBinding);
            plusActiveAction.started += OnPlusActive;
            plusActiveAction.performed += OnPlusActive;
            plusActiveAction.canceled += OnPlusActive;
            plusActiveAction.Enable();
            zRotActiveAction = new InputAction("PlusCamera Z Rot Active");
            zRotActiveAction.AddBinding(Options.Instance.plusZrotActiveKeyBinding);
            zRotActiveAction.started += OnZrotActive;
            zRotActiveAction.performed += OnZrotActive;
            zRotActiveAction.canceled += OnZrotActive;
            posActiveAction = new InputAction("PlusCamera Pos Active");
            posActiveAction.AddBinding(Options.Instance.plusPosActiveKeyBinding);
            posActiveAction.started += OnPosActive;
            posActiveAction.performed += OnPosActive;
            posActiveAction.canceled += OnPosActive;
            rotActiveAction = new InputAction("PlusCamera Rot Active");
            rotActiveAction.AddBinding(Options.Instance.plusRotActiveKeyBinding);
            rotActiveAction.started += OnRotActive;
            rotActiveAction.performed += OnRotActive;
            rotActiveAction.canceled += OnRotActive;
            moveActiveAction = new InputAction("PlusCamera Move Action");
            moveActiveAction.AddBinding("<Mouse>/delta");
            moveActiveAction.started += OnMoveAction;
            moveActiveAction.performed += OnMoveAction;
            moveActiveAction.canceled += OnMoveAction;
            scrollActiveAction = new InputAction("PlusCamera Scroll Action");
            scrollActiveAction.AddBinding("<Mouse>/scroll/y");
            scrollActiveAction.started += OnScrollAction;
            scrollActiveAction.performed += OnScrollAction;
            scrollActiveAction.canceled += OnScrollAction;
            customStandaloneInputModule = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
        }
        private void LateUpdate()
        {
            if (!canPlusCamera) return;
            if (IsPointerOverEditorUI()) return;
            if (MultiDisplayController.activeWindowNumber == -1) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;
            if (canRotCamera && canPosCamera)
            {
                if (fovTargetMainCamera && MultiDisplayController.activeWindowNumber == 0)
                    Settings.Instance.CameraFOV = Options.Instance.plusDefaultFOV;
                else
                    targetCamera[MultiDisplayController.activeWindowNumber].fieldOfView = Options.Instance.plusDefaultFOV;
            }
            else if (canZrotCamera && canPosCamera)
            {
                var cameraRot = targetCamera[MultiDisplayController.activeWindowNumber].transform.eulerAngles;
                cameraRot.z = 0;
                targetCamera[MultiDisplayController.activeWindowNumber].transform.rotation = Quaternion.Euler(cameraRot);
            }
        }

        private bool IsPointerOverEditorUI()
        {
            return customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(0, true) ||
                (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject());
        }

        public void Deactivate()
        {
            var wasActive = canPlusCamera;

            canPlusCamera = false;
            canZrotCamera = false;
            canPosCamera = false;
            canRotCamera = false;
            mouseX = mouseY = 0f;

            scrollActiveAction?.Disable();
            moveActiveAction?.Disable();
            rotActiveAction?.Disable();
            posActiveAction?.Disable();
            zRotActiveAction?.Disable();

            if (Options.Instance.cameraMovementEnable)
            {
                CameraMovementController.orbitCamera?.orbitActiveAction.Enable();
                CameraMovementController.defaultCamera?.defaultActiveAction.Enable();
            }

            UI.EnableAction(typeof(PlusCameraController), actionMapsDisableTimeLine);

            if (wasActive || Cursor.lockState == CursorLockMode.Locked)
                UI.SetLockState(false);
        }

        public void Shutdown()
        {
            canPlusCamera = false;
            canZrotCamera = false;
            canPosCamera = false;
            canRotCamera = false;
            mouseX = mouseY = 0f;

            plusActiveAction?.Disable();
            scrollActiveAction?.Disable();
            moveActiveAction?.Disable();
            rotActiveAction?.Disable();
            posActiveAction?.Disable();
            zRotActiveAction?.Disable();
            UI.EnableAction(typeof(PlusCameraController), actionMapsDisableTimeLine);
        }

        public void DisposeInputActions()
        {
            if (inputActionsDisposed)
                return;

            inputActionsDisposed = true;
            Shutdown();
            DisposeInputAction(ref plusActiveAction);
            DisposeInputAction(ref zRotActiveAction);
            DisposeInputAction(ref posActiveAction);
            DisposeInputAction(ref rotActiveAction);
            DisposeInputAction(ref moveActiveAction);
            DisposeInputAction(ref scrollActiveAction);
        }

        private static void DisposeInputAction(ref InputAction action)
        {
            action?.Disable();
            action?.Dispose();
            action = null;
        }

        private void OnDestroy()
        {
            DisposeInputActions();
        }

        public void OnPlusActive(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                Deactivate();
                return;
            }
            if (!context.performed) return;
            if (UI.ShouldBlockPluginShortcut()) return;
            if (!UI.keyDisable) return;
            if (MultiDisplayController.activeWindowNumber == -1) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;
            canPlusCamera = true;
            UI.DisableAction(typeof(PlusCameraController), actionMapsDisableTimeLine);
            CameraMovementController.defaultCamera.defaultActiveAction.Disable();
            CameraMovementController.orbitCamera.orbitActiveAction.Disable();
            zRotActiveAction.Enable();
            posActiveAction.Enable();
            rotActiveAction.Enable();
            moveActiveAction.Enable();
            scrollActiveAction.Enable();
            if (MultiDisplayController.activeWindowNumber == 0)
                UI.SetLockState(true);
        }
        public void OnZrotActive(InputAction.CallbackContext context)
        {
            canZrotCamera = context.performed;
        }
        public void OnPosActive(InputAction.CallbackContext context)
        {
            canPosCamera = context.performed;
        }
        public void OnRotActive(InputAction.CallbackContext context)
        {
            canRotCamera = context.performed;
        }
        public void OnMoveAction(InputAction.CallbackContext context)
        {
            if (!canPlusCamera) return;
            if (IsPointerOverEditorUI()) return;
            if (MultiDisplayController.activeWindowNumber == -1) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;

            var deltaMouseMovement = context.ReadValue<Vector2>();
            mouseX = deltaMouseMovement.x;
            mouseY = deltaMouseMovement.y;
            var camera = targetCamera[MultiDisplayController.activeWindowNumber].gameObject;
            if (canPosCamera)
            {
                Vector3 up = camera.transform.TransformDirection(Vector3.up);
                Vector3 right = camera.transform.TransformDirection(Vector3.right);
                camera.transform.position += right * mouseX * -Options.Instance.plusPosSensitivity + up * mouseY * -Options.Instance.plusPosSensitivity;
            }
            else if (canRotCamera)
            {
                var cameraRot = camera.transform.eulerAngles;
                cameraRot.x += mouseY * -Options.Instance.plusRotSensitivity;
                cameraRot.y += mouseX * Options.Instance.plusRotSensitivity;
                camera.transform.rotation = Quaternion.Euler(cameraRot);
            }
        }
        public void OnScrollAction(InputAction.CallbackContext context)
        {
            if (!canPlusCamera) return;
            if (IsPointerOverEditorUI()) return;
            if (MultiDisplayController.activeWindowNumber == -1) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;

            var camera = targetCamera[MultiDisplayController.activeWindowNumber].gameObject;
            var value = context.ReadValue<float>();
            if (canZrotCamera)
            {
                var cameraRot = camera.transform.eulerAngles;
                cameraRot.z += value * Options.Instance.plusZrotSensitivity;
                camera.transform.rotation = Quaternion.Euler(cameraRot);
            }
            else if (canRotCamera)
            {
                if (fovTargetMainCamera && MultiDisplayController.activeWindowNumber == 0)
                    Settings.Instance.CameraFOV -= value * Options.Instance.plusFovSensitivity;
                else
                    targetCamera[MultiDisplayController.activeWindowNumber].fieldOfView -= value * Options.Instance.plusFovSensitivity;
            }
            else
                camera.transform.position += camera.transform.forward * value * Options.Instance.plusZoomSensitivity;
        }
    }
}
