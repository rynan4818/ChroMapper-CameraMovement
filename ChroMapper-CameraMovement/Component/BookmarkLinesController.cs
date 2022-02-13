using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
                    text.transform.localPosition = new Vector3(0, bookmark.Data.Time * EditorScaleController.EditorScale, 0);
                    bookmarkTexts.Add(bookmark, (text, true));
                }
                bookmarkTexts[bookmark].Item1.text = $"{counter}: {bookmark.Data.Name}";
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
                bookmarkTexts[kvp].Item1.transform.localPosition = new Vector3(0, kvp.Data.Time * EditorScaleController.EditorScale, 0);
        }
    }
}
