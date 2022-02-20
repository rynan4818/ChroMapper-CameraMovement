using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.IO;
using UnityEngine;
using TMPro;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.HarmonyPatches;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.CameraPlus;

namespace ChroMapper_CameraMovement.Component
{
    public class CameraMovementController : MonoBehaviour
    {
        public CameraMovement _cameraMovement;
        private UI _ui;
        public static AudioTimeSyncController atsc;
        public static AutoSaveController autoSave;
        public static BookmarkManager bookmarkManager;
        public static SpectrogramSideSwapper spectrogramSideSwapper;
        public static EventsContainer eventContainer;
        public static BookmarkLinesController bookmarkLinesController;
        public static GameObject cm_MapEditorCamera;
        public static GameObject cm_GridX;
        public static GameObject cm_interface;
        public static GameObject cm_Grid;
        public static GameObject cm_measureGrid16_1;
        public static GameObject cm_measureGrid8_1;
        public static GameObject cm_measureGrid4_1;
        public static GameObject cm_measureGrid1;
        public static GameObject cm_NoteFrontBase;
        public static GameObject cm_NoteBackBase;
        public static GameObject cm_EventFrontBase;
        public static GameObject cm_EventBackBase;
        public static GameObject cm_baseTransparent;
        public static GameObject cm_eventLabel;
        public static GameObject cm_BPMchangeFrontBase;
        public static GameObject cm_BPMchangeBackBase;
        public static GameObject cm_BPMChangeLable;
        public static GameObject cm_MeasureLinesCanvas;
        public static GameObject avatarHead;
        public static GameObject avatarArm;
        public static GameObject avatarBody;
        public static GameObject avatarLeg;
        public static GameObject avatarHair;
        public static GameObject bookmarkLines;
        public static Camera sub_camera;
        public static GameObject avatarModel;

        public bool _reload = false;
        public float beforeSeconds;
        public float gridXBaseAlpha = 0.05f;
        public float gridXGridAlpha = 0.5f;
        public float interfaceOpacity = 0.2f;
        public float gridGridAlpha = 0.5f;
        public Color measureGridColor16_1;
        public Color measureGridColor8_1;
        public Color measureGridColor4_1;
        public Color measureGridColor1;
        public Color baseTransparentColor;
        public Vector3 beforePositon = Vector3.zero;
        public Quaternion beforeRotation = Quaternion.Euler(0, 0, 0);
        public bool waveFormIsNoteSide;
        public bool beforeWaveFormIsNoteSide;
        public EventsContainer.PropMode profMode;
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
        public List<BookmarkContainer> bookmarkContainers;
        public bool init = false;
        public bool customEventsObject = false;
        public string currentAvatarFile = "";
        public void UI_set(UI ui)
        {
            this._ui = ui;
        }
        public void BookmarkContainerGet()
        {
            Type type = bookmarkManager.GetType();
            FieldInfo field = type.GetField("bookmarkContainers", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            bookmarkContainers = (List<BookmarkContainer>)(field.GetValue(bookmarkManager));
        }
        public void BookmarkTrackSet()
        {
            if (Options.BookmarkLines)
                bookmarkLinesController.RefreshBookmarkLines(bookmarkContainers);
        }
        public void BookmarkWidthChange()
        {
            bookmarkContainers.ForEach(container =>
            {
                var rectTransform = (RectTransform)container.transform;
                rectTransform.sizeDelta = new Vector2(Options.BookMarkWidth, 20f);
            });
        }
        public string ScriptGet()
        {
            return Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, Options.ScriptFileName).Replace("/", "\\");
        }
        public string MapGet()
        {
            return Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, BeatSaberSongContainer.Instance.DifficultyData.BeatmapFilename).Replace("/", "\\");
        }
        public void MapSave()
        {
            autoSave.Save();
        }
        public void CameraPositionAndRotationSet(Vector3 position,Vector3 rotation)
        {
            position += new Vector3(Options.OriginXoffset, Options.OriginYoffset, Options.OriginZoffset);
            position *= Options.AvatarCameraScale;
            position += new Vector3(0, Options.OrigenMatchOffsetY, Options.OrigenMatchOffsetZ);
            cm_MapEditorCamera.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
        }
        public Vector3 CameraPositionGet()
        {
            var cameraPosition = new Vector3(cm_MapEditorCamera.transform.position.x, cm_MapEditorCamera.transform.position.y - Options.OrigenMatchOffsetY, cm_MapEditorCamera.transform.position.z - Options.OrigenMatchOffsetZ);
            cameraPosition /= Options.AvatarCameraScale;
            cameraPosition -= new Vector3(Options.OriginXoffset, Options.OriginYoffset, Options.OriginZoffset);
            return cameraPosition;
        }
        public Vector3 CameraRotationGet()
        {
            return cm_MapEditorCamera.transform.eulerAngles;
        }
        public Vector3 AvatarPositionGet()
        {
            return new Vector3(Options.OriginXoffset, Options.AvatarHeadHight + Options.OriginYoffset, Options.OriginZoffset);
        }
        public bool SavingThread()
        {
            Type type = autoSave.GetType();
            FieldInfo field = type.GetField("savingThread", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            Thread savingThread = (Thread)(field.GetValue(autoSave));
            return savingThread != null && savingThread.IsAlive;
        }
        public void UiHidden()
        {
            if (Options.UIhidden)
                beforeWaveFormIsNoteSide = spectrogramSideSwapper.IsNoteSide;
            Reload();
        }
        public void BookMarkChangeUpdate()
        {
            BookmarkContainerGet();
            BookmarkWidthChange();
            BookmarkTrackSet();
            BookMarkUpdate();
        }

        public void BookMarkUpdate()
        {
            if (init)
            {
                if (bookmarkContainers.Count > 0)
                {
                    var lastBookmark = bookmarkContainers.FindLast(x => x.Data.Time <= atsc.CurrentBeat);
                    if (bookmarkContainers.IndexOf(lastBookmark) == -1)
                    {
                        this._ui.CurrentBookmarkUpdate("", 0);
                    }
                    else
                    {
                        this._ui.CurrentBookmarkUpdate(lastBookmark.Data.Name, bookmarkContainers.IndexOf(lastBookmark) + 1);
                    }
                }
                else
                {
                    this._ui.CurrentBookmarkUpdate("", 0);
                }
            }
        }
        public void BookmarkChange(int bookmarkNo)
        {
            var bookmarkContainer = bookmarkContainers[bookmarkNo - 1];
            PersistentUI.Instance.ShowInputBox("Mapper", "bookmark.update.dialog", (res) =>
            {
                Type type = bookmarkContainer.GetType();
                MethodInfo method = type.GetMethod("HandleNewBookmarkName", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(bookmarkContainer, new object[] { res });
            }
            , null, bookmarkContainer.Data.Name);
        }
        public void BookmarkDelete(int bookmarkNo)
        {
            PersistentUI.Instance.ShowDialogBox("Mapper", "bookmark.delete", (res) =>
            {
                if (res == 0)
                {
                    var bookmarkContainer = bookmarkContainers[bookmarkNo - 1];
                    Type type = bookmarkContainer.GetType();
                    MethodInfo method = type.GetMethod("HandleDeleteBookmark", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(bookmarkContainer, new object[] { res });
                }
            }
            , PersistentUI.DialogBoxPresetType.YesNo);
        }
        public void BookmarkNew(string bookmarkName)
        {
            Type type = bookmarkManager.GetType();
            MethodInfo method;
            try
            {
                Debug.Log("HandleNewBookmarkName");
                method = type.GetMethod("HandleNewBookmarkName", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(bookmarkManager, new object[1] { bookmarkName });
            }
            catch
            {
                Debug.Log("CreateNewBookmark");
                //2022.2.7 "New bookmark dialog remade with CMUI"  SHA-1:795115393a1fb265ee6b77f7616941ce62b0e208 での変更対応
                method = type.GetMethod("CreateNewBookmark", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(bookmarkManager, new object[2] { bookmarkName, null });
            }
        }

        public void Reload()
        {
            StartCoroutine("CustomAvatarLoad");
            //サンプル アリシア・ソリッドを元に 頭の高さ1.43m、大きさ0.25m  腕の長さ1.12mの時
            var avatarHeadPosition = new Vector3(Options.OriginXoffset, Options.AvatarHeadHight + Options.OriginYoffset, Options.OriginZoffset) * Options.AvatarCameraScale;
            avatarHeadPosition.y += Options.OrigenMatchOffsetY;
            avatarHeadPosition.z += Options.OrigenMatchOffsetZ;
            avatarHead.transform.position = avatarHeadPosition;
            avatarHead.transform.localScale = new Vector3(Options.AvatarHeadSize, Options.AvatarHeadSize, Options.AvatarHeadSize) * Options.AvatarCameraScale;
            //首の高さ 1.43-0.25÷2=1.305m
            //胴体の中心からの高さは0.3m→1.305m÷0.3m=4.35
            var body_size_y = (Options.AvatarHeadHight - (Options.AvatarHeadSize / 2.0f)) / 4.35f;
            //胴体の直径0.2mは胴体の中心からの高さの1/3
            var body_size_x = body_size_y / 1.5f;
            //胴体の中心1.005は首の高さ1.305m-胴体の中心からの高さ0.3m=1.005
            var body_pos_y = Options.AvatarHeadHight - (Options.AvatarHeadSize / 2.0f) - body_size_y;
            var avatarBodyPosition = new Vector3(Options.OriginXoffset, body_pos_y + Options.OriginYoffset, Options.OriginZoffset) * Options.AvatarCameraScale;
            avatarBodyPosition.y += Options.OrigenMatchOffsetY;
            avatarBodyPosition.z += Options.OrigenMatchOffsetZ;
            avatarBody.transform.position = avatarBodyPosition;
            avatarBody.transform.localScale = new Vector3(body_size_x, body_size_y, body_size_x * 0.8f) * Options.AvatarCameraScale;
            //腕の中心高さ1.25mは首の高さ1.305mの 1.305m÷1.25m=1.044
            var arm_pos_y = (Options.AvatarHeadHight - (Options.AvatarHeadSize / 2.0f)) / 1.044f;
            //腕の大きさ0.06mは腕の長さ1.25m→1.25÷0.06=20.83
            var arm_size_yz = Options.AvatarArmSize / 20.83f;
            var avatarArmPosition = new Vector3(Options.OriginXoffset, arm_pos_y + Options.OriginYoffset, Options.OriginZoffset) * Options.AvatarCameraScale;
            avatarArmPosition.y += Options.OrigenMatchOffsetY;
            avatarArmPosition.z += Options.OrigenMatchOffsetZ;
            avatarArm.transform.position = avatarArmPosition;
            avatarArm.transform.localScale = new Vector3(Options.AvatarArmSize, arm_size_yz, arm_size_yz) * Options.AvatarCameraScale;
            //足の高さは腕の中心から腕のサイズを引いたもの
            var leg_size_y = arm_pos_y - (arm_size_yz / 2.0f);
            var leg_pos_y = leg_size_y / 2.0f;
            var leg_size_xz = leg_size_y / 12f;
            var avatarLegPosition = new Vector3(Options.OriginXoffset, leg_pos_y + Options.OriginYoffset, Options.OriginZoffset) * Options.AvatarCameraScale;
            avatarLegPosition.y += Options.OrigenMatchOffsetY;
            avatarLegPosition.z += Options.OrigenMatchOffsetZ;
            avatarLeg.transform.position = avatarLegPosition;
            avatarLeg.transform.localScale = new Vector3(leg_size_xz, leg_size_y, leg_size_xz) * Options.AvatarCameraScale;
            //おさげ
            var avatarHairPosition = new Vector3(Options.OriginXoffset, Options.AvatarHeadHight - Options.AvatarHeadSize + Options.OriginYoffset, Options.AvatarHeadSize / -2.0f + Options.OriginZoffset) * Options.AvatarCameraScale;
            avatarHairPosition.y += Options.OrigenMatchOffsetY;
            avatarHairPosition.z += Options.OrigenMatchOffsetZ;
            avatarHair.transform.position = avatarHairPosition;
            avatarHair.transform.localScale = new Vector3(Options.AvatarHeadSize / 1.4f, Options.AvatarHeadSize * 2.0f, Options.AvatarHeadSize / 17f) * Options.AvatarCameraScale;
            if (Options.SimpleAvatar && Options.Avatar)
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
            beforeSeconds = 0;
            _reload = true;
            _cameraMovement.LoadCameraData(ScriptGet());
            _cameraMovement.MovementPositionReset();
            BookmarkWidthChange();
            BookmarkTrackSet();
            bookmarkLines.SetActive(Options.BookmarkLines);
            if (Options.BookmarkLines)
            {
                EventBpmOffset(Options.BookmarkInsertOffset);
            }
            else
            {
                EventBpmOffset(0);
            }
            sub_camera.gameObject.SetActive(!(Options.Movement || !Options.SubCamera));
            sub_camera.rect = new Rect(Options.SubCameraRectX, Options.SubCameraRectY, Options.SubCameraRectW, Options.SubCameraRectH);
            if (Options.UIhidden)
            {
                cm_GridX.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_BaseAlpha", 0);
                cm_GridX.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_GridAlpha", 0);
                cm_Grid.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_GridAlpha", 0);
                cm_measureGrid16_1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", new Color(0, 0, 0, 0));
                cm_measureGrid8_1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", new Color(0, 0, 0, 0));
                cm_measureGrid4_1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", new Color(0, 0, 0, 0));
                cm_measureGrid1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", new Color(0, 0, 0, 0));
                cm_baseTransparent.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(0, 0, 0, 0));
                cm_interface.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_OPACITY", 0);
                cm_MeasureLinesCanvas.gameObject.GetComponent<Canvas>().enabled = false;
                spectrogramSideSwapper.IsNoteSide = true;
                spectrogramSideSwapper.SwapSides();
                EventBpmOffset(200f);
                if (Settings.Instance.Load_Notes || Settings.Instance.Load_Obstacles)
                {
                    cm_NoteFrontBase.SetActive(false);
                    cm_NoteBackBase.SetActive(false);
                }
                if (Settings.Instance.Load_Events)
                {
                    cm_EventFrontBase.SetActive(false);
                    cm_EventBackBase.SetActive(false);
                    cm_eventLabel.gameObject.GetComponent<Canvas>().enabled = false;
                    profMode = eventContainer.PropagationEditing;
                    eventContainer.PropagationEditing = EventsContainer.PropMode.Prop;
                }
                if (Settings.Instance.Load_Others)
                {
                    cm_BPMchangeFrontBase.SetActive(false);
                    cm_BPMchangeBackBase.SetActive(false);
                    cm_BPMChangeLable.gameObject.GetComponent<Canvas>().enabled = false;
                }
            }
            else
            {
                cm_GridX.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_BaseAlpha", gridXBaseAlpha);
                cm_GridX.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_GridAlpha", gridXGridAlpha);
                cm_Grid.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_GridAlpha", gridGridAlpha);
                cm_measureGrid16_1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", measureGridColor16_1);
                cm_measureGrid8_1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", measureGridColor8_1);
                cm_measureGrid4_1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", measureGridColor4_1);
                cm_measureGrid1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", measureGridColor1);
                cm_baseTransparent.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", baseTransparentColor);
                cm_interface.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_OPACITY", interfaceOpacity);
                cm_MeasureLinesCanvas.gameObject.GetComponent<Canvas>().enabled = true;
                spectrogramSideSwapper.IsNoteSide = !beforeWaveFormIsNoteSide;
                spectrogramSideSwapper.SwapSides();
                if (Settings.Instance.Load_Notes || Settings.Instance.Load_Obstacles)
                {
                    cm_NoteFrontBase.SetActive(true);
                    cm_NoteBackBase.SetActive(true);
                }
                if (Settings.Instance.Load_Events)
                {
                    cm_EventFrontBase.SetActive(true);
                    cm_EventBackBase.SetActive(true);
                    cm_eventLabel.gameObject.GetComponent<Canvas>().enabled = true;
                    eventContainer.PropagationEditing = profMode;
                }
                if (Settings.Instance.Load_Others)
                {
                    cm_BPMchangeFrontBase.SetActive(true);
                    cm_BPMchangeBackBase.SetActive(true);
                    cm_BPMChangeLable.gameObject.GetComponent<Canvas>().enabled = true;
                }
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
        public void WaveFormOffset()
        {
            float offset;
            if (Options.BookmarkLines)
            {
                offset = Options.BookmarkInsertOffset;
            }
            else
            {
                offset = 0;
            }
            if (Options.UIhidden)
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

        public IEnumerator CustomAvatarLoad()
        {
            if (currentAvatarFile != Options.CustomAvatarFileName && Options.CustomAvatar)
            {
                currentAvatarFile = "";
                if (avatarModel != null)
                    Destroy(avatarModel);
                var avatarFullPath = Path.Combine(Environment.CurrentDirectory, Options.CustomAvatarFileName);
                if (File.Exists(avatarFullPath))
                {
                    var request = AssetBundle.LoadFromFileAsync(avatarFullPath);
                    yield return request;
                    if (request.isDone && request.assetBundle)
                    {
                        var assetBundleRequest = request.assetBundle.LoadAssetWithSubAssetsAsync<GameObject>("_CustomAvatar");
                        yield return assetBundleRequest;
                        if (assetBundleRequest.isDone && assetBundleRequest.asset != null)
                        {
                            request.assetBundle.Unload(false);
                            try
                            {
                                avatarModel = (GameObject)Instantiate(assetBundleRequest.asset);
                                currentAvatarFile = Options.CustomAvatarFileName;
                            }
                            catch
                            {
                                Debug.LogError("Avatar Instantiate ERR!");
                            }
                        }
                        else
                        {
                            Debug.LogError("Avatar Load2 ERR!");
                            request.assetBundle.Unload(false);
                        }
                    }
                    else
                    {
                        Debug.LogError("Avatar Load1 ERR!");
                    }
                }
                else
                {
                    Debug.LogError("Avatar File ERR!");
                }
            }
            if (avatarModel != null)
            {
                avatarModel.transform.localScale = new Vector3(Options.AvatarScale, Options.AvatarScale, Options.AvatarScale) * Options.AvatarCameraScale;
                var avatarPosition = new Vector3(Options.OriginXoffset, Options.OriginYoffset + Options.AvatarYoffset, Options.OriginZoffset) * Options.AvatarCameraScale;
                avatarPosition.y = Options.OrigenMatchOffsetY;
                avatarPosition.z = Options.OrigenMatchOffsetZ;
                avatarModel.transform.localPosition = avatarPosition;
                if (Options.CustomAvatar && Options.Avatar)
                {
                    avatarModel.SetActive(true);
                }
                else
                {
                    avatarModel.SetActive(false);
                }
            }

        }

        private IEnumerator Start()
        {
            _cameraMovement = new CameraMovement();
            atsc = FindObjectOfType<AudioTimeSyncController>();
            autoSave = FindObjectOfType<AutoSaveController>();
            spectrogramSideSwapper = FindObjectOfType<SpectrogramSideSwapper>();

            cm_MapEditorCamera = GameObject.Find("MapEditor Camera");
            cm_measureGrid16_1 = GameObject.Find("1/16th Measure Grid");
            cm_measureGrid8_1 = GameObject.Find("1/8th Measure Grid");
            cm_measureGrid4_1 = GameObject.Find("1/4th Measure Grid");
            cm_measureGrid1 = GameObject.Find("One Measure Grid");
            cm_MeasureLinesCanvas = GameObject.Find("Measure Lines Canvas");
            cm_interface = GameObject.Find("Interface");
            cm_GridX = GameObject.Find("Grid X");
            cm_Grid = GameObject.Find("Grid");
            cm_baseTransparent = GameObject.Find("Base Transparent");

            measureGridColor16_1 = cm_measureGrid16_1.gameObject.GetComponent<Renderer>().sharedMaterial.GetColor("_GridColour");
            measureGridColor8_1 = cm_measureGrid8_1.gameObject.GetComponent<Renderer>().sharedMaterial.GetColor("_GridColour");
            measureGridColor4_1 = cm_measureGrid4_1.gameObject.GetComponent<Renderer>().sharedMaterial.GetColor("_GridColour");
            measureGridColor1 = cm_measureGrid1.gameObject.GetComponent<Renderer>().sharedMaterial.GetColor("_GridColour");
            baseTransparentColor = cm_baseTransparent.gameObject.GetComponent<Renderer>().sharedMaterial.GetColor("_Color");
            interfaceOpacity = cm_interface.gameObject.GetComponent<Renderer>().sharedMaterial.GetFloat("_OPACITY");
            gridXBaseAlpha = cm_GridX.gameObject.GetComponent<Renderer>().sharedMaterial.GetFloat("_BaseAlpha");
            gridXGridAlpha = cm_GridX.gameObject.GetComponent<Renderer>().sharedMaterial.GetFloat("_GridAlpha");
            gridGridAlpha = cm_Grid.gameObject.GetComponent<Renderer>().sharedMaterial.GetFloat("_GridAlpha");
            if (Settings.Instance.Load_Notes || Settings.Instance.Load_Obstacles)
            {
                cm_NoteFrontBase = GameObject.Find("Note Grid Front Scaling Offset/Base");
                cm_NoteBackBase = GameObject.Find("Note Grid Back Scaling Offset/Base");
            }
            if (Settings.Instance.Load_Events)
            {
                eventContainer = FindObjectOfType<EventsContainer>();
                cm_EventFrontBase = GameObject.Find("Event Grid Front Scaling Offset/Base");
                cm_EventBackBase = GameObject.Find("Event Grid Back Scaling Offset/Base");
                cm_eventLabel = GameObject.Find("Event Label");
            }
            if (Settings.Instance.Load_Others)
            {
                cm_BPMchangeFrontBase = GameObject.Find("BPM Changes Grid Front Scaling Offset/Base");
                cm_BPMchangeBackBase = GameObject.Find("BPM Changes Grid Back Scaling Offset/Base");
                cm_BPMChangeLable = GameObject.Find("BPM Change Label/");
            }
            avatarHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            avatarArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            avatarLeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            avatarBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            avatarHair = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            //BookMarkTrack構築
            var measureLines = GameObject.Find("Moveable Grid/Measure Lines");
            bookmarkLines = Instantiate(measureLines);
            var moveableGrid = GameObject.Find("Moveable Grid");
            bookmarkLines.name = "Bookmark Lines";
            bookmarkLines.transform.parent = moveableGrid.transform;
            bookmarkLines.gameObject.GetComponent<GridChild>().Order = 1;
            bookmarkLines.gameObject.GetComponent<GridChild>().LocalOffset = new Vector3(1.6f, 0, 0);
            var bookmarkLinesCanvas = bookmarkLines.transform.Find("Measure Lines Canvas").gameObject;
            bookmarkLinesCanvas.name = "Bookmark Lines Canvas";
            Destroy(bookmarkLinesCanvas.gameObject.GetComponent<MeasureLinesRenderingOrderController>());
            bookmarkLinesCanvas.gameObject.GetComponent<Track>().ObjectParentTransform = bookmarkLinesCanvas.gameObject.GetComponent<RectTransform>();
            var bookmarkLinesRenderingOrderController = bookmarkLinesCanvas.AddComponent<BookmarkLinesRenderingOrderController>();
            bookmarkLinesRenderingOrderController.effectingCanvas = bookmarkLinesCanvas.gameObject.GetComponent<Canvas>();
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

            eventGridChild = GameObject.Find("Rotating/Event Grid").GetComponent<GridChild>();
            eventLabelChild = GameObject.Find("Rotating/Event Label").GetComponent<GridChild>();
            eventsGridChild = GameObject.Find("Moveable Grid/Events Grid").GetComponent<GridChild>();
            bpmChangesChild = GameObject.Find("Rotating/BPM Changes Grid").GetComponent<GridChild>();
            bpmChangesGridChild = GameObject.Find("Moveable Grid/BPM Changes Grid").GetComponent<GridChild>();
            spectrogramGridChild = GameObject.Find("Rotating/Spectrogram Grid").GetComponent<GridChild>();
            waveformGridChild = GameObject.Find("Moveable Grid/Waveform Chunks Grid").GetComponent<GridChild>();
            eventGridChildLocalOffset = eventGridChild.LocalOffset;
            eventLabelChildLocalOffset = eventLabelChild.LocalOffset;
            eventsGridChildLocalOffset = eventsGridChild.LocalOffset;
            bpmChangesChildLocalOffset = bpmChangesChild.LocalOffset;
            bpmChangesGridChildLocalOffset = bpmChangesGridChild.LocalOffset;
            spectrogramGridChildLocalOffset = spectrogramGridChild.LocalOffset;
            waveformGridChildLocalOffset = waveformGridChild.LocalOffset;

            try
            {
                customEventsChild = GameObject.Find("Rotating/Custom Events Grid").GetComponent<GridChild>();
                customEventsLabelsChild = GameObject.Find("Rotating/Custom Events Grid Labels").GetComponent<GridChild>();
                customEventsGridChild = GameObject.Find("Moveable Grid/Custom Events Grid").GetComponent<GridChild>();
                customEventsChildLocalOffset = customEventsChild.LocalOffset; ;
                customEventsLabelsChildLocalOffset = customEventsLabelsChild.LocalOffset;
                customEventsGridChildLocalOffset = customEventsGridChild.LocalOffset;
                customEventsObject = true;
            }
            catch
            {
                Debug.LogWarning("CameraMovement:customEvents object err");
                customEventsObject = false;
            }


            beforeWaveFormIsNoteSide = spectrogramSideSwapper.IsNoteSide;

            sub_camera = new GameObject("Sub Camera").AddComponent<Camera>();
            sub_camera.clearFlags = CameraClearFlags.SolidColor;
            sub_camera.backgroundColor = new Color(0, 0, 0, 255);

            yield return new WaitForSeconds(0.5f); //BookmarkManagerのStart()が0.1秒待つので0.5秒待つことにする。
            bookmarkManager = FindObjectOfType<BookmarkManager>();

            bookmarkManager.BookmarksUpdated += BookMarkChangeUpdate;
            BookmarkContainerPatch.OnNewBookmarkName += BookMarkChangeUpdate;
            SpectrogramSideSwapperPatch.OnSwapSides += WaveFormOffset;
            init = true;
            BookMarkChangeUpdate();
            this.Reload();
        }
        private void Update()
        {
            if (_reload || beforeSeconds != atsc.CurrentSeconds)
            {
                _reload = false;
                if (beforeSeconds > atsc.CurrentSeconds)
                {
                    _cameraMovement.MovementPositionReset();
                    beforeSeconds = 0;
                }
                beforeSeconds = atsc.CurrentSeconds;
                BookMarkUpdate();
                _cameraMovement.CameraUpdate(atsc.CurrentSeconds, cm_MapEditorCamera, sub_camera , AvatarPositionGet());
            }
            if (beforePositon != cm_MapEditorCamera.transform.position || beforeRotation != cm_MapEditorCamera.transform.rotation)
            {
                _ui.CameraPosRotUpdate();
                beforePositon = cm_MapEditorCamera.transform.position;
                beforeRotation = cm_MapEditorCamera.transform.rotation;
            }
        }

        private void OnDisable()
        {
            bookmarkManager.BookmarksUpdated -= BookMarkChangeUpdate;
            BookmarkContainerPatch.OnNewBookmarkName -= BookMarkChangeUpdate;
            SpectrogramSideSwapperPatch.OnSwapSides -= WaveFormOffset;
        }

    }
}
