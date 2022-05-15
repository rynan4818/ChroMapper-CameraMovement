using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class WindowController
{
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(String className, String windowName);

    // Sets window attributes
    [DllImport("user32.dll")]
    public static extern int SetWindowLongPtr(IntPtr hWnd, int nIndex, int dwNewLong);

    // Gets window attributes
    [DllImport("user32.dll")]
    public static extern int GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Unicode)]
    static extern bool SetWindowText(IntPtr hwnd, string txt);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr GetForegroundWindow();

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    // WindowsAPI 定数設定
    // ハンドル取得指定
    public static int GWL_STYLE = -16;           // ウィンドウスタイルのハンドル
    // ウィンドウスタイル
    public static int WS_BORDER = 0x00800000;       // 境界線を持つウィンドウ
    public static int WS_DLGFRAME = 0x00400000;     // ダイアログボックスで一般的に使われるスタイルの境界を持つウィンドウ
    public static int WS_SIZEBOX = 0x00040000;      // サイズ変更境界を持つウィンドウ
    public static int WS_SYSMENU = 0x00080000;      // タイトルバー上にウィンドウメニューボックスを持つウィンドウ
    public static int WS_MAXIMIZEBOX = 0x00010000;  // 最大化ボタンを持つウィンドウ
    public static int WS_MINIMIZEBOX = 0x00020000;  // 最小化ボタンを持つウィンドウ
    public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME;  // タイトルバーを持つウィンドウ
    // ウィンドウサイズ・位置フラグ
    public static int SWP_NOMOVE = 0x0002;   // 現在の位置を維持する（x パラメータと y パラメータを無視)
    public static int SWP_NOSIZE = 0x0001;   // 現在のサイズを維持する（cx パラメータと cy パラメータを無視)
    public static int SWP_NOZORDER = 0x0004; // 現在の Z オーダーを維持する（hWndInsertAfter パラメータを無視)

    public static Dictionary<IntPtr, (int, int)> windowSizes = new Dictionary<IntPtr, (int, int)>();

    public static (bool, IntPtr, int, int, string) windowReplace(string name, string new_name, int x, int y, double size, bool? hideTitleBar, int? insertAfter = 0)
    {
        var window = FindWindow(null, name);
        return windowReplace(window, new_name, x, y, size, hideTitleBar, insertAfter);
    }
    public static (bool, IntPtr, int, int, string) windowReplace(IntPtr window, string new_name, int x, int y, double size, bool? hideTitleBar, int? insertAfter = 0)
    {
        RECT rect;
        GetWindowRect(window, out rect);
        var org_width = rect.right - rect.left;
        var org_height = rect.bottom - rect.top;
        var width = (int)(org_width * size);
        var height = (int)(org_height * size);
        return windowReplace(window, new_name, x, y, width, height, hideTitleBar, insertAfter);
    }

    public static (bool, IntPtr, int, int, string) windowReplace(string name, string new_name, int x, int y, int width, int height, bool? hideTitleBar, int? insertAfter = 0)
    {
        var window = FindWindow(null, name);
        return windowReplace(window, new_name, x, y, width, height, hideTitleBar, insertAfter);
    }
    public static (bool, IntPtr, int, int, string) windowReplace(IntPtr window, string new_name, int x, int y, int width, int height, bool? hideTitleBar, int? insertAfter = 0)
    {
        if (new_name != null)
            SetWindowText(window, new_name);
        RECT rect;
        GetWindowRect(window, out rect);
        var org_width = rect.right - rect.left;
        var org_height = rect.bottom - rect.top;
        windowSizes[window] = (org_width, org_height);

        var style = GetWindowLongPtr(window, GWL_STYLE);
        if (hideTitleBar == true)
            SetWindowLongPtr(window, GWL_STYLE, (style & ~WS_CAPTION));
        else
        {
            style |= WS_CAPTION;
            style |= WS_SIZEBOX;
            style |= WS_SYSMENU;
            style |= WS_MAXIMIZEBOX;
            style |= WS_MINIMIZEBOX;
            SetWindowLongPtr(window, GWL_STYLE, style);
            if (hideTitleBar == null)
                return (true ,window, org_width, org_height, new_name);
        }
        var wFlags = 0;
        if (x * y == 0)
            wFlags |= SWP_NOMOVE;
        if (width * height == 0)
            wFlags |= SWP_NOSIZE;
        var hWndInsertAfter = 0;
        if (insertAfter == null)
            wFlags |= SWP_NOZORDER;
        else
            hWndInsertAfter = (int)insertAfter;
        SetWindowPos(window, hWndInsertAfter, x, y, width, height, wFlags);
        windowSizes[window] = (width, height);
        return (true ,window, org_width, org_height, new_name);
    }
    public static void windowAspectResize((bool, IntPtr, int, int, string)  windows)
    {
        var window = windows.Item2;
        RECT rect;
        GetWindowRect(window, out rect);
        var width = rect.right - rect.left;
        var height = rect.bottom - rect.top;
        if (windowSizes[window].Item1 == width && windowSizes[window].Item2 == height)
            return;
        var org_width = windows.Item3;
        var org_height = windows.Item4;
        var wFlags = 0;
        wFlags |= SWP_NOMOVE;
        wFlags |= SWP_NOZORDER;
        var new_height = (int)((double)org_height / (double)org_width * (double)width);
        SetWindowPos(window, 0, 0, 0, width, new_height, wFlags);
        windowSizes[window] = (width, new_height);
    }
    public static IntPtr getForegroundWindowHandle()
    {
        return GetForegroundWindow();
    }
    public static (int, int, int, int) getWindowPosSize(string name)
    {
        var window = FindWindow(null, name);
        return getWindowPosSize(window);
    }

    public static (int, int, int, int) getWindowPosSize(IntPtr window)
    {
        RECT rect;
        GetWindowRect(window, out rect);
        return (rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
    }

}