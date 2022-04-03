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
    public class OrbitCameraController : MonoBehaviour
    {
        public InputAction orbitActiveAction;
        public InputAction orbitSubActiveAction;
        public InputAction moveActiveAction;
        public InputAction rotActiveAction;
        public InputAction mouseMoveAction;
        public InputAction zoomActiveAction;
        public CustomStandaloneInputModule customStandaloneInputModule;
        public GameObject targetPosObject;
        public bool canOrbitCamera;
        public bool canOrbitSubCamera;
        public bool canMoveCamera;
        public bool canRotCamera;
        public float mouseX;
        public float mouseY;
        public Camera targetCamera { get; set; } = null;
        public GameObject targetObject { get; set; } = null;
        public Vector3 offset { get; set; } = Vector3.zero;
        public float distance { get; set; } = 5f;
        public float azimuthalAngle { get; set; } = 90f;
        public float elevationAngle { get; set; } = 90f;
        public float minElevationAngle { get; set; } = 0.01f;
        public float maxElevationAngle { get; set; } = 179.99f;

        private static readonly Type[] actionMapsEnabledWhenNodeEditing =
        {
            typeof(CMInput.ISavingActions)
        };

        private static Type[] actionMapsDisabled => typeof(CMInput).GetNestedTypes()
            .Where(x => x.IsInterface && !actionMapsEnabledWhenNodeEditing.Contains(x)).ToArray();

        private void Start()
        {
            orbitActiveAction = new InputAction("Orbit Active");
            orbitActiveAction.AddBinding(Options.Instance.orbitActiveKeyBinding);
            orbitActiveAction.started += OnOrbitActive;
            orbitActiveAction.performed += OnOrbitActive;
            orbitActiveAction.canceled += OnOrbitActive;
            orbitActiveAction.Enable();
            orbitSubActiveAction = new InputAction("Orbit Sub Active");
            orbitSubActiveAction.AddBinding(Options.Instance.orbitSubActiveKeyBinding);
            orbitSubActiveAction.started += OnOrbitSubActive;
            orbitSubActiveAction.performed += OnOrbitSubActive;
            orbitSubActiveAction.canceled += OnOrbitSubActive;
            moveActiveAction = new InputAction("Orbit Move Active");
            moveActiveAction.AddBinding(Options.Instance.orbitMoveActiveKeyBinding);
            moveActiveAction.started += OnMoveActive;
            moveActiveAction.performed += OnMoveActive;
            moveActiveAction.canceled += OnMoveActive;
            rotActiveAction = new InputAction("Orbit Rot Active");
            rotActiveAction.AddBinding(Options.Instance.orbitRotActiveKeyBinding);
            rotActiveAction.started += OnRotActive;
            rotActiveAction.performed += OnRotActive;
            rotActiveAction.canceled += OnRotActive;
            mouseMoveAction = new InputAction("Orbit Mouse Move");
            mouseMoveAction.AddBinding("<Mouse>/delta");
            mouseMoveAction.started += OnMoveAction;
            mouseMoveAction.performed += OnMoveAction;
            mouseMoveAction.canceled += OnMoveAction;
            zoomActiveAction = new InputAction("Orbit Zoom Active");
            zoomActiveAction.AddBinding("<Mouse>/scroll/y");
            zoomActiveAction.started += OnZoomActive;
            zoomActiveAction.performed += OnZoomActive;
            zoomActiveAction.canceled += OnZoomActive;
            customStandaloneInputModule = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
            targetPosObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            targetPosObject.SetActive(false);
            targetPosObject.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        }
        private void LateUpdate()
        {
            if (!canOrbitCamera) return;
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (targetCamera == null || targetObject == null) return;
            if (canRotCamera && canMoveCamera)
            {
                if (canOrbitSubCamera)
                    Settings.Instance.CameraFOV = Options.Instance.orbitDefaultFOV;
                else
                    offset = Vector3.zero;
            }
            if (canRotCamera)
            {
                azimuthalAngle = Mathf.Repeat(azimuthalAngle - mouseX * Options.Instance.orbitRotSensitivity, 360);
                elevationAngle = Mathf.Clamp(elevationAngle + mouseY * Options.Instance.orbitRotSensitivity, minElevationAngle, maxElevationAngle);
            }
            if (canMoveCamera)
            {
                var inputHorizontal = mouseX * Options.Instance.orbitOffsetSensitivity;
                var moveForward = targetCamera.transform.right * inputHorizontal;
                offset -= moveForward;
                offset = new Vector3(offset.x, offset.y - mouseY * Options.Instance.orbitOffsetSensitivity, offset.z);
            }
            var lookAtPos = targetObject.transform.position + offset;
            var da = azimuthalAngle * Mathf.Deg2Rad;
            var de = elevationAngle * Mathf.Deg2Rad;
            targetCamera.transform.position = new Vector3(
                lookAtPos.x + distance * Mathf.Sin(de) * Mathf.Cos(da),
                lookAtPos.y + distance * Mathf.Cos(de),
                lookAtPos.z + distance * Mathf.Sin(de) * Mathf.Sin(da)
            );
            targetCamera.transform.LookAt(lookAtPos);
            targetPosObject.transform.localPosition = lookAtPos;
        }

        public void OnOrbitActive(InputAction.CallbackContext context)
        {
            if (!UI.keyDisable) return;
            if (targetCamera == null || targetObject == null) return;
            canOrbitCamera = context.performed;
            if (canOrbitCamera)
            {
                distance = Vector3.Distance(targetObject.transform.position, targetCamera.transform.position);
                var planeCamera = Vector3.ProjectOnPlane(targetCamera.transform.position, Vector3.up);
                var planeTarget = Vector3.ProjectOnPlane(targetObject.transform.position, Vector3.up);
                var diff = planeCamera - planeTarget;
                var axis = Vector3.Cross(Vector3.forward, diff);
                azimuthalAngle = Vector3.Angle(Vector3.forward, diff);
                if (axis.y < 0)
                    azimuthalAngle += 90;
                else
                {
                    if (azimuthalAngle <= 90)
                        azimuthalAngle = 90 - azimuthalAngle;
                    else
                        azimuthalAngle = 360 - azimuthalAngle + 90;
                }
                diff = targetCamera.transform.position - targetObject.transform.position;
                elevationAngle = Mathf.Clamp(Vector3.Angle(Vector3.up, diff), minElevationAngle, maxElevationAngle);
                Plugin.movement.KeyDisable();
                UI.DisableAction(actionMapsDisabled);
                orbitSubActiveAction.Enable();
                moveActiveAction.Enable();
                rotActiveAction.Enable();
                zoomActiveAction.Enable();
                mouseMoveAction.Enable();
                targetPosObject.SetActive(true);
            }
            else
            {
                orbitSubActiveAction.Disable();
                moveActiveAction.Disable();
                rotActiveAction.Disable();
                zoomActiveAction.Disable();
                mouseMoveAction.Disable();
                UI.EnableAction(actionMapsDisabled);
                Plugin.movement.KeyEnable();
                canMoveCamera = false;
                canRotCamera = false;
                canOrbitSubCamera = false;
                targetPosObject.SetActive(false);
            }
        }
        public void OnOrbitSubActive(InputAction.CallbackContext context)
        {
            canOrbitSubCamera = context.performed;
        }
        public void OnMoveActive(InputAction.CallbackContext context)
        {
            canMoveCamera = context.performed;
        }
        public void OnRotActive(InputAction.CallbackContext context)
        {
            canRotCamera = context.performed;
        }
        public void OnMoveAction(InputAction.CallbackContext context)
        {
            var deltaMouseMovement = context.ReadValue<Vector2>();
            mouseX = deltaMouseMovement.x;
            mouseY = deltaMouseMovement.y;
        }
        public void OnZoomActive(InputAction.CallbackContext context)
        {
            if (!canOrbitCamera) return;
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (targetCamera == null || targetObject == null) return;
            if (canOrbitSubCamera)
            {
                var value = context.ReadValue<float>();
                Settings.Instance.CameraFOV -= value * Options.Instance.orbitFovSensitivity;
            }
            else
            {
                var value = context.ReadValue<float>() * Options.Instance.orbitZoomSensitivity;
                value = distance - value;
                distance = Mathf.Clamp(value, Options.Instance.orbitMinDistance, Options.Instance.orbitMaxDistance);
            }
            if (customStandaloneInputModule.IsPointerOverGameObject<GraphicRaycaster>(-1, true)) return;
            if (!context.performed) return;
        }
    }
}
