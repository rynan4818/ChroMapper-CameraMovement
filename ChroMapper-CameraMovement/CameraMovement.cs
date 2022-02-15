using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using UnityEngine;
using TMPro;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.HarmonyPatches;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

namespace ChroMapper_CameraMovement
{
    public class CameraMovement : MonoBehaviour
    {
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

        public bool dataLoaded = false;
        public CameraData data = new CameraData();
        public Vector3 StartPos = Vector3.zero;
        public Vector3 EndPos = Vector3.zero;
        public Vector3 StartRot = Vector3.zero;
        public Vector3 EndRot = Vector3.zero;
        public Vector3 StartHeadOffset = Vector3.zero;
        public Vector3 EndHeadOffset = Vector3.zero;
        public float StartFOV = 0;
        public float EndFOV = 0;
        public bool easeTransition = true;
        public float movePerc;
        public int eventID;
        public float movementStartTime, movementEndTime, movementNextStartTime;
        public DateTime movementStartDateTime, movementEndDateTime, movementDelayEndDateTime;
        public bool _reload = false;
        public DateTime _pauseTime;
        public bool turnToHead;
        public bool turnToHeadHorizontal;
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
        public Quaternion beforeRotation = Quaternion.Euler(0,0,0);
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
        public class Movements
        {
            public Vector3 StartPos;
            public Vector3 StartRot;
            public Vector3 StartHeadOffset;
            public float StartFOV;
            public Vector3 EndPos;
            public Vector3 EndRot;
            public Vector3 EndHeadOffset;
            public float EndFOV;
            public float Duration;
            public float Delay;
            public VisibleObject SectionVisibleObject;
            public bool TurnToHead = false;
            public bool TurnToHeadHorizontal = false;
            public bool EaseTransition = true;
        }
        public class CameraData
        {
            public bool ActiveInPauseMenu = true;
            public bool TurnToHeadUseCameraSetting = false;
            public List<Movements> Movements = new List<Movements>();

            public bool LoadFromJson(string jsonString)
            {
                Movements.Clear();
                MovementScriptJson movementScriptJson = null;
                string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string sepCheck = (sep == "." ? "," : ".");
                try
                {
                    movementScriptJson = JsonConvert.DeserializeObject<MovementScriptJson>(jsonString);
                }
                catch (Exception ex)
                {
                    Debug.Log($"CameraMovement:JSON file syntax error. {ex.Message}");
                }
                if (movementScriptJson != null && movementScriptJson.Jsonmovement != null)
                {
                    if (movementScriptJson.ActiveInPauseMenu != null)
                        ActiveInPauseMenu = System.Convert.ToBoolean(movementScriptJson.ActiveInPauseMenu);
                    if (movementScriptJson.TurnToHeadUseCameraSetting != null)
                        TurnToHeadUseCameraSetting = System.Convert.ToBoolean(movementScriptJson.TurnToHeadUseCameraSetting);

                    foreach (JSONMovement jsonmovement in movementScriptJson.Jsonmovement)
                    {
                        Movements newMovement = new Movements();

                        AxizWithFoVElements startPos = jsonmovement.startPos;
                        AxisElements startRot = new AxisElements();
                        AxisElements startHeadOffset = new AxisElements();
                        if (jsonmovement.startRot != null) startRot = jsonmovement.startRot;
                        if (jsonmovement.startHeadOffset != null) startHeadOffset = jsonmovement.startHeadOffset;

                        if (startPos.x != null) newMovement.StartPos = new Vector3(float.Parse(startPos.x.Contains(sepCheck) ? startPos.x.Replace(sepCheck, sep) : startPos.x),
                                                                                    float.Parse(startPos.y.Contains(sepCheck) ? startPos.y.Replace(sepCheck, sep) : startPos.y),
                                                                                    float.Parse(startPos.z.Contains(sepCheck) ? startPos.z.Replace(sepCheck, sep) : startPos.z));
                        if (startRot.x != null) newMovement.StartRot = new Vector3(float.Parse(startRot.x.Contains(sepCheck) ? startRot.x.Replace(sepCheck, sep) : startRot.x),
                                                                                    float.Parse(startRot.y.Contains(sepCheck) ? startRot.y.Replace(sepCheck, sep) : startRot.y),
                                                                                    float.Parse(startRot.z.Contains(sepCheck) ? startRot.z.Replace(sepCheck, sep) : startRot.z));
                        else
                            newMovement.StartRot = Vector3.zero;

                        if (startHeadOffset.x != null) newMovement.StartHeadOffset = new Vector3(float.Parse(startHeadOffset.x.Contains(sepCheck) ? startHeadOffset.x.Replace(sepCheck, sep) : startHeadOffset.x),
                                                                                    float.Parse(startHeadOffset.y.Contains(sepCheck) ? startHeadOffset.y.Replace(sepCheck, sep) : startHeadOffset.y),
                                                                                    float.Parse(startHeadOffset.z.Contains(sepCheck) ? startHeadOffset.z.Replace(sepCheck, sep) : startHeadOffset.z));
                        else
                            newMovement.StartHeadOffset = Vector3.zero;

                        if (startPos.FOV != null)
                            newMovement.StartFOV = float.Parse(startPos.FOV.Contains(sepCheck) ? startPos.FOV.Replace(sepCheck, sep) : startPos.FOV);
                        else
                            newMovement.StartFOV = 0;

                        AxizWithFoVElements endPos = jsonmovement.endPos;
                        AxisElements endRot = new AxisElements();
                        AxisElements endHeadOffset = new AxisElements();
                        if (jsonmovement.endRot != null) endRot = jsonmovement.endRot;
                        if (jsonmovement.endHeadOffset != null) endHeadOffset = jsonmovement.endHeadOffset;

                        if (endPos.x != null) newMovement.EndPos = new Vector3(float.Parse(endPos.x), float.Parse(endPos.y), float.Parse(endPos.z));
                        if (endRot.x != null) newMovement.EndRot = new Vector3(float.Parse(endRot.x), float.Parse(endRot.y), float.Parse(endRot.z));
                        if (endPos.x != null) newMovement.EndPos = new Vector3(float.Parse(endPos.x.Contains(sepCheck) ? endPos.x.Replace(sepCheck, sep) : endPos.x),
                                                                                    float.Parse(endPos.y.Contains(sepCheck) ? endPos.y.Replace(sepCheck, sep) : endPos.y),
                                                                                    float.Parse(endPos.z.Contains(sepCheck) ? endPos.z.Replace(sepCheck, sep) : endPos.z));
                        if (endRot.x != null) newMovement.EndRot = new Vector3(float.Parse(endRot.x.Contains(sepCheck) ? endRot.x.Replace(sepCheck, sep) : endRot.x),
                                                                                    float.Parse(endRot.y.Contains(sepCheck) ? endRot.y.Replace(sepCheck, sep) : endRot.y),
                                                                                    float.Parse(endRot.z.Contains(sepCheck) ? endRot.z.Replace(sepCheck, sep) : endRot.z));
                        else
                            newMovement.EndRot = Vector3.zero;
                        if (endHeadOffset.x != null) newMovement.EndHeadOffset = new Vector3(float.Parse(endHeadOffset.x.Contains(sepCheck) ? endHeadOffset.x.Replace(sepCheck, sep) : endHeadOffset.x),
                                                            float.Parse(endHeadOffset.y.Contains(sepCheck) ? endHeadOffset.y.Replace(sepCheck, sep) : endHeadOffset.y),
                                                            float.Parse(endHeadOffset.z.Contains(sepCheck) ? endHeadOffset.z.Replace(sepCheck, sep) : endHeadOffset.z));
                        else
                            newMovement.EndHeadOffset = Vector3.zero;


                        if (endPos.FOV != null)
                            newMovement.EndFOV = float.Parse(endPos.FOV.Contains(sepCheck) ? endPos.FOV.Replace(sepCheck, sep) : endPos.FOV);
                        else
                            newMovement.EndFOV = 0;

                        if (jsonmovement.visibleObject != null) newMovement.SectionVisibleObject = jsonmovement.visibleObject;
                        if (jsonmovement.TurnToHead != null) newMovement.TurnToHead = System.Convert.ToBoolean(jsonmovement.TurnToHead);
                        if (jsonmovement.TurnToHeadHorizontal != null) newMovement.TurnToHeadHorizontal = System.Convert.ToBoolean(jsonmovement.TurnToHeadHorizontal);
                        if (jsonmovement.Delay != null) newMovement.Delay = float.Parse(jsonmovement.Delay.Contains(sepCheck) ? jsonmovement.Delay.Replace(sepCheck, sep) : jsonmovement.Delay);
                        if (jsonmovement.Duration != null) newMovement.Duration = Mathf.Clamp(float.Parse(jsonmovement.Duration.Contains(sepCheck) ? jsonmovement.Duration.Replace(sepCheck, sep) : jsonmovement.Duration), 0.01f, float.MaxValue); // Make sure duration is at least 0.01 seconds, to avoid a divide by zero error

                        if (jsonmovement.EaseTransition != null)
                            newMovement.EaseTransition = System.Convert.ToBoolean(jsonmovement.EaseTransition);

                        Movements.Add(newMovement);
                    }
                    return true;
                }
                return false;
            }
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
            return Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, Options.ScriptFileName).Replace("/","\\");
        }
        public string MapGet()
        {
            return Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, BeatSaberSongContainer.Instance.DifficultyData.BeatmapFilename).Replace("/", "\\");
        }
        public void MapSave()
        {
            autoSave.Save();
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
            MethodInfo method = type.GetMethod("HandleNewBookmarkName", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(bookmarkManager, new object[1] { bookmarkName });
        }

        public void Reload()
        {
            StartCoroutine("CustomAvatarLoad");
            //サンプル アリシア・ソリッドを元に 頭の高さ1.43m、大きさ0.25m  腕の長さ1.12mの時
            avatarHead.transform.position = new Vector3(0, Options.AvatarHeadHight + Options.CameraYoffset, 0 + Options.CameraZoffset);
            avatarHead.transform.localScale = new Vector3(Options.AvatarHeadSize, Options.AvatarHeadSize, Options.AvatarHeadSize);
            //首の高さ 1.43-0.25÷2=1.305m
            //胴体の中心からの高さは0.3m→1.305m÷0.3m=4.35
            var body_size_y = (Options.AvatarHeadHight - (Options.AvatarHeadSize / 2.0f)) / 4.35f;
            //胴体の直径0.2mは胴体の中心からの高さの1/3
            var body_size_x = body_size_y / 1.5f;
            //胴体の中心1.005は首の高さ1.305m-胴体の中心からの高さ0.3m=1.005
            var body_pos_y = Options.AvatarHeadHight - (Options.AvatarHeadSize / 2.0f) - body_size_y;
            avatarBody.transform.position = new Vector3(0, body_pos_y + Options.CameraYoffset, 0 + Options.CameraZoffset);
            avatarBody.transform.localScale = new Vector3(body_size_x, body_size_y, body_size_x * 0.8f);
            //腕の中心高さ1.25mは首の高さ1.305mの 1.305m÷1.25m=1.044
            var arm_pos_y = (Options.AvatarHeadHight - (Options.AvatarHeadSize / 2.0f)) / 1.044f;
            //腕の大きさ0.06mは腕の長さ1.25m→1.25÷0.06=20.83
            var arm_size_yz = Options.AvatarArmSize / 20.83f;
            avatarArm.transform.position = new Vector3(0, arm_pos_y + Options.CameraYoffset, 0 + Options.CameraZoffset);
            avatarArm.transform.localScale = new Vector3(Options.AvatarArmSize, arm_size_yz, arm_size_yz);
            //足の高さは腕の中心から腕のサイズを引いたもの
            var leg_size_y = arm_pos_y - (arm_size_yz / 2.0f);
            var leg_pos_y = leg_size_y / 2.0f;
            var leg_size_xz = leg_size_y / 12f;
            avatarLeg.transform.position = new Vector3(0, leg_pos_y + Options.CameraYoffset, 0 + Options.CameraZoffset);
            avatarLeg.transform.localScale = new Vector3(leg_size_xz, leg_size_y, leg_size_xz);
            //おさげ
            avatarHair.transform.localScale = new Vector3(Options.AvatarHeadSize / 1.4f, Options.AvatarHeadSize * 2.0f, Options.AvatarHeadSize / 17f);
            avatarHair.transform.position = new Vector3(0, Options.AvatarHeadHight - Options.AvatarHeadSize + Options.CameraYoffset, Options.AvatarHeadSize / -2.0f + Options.CameraZoffset);
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
            movementNextStartTime = 0;
            eventID = 0;
            beforeSeconds = 0;
            _reload = true;
            LoadCameraData(ScriptGet());
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
                cm_measureGrid4_1.gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_GridColour", new Color(0,0,0,0));
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
                avatarModel.transform.localScale = new Vector3(Options.AvatarScale, Options.AvatarScale, Options.AvatarScale);
                avatarModel.transform.localPosition = new Vector3(0, Options.CameraYoffset + Options.AvatarYoffset, Options.CameraZoffset);
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
            bookmarkLines.gameObject.GetComponent<GridChild>().LocalOffset = new Vector3(1.6f,0,0);
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
            if (dataLoaded && (_reload || beforeSeconds != atsc.CurrentSeconds))
            {
                _reload = false;
                if (beforeSeconds > atsc.CurrentSeconds)
                {
                    movementNextStartTime = 0;
                    eventID = 0;
                    beforeSeconds = 0;
                }
                beforeSeconds = atsc.CurrentSeconds;
                BookMarkUpdate();

                while (movementNextStartTime <= atsc.CurrentSeconds)
                    UpdatePosAndRot();

                float difference = movementEndTime - movementStartTime;
                float current = atsc.CurrentSeconds - movementStartTime;
                if (difference != 0)
                    movePerc = Mathf.Clamp(current / difference, 0, 1);

                Vector3 cameraPos = LerpVector3(StartPos, EndPos, Ease(movePerc));
                Vector3 cameraRot = LerpVector3(StartRot, EndRot, Ease(movePerc));
                cameraPos.y += Options.CameraYoffset;
                cameraPos.z += Options.CameraZoffset;
                if (turnToHead)
                {
                    Vector3 turnToTarget = new Vector3(0, Options.AvatarHeadHight, 0);  //アバターの頭の位置
                    turnToTarget += LerpVector3(StartHeadOffset, EndHeadOffset, Ease(movePerc));
                    var direction = turnToTarget - cameraPos;
                    var lookRotation = Quaternion.LookRotation(direction);
                    if (turnToHeadHorizontal)
                        cameraRot = new Vector3(cameraRot.x, lookRotation.eulerAngles.y, cameraRot.z);
                    else
                    {
                        if (Options.Movement)
                            cm_MapEditorCamera.transform.SetPositionAndRotation(cameraPos, lookRotation);
                        sub_camera.transform.SetPositionAndRotation(cameraPos, lookRotation);
                    }
                }
                if (!(turnToHead && turnToHeadHorizontal))
                {
                    if (Options.Movement)
                        cm_MapEditorCamera.transform.SetPositionAndRotation(cameraPos, Quaternion.Euler(cameraRot));
                    sub_camera.transform.SetPositionAndRotation(cameraPos, Quaternion.Euler(cameraRot));
                }

                Settings.Instance.CameraFOV = Mathf.Lerp(StartFOV, EndFOV, Ease(movePerc));
            }
            if (beforePositon != cm_MapEditorCamera.transform.position || beforeRotation != cm_MapEditorCamera.transform.rotation)
            {
                _ui.CameraPosRotUpdate(cm_MapEditorCamera.transform);
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

        protected Vector3 LerpVector3(Vector3 from, Vector3 to, float percent)
        {
            return new Vector3(Mathf.LerpAngle(from.x, to.x, percent), Mathf.LerpAngle(from.y, to.y, percent), Mathf.LerpAngle(from.z, to.z, percent));
        }
        protected bool LoadCameraData(string pathFile)
        {
            string path = pathFile;
            dataLoaded = false;

            if (File.Exists(path))
            {
                string jsonText = File.ReadAllText(path);
                if (data.LoadFromJson(jsonText))
                {
                    if (data.Movements.Count == 0)
                    {
                        Debug.Log("CameraMovement:No movement data!");
                        return false;
                    }
                    eventID = 0;
                    UpdatePosAndRot();
                    dataLoaded = true;

                    Debug.Log($"CameraMovement:Found {data.Movements.Count} entries in: {path}");
                    return true;
                }
            }
            return false;
        }

        protected void FindShortestDelta(ref Vector3 from, ref Vector3 to)
        {
            if (Mathf.DeltaAngle(from.x, to.x) < 0)
                from.x += 360.0f;
            if (Mathf.DeltaAngle(from.y, to.y) < 0)
                from.y += 360.0f;
            if (Mathf.DeltaAngle(from.z, to.z) < 0)
                from.z += 360.0f;
        }
        protected void UpdatePosAndRot()
        {
            if (eventID >= data.Movements.Count)
                eventID = 0;

            turnToHead = data.TurnToHeadUseCameraSetting ? Options.TurnToHead : data.Movements[eventID].TurnToHead;
            turnToHeadHorizontal = data.Movements[eventID].TurnToHeadHorizontal;

            easeTransition = data.Movements[eventID].EaseTransition;

            StartRot = new Vector3(data.Movements[eventID].StartRot.x, data.Movements[eventID].StartRot.y, data.Movements[eventID].StartRot.z);
            StartPos = new Vector3(data.Movements[eventID].StartPos.x, data.Movements[eventID].StartPos.y, data.Movements[eventID].StartPos.z);

            EndRot = new Vector3(data.Movements[eventID].EndRot.x, data.Movements[eventID].EndRot.y, data.Movements[eventID].EndRot.z);
            EndPos = new Vector3(data.Movements[eventID].EndPos.x, data.Movements[eventID].EndPos.y, data.Movements[eventID].EndPos.z);

            StartHeadOffset = new Vector3(data.Movements[eventID].StartHeadOffset.x, data.Movements[eventID].StartHeadOffset.y, data.Movements[eventID].StartHeadOffset.z);
            EndHeadOffset = new Vector3(data.Movements[eventID].EndHeadOffset.x, data.Movements[eventID].EndHeadOffset.y, data.Movements[eventID].EndHeadOffset.z);

            if (data.Movements[eventID].StartFOV != 0)
                StartFOV = data.Movements[eventID].StartFOV;
            else
                StartFOV = Settings.Instance.CameraFOV;
            if (data.Movements[eventID].EndFOV != 0)
                EndFOV = data.Movements[eventID].EndFOV;
            else
                EndFOV = Settings.Instance.CameraFOV;

            FindShortestDelta(ref StartRot, ref EndRot);

            movementStartTime = movementNextStartTime;
            movementEndTime = movementNextStartTime + data.Movements[eventID].Duration;
            movementNextStartTime = movementEndTime + data.Movements[eventID].Delay;

            eventID++;
        }
        protected float Ease(float p)
        {
            if (!easeTransition)
                return p;

            if (p < 0.5f) //Cubic Hopefully
            {
                return 4 * p * p * p;
            }
            else
            {
                float f = ((2 * p) - 2);
                return 0.5f * f * f * f + 1;
            }
        }

    }
}
