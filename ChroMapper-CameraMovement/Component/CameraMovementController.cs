﻿using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using ChroMapper_CameraMovement.HarmonyPatches;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.CameraPlus;
using ChroMapper_CameraMovement.Controller;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Util;
using System.Collections.Generic;

namespace ChroMapper_CameraMovement.Component
{
    public class CameraMovementController : MonoBehaviour
    {
        public const int mappingLayer = 11;
        public const int avatarLayer = 25;
        public const int bookmarkLinesLayer = 26;
        public const int subCameraArrowSphereLayer = 27;
        public CameraMovement _cameraMovement { get; set; }
        public BookmarkController _bookmarkController { get; set; }
        public static AudioTimeSyncController atsc;
        public static AutoSaveController autoSave;
        public static SpectrogramSideSwapper spectrogramSideSwapper;
        public static BookmarkLinesController bookmarkLinesController;
        public static VRMAvatarController vrmAvatarController;
        public static MultiDisplayController multiDisplayController;
        public static OrbitCameraController orbitCamera;
        public static PlusCameraController plusCamera;
        public static DefaultCameraController defaultCamera;
        public static MovementPlayerController movementPlayerController;
        public static MovementPlayerOptions movementPlayerOptions;
        public static NalulunaAvatarsEventController nalulunaAvatarsEventController;
        public static GameObject cm_MapEditorCamera;
        public static GameObject cm_UIMode;
        public static GameObject avatarHead;
        public static GameObject avatarArm;
        public static GameObject avatarBody;
        public static GameObject avatarLeg;
        public static GameObject avatarHair;
        public static GameObject bookmarkLines;
        public static GameObject bookmarkLinesCanvas;
        public static GameObject subCameraArrow;
        public static GameObject playersPlace;
        public static TrailRenderer subCameraArrowTrail;
        public static Camera subCamera;
        public static Camera layoutCamera;
        public static GameObject avatarModel;
        public static GameObject saberModel;
        public static InputAction previewAction;
        public static InputAction scriptMapperAction;
        public static InputAction dragWindowsAction;
        public static InputAction subCameraRectAction;
        public static InputAction inputFocusMoveAction;
        public static InputAction input1upAction;
        public static InputAction input1downAction;
        public static InputAction input10upAction;
        public static InputAction input10downAction;
        public static InputAction input100upAction;
        public static InputAction input100downAction;
        public static InputAction input1000upAction;
        public static InputAction input1000downAction;

        public bool _reload = false;
        public float beforeSeconds;
        public Vector3 beforePositon = Vector3.zero;
        public Quaternion beforeRotation = Quaternion.Euler(0, 0, 0);
        public float beforeFOV = Settings.Instance.CameraFOV;
        public bool waveFormIsNoteSide;
        public bool beforeWaveFormIsNoteSide;
        public GridChild eventGridChild;
        public GridChild eventLabelChild;
        public GridChild bpmChangesChild;
        public GridChild customEventsChild;
        public GridChild customEventsLabelsChild;
        public GridChild eventsGridChild;
        public GridChild bpmChangesGridChild;
        public GridChild customEventsGridChild;
        public GridChild spectrogramGridChild;
        public GridChild waveformGridChild;
        public Vector3 eventGridChildLocalOffset;
        public Vector3 eventLabelChildLocalOffset;
        public Vector3 bpmChangesChildLocalOffset;
        public Vector3 customEventsChildLocalOffset;
        public Vector3 customEventsLabelsChildLocalOffset;
        public Vector3 eventsGridChildLocalOffset;
        public Vector3 bpmChangesGridChildLocalOffset;
        public Vector3 customEventsGridChildLocalOffset;
        public Vector3 spectrogramGridChildLocalOffset;
        public Vector3 waveformGridChildLocalOffset;
        public bool customEventsObject = false;
        public string currentAvatarFile = "";
        public string currentSaberFile = "";
        public bool previewMode = false;
        public (bool, bool, bool, bool, bool, bool, Vector3, Quaternion, float) previewEve;
        public bool subCameraRectPos = true;
        public bool dragWindowKeyEnable { set; get; } = false;
        public int customAvatarLoad = 0;
        public int vrmAvatarLoad = 0;
        public int customSaberLoad = 0;
        public bool movementPlayerLoadActive;
        public float playersPlaceDefault = 0;
        public static List<(Transform, Vector3, Quaternion, Vector3)> avatarDefaultTransform { set; get; }
        public static List<(Transform, Vector3, Quaternion, Vector3)> saberDefaultTransform { set; get; }

        public static string ScriptGet()
        {
            return Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, Options.Instance.scriptFileName).Replace("/", "\\");
        }
        public static string MapGet()
        {
            return Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, BeatSaberSongContainer.Instance.DifficultyData.BeatmapFilename).Replace("/", "\\");
        }
        public void CameraPositionAndRotationSet(Vector3 position,Vector3 rotation)
        {
            position += new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset, Options.Instance.originZoffset);
            position *= Options.Instance.avatarCameraScale;
            position += new Vector3(0, Options.Instance.originMatchOffsetY, Options.Instance.originMatchOffsetZ);
            if (Options.Instance.cameraControlSub)
                subCamera.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
            else if (Options.Instance.cameraControlLay)
                layoutCamera.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
            else
                cm_MapEditorCamera.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
        }
        public void CameraFOVSet(float fov)
        {
            if (Options.Instance.cameraControlSub)
                subCamera.fieldOfView = fov;
            else if (Options.Instance.cameraControlLay)
                layoutCamera.fieldOfView = fov;
            else
                Settings.Instance.CameraFOV = fov;
        }
        public Vector3 CameraPositionGet()
        {
            GameObject targetCamera;
            if (Options.Instance.cameraControlSub)
                targetCamera = subCamera.gameObject;
            else if (Options.Instance.cameraControlLay)
                targetCamera = layoutCamera.gameObject;
            else
                targetCamera = cm_MapEditorCamera;
            var cameraPosition = new Vector3(targetCamera.transform.position.x, targetCamera.transform.position.y - Options.Instance.originMatchOffsetY, targetCamera.transform.position.z - Options.Instance.originMatchOffsetZ);
            cameraPosition /= Options.Instance.avatarCameraScale;
            cameraPosition -= new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset, Options.Instance.originZoffset);
            return cameraPosition;
        }
        public Transform CameraTransformGet()
        {
            if (Options.Instance.cameraControlSub)
                return subCamera.transform;
            else if (Options.Instance.cameraControlLay)
                return layoutCamera.transform;
            else
                return cm_MapEditorCamera.transform;
        }
        public float CameraFOVGet()
        {
            if (Options.Instance.cameraControlSub)
                return subCamera.fieldOfView;
            else if (Options.Instance.cameraControlLay)
                return layoutCamera.fieldOfView;
            else
                return Settings.Instance.CameraFOV;
        }

        public Vector3 AvatarPositionGet()
        {
            return new Vector3(Options.Instance.originXoffset, Options.Instance.avatarHeadHight + Options.Instance.originYoffset, Options.Instance.originZoffset);
        }
        public void UiHidden()
        {
            if (Options.Instance.uIhidden)
                beforeWaveFormIsNoteSide = spectrogramSideSwapper.IsNoteSide;
            Reload();
        }

        public void Reload()
        {
            UI.KeyDisableCheck();
            StartCoroutine(this.VRMAvatarLoadAsync());
            StartCoroutine(this.CustomAvatarLoad());
            StartCoroutine(this.CustomrSaberload());
            StartCoroutine(this.MovementPlayerLoad());
            if (playersPlace != null)
            {
                var a = playersPlace.transform.position;
                playersPlace.transform.position = new Vector3(a.x, playersPlaceDefault + movementPlayerOptions.playersPlaceOffset, a.z);
            }
            cm_MapEditorCamera.GetComponent<Camera>().cullingMask |= 1 << avatarLayer;
            //サンプル アリシア・ソリッドを元に 頭の高さ1.43m、大きさ0.25m  腕の長さ1.12mの時
            var avatarHeadPosition = new Vector3(Options.Instance.originXoffset, Options.Instance.avatarHeadHight + Options.Instance.originYoffset, Options.Instance.originZoffset) * Options.Instance.avatarCameraScale;
            avatarHeadPosition.y += Options.Instance.originMatchOffsetY;
            avatarHeadPosition.z += Options.Instance.originMatchOffsetZ;
            avatarHead.transform.position = avatarHeadPosition;
            avatarHead.transform.localScale = new Vector3(Options.Instance.avatarHeadSize, Options.Instance.avatarHeadSize, Options.Instance.avatarHeadSize) * Options.Instance.avatarCameraScale;
            //首の高さ 1.43-0.25÷2=1.305m
            //胴体の中心からの高さは0.3m→1.305m÷0.3m=4.35
            var body_size_y = (Options.Instance.avatarHeadHight - (Options.Instance.avatarHeadSize / 2.0f)) / 4.35f;
            //胴体の直径0.2mは胴体の中心からの高さの1/3
            var body_size_x = body_size_y / 1.5f;
            //胴体の中心1.005は首の高さ1.305m-胴体の中心からの高さ0.3m=1.005
            var body_pos_y = Options.Instance.avatarHeadHight - (Options.Instance.avatarHeadSize / 2.0f) - body_size_y;
            var avatarBodyPosition = new Vector3(Options.Instance.originXoffset, body_pos_y + Options.Instance.originYoffset, Options.Instance.originZoffset) * Options.Instance.avatarCameraScale;
            avatarBodyPosition.y += Options.Instance.originMatchOffsetY;
            avatarBodyPosition.z += Options.Instance.originMatchOffsetZ;
            avatarBody.transform.position = avatarBodyPosition;
            avatarBody.transform.localScale = new Vector3(body_size_x, body_size_y, body_size_x * 0.8f) * Options.Instance.avatarCameraScale;
            //腕の中心高さ1.25mは首の高さ1.305mの 1.305m÷1.25m=1.044
            var arm_pos_y = (Options.Instance.avatarHeadHight - (Options.Instance.avatarHeadSize / 2.0f)) / 1.044f;
            //腕の大きさ0.06mは腕の長さ1.25m→1.25÷0.06=20.83
            var arm_size_yz = Options.Instance.avatarArmSize / 20.83f;
            var avatarArmPosition = new Vector3(Options.Instance.originXoffset, arm_pos_y + Options.Instance.originYoffset, Options.Instance.originZoffset) * Options.Instance.avatarCameraScale;
            avatarArmPosition.y += Options.Instance.originMatchOffsetY;
            avatarArmPosition.z += Options.Instance.originMatchOffsetZ;
            avatarArm.transform.position = avatarArmPosition;
            avatarArm.transform.localScale = new Vector3(Options.Instance.avatarArmSize, arm_size_yz, arm_size_yz) * Options.Instance.avatarCameraScale;
            //足の高さは腕の中心から腕のサイズを引いたもの
            var leg_size_y = arm_pos_y - (arm_size_yz / 2.0f);
            var leg_pos_y = leg_size_y / 2.0f;
            var leg_size_xz = leg_size_y / 12f;
            var avatarLegPosition = new Vector3(Options.Instance.originXoffset, leg_pos_y + Options.Instance.originYoffset, Options.Instance.originZoffset) * Options.Instance.avatarCameraScale;
            avatarLegPosition.y += Options.Instance.originMatchOffsetY;
            avatarLegPosition.z += Options.Instance.originMatchOffsetZ;
            avatarLeg.transform.position = avatarLegPosition;
            avatarLeg.transform.localScale = new Vector3(leg_size_xz, leg_size_y, leg_size_xz) * Options.Instance.avatarCameraScale;
            //おさげ
            var avatarHairPosition = new Vector3(Options.Instance.originXoffset, Options.Instance.avatarHeadHight - Options.Instance.avatarHeadSize + Options.Instance.originYoffset, Options.Instance.avatarHeadSize / -2.0f + Options.Instance.originZoffset) * Options.Instance.avatarCameraScale;
            avatarHairPosition.y += Options.Instance.originMatchOffsetY;
            avatarHairPosition.z += Options.Instance.originMatchOffsetZ;
            avatarHair.transform.position = avatarHairPosition;
            avatarHair.transform.localScale = new Vector3(Options.Instance.avatarHeadSize / 1.4f, Options.Instance.avatarHeadSize * 2.0f, Options.Instance.avatarHeadSize / 17f) * Options.Instance.avatarCameraScale;
            if (Options.Instance.simpleAvatar && Options.Instance.avatar && Options.Instance.cameraMovementEnable)
            {
                avatarHead.gameObject.SetActive(true);
                avatarArm.gameObject.SetActive(true);
                avatarBody.gameObject.SetActive(true);
                avatarLeg.gameObject.SetActive(true);
                avatarHair.gameObject.SetActive(true);
            }
            else
            {
                avatarHead.gameObject.SetActive(false);
                avatarArm.gameObject.SetActive(false);
                avatarBody.gameObject.SetActive(false);
                avatarLeg.gameObject.SetActive(false);
                avatarHair.gameObject.SetActive(false);
            }
            subCameraArrow.SetActive(Options.Instance.subCameraModel && Options.Instance.cameraMovementEnable);
            subCameraArrowTrail.Clear();
            OnPlayToggle(atsc.IsPlaying);
            beforeSeconds = 0;
            _reload = true;
            _cameraMovement.LoadCameraData(ScriptGet());
            _cameraMovement.MovementPositionReset();
            _bookmarkController?.BookmarkContainerGet();
            _bookmarkController?.BookmarkWidthChange();
            _bookmarkController?.BookmarkTrackSet();
            bookmarkLines.SetActive(Options.Instance.bookmarkLines && Options.Instance.cameraMovementEnable);
            if (Options.Instance.bookmarkLines && Options.Instance.cameraMovementEnable)
                EventBpmOffset(Options.Instance.bookmarkInsertOffset);
            else
                EventBpmOffset(0);
            orbitCamera.targetCamera[0] = cm_MapEditorCamera.GetComponent<Camera>();
            orbitCamera.fovTargetMainCamera = true;
            plusCamera.targetCamera[0] = cm_MapEditorCamera.GetComponent<Camera>();
            plusCamera.fovTargetMainCamera = true;
            defaultCamera.targetCamera[0] = cm_MapEditorCamera.GetComponent<Camera>();
            if (!MultiDisplayController.subActive)
            {
                subCamera.gameObject.SetActive(!(Options.Instance.movement || !Options.Instance.subCamera) && Options.Instance.cameraMovementEnable);
                subCamera.rect = new Rect(Options.Instance.subCameraRectX, Options.Instance.subCameraRectY, Options.Instance.subCameraRectW, Options.Instance.subCameraRectH);
                if (Options.Instance.cameraKeyMouseControlSub && subCamera.gameObject.activeSelf && Options.Instance.cameraControlSub)
                {
                    orbitCamera.targetCamera[0] = subCamera.GetComponent<Camera>();
                    orbitCamera.fovTargetMainCamera = false;
                    plusCamera.targetCamera[0] = subCamera.GetComponent<Camera>();
                    plusCamera.fovTargetMainCamera = false;
                    defaultCamera.targetCamera[0] = subCamera.GetComponent<Camera>();
                }
            }
            if (Options.Instance.uIhidden && Options.Instance.cameraMovementEnable)
            {
                spectrogramSideSwapper.IsNoteSide = true;
                //Grid   Spectrogram  Note  Rotation  Event  BPM Change  CustomEvent
                //Order  -1  , 3      0     1         2      4           5
                //offset 3.5 , 2.5    0     0         0.5    0.5         1.5
                //SwapSides()は Spectrogram Grid のorder を -1 と 3、offset を 3.5f　と 2.5fで切り替える
                spectrogramSideSwapper.SwapSides();
                EventBpmOffset(200f);
                var main_camera = cm_MapEditorCamera.GetComponent<Camera>();
                main_camera.cullingMask &= ~(1 << mappingLayer);
                main_camera.cullingMask &= ~(1 << bookmarkLinesLayer);
            }
            else
            {
                spectrogramSideSwapper.IsNoteSide = !beforeWaveFormIsNoteSide;
                spectrogramSideSwapper.SwapSides();
                var main_camera = cm_MapEditorCamera.GetComponent<Camera>();
                main_camera.cullingMask |= 1 << mappingLayer;
                main_camera.cullingMask |= 1 << bookmarkLinesLayer;
            }
        }
        public void EventBpmOffset(float offset)
        {
            eventGridChild.LocalOffset = new Vector3(eventGridChildLocalOffset.x + offset, eventGridChildLocalOffset.y, eventGridChildLocalOffset.z);
            eventLabelChild.LocalOffset = new Vector3(eventLabelChildLocalOffset.x + offset, eventLabelChildLocalOffset.y, eventLabelChildLocalOffset.z);
            bpmChangesChild.LocalOffset = new Vector3(bpmChangesChildLocalOffset.x + offset, bpmChangesChildLocalOffset.y, bpmChangesChildLocalOffset.z);
            eventsGridChild.LocalOffset = new Vector3(eventsGridChildLocalOffset.x + offset, eventsGridChildLocalOffset.y, eventsGridChildLocalOffset.z);
            bpmChangesGridChild.LocalOffset = new Vector3(bpmChangesGridChildLocalOffset.x + offset, bpmChangesGridChildLocalOffset.y, bpmChangesGridChildLocalOffset.z);
            if (customEventsObject)
            {
                customEventsChild.LocalOffset = new Vector3(customEventsChildLocalOffset.x + offset, customEventsChildLocalOffset.y, customEventsChildLocalOffset.z);
                customEventsLabelsChild.LocalOffset = new Vector3(customEventsLabelsChildLocalOffset.x + offset, customEventsLabelsChildLocalOffset.y, customEventsLabelsChildLocalOffset.z);
                customEventsGridChild.LocalOffset = new Vector3(customEventsGridChildLocalOffset.x + offset, customEventsGridChildLocalOffset.y, customEventsGridChildLocalOffset.z);
            }
        }
        public void OnPlayToggle(bool playing)
        {
            if (!Options.Instance.cameraMovementEnable || !Options.Instance.subCameraModel) return;
            subCameraArrowTrail.enabled = playing;
            if (!playing)
                subCameraArrowTrail.Clear();
        }

        public void WaveFormOffset()
        {
            if (!Options.Instance.cameraMovementEnable) return;
            float offset;
            if (Options.Instance.bookmarkLines)
            {
                offset = Options.Instance.bookmarkInsertOffset;
            }
            else
            {
                offset = 0;
            }
            if (Options.Instance.uIhidden)
            {
                offset = 200f;
            }
            waveFormIsNoteSide = spectrogramSideSwapper.IsNoteSide;
            if (!waveFormIsNoteSide)
            {
                spectrogramGridChild.LocalOffset = new Vector3(spectrogramGridChildLocalOffset.x + offset, spectrogramGridChildLocalOffset.y, spectrogramGridChildLocalOffset.z);
                waveformGridChild.LocalOffset = new Vector3(waveformGridChildLocalOffset.x + offset, waveformGridChildLocalOffset.y, waveformGridChildLocalOffset.z);
            }
        }

        public IEnumerator VRMAvatarLoadAsync()
        {
            if (!Regex.IsMatch(Path.GetExtension(Options.Instance.avatarFileName), @"\.vrm$", RegexOptions.IgnoreCase) || vrmAvatarLoad == 1)
                yield break;
            vrmAvatarLoad = 1;
            customAvatarLoad = 0;
            if (avatarModel != null)
            {
                Destroy(avatarModel);
                Resources.UnloadUnusedAssets();
            }
            if (Options.Instance.customAvatar && Options.Instance.avatar && Options.Instance.cameraMovementEnable)
            {
                if (currentAvatarFile != Options.Instance.avatarFileName)
                {
                    currentAvatarFile = Options.Instance.avatarFileName;
                    var task = vrmAvatarController.LoadModelAsync();
                    yield return new WaitUntil(() => task.IsCompleted);
                    if (task.IsFaulted && task.Exception != null)
                        throw task.Exception;
                }
                else
                {
                    VRMAvatarController.SetDefaultTransform();
                    VRMAvatarController.AvatarTransformSet();
                    VRMAvatarController.AvatarEnable();
                }
            }
            else
            {
                VRMAvatarController.AvatarDisable();
            }
            if (VRMAvatarController.avatar == null)
                vrmAvatarLoad = 0;
            else
                vrmAvatarLoad = 2;
        }

        public IEnumerator CustomAvatarLoad()
        {
            if (customAvatarLoad == 1)
                yield break;
            if (Regex.IsMatch(Path.GetExtension(Options.Instance.avatarFileName), @"\.avatar$", RegexOptions.IgnoreCase) && currentAvatarFile != Options.Instance.avatarFileName && Options.Instance.customAvatar && Options.Instance.cameraMovementEnable)
            {
                customAvatarLoad = 1;
                vrmAvatarLoad = 0;
                currentAvatarFile = "";
                VRMAvatarController.AvatarDisable();
                if (avatarModel != null)
                {
                    Destroy(avatarModel);
                    Resources.UnloadUnusedAssets();
                }
                if (File.Exists(Options.Instance.avatarFileName))
                {
                    yield return UnityUtility.AssetLoadCoroutine(Options.Instance.avatarFileName, "_CustomAvatar", avatarLayer,(result, model) =>
                    {
                        if (result)
                        {
                            currentAvatarFile = Options.Instance.avatarFileName;
                            avatarModel = model;
                            avatarDefaultTransform = new List<(Transform, Vector3, Quaternion, Vector3)>
                            {
                                (avatarModel.transform, avatarModel.transform.position, avatarModel.transform.rotation, avatarModel.transform.localScale)
                            };
                            foreach (var tarns in avatarModel.GetComponentsInChildren<Transform>())
                                avatarDefaultTransform.Add((tarns, tarns.position, tarns.rotation, tarns.localScale));
                        }
                        else
                            avatarModel = null;
                    });
                }
                else
                {
                    Debug.LogError("Avatar File ERR!");
                }
            }
            if (avatarModel == null)
            {
                customAvatarLoad = 0;
            }
            else
            {
                customAvatarLoad = 2;
                foreach (var trans in avatarDefaultTransform)
                {
                    trans.Item1.position = trans.Item2;
                    trans.Item1.rotation = trans.Item3;
                    trans.Item1.localScale = trans.Item4;
                }
                avatarModel.transform.localScale = new Vector3(Options.Instance.avatarScale, Options.Instance.avatarScale, Options.Instance.avatarScale) * Options.Instance.avatarCameraScale;
                var avatarPosition = new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset + Options.Instance.avatarYoffset, Options.Instance.originZoffset) * Options.Instance.avatarCameraScale;
                avatarPosition.y = Options.Instance.originMatchOffsetY;
                avatarPosition.z = Options.Instance.originMatchOffsetZ;
                avatarModel.transform.localPosition = avatarPosition;
                if (Options.Instance.customAvatar && Options.Instance.avatar && Options.Instance.cameraMovementEnable)
                {
                    avatarModel.SetActive(true);
                }
                else
                {
                    avatarModel.SetActive(false);
                }
            }
        }

        public IEnumerator CustomrSaberload()
        {
            if (customSaberLoad == 1)
                yield break;
            if (Regex.IsMatch(Path.GetExtension(movementPlayerOptions.saberFileName), @"\.saber", RegexOptions.IgnoreCase) && currentSaberFile != movementPlayerOptions.saberFileName && Options.Instance.movementPlayer && Options.Instance.cameraMovementEnable)
            {
                customSaberLoad = 1;
                if (saberModel != null)
                {
                    Destroy(saberModel);
                    Resources.UnloadUnusedAssets();
                }
                var saberFile = movementPlayerOptions.saberFileName;
                if (File.Exists(saberFile))
                {
                    yield return UnityUtility.AssetLoadCoroutine(saberFile, "_CustomSaber", avatarLayer, (result, model) =>
                    {
                        if (result)
                        {
                            currentSaberFile = saberFile;
                            saberModel = model;
                            saberDefaultTransform = new List<(Transform, Vector3, Quaternion, Vector3)>
                            {
                                (saberModel.transform, saberModel.transform.position, saberModel.transform.rotation, saberModel.transform.localScale)
                            };
                            foreach (var tarns in saberModel.GetComponentsInChildren<Transform>())
                                saberDefaultTransform.Add((tarns, tarns.position, tarns.rotation, tarns.localScale));
                        }
                        else
                        {
                            saberModel = null;
                        }
                    });
                }
                else
                {
                    Debug.LogError("Saber File ERR!");
                }
            }
            if (saberModel == null)
            {
                customSaberLoad = 0;
            }
            else
            {
                customSaberLoad = 2;
                foreach (var trans in saberDefaultTransform)
                {
                    trans.Item1.position = trans.Item2;
                    trans.Item1.rotation = trans.Item3;
                    trans.Item1.localScale = trans.Item4;
                }
                saberModel.transform.localScale = Vector3.one * Options.Instance.avatarCameraScale;
                var saberPosition = new Vector3(Options.Instance.originXoffset, Options.Instance.originYoffset + Options.Instance.avatarYoffset, Options.Instance.originZoffset) * Options.Instance.avatarCameraScale;
                saberPosition.y = Options.Instance.originMatchOffsetY;
                saberPosition.z = Options.Instance.originMatchOffsetZ;
                saberModel.transform.localPosition = saberPosition;
                if (Options.Instance.movementPlayer && Options.Instance.cameraMovementEnable && CameraMovementController.movementPlayerOptions.saberEnabled)
                {
                    saberModel.SetActive(true);
                }
                else
                {
                    saberModel.SetActive(false);
                }
            }
        }
        public IEnumerator MovementPlayerLoad()
        {
            if (!Options.Instance.movementPlayer || !Options.Instance.cameraMovementEnable)
                yield break;
            movementPlayerLoadActive = true;
            if (vrmAvatarLoad == 1)
                yield return new WaitUntil(() => vrmAvatarLoad != 1);
            if (customAvatarLoad == 1)
                yield return new WaitUntil(() => customAvatarLoad != 1);
            if (customSaberLoad == 1)
                yield return new WaitUntil(() => customSaberLoad != 1);
            GameObject avatar = null;
            if (customAvatarLoad == 2 && avatarModel != null)
                avatar = avatarModel;
            else if (vrmAvatarLoad == 2 && VRMAvatarController.avatar != null)
                avatar = VRMAvatarController.avatar.Root;
            GameObject saber = null;
            if (customSaberLoad == 2 && saberModel != null)
                saber = saberModel;
            var task = movementPlayerController.SetMovementDataAsync(avatar, saber);
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted && task.Exception != null)
                throw task.Exception;
            movementPlayerLoadActive = false;
            movementPlayerController.MovementUpdate(atsc.CurrentSeconds);
        }

        public void OnPreview()
        {
            previewMode = !previewMode;
            if (previewMode)
            {
                previewEve = (Options.Instance.movement, Options.Instance.uIhidden, Options.Instance.bookmarkLines, Options.Instance.subCamera, Options.Instance.bookmarkEdit, Options.Instance.cameraControl,
                    cm_MapEditorCamera.transform.position, cm_MapEditorCamera.transform.rotation, Settings.Instance.CameraFOV);
                Options.Instance.movement = true;
                Options.Instance.uIhidden = false;
                Options.Instance.bookmarkLines = false;
                Options.Instance.subCamera = false;
                Options.Instance.bookmarkEdit = false;
                Options.Instance.cameraControl = false;
                Reload();
                cm_UIMode.GetComponent<UIMode>().SetUIMode(UIModeType.Preview, false);
            }
            else
            {
                Options.Instance.movement = previewEve.Item1;
                Options.Instance.uIhidden = previewEve.Item2;
                Options.Instance.bookmarkLines = previewEve.Item3;
                Options.Instance.subCamera = previewEve.Item4;
                Options.Instance.bookmarkEdit = previewEve.Item5;
                Options.Instance.cameraControl = previewEve.Item6;
                cm_MapEditorCamera.transform.position = previewEve.Item7;
                cm_MapEditorCamera.transform.rotation = previewEve.Item8;
                Settings.Instance.CameraFOV = previewEve.Item9;
                Reload();
                cm_UIMode.GetComponent<UIMode>().SetUIMode(UIModeType.Normal, false);
            }
        }

        public void OnDragWIndows(InputAction.CallbackContext context)
        {
            dragWindowKeyEnable = context.ReadValueAsButton();
        }

        public void OnSubCameraRect(InputAction.CallbackContext context)
        {
            var rect = context.ReadValue<Vector2>();
            var scale = 100;
            if (dragWindowKeyEnable)
                scale = 10;
            if (subCameraRectPos)
            {
                Options.Instance.subCameraRectX += rect.x / scale;
                if (Options.Instance.subCameraRectX > 1) Options.Instance.subCameraRectX = 1;
                if (Options.Instance.subCameraRectX < 0) Options.Instance.subCameraRectX = 0;
                Options.Instance.subCameraRectY += rect.y / scale;
                if (Options.Instance.subCameraRectY > 1) Options.Instance.subCameraRectY = 1;
                if (Options.Instance.subCameraRectY < 0) Options.Instance.subCameraRectY = 0;
            }
            else
            {
                Options.Instance.subCameraRectW += rect.x / scale;
                if (Options.Instance.subCameraRectW > 1) Options.Instance.subCameraRectW = 1;
                if (Options.Instance.subCameraRectW < 0) Options.Instance.subCameraRectW = 0;
                Options.Instance.subCameraRectH += rect.y / scale;
                if (Options.Instance.subCameraRectH > 1) Options.Instance.subCameraRectH = 1;
                if (Options.Instance.subCameraRectH < 0) Options.Instance.subCameraRectH = 0;
            }
            subCamera.rect = new Rect(Options.Instance.subCameraRectX, Options.Instance.subCameraRectY, Options.Instance.subCameraRectW, Options.Instance.subCameraRectH);
            UI._settingMenuUI.SubCameraRectSet();
        }

        public void SubCameraRectEnable (bool posEnable)
        {
            subCameraRectAction.Enable();
            subCameraRectPos = posEnable;
        }

        public void SubCameraRectDisable()
        {
            subCameraRectAction.Disable();
        }

        public bool SubCameraRectEnableGet()
        {
            return subCameraRectAction.enabled;
        }

        public void KeyDisable()
        {
            previewAction.Disable();
            scriptMapperAction.Disable();
            dragWindowsAction.Disable();
            subCameraRectAction.Disable();
            orbitCamera.orbitActiveAction.Disable();
            plusCamera.plusActiveAction.Disable();
            defaultCamera.defaultActiveAction.Disable();
            if (Options.Instance.cameraMovementEnable)
            {
                inputFocusMoveAction.Enable();
                input1upAction.Enable();
                input1downAction.Enable();
                input10upAction.Enable();
                input10downAction.Enable();
                input100upAction.Enable();
                input100downAction.Enable();
                input1000upAction.Enable();
                input1000downAction.Enable();
            }
        }

        public void KeyEnable()
        {
            previewAction.Enable();
            scriptMapperAction.Enable();
            dragWindowsAction.Enable();
            orbitCamera.orbitActiveAction.Enable();
            plusCamera.plusActiveAction.Enable();
            defaultCamera.defaultActiveAction.Enable();
            inputFocusMoveAction.Disable();
            input1upAction.Disable();
            input1downAction.Disable();
            input10upAction.Disable();
            input10downAction.Disable();
            input100upAction.Disable();
            input100downAction.Disable();
            input1000upAction.Disable();
            input1000downAction.Disable();
        }
        private IEnumerator Start()
        {
            _cameraMovement = new CameraMovement();
            atsc = FindObjectOfType<AudioTimeSyncController>();
            autoSave = FindObjectOfType<AutoSaveController>();
            spectrogramSideSwapper = FindObjectOfType<SpectrogramSideSwapper>();
            vrmAvatarController = new VRMAvatarController();
            movementPlayerController = new MovementPlayerController();
            movementPlayerOptions = MovementPlayerOptions.SettingLoad();

            orbitCamera = this.gameObject.AddComponent<OrbitCameraController>();
            plusCamera = this.gameObject.AddComponent<PlusCameraController>();
            defaultCamera = this.gameObject.AddComponent<DefaultCameraController>();

            cm_MapEditorCamera = GameObject.Find("MapEditor Camera");
            cm_UIMode = GameObject.Find("UI Mode");

            VRMAvatarController.cm_MapEditorCamera = cm_MapEditorCamera;

            avatarHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            avatarArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            avatarLeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            avatarBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            avatarHair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            avatarHead.layer = avatarLayer;
            avatarArm.layer = avatarLayer;
            avatarLeg.layer = avatarLayer;
            avatarBody.layer = avatarLayer;
            avatarHair.layer = avatarLayer;
            avatarHead.SetActive(false);
            avatarArm.SetActive(false);
            avatarLeg.SetActive(false);
            avatarBody.SetActive(false);
            avatarHair.SetActive(false);

            //BookMarkTrack構築
            var measureLines = GameObject.Find("Moveable Grid/Measure Lines");
            bookmarkLines = Instantiate(measureLines);
            var moveableGrid = GameObject.Find("Moveable Grid");
            bookmarkLines.name = "Bookmark Lines";
            bookmarkLines.transform.parent = moveableGrid.transform;
            bookmarkLines.gameObject.GetComponent<GridChild>().Order = 1;
            bookmarkLines.gameObject.GetComponent<GridChild>().LocalOffset = new Vector3(1.6f, 0, 0);
            bookmarkLinesCanvas = bookmarkLines.transform.Find("Measure Lines Canvas").gameObject;
            bookmarkLinesCanvas.name = "Bookmark Lines Canvas";
            Destroy(bookmarkLinesCanvas.gameObject.GetComponent<MeasureLinesRenderingOrderController>());
            bookmarkLinesCanvas.gameObject.GetComponent<Track>().ObjectParentTransform = bookmarkLinesCanvas.gameObject.GetComponent<RectTransform>();
            var bookmarkLinesRenderingOrderController = bookmarkLinesCanvas.AddComponent<BookmarkLinesRenderingOrderController>();
            bookmarkLinesRenderingOrderController.effectingCanvas = bookmarkLinesCanvas.gameObject.GetComponent<Canvas>();
            bookmarkLinesRenderingOrderController.bookmarkLines = bookmarkLines;
            bookmarkLinesRenderingOrderController.cm_MapEditorCamera = cm_MapEditorCamera;
            bookmarkLinesRenderingOrderController.bookmarkLinesLayer = bookmarkLinesLayer;
            bookmarkLinesRenderingOrderController.BookmakLinesCameraINIT(Options.Instance.bookmarkLinesShowOnTop);
            var bookmarkLine = bookmarkLinesCanvas.transform.Find("Measure Line").gameObject;
            bookmarkLine.name = "Bookmark Line";
            bookmarkLinesController = moveableGrid.AddComponent<BookmarkLinesController>();
            bookmarkLinesController.bookmarkLinePrefab = bookmarkLine.gameObject.GetComponent<TextMeshProUGUI>();
            bookmarkLinesController.bookmarkLinePrefab.text = "";
            bookmarkLinesController.atsc = atsc;
            bookmarkLinesController.parent = bookmarkLinesCanvas.gameObject.GetComponent<RectTransform>();
            bookmarkLinesController.frontNoteGridScaling = GameObject.Find("Rotating/Note Grid/Note Grid Front Scaling Offset").transform;
            var audioTimeSyncControllerObject = GameObject.Find("Editor/Audio Time Sync Controller");
            var audioTimeSyncController = audioTimeSyncControllerObject.gameObject.GetComponent<AudioTimeSyncController>();
            Type type = audioTimeSyncController.GetType();
            FieldInfo field = type.GetField("otherTracks", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            Track[] otherTracks = (Track[])(field.GetValue(audioTimeSyncController));
            Array.Resize(ref otherTracks, otherTracks.Length + 1);
            otherTracks[otherTracks.Length - 1] = bookmarkLinesCanvas.gameObject.GetComponent<Track>();
            field.SetValue(audioTimeSyncController, otherTracks);
            UnityUtility.AllSetLayer(measureLines, mappingLayer);
            UnityUtility.AllSetLayer(bookmarkLines, bookmarkLinesLayer);

            eventGridChild = GameObject.Find("Rotating/Event Grid").GetComponent<GridChild>();
            eventLabelChild = GameObject.Find("Rotating/Event Label").GetComponent<GridChild>();
            eventsGridChild = GameObject.Find("Moveable Grid/Events Grid").GetComponent<GridChild>();
            bpmChangesChild = GameObject.Find("Rotating/BPM Events Grid").GetComponent<GridChild>();
            bpmChangesGridChild = GameObject.Find("Moveable Grid/BPM Events Grid").GetComponent<GridChild>();
            spectrogramGridChild = GameObject.Find("Rotating/Spectrogram Grid").GetComponent<GridChild>();
            waveformGridChild = GameObject.Find("Moveable Grid/Waveform Chunks Grid").GetComponent<GridChild>();
            eventGridChildLocalOffset = eventGridChild.LocalOffset;
            eventLabelChildLocalOffset = eventLabelChild.LocalOffset;
            eventsGridChildLocalOffset = eventsGridChild.LocalOffset;
            bpmChangesChildLocalOffset = bpmChangesChild.LocalOffset;
            bpmChangesGridChildLocalOffset = bpmChangesGridChild.LocalOffset;
            spectrogramGridChildLocalOffset = spectrogramGridChild.LocalOffset;
            waveformGridChildLocalOffset = waveformGridChild.LocalOffset;

            beforeWaveFormIsNoteSide = spectrogramSideSwapper.IsNoteSide;

            subCamera = new GameObject("Preview Camera").AddComponent<Camera>();
            subCamera.clearFlags = CameraClearFlags.SolidColor;
            subCamera.backgroundColor = new Color(0, 0, 0, 255);
            subCameraArrow = new GameObject("Sub Camera Arrow");
            subCameraArrow.transform.parent = subCamera.transform;
            subCameraArrow.layer = subCameraArrowSphereLayer;
            subCameraArrowTrail = subCameraArrow.AddComponent<TrailRenderer>();
            subCameraArrowTrail.material = new Material(Shader.Find("Sprites/Default"));
            subCameraArrowTrail.material.SetColor("_Color", Color.HSVToRGB(0.2f, 1f, 1f));
            subCameraArrowTrail.startWidth = 0.1f;
            subCameraArrowTrail.endWidth = 0.02f;
            subCameraArrowTrail.time = Options.Instance.subCameraModelTrailTime;
            subCameraArrowTrail.enabled = false;

            var subCameraArrowSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            subCameraArrowSphere.layer = subCameraArrowSphereLayer;
            subCameraArrowSphere.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            subCameraArrowSphere.transform.localPosition = new Vector3(0, 0, -0.2f);
            var subCameraArrowSphereMeshRenderer = subCameraArrowSphere.GetComponent<Renderer>();
            subCameraArrowSphereMeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
            subCameraArrowSphereMeshRenderer.material.SetColor("_Color", Color.HSVToRGB(0.8f, 1f, 1f));
            subCameraArrowSphere.transform.parent = subCameraArrow.transform;

            var subCameraArrowCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            subCameraArrowCube.layer = subCameraArrowSphereLayer;
            subCameraArrowCube.transform.localScale = new Vector3(0.4f, 0.4f, 0.8f);
            subCameraArrowCube.transform.localPosition = new Vector3(0, 0, -0.6f);
            var subCameraArrowCubeMeshRenderer = subCameraArrowCube.GetComponent<Renderer>();
            subCameraArrowCubeMeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
            subCameraArrowCubeMeshRenderer.material.SetColor("_Color", Color.HSVToRGB(0.4f, 1f, 1f));
            subCameraArrowCube.transform.parent = subCameraArrow.transform;

            var subCameraArrowCube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            subCameraArrowCube2.layer = subCameraArrowSphereLayer;
            subCameraArrowCube2.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);
            subCameraArrowCube2.transform.localPosition = new Vector3(0, -0.4f, -0.6f);
            var subCameraArrowCubeMeshRenderer2 = subCameraArrowCube2.GetComponent<Renderer>();
            subCameraArrowCubeMeshRenderer2.material = new Material(Shader.Find("Sprites/Default"));
            subCameraArrowCubeMeshRenderer2.material.SetColor("_Color", Color.HSVToRGB(0.5f, 1f, 1f));
            subCameraArrowCube2.transform.parent = subCameraArrow.transform;

            if (Options.Instance.subCameraNoUI)
            {
                subCamera.cullingMask &= ~(1 << mappingLayer);
                subCamera.cullingMask &= ~(1 << bookmarkLinesLayer);
                subCamera.cullingMask &= ~(1 << subCameraArrowSphereLayer);
            }
            subCamera.gameObject.SetActive(false);
            subCameraArrow.SetActive(false);

            layoutCamera = new GameObject("Sub Camera").AddComponent<Camera>();
            layoutCamera.clearFlags = CameraClearFlags.SolidColor;
            layoutCamera.backgroundColor = new Color(0, 0, 0, 255);
            if (Options.Instance.layoutCameraNoUI)
            {
                layoutCamera.cullingMask &= ~(1 << mappingLayer);
                layoutCamera.cullingMask &= ~(1 << bookmarkLinesLayer);
                layoutCamera.cullingMask &= ~(1 << subCameraArrowSphereLayer);
            }
            layoutCamera.gameObject.SetActive(false);

            previewAction = new InputAction("Preview", binding: Options.Instance.previewKeyBinding);
            previewAction.performed += context => OnPreview();
            previewAction.Enable();
            scriptMapperAction = new InputAction("Script Mapper run", binding: Options.Instance.scriptMapperKeyBinding);
            scriptMapperAction.performed += context => ScriptMapperController.Instance.ScriptMapperRun();
            scriptMapperAction.Enable();
            dragWindowsAction = new InputAction("Drag Windows", binding: Options.Instance.dragWindowKeyBinding);
            dragWindowsAction.started += OnDragWIndows;
            dragWindowsAction.performed += OnDragWIndows;
            dragWindowsAction.canceled += OnDragWIndows;
            dragWindowsAction.Enable();
            subCameraRectAction = new InputAction("Sub Camera");
            subCameraRectAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            subCameraRectAction.performed += OnSubCameraRect;
            subCameraRectAction.Disable();
            inputFocusMoveAction = new InputAction("InputFocusMove", binding: Options.Instance.inputFocusMoveKeyBinding);
            inputFocusMoveAction.performed += UI.InputFocusMove;
            inputFocusMoveAction.canceled += UI.InputFocusMove;
            inputFocusMoveAction.Disable();
            input1upAction = new InputAction("Input1upAction", binding: Options.Instance.input1upKeyBinding);
            input1upAction.performed += context => UI.InputRound(context,1);
            input1upAction.canceled += context => UI.InputRound(context, 1);
            input1upAction.Disable();
            input1downAction = new InputAction("Input1downAction", binding: Options.Instance.input1downKeyBinding);
            input1downAction.performed += context => UI.InputRound(context, -1);
            input1downAction.canceled += context => UI.InputRound(context, -1);
            input1downAction.Disable();
            input10upAction = new InputAction("Input10upAction", binding: Options.Instance.input10upKeyBinding);
            input10upAction.performed += context => UI.InputRound(context, 10);
            input10upAction.canceled += context => UI.InputRound(context, 10);
            input10upAction.Disable();
            input10downAction = new InputAction("Input10downAction", binding: Options.Instance.input10downKeyBinding);
            input10downAction.performed += context => UI.InputRound(context, -10);
            input10downAction.canceled += context => UI.InputRound(context, -10);
            input10downAction.Disable();
            input100upAction = new InputAction("Input100upAction");
            input100upAction.AddCompositeBinding("ButtonWithOneModifier")
                .With("Modifier", Options.Instance.input100modifierKeyBinding)
                .With("Button", Options.Instance.input1upKeyBinding);
            input100upAction.performed += context => UI.InputRound(context, 100);
            input100upAction.canceled += context => UI.InputRound(context, 100);
            input100upAction.Disable();
            input100downAction = new InputAction("Input100downAction");
            input100downAction.AddCompositeBinding("ButtonWithOneModifier")
                .With("Modifier", Options.Instance.input100modifierKeyBinding)
                .With("Button", Options.Instance.input1downKeyBinding);
            input100downAction.performed += context => UI.InputRound(context, -100);
            input100downAction.canceled += context => UI.InputRound(context, -100);
            input100downAction.Disable();
            input1000upAction = new InputAction("Input1000upAction");
            input1000upAction.AddCompositeBinding("ButtonWithOneModifier")
                .With("Modifier", Options.Instance.input100modifierKeyBinding)
                .With("Button", Options.Instance.input10upKeyBinding);
            input1000upAction.performed += context => UI.InputRound(context, 1000);
            input1000upAction.canceled += context => UI.InputRound(context, 1000);
            input1000upAction.Disable();
            input1000downAction = new InputAction("Input1000downAction");
            input1000downAction.AddCompositeBinding("ButtonWithOneModifier")
                .With("Modifier", Options.Instance.input100modifierKeyBinding)
                .With("Button", Options.Instance.input10downKeyBinding);
            input1000downAction.performed += context => UI.InputRound(context, -1000);
            input1000downAction.canceled += context => UI.InputRound(context, -1000);
            input1000downAction.Disable();

            yield return new WaitForSeconds(0.5f); //待機しないとエラーになる物用

            //待機後でないとカスタムイベントオブジェクトが見つからないので、ここに置く。
            try
            {
                customEventsChild = GameObject.Find("Rotating/Custom Events Grid").GetComponent<GridChild>();
                customEventsLabelsChild = GameObject.Find("Rotating/Custom Events Grid Labels").GetComponent<GridChild>();
                customEventsGridChild = GameObject.Find("Moveable Grid/Custom Events Grid").GetComponent<GridChild>();
                customEventsChildLocalOffset = customEventsChild.LocalOffset;
                customEventsLabelsChildLocalOffset = customEventsLabelsChild.LocalOffset;
                customEventsGridChildLocalOffset = customEventsGridChild.LocalOffset;
                customEventsObject = true;
            }
            catch
            {
                Debug.LogWarning("CameraMovement:customEvents object err");
                customEventsObject = false;
            }
            nalulunaAvatarsEventController = this.gameObject.AddComponent<NalulunaAvatarsEventController>();
            playersPlace = GameObject.Find("PlayersPlace");
            if (playersPlace != null)
                playersPlaceDefault = playersPlace.transform.position.y;
            //BookmarkManagerのStart()が0.1秒待つので0.5秒待つことにする。
            _bookmarkController = new BookmarkController();
            _bookmarkController.bookmarkLinesController = bookmarkLinesController;
            _bookmarkController.atsc = atsc;
            _bookmarkController.Start();
            SpectrogramSideSwapper_SwapSidesPatch.OnSwapSides += WaveFormOffset;
            atsc.PlayToggle += OnPlayToggle;
            orbitCamera.targetObject = avatarHead;
            multiDisplayController = cm_MapEditorCamera.gameObject.AddComponent<MultiDisplayController>();
            if (MultiDisplayController.activeWindow > 1)
                multiDisplayController.SetTargetDisplay();
            KeyDisable();
        }
        private void Update()
        {
            UI.QueuedActionMaps();
            if (!Options.Instance.cameraMovementEnable) return;
            if (_reload || beforeSeconds != atsc.CurrentSeconds)
            {
                _reload = false;
                if (beforeSeconds > atsc.CurrentSeconds)
                {
                    _cameraMovement.MovementPositionReset();
                    movementPlayerController.MovementPositionReset();
                    if (VRMAvatarController.avatar != null && VRMAvatarController.avatar.Root.activeSelf && VRMAvatarController.blendShapeController != null)
                        VRMAvatarController.blendShapeController.SongTimeReset();
                    beforeSeconds = 0;
                }
                beforeSeconds = atsc.CurrentSeconds;
                _bookmarkController?.BookMarkUpdate();
                _cameraMovement.CameraUpdate(atsc.CurrentSeconds, cm_MapEditorCamera, subCamera , AvatarPositionGet());
                if (!movementPlayerLoadActive && Options.Instance.movementPlayer)
                    movementPlayerController.MovementUpdate(atsc.CurrentSeconds);
                if (VRMAvatarController.avatar != null && VRMAvatarController.avatar.Root.activeSelf && VRMAvatarController.blendShapeController != null)
                    VRMAvatarController.blendShapeController.SongTimeUpdate(atsc.CurrentSeconds);
            }
            GameObject targetCamera;
            float targetFOV;
            if (Options.Instance.cameraControlSub)
            {
                targetCamera = subCamera.gameObject;
                targetFOV = subCamera.fieldOfView;
            }
            else if (Options.Instance.cameraControlLay)
            {
                targetCamera = layoutCamera.gameObject;
                targetFOV = layoutCamera.fieldOfView;
            }
            else
            {
                targetCamera = cm_MapEditorCamera;
                targetFOV = Settings.Instance.CameraFOV;
            }
            if (beforePositon != targetCamera.transform.position || beforeRotation != targetCamera.transform.rotation || beforeFOV != targetFOV)
            {
                UI._cameraControlMenuUI.CameraPosRotUpdate();
                beforePositon = targetCamera.transform.position;
                beforeRotation = targetCamera.transform.rotation;
                beforeFOV = targetFOV;
            }
            if (!Options.Instance.cameraDirectionScrollReversal) return;
            if (Options.Instance.cameraControlSub)
            {
                if (subCamera.transform.rotation.eulerAngles.y > 90 && subCamera.transform.rotation.eulerAngles.y < 270)
                    PrecisionScrollSet(!Options.Instance.defaultInvertScrollTime);
                else
                    PrecisionScrollSet(Options.Instance.defaultInvertScrollTime);
            }
            else if (Options.Instance.cameraControlLay)
            {
                if (layoutCamera.transform.rotation.eulerAngles.y > 90 && layoutCamera.transform.rotation.eulerAngles.y < 270)
                    PrecisionScrollSet(!Options.Instance.defaultInvertScrollTime);
                else
                    PrecisionScrollSet(Options.Instance.defaultInvertScrollTime);
            }
            else
            {
                if (cm_MapEditorCamera.transform.rotation.eulerAngles.y > 90 && cm_MapEditorCamera.transform.rotation.eulerAngles.y < 270)
                    PrecisionScrollSet(!Options.Instance.defaultInvertScrollTime);
                else
                    PrecisionScrollSet(Options.Instance.defaultInvertScrollTime);
            }
        }

        private void PrecisionScrollSet(bool value)
        {
            if (Settings.Instance.InvertScrollTime == value) return;
            Settings.Instance.InvertScrollTime = value;
        }

        private void OnDestroy()
        {
            _bookmarkController.OnDestroy();
            SpectrogramSideSwapper_SwapSidesPatch.OnSwapSides -= WaveFormOffset;
            atsc.PlayToggle -= OnPlayToggle;
            previewAction.Disable();
            scriptMapperAction.Disable();
            dragWindowsAction.Disable();
            subCameraRectAction.Disable();
            if (VRMAvatarController.loadActive)
                Debug.LogWarning("VRM Avatar Load Active!");
        }

    }
}
