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
    public class DefaultCameraController : MonoBehaviour
    {
        public InputAction defaultActiveAction;
        public InputAction zRotActiveAction;
        public InputAction posActiveAction;
        public InputAction rotActiveAction;
        public InputAction moveActiveAction;
        public InputAction scrollActiveAction;
        public CustomStandaloneInputModule customStandaloneInputModule;
        public bool canDefaultCamera = false;
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
            defaultActiveAction = new InputAction("DefaultCamera Active");
            defaultActiveAction.AddBinding(Options.Instance.plusActiveKeyBinding);
            defaultActiveAction.started += OnDefaultActive;
            defaultActiveAction.performed += OnDefaultActive;
            defaultActiveAction.canceled += OnDefaultActive;
            defaultActiveAction.Enable();
            zRotActiveAction = new InputAction("DefaultCamera Z Rot Active");
            zRotActiveAction.AddBinding(Options.Instance.plusZrotActiveKeyBinding);
            zRotActiveAction.started += OnZrotActive;
            zRotActiveAction.performed += OnZrotActive;
            zRotActiveAction.canceled += OnZrotActive;
            posActiveAction = new InputAction("DefaultCamera Pos Active");
            posActiveAction.AddBinding(Options.Instance.plusPosActiveKeyBinding);
            posActiveAction.started += OnPosActive;
            posActiveAction.performed += OnPosActive;
            posActiveAction.canceled += OnPosActive;
            rotActiveAction = new InputAction("DefaultCamera Rot Active");
            rotActiveAction.AddBinding(Options.Instance.plusRotActiveKeyBinding);
            rotActiveAction.started += OnRotActive;
            rotActiveAction.performed += OnRotActive;
            rotActiveAction.canceled += OnRotActive;
            moveActiveAction = new InputAction("DefaultCamera Move Action");
            moveActiveAction.AddBinding("<Mouse>/delta");
            moveActiveAction.started += OnMoveAction;
            moveActiveAction.performed += OnMoveAction;
            moveActiveAction.canceled += OnMoveAction;
            scrollActiveAction = new InputAction("DefaultCamera Scroll Action");
            scrollActiveAction.AddBinding("<Mouse>/scroll/y");
            scrollActiveAction.started += OnScrollAction;
            scrollActiveAction.performed += OnScrollAction;
            scrollActiveAction.canceled += OnScrollAction;
            customStandaloneInputModule = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
        }
        public void OnDefaultActive(InputAction.CallbackContext context)
        {
            if (!UI.keyDisable) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;
            canDefaultCamera = context.performed;
            if (canDefaultCamera)
            {

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
            if (!canDefaultCamera) return;
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;

            var deltaMouseMovement = context.ReadValue<Vector2>();
            mouseX = deltaMouseMovement.x;
            mouseY = deltaMouseMovement.y;
        }
        public void OnScrollAction(InputAction.CallbackContext context)
        {
            if (!canDefaultCamera) return;
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (targetCamera[MultiDisplayController.activeWindowNumber] == null) return;

        }
    }
}
