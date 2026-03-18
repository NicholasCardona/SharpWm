using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SharpWM.Common;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace SharpWM.Platform;

[SupportedOSPlatform("windows5.0")]
public static class WindowService
{
    private const SET_WINDOW_POS_FLAGS TileFlags =
        SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE |
        SET_WINDOW_POS_FLAGS.SWP_NOZORDER   |
        SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextW(nint hWnd, System.Text.StringBuilder buf, int nMax);

    public static bool SetWindowBounds(nint handle, Rect rect) =>
        PInvoke.SetWindowPos(new HWND(handle), default,
            rect.X, rect.Y, rect.Width, rect.Height, TileFlags);

    public static Rect GetWindowBounds(nint handle)
    {
        if (!PInvoke.GetWindowRect(new HWND(handle), out var rc))
            return Rect.Empty;
        return new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
    }

    public static string GetWindowTitle(nint handle)
    {
        var sb = new System.Text.StringBuilder(256);
        int len = GetWindowTextW(handle, sb, sb.Capacity);
        return len > 0 ? sb.ToString() : string.Empty;
    }

    public static bool Focus(nint handle) =>
        PInvoke.SetForegroundWindow(new HWND(handle));

    public static bool IsManageable(nint handle)
    {
        var hwnd = new HWND(handle);
        return PInvoke.IsWindowVisible(hwnd) && !PInvoke.IsIconic(hwnd);
    }
}