using System;
using System.Linq;
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
        private static readonly Type[] actionMapsEnabledWhenNodeEditing =
        {
            typeof(CMInput.ISavingActions)
        };

        private static Type[] actionMapsDisabled => typeof(CMInput).GetNestedTypes()
            .Where(x => x.IsInterface && !actionMapsEnabledWhenNodeEditing.Contains(x)).ToArray();
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
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;
            if (canRotCamera && canPosCamera)
            {
                if (MultiDisplayController.activeWindowNumber == 0)
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

        public void OnPlusActive(InputAction.CallbackContext context)
        {
            if (!UI.keyDisable) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;
            canPlusCamera = context.performed;
            if (canPlusCamera)
            {
                Plugin.movement.KeyDisable();
                UI.DisableAction(actionMapsDisabled);
                zRotActiveAction.Enable();
                posActiveAction.Enable();
                rotActiveAction.Enable();
                moveActiveAction.Enable();
                scrollActiveAction.Enable();
            }
            else
            {
                scrollActiveAction.Disable();
                moveActiveAction.Disable();
                rotActiveAction.Disable();
                posActiveAction.Disable();
                zRotActiveAction.Disable();
                UI.EnableAction(actionMapsDisabled);
                Plugin.movement.KeyEnable();
                canZrotCamera = false;
                canPosCamera = false;
                canRotCamera = false;
            }
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
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
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
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
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
                if (MultiDisplayController.activeWindowNumber == 0)
                    Settings.Instance.CameraFOV -= value * Options.Instance.plusFovSensitivity;
                else
                    targetCamera[MultiDisplayController.activeWindowNumber].fieldOfView -= value * Options.Instance.plusFovSensitivity;
            }
            else
                camera.transform.position += camera.transform.forward * value * Options.Instance.plusZoomSensitivity;
        }
    }
}
