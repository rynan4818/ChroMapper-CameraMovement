using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Component
{
    public class DefaultCameraController : MonoBehaviour
    {
        public InputAction defaultActiveAction;
        public InputAction elevateActiveAction;
        public InputAction rotActiveAction;
        public InputAction moveActiveAction;
        public InputAction zRotResetActiveAction;
        public CustomStandaloneInputModule customStandaloneInputModule;
        public bool canDefaultCamera = false;
        public bool zRotReset = false;
        public float mouseX;
        public float mouseY;
        public float x;
        public float y;
        public float z;
        public Camera[] targetCamera { get; set; } = { null, null, null };
        private static readonly Type[] actionMapsDisableTimeLine = { typeof(CMInput.ITimelineActions), typeof(CMInput.IPlaybackActions) };

        private void Start()
        {
            //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/ActionBindings.html
            defaultActiveAction = new InputAction("DefaultCamera Active");
            defaultActiveAction.AddBinding(Options.Instance.defaultCameraActiveKeyBinding);
            defaultActiveAction.started += OnDefaultActive;
            defaultActiveAction.performed += OnDefaultActive;
            defaultActiveAction.canceled += OnDefaultActive;
            defaultActiveAction.Enable();
            elevateActiveAction = new InputAction("DefaultCamera Elevate Action");
            elevateActiveAction.AddCompositeBinding("1DAxis")
                .With("positive", Options.Instance.defaultCameraElevatePositiveKeyBinding)
                .With("negative", Options.Instance.defaultCameraElevateNegativeKeyBinding);
            elevateActiveAction.started += OnElevateCamera;
            elevateActiveAction.performed += OnElevateCamera;
            elevateActiveAction.canceled += OnElevateCamera;
            moveActiveAction = new InputAction("DefaultCamera Move Action");
            moveActiveAction.AddCompositeBinding("2DVector(mode=2)")
                .With("up", Options.Instance.defaultCameraMoveUpKeyBinding)
                .With("left", Options.Instance.defaultCameraMoveLeftKeyBinding)
                .With("down", Options.Instance.defaultCameraMoveDownKeyBinding)
                .With("right", Options.Instance.defaultCameraMoveRightKeyBinding);
            moveActiveAction.started += OnMoveCamera;
            moveActiveAction.performed += OnMoveCamera;
            moveActiveAction.canceled += OnMoveCamera;
            rotActiveAction = new InputAction("DefaultCamera Rotate Action");
            rotActiveAction.AddBinding("<Mouse>/delta");
            rotActiveAction.started += OnRotateCamera;
            rotActiveAction.performed += OnRotateCamera;
            rotActiveAction.canceled += OnRotateCamera;
            zRotResetActiveAction = new InputAction("DefaultCamera ZrotReset Action");
            zRotResetActiveAction.AddBinding(Options.Instance.defaultCameraZrotResetKeyBinding);
            zRotResetActiveAction.started += OnZrotResetActive;
            zRotResetActiveAction.performed += OnZrotResetActive;
            zRotResetActiveAction.canceled += OnZrotResetActive;
            customStandaloneInputModule = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
        }
        private void LateUpdate()
        {
            if (PauseManager.IsPaused || SceneTransitionManager.IsLoading)
                return;
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (MultiDisplayController.activeWindowNumber == -1) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;
            if (canDefaultCamera)
            {
                var movementSpeedInFrame = Settings.Instance.Camera_MovementSpeed * Time.deltaTime;
                var sideTranslation = movementSpeedInFrame * new Vector3(x, 0, z);
                targetCamera[MultiDisplayController.activeWindowNumber].transform.Translate(sideTranslation);
                targetCamera[MultiDisplayController.activeWindowNumber].transform.Translate(movementSpeedInFrame * y * Vector3.up, Space.World);
                var eulerAngles = targetCamera[MultiDisplayController.activeWindowNumber].transform.eulerAngles;
                var ex = eulerAngles.x;
                ex = ex > 180 ? ex - 360 : ex;
                eulerAngles.x = Mathf.Clamp(ex + -mouseY, -89.5f, 89.5f);
                eulerAngles.y += mouseX;
                if (zRotReset || Options.Instance.defaultCameraZrotReset)
                    eulerAngles.z = 0;
                targetCamera[MultiDisplayController.activeWindowNumber].transform.eulerAngles = eulerAngles;
            }
            else
                z = x = 0;
        }
        public void OnDefaultActive(InputAction.CallbackContext context)
        {
            if (!UI.keyDisable) return;
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (MultiDisplayController.activeWindowNumber == -1) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            canDefaultCamera = context.performed;
            if (canDefaultCamera)
            {
                UI.DisableAction(actionMapsDisableTimeLine);
                CameraMovementController.orbitCamera.orbitActiveAction.Disable();
                CameraMovementController.plusCamera.plusActiveAction.Disable();
                rotActiveAction.Enable();
                elevateActiveAction.Enable();
                moveActiveAction.Enable();
                zRotResetActiveAction.Enable();
                UI.SetLockState(true);
            }
            else
            {
                zRotResetActiveAction.Disable();
                moveActiveAction.Disable();
                elevateActiveAction.Disable();
                rotActiveAction.Disable();
                CameraMovementController.orbitCamera.orbitActiveAction.Enable();
                CameraMovementController.plusCamera.plusActiveAction.Enable();
                UI.EnableAction(actionMapsDisableTimeLine);
                UI.SetLockState(false);
            }
        }
        public void OnMoveCamera(InputAction.CallbackContext context)
        {
            var move = context.ReadValue<Vector2>();
            x = move.x;
            z = move.y;
        }
        public void OnElevateCamera(InputAction.CallbackContext context)
        {
            y = context.ReadValue<float>();
        }

        public void OnRotateCamera(InputAction.CallbackContext context)
        {
            if (!canDefaultCamera) return;

            var deltaMouseMovement = context.ReadValue<Vector2>();
            mouseX = deltaMouseMovement.x * Settings.Instance.Camera_MouseSensitivity / 10f;
            mouseY = deltaMouseMovement.y * Settings.Instance.Camera_MouseSensitivity / 10f;
        }
        public void OnZrotResetActive(InputAction.CallbackContext context)
        {
            zRotReset = context.performed;
        }
    }
}
