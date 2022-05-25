using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ChroMapper_CameraMovement.UserInterface;

namespace ChroMapper_CameraMovement.Component
{
    public class DefaultCameraController : MonoBehaviour
    {
        public InputAction defaultActiveAction;
        public InputAction elevateActiveAction;
        public InputAction rotActiveAction;
        public InputAction moveActiveAction;
        public InputAction scrollActiveAction;
        public CustomStandaloneInputModule customStandaloneInputModule;
        public bool canDefaultCamera = false;
        public bool disableMainDefaultCamera = false;
        public float mouseX;
        public float mouseY;
        public float x;
        public float y;
        public float z;
        public Camera[] targetCamera { get; set; } = { null, null, null };
        private static readonly Type[] actionMapsDisableDefaultCamera = { typeof(CMInput.ICameraActions) };
        private static readonly Type[] actionMapsDisableTimeLine = { typeof(CMInput.ITimelineActions) };

        private void Start()
        {
            //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/ActionBindings.html
            defaultActiveAction = new InputAction("DefaultCamera Active");
            defaultActiveAction.AddBinding("<Mouse>/rightButton");
            defaultActiveAction.started += OnDefaultActive;
            defaultActiveAction.performed += OnDefaultActive;
            defaultActiveAction.canceled += OnDefaultActive;
            defaultActiveAction.Enable();
            elevateActiveAction = new InputAction("DefaultCamera Elevate Action");
            elevateActiveAction.AddCompositeBinding("1DAxis")
                .With("positive", "<Keyboard>/space")
                .With("negative", "<Keyboard>/ctrl");
            elevateActiveAction.started += OnElevateCamera;
            elevateActiveAction.performed += OnElevateCamera;
            elevateActiveAction.canceled += OnElevateCamera;
            moveActiveAction = new InputAction("DefaultCamera Move Action");
            moveActiveAction.AddCompositeBinding("2DVector(mode=2)")
                .With("up", "<Keyboard>/w")
                .With("left", "<Keyboard>/a")
                .With("down", "<Keyboard>/s")
                .With("right", "<Keyboard>/d");
            moveActiveAction.started += OnMoveCamera;
            moveActiveAction.performed += OnMoveCamera;
            moveActiveAction.canceled += OnMoveCamera;
            rotActiveAction = new InputAction("DefaultCamera Rotate Action");
            rotActiveAction.AddBinding("<Mouse>/delta");
            rotActiveAction.started += OnRotateCamera;
            rotActiveAction.performed += OnRotateCamera;
            rotActiveAction.canceled += OnRotateCamera;
            scrollActiveAction = new InputAction("DefaultCamera Scroll Action");
            customStandaloneInputModule = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
            //キーバンドはdefaultの物を取りたい
        }
        private void LateUpdate()
        {
            if (PauseManager.IsPaused || SceneTransitionManager.IsLoading)
                return;
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (MultiDisplayController.activeWindowNumber == 0)
            {
                if (disableMainDefaultCamera)
                {
                    UI.EnableAction(actionMapsDisableDefaultCamera);
                    disableMainDefaultCamera = false;
                }
            }
            else if (MultiDisplayController.activeWindowNumber > 0)
            {
                if (!disableMainDefaultCamera)
                {
                    UI.DisableAction(actionMapsDisableDefaultCamera);
                    disableMainDefaultCamera = true;
                }
            }
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
                eulerAngles.z = 0;
                targetCamera[MultiDisplayController.activeWindowNumber].transform.eulerAngles = eulerAngles;
            }
            else
            {
                z = x = 0;
            }
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
                Plugin.orbitCamera.orbitActiveAction.Disable();
                Plugin.plusCamera.plusActiveAction.Disable();
                rotActiveAction.Enable();
                elevateActiveAction.Enable();
                moveActiveAction.Enable();
                scrollActiveAction.Enable();
            }
            else
            {
                scrollActiveAction.Disable();
                moveActiveAction.Disable();
                elevateActiveAction.Disable();
                rotActiveAction.Disable();
                Plugin.orbitCamera.orbitActiveAction.Enable();
                Plugin.plusCamera.plusActiveAction.Enable();
                UI.EnableAction(actionMapsDisableTimeLine);
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
    }
}
