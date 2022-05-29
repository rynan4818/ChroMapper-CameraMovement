using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using ChroMapper_CameraMovement.Configuration;

namespace ChroMapper_CameraMovement.Component
{
    public class BookmarkLinesController : MonoBehaviour
    {
        public TextMeshProUGUI bookmarkLinePrefab;
        public AudioTimeSyncController atsc;
        public RectTransform parent;
        public Transform frontNoteGridScaling;
        public List<BookmarkContainer> bookmarkContainers = new List<BookmarkContainer>();
        public readonly Dictionary<BookmarkContainer, (TextMeshProUGUI, bool)> bookmarkTexts = new Dictionary<BookmarkContainer, (TextMeshProUGUI, bool)>();
        public const float bookmarkXposition = -6f;

        public bool init;
        public float previousAtscBeat = -1;

        private void Start()
        {
            EditorScaleController.EditorScaleChangedEvent += EditorScaleUpdated;
        }

        private void LateUpdate()
        {
            if (atsc.CurrentBeat == previousAtscBeat || !init) return;
            previousAtscBeat = atsc.CurrentBeat;
            RefreshVisibility();
        }

        private void OnDestroy()
        {
            EditorScaleController.EditorScaleChangedEvent -= EditorScaleUpdated;
        }

        private void EditorScaleUpdated(float obj) => RefreshPositions();

        public void RefreshBookmarkLines(List<BookmarkContainer> bc)
        {
            bookmarkContainers = bc;
            init = false;
            if (bookmarkContainers.Count == 0) return;
            Debug.Log("BookmarkLinesController Refreshing measure lines...");

            var checkContainers = bookmarkTexts.Keys.ToList();
            var counter = 1;
            bookmarkContainers.ForEach(bookmark =>
            {
                checkContainers.Remove(bookmark);
                if (!bookmarkTexts.ContainsKey(bookmark))
                {
                    var text = Instantiate(bookmarkLinePrefab, parent);
                    text.transform.localPosition = new Vector3(bookmarkXposition, bookmark.Data.Time * EditorScaleController.EditorScale, 0);
                    bookmarkTexts.Add(bookmark, (text, true));
                }
                bookmarkTexts[bookmark].Item1.text = $"<mark=#{ColorUtility.ToHtmlStringRGB(bookmark.Data.Color)}50><size=30%><voffset=0.2><s> <indent=6> </s></voffset></size></mark> <size={Options.Instance.bookmarkLinesFontSize}%>{counter}: {bookmark.Data.Name}</size><color=#00000000>.</color>";
                counter++;
            });
            checkContainers.ForEach(Container =>
            {
                Destroy(bookmarkTexts[Container].Item1.gameObject);
                bookmarkTexts.Remove(Container);
            });

            init = true;
            RefreshVisibility();
            RefreshPositions();
        }

        private void RefreshVisibility()
        {
            var currentBeat = atsc.CurrentBeat;
            var beatsAhead = frontNoteGridScaling.localScale.z / EditorScaleController.EditorScale;
            var beatsBehind = beatsAhead / 4f;

            foreach (var kvp in bookmarkContainers)
            {
                var time = kvp.Data.Time;
                var text = bookmarkTexts[kvp].Item1;
                var enabled = time >= currentBeat - beatsBehind && time <= currentBeat + beatsAhead;

                if (bookmarkTexts[kvp].Item2 != enabled)
                {
                    text.gameObject.SetActive(enabled);
                    bookmarkTexts[kvp] = (text, enabled);
                }
            }
        }

        private void RefreshPositions()
        {
            foreach (var kvp in bookmarkContainers)
                bookmarkTexts[kvp].Item1.transform.localPosition = new Vector3(bookmarkXposition, kvp.Data.Time * EditorScaleController.EditorScale, 0);
        }
    }
}
