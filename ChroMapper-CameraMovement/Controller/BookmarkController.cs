using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using UnityEngine;
using ChroMapper_CameraMovement.Component;
using ChroMapper_CameraMovement.Configuration;
using ChroMapper_CameraMovement.UserInterface;
using ChroMapper_CameraMovement.HarmonyPatches;
using SFB;

namespace ChroMapper_CameraMovement.Controller
{
    public class BookmarkController
    {
        public BookmarkManager bookmarkManager;
        public List<BookmarkContainer> bookmarkContainers;
        public AudioTimeSyncController atsc;
        public BookmarkLinesController bookmarkLinesController { get; set; }
        public void BookmarkContainerGet()
        {
            Type type = bookmarkManager.GetType();
            FieldInfo field = type.GetField("bookmarkContainers", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            bookmarkContainers = (List<BookmarkContainer>)(field.GetValue(bookmarkManager));
        }
        public void BookmarkTrackSet()
        {
            if (Options.Instance.bookmarkLines && Options.Instance.cameraMovementEnable)
                bookmarkLinesController.RefreshBookmarkLines(bookmarkContainers);
        }
        public void BookmarkWidthChange()
        {
            var bookmarkWidth = 10f;
            if (Options.Instance.cameraMovementEnable)
                bookmarkWidth = Options.Instance.bookmarkWidth;
            bookmarkContainers.ForEach(container =>
            {
                var rectTransform = (RectTransform)container.transform;
                rectTransform.sizeDelta = new Vector2(bookmarkWidth, 20f);
            });
        }
        public void BookMarkChangeUpdate()
        {
            if (!Options.Instance.cameraMovementEnable) return;
            BookmarkContainerGet();
            BookmarkWidthChange();
            BookmarkTrackSet();
            BookMarkUpdate();
        }
        public void BookMarkUpdate()
        {
            if (bookmarkContainers.Count > 0)
            {
                var lastBookmark = bookmarkContainers.FindLast(x => x.Data.SongBpmTime <= atsc.CurrentSongBpmTime);
                if (bookmarkContainers.IndexOf(lastBookmark) == -1)
                {
                    UI._bookmarkMenuUI.CurrentBookmarkUpdate("", 0, 0);
                }
                else
                {
                    UI._bookmarkMenuUI.CurrentBookmarkUpdate(lastBookmark.Data.Name, bookmarkContainers.IndexOf(lastBookmark) + 1, lastBookmark.Data.JsonTime);
                }
            }
            else
            {
                UI._bookmarkMenuUI.CurrentBookmarkUpdate("", 0, 0);
            }
        }
        public void BookmarkExport()
        {
            var paths = StandaloneFileBrowser.SaveFilePanel("Bookmark Export CSV File", BeatSaberSongContainer.Instance.Song.Directory, "bookmark.csv", "csv");
            if (paths.Length > 0)
            {
                var sb = new StringBuilder();
                bookmarkContainers.ForEach(container =>
                {
                    var row = $"{container.Data.JsonTime},\"{container.Data.Name}\"";
                    sb.AppendLine(row);
                });
                try
                {
                    using (var file = new StreamWriter(paths, false))
                    {
                        file.Write(sb.ToString());
                    }
                }
                catch
                {
                    Debug.LogError("Bookmark CSV File Write ERROR");
                }
            }
        }
        public void BookmarkImport()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Bookmark Import CSV File", BeatSaberSongContainer.Instance.Song.Directory, "csv", false)[0];
            if (File.Exists(paths))
            {
                var replaceBookmak = new List<(float, string)>();
                try
                {
                    using (var sr = new StreamReader(paths))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var values = line.Split(new Char[] { ',' }, 2);
                            var name = values[1].Replace("\"", "");
                            float time;
                            if (float.TryParse(values[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out time))
                            {
                                replaceBookmak.Add((time, name));
                            }
                        }
                    }
                }
                catch
                {
                    Debug.LogError("Bookmark CSV File Import ERROR");
                    return;
                }
                if (replaceBookmak.Count > 0)
                {
                    //このやり方は駄目なので、ちゃんとしたやリ方を考える
                    /*
                    bookmarkContainers.ForEach(container =>
                    {
                        Type type = container.GetType();
                        MethodInfo method = type.GetMethod("HandleDeleteBookmark", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                        method.Invoke(container, new object[] { 0 });
                    });
                    replaceBookmak.ForEach(bookmark =>
                    {
                        atsc.MoveToTimeInBeats(bookmark.Item1);
                        Type type = bookmarkManager.GetType();
                        MethodInfo method;
                        try
                        {
                            method = type.GetMethod("HandleNewBookmarkName", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                            method.Invoke(bookmarkManager, new object[1] { bookmark.Item2 });
                        }
                        catch
                        {
                            //2022.2.7 "New bookmark dialog remade with CMUI"  SHA-1:795115393a1fb265ee6b77f7616941ce62b0e208 での変更対応
                            method = type.GetMethod("CreateNewBookmark", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                            method.Invoke(bookmarkManager, new object[2] { bookmark.Item2, null });
                        }
                    });
                    */
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
                method = type.GetMethod("HandleNewBookmarkName", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(bookmarkManager, new object[1] { bookmarkName });
            }
            catch
            {
                //2022.2.7 "New bookmark dialog remade with CMUI"  SHA-1:795115393a1fb265ee6b77f7616941ce62b0e208 での変更対応
                method = type.GetMethod("CreateNewBookmark", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(bookmarkManager, new object[2] { bookmarkName, null });
            }
        }

        public void Start()
        {
            bookmarkManager = UnityEngine.Object.FindObjectOfType<BookmarkManager>();
            bookmarkManager.BookmarksUpdated += BookMarkChangeUpdate;
            BookmarkContainerPatch.OnNewBookmarkName += BookMarkChangeUpdate;
            BookMarkChangeUpdate();
        }
        public void OnDestroy()
        {
            bookmarkManager.BookmarksUpdated -= BookMarkChangeUpdate;
            BookmarkContainerPatch.OnNewBookmarkName -= BookMarkChangeUpdate;
        }
    }
}
